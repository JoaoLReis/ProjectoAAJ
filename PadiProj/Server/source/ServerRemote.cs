using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Interfaces;
using Containers;

namespace Server.source
{
    public enum STATE { FROZEN, ALIVE, FAILED, PARTICIPANT, COORDINATOR };

    //Prepare delegate.
    public delegate void RemoteAsyncPrepare(Transaction t, string _coordinatorURL);
    public delegate bool RemoteAsyncValidate(Transaction t);
    public delegate bool RemoteAsyncAbort(Transaction t);

    //Commit local changes delegate.
    public delegate void RemoteAsyncCommitLocalChanges();

    class ServerRemote : MarshalByRefObject, RemoteServerInterface
    {
        //URLs for future use.
        private string _ownURL;
        private string _replicaURL;
        private string _masterURL;

        //Padint Database.
        private Dictionary<int, PadIntValue> _padInts;

        //Last commited Transaction.
        private int _lastTicketTrans = 0;

        //Active transaction.
        private Transaction _activeTransaction;

        //Master remote reference.
        private RemoteMasterInterface _master;

        //Previous and current state of the server.
        private STATE _status;
        private STATE _prevStatus;

        //List of all participants.
        List<String> _participants;
        //Changes to be commited to the local database.
        List<PadIntValue> _valuesToBeChanged;

        //Handlers.
        AutoResetEvent[] _handles;
        AutoResetEvent[] _unComTransHandles;


        //Participants handlers.
        Dictionary<String, int> _partHandlers;

        //Coordinator URL keeps being updated.
        String _coordinatorURL;

        int _handlerID;

        List<Transaction> _transactionsUncommited;

        //Makes this remote objects lease indefinite.
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public ServerRemote()
        {
            _participants = new List<String>();
            _valuesToBeChanged = new List<PadIntValue>();
            _padInts = new Dictionary<int, PadIntValue>();
            _status = STATE.ALIVE;
            _prevStatus = STATE.ALIVE;
            _partHandlers = new Dictionary<string, int>();
            _coordinatorURL = "";
            _handlerID = 0;
            _transactionsUncommited = new List<Transaction>();
        }

        //Registers this server on the master server.
        internal void regToMaster(int localport)
        {
            _ownURL = "tcp://localhost:" + localport + "/Server";
            _master = (RemoteMasterInterface)Activator.GetObject(
            typeof(RemoteMasterInterface),
            "tcp://localhost:" + Interfaces.Constants.MasterPort + "/master");
            _master.regServer(_ownURL);
        }

        //Checks the current status of the server and throws exceptions acordingly.
        //This function is a helper function to be invoked by other methods in this class.
        private void checkStatus()
        {
            if (_status == STATE.FROZEN)
            {
                throw new FreezeException("Frozen Server.");
            }
            else if (_status == STATE.FAILED)
            {
                throw new FailException("Failed Server.");
            }
        }

        //Coordinator Function invoked by a participant to the coordinator after a prepare is done. 
        public void prepared(string url, bool sucessfull)
        {
            //Currently we compare only size.
            //And we store only.
            _handles[(_partHandlers[url])].Set();
        }

        //Coordinator Function invoked by a participant to the coordinator after a commit is done. 
        public void commited(string url, bool sucessfull)
        {
            //Currently we compare only size.
            //And we store only.
            if (!sucessfull)
                abort(_activeTransaction);
            _handles[(_partHandlers[url])].Set();
        }

        //Participant prepare function invoked by a coordinator.
        public void prepare(Transaction t, string coordinatorURL)
        {
            checkStatus();
            _prevStatus = _status;
            _coordinatorURL = coordinatorURL;
            foreach (Request r in t.getRequests())
            {
                PadIntValue value;
                if (_padInts.TryGetValue(r.involved(), out value))
                {
                    //add transaction t to the list of uncommited transactions
                    _transactionsUncommited.Add(t);

                    if (r.isWrite())
                    {
                        value.setValue(r.getVal());
                        _valuesToBeChanged.Add(value);
                        Console.WriteLine("Preparing write to padint " + value.getId() + " the value " + value.getValue());
                    }
                    else
                    {
                        Console.WriteLine("Preparing Read of padint " + value.getId() + "it has the value " + value.getValue());
                }
            }
            }
            try
            {
                RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface), coordinatorURL);
                serv.prepared(_ownURL, true);
            }
            catch(TxException e)
            {
                Console.WriteLine("Failed at remote prepare");
            }

        }

        //Prepares the execution of a list of requests.
        private void prepExec(Transaction t)
        {
            List<Request> requests = t.getRequests();
            //Current handler being added.
            int _handlerID = 0;
            foreach (Request r in requests)
            {
                PadIntValue value;
                //Does the current server has the padint involved in the request?
                if (_padInts.TryGetValue(r.involved(), out value))
                {
                    //add transaction t to the list of uncommited transactions
                    _transactionsUncommited.Add(t);

                    //Is it a write?
                    if (r.isWrite())
                    {
                        value.setValue(r.getVal());
                        _valuesToBeChanged.Add(value);
                        Console.WriteLine("Preparing write to padint " + value.getId() + " the value " + value.getValue());
                    }
                    else
                    {
                        Console.WriteLine("Preparing Read of padint " + value.getId() + "it has the value " + value.getValue());
                    }
                }
                else
                {
                    //We dont have the padint involved in this request does it exist? if yes add the participant that has the required padint.
                    try
                    {
                        string serverURL = _master.getServer(r.involved());
                        if(!_participants.Contains(serverURL))
                        {
                        _participants.Add(serverURL);
                        _partHandlers.Add(serverURL, _handlerID);
                        _handlerID++;
                        Console.WriteLine("Padint not present on this server. Executing prepare on other servers...");
                    }
                    }
                    catch (TxException e)
                    {
                        Console.WriteLine(e.Message);
                        throw e;
                    }
                }
            }

            _handles = new AutoResetEvent[_participants.Count()];
            for (int i = 0; i < _handlerID; i++)
            {
                _handles[i] = new AutoResetEvent(false);
            }          
        }

        //Requests a unique transaction timestamp to the master.
        private void requestTransID()
        {
            _master = (RemoteMasterInterface)Activator.GetObject(
            typeof(RemoteMasterInterface),
            "tcp://localhost:" + Interfaces.Constants.MasterPort + "/master");
        //request to the master a Transaction ID
            _master.getTimeStamp();
        }

        public bool validateLocal(Transaction t)
        {
            //Start the validating proccess
            if (t.getTicket() < _lastTicketTrans)
                return false;
            else if (t.getTicket() == _lastTicketTrans + 1)
                return true;
            else
            {
                if (!WaitHandle.WaitAll(_handles, 10))
                    return false;
                else
                {
                    foreach (Transaction tran_last in _transactionsUncommited)
                    {
                        foreach (Request req_last in tran_last.getRequests())
                            foreach (Request req_act in t.getRequests())
                                if (req_last.involved() == req_act.involved() && req_last.isWrite() == req_act.isWrite())
                                    return false;
                    }
                    foreach (String participant in _participants)
                    {
                        try
                        {
                            RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                            typeof(RemoteServerInterface), participant);
                            RemoteAsyncValidate validate = new RemoteAsyncValidate(serv.validate);
                            validate.BeginInvoke(t, null, null);
                        }
                        catch (TxException e)
                        {

                        }
                    }
                    return true;
                }
            }
        }


        //Validates the current transaction.
        public bool validate(Transaction t)
        {
            //Start the validating proccess
            if (t.getTicket() < _lastTicketTrans)
                return false;// Probably never happens since t ticket will never be lower than last seen

            else if (t.getTicket() == _lastTicketTrans+1)
                return true;

            else
            {
                if (!WaitHandle.WaitAll(_unComTransHandles, 10))
                    return false;
                else
                {
                    foreach (Transaction tran_last in _transactionsUncommited) {
                        foreach (Request req_last in tran_last.getRequests())
                            foreach (Request req_act in t.getRequests())
                                if (req_last.involved() == req_act.involved() && req_last.isWrite() == req_act.isWrite())
                                    return false;
                    }
                    if (validateLocal(t))
                        return true;
                    return false;
        }
            }
        }

        public void registerReplica()
        {
            //to be invoked by a replica
            try
            {
                _master.requestServerReplica(this._ownURL);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                registerReplica();
            }
        }

        //Function invoked to update the database with the changes to be made.
        public void commitLocalChanges()
        {
            foreach (PadIntValue item in _valuesToBeChanged)
            {
                _padInts[(item.getId())].setValue(item.getValue());
            }
                RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface), _coordinatorURL);
                serv.commited(_ownURL, true);
                _status = STATE.ALIVE;

            
            }

        private void resetHandles()
        {
            foreach(AutoResetEvent e in _handles)
            {
                e.Reset();
            }
        }

        private void broadcast(List<String> participants, int ticket)
        {
            _master.broadcast(participants, ticket);
        }

        //Function invoked by a client to commit a transaction.
        public bool commit(Transaction t)
        {
            //Checks the server status.
            checkStatus();

            //Updates current transaction.
            _activeTransaction = t;

            //Sets coordinator STATE.
            _prevStatus = _status;

            //Writes to the _valuesToBeChanged list the changes to be executed on this server.
            //Determines who are the participants and stores them on _participants.
            prepExec(t);

            //Invokes prepare statement on all participants.
            foreach (String p in _participants)
            {
                try
                {
                    RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                    typeof(RemoteServerInterface), p);
                    RemoteAsyncPrepare prep = new RemoteAsyncPrepare(serv.prepare);
                    prep.BeginInvoke(t, _ownURL, null, null);
                }
                catch(TxException e)
                {
                    //TODO
                        //Console.WriteLine(e.Message);
                        throw e;
                }
            }

            if(_participants.Count() > 0)
                if (!WaitHandle.WaitAll(_handles, 20))
                    throw new TxException("Receiving Prepares failed");

            broadCast(_participants, t.getTicket());

            resetHandles();

            t.setTicket(_master.getTicket());

            if(validate(t))
            {
                //Commiting local changes
                foreach (PadIntValue item in _valuesToBeChanged)
                {
                    _padInts[(item.getId())].setValue(item.getValue());
                }
                foreach (String p in _participants)
                {
                    try
                    {
                        RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                        typeof(RemoteServerInterface), p);
                        RemoteAsyncCommitLocalChanges prep = new RemoteAsyncCommitLocalChanges(serv.commitLocalChanges);
                        prep.BeginInvoke(null, null);
                    }
                    catch(TxException e)
                    {
                        Console.WriteLine(e.Message);
                        throw e;
                    }
                }

                if (_participants.Count() > 0)
                {
                    if (!WaitHandle.WaitAll(_handles, 20))
                        throw new TxException("Receiving Commit failed");
                    resetHandles();
                }

                if (_transactionsUncommited.Contains(t))
                    _transactionsUncommited.Remove(t);

                _prevStatus = STATE.ALIVE;
                _status = STATE.ALIVE;
                _participants.Clear();
                _valuesToBeChanged.Clear();
                _handles = null;
                _partHandlers.Clear();
                Console.WriteLine("Transaction Successfull.");
                return true;
            }
            else
            {
                return abort(t);
            }
        }

        public bool abort(Transaction t)
        { 
        //abort a transaction
            
            _valuesToBeChanged.Clear();     //Discutir esta parte, porque está-se a eliminar tudo mesmo o que pertence a outras transacções
            t.getRequests().Clear();

            foreach (String p in _participants)
            {
                try
                {
                    RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                    typeof(RemoteServerInterface), p);
                    RemoteAsyncAbort abort = new RemoteAsyncAbort(serv.abort);
                    abort.BeginInvoke(t, null, null);
                }
                catch (TxException e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return false;
        }

        //Creates a transaction and generates a timestamp.
        public Transaction begin()
        {
            checkStatus();
            _master = (RemoteMasterInterface)Activator.GetObject(
            typeof(RemoteMasterInterface),
            "tcp://localhost:" + Interfaces.Constants.MasterPort + "/master");

            try
            {
                DateTime dt = _master.getTimeStamp();
                return( new Transaction(dt, null));
            }
            catch(TxException e)
            {
                Console.WriteLine(e.Message);
                throw e;
            }
        }

        //Function that registers a padint on this server.
        public PadIntValue CreatePadInt(int uid)
        {
            try
            {
               /* _master = (RemoteMasterInterface)Activator.GetObject(
                typeof(RemoteMasterInterface),
                "tcp://localhost:" + Interfaces.Constants.MasterPort + "/master");*/
                if( _master.regPadint(uid, _ownURL))
                {
                    PadIntValue v = new PadIntValue(uid, 0);
                    _padInts.Add(uid, v);
                    Console.WriteLine("CreatedPadint: " + v.getId());
                    return v;
                }
                else
                {
                    //TODO
                    Console.WriteLine("Duplicated Padint");
                    throw new TxException("Duplicated Padint");
                }
           }
            catch (TxException e)
           {
               Console.WriteLine("Error Creating Padint from master.");
                //throws either a new exception or the same returned from the master.
               throw new TxException("Error Creating Padint from master.");
           }
        }

        //Function that checks localy if the padint exists, if not it must ask master where it is located.
        public PadIntValue AccessPadInt(int uid)
        {
            /*_master = (RemoteMasterInterface)Activator.GetObject(
            typeof(RemoteMasterInterface),
            "tcp://localhost:" + Interfaces.Constants.MasterPort + "/master");*/

            try
            {
                //Checks to see if the padint is present locally.
	            if (_padInts.ContainsKey(uid)) 
	            {
                    return _padInts[uid];
	            }

                string serverURL = _master.getServer(uid);
                RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface), serverURL);

                return serv.AccessPadInt(uid);
            }
            catch(TxException e)
            {
                //throws either a new exception or the same returned from the master.
                return null;
            }
        }

        private void printPadints()
        {
            Console.WriteLine(" | *------------* | ");
            foreach (int key in _padInts.Keys)
	        {
                PadIntValue item = _padInts[key];
		        Console.WriteLine("Padint: " + item.getId() + " with value: " + item.getValue());
	        }
            Console.WriteLine(" | *------------* | ");
        }

        public void status() 
        {
            if(_status == STATE.ALIVE)
                Console.Out.WriteLine("Server at: " + _ownURL + "STATE.ALIVE");
            else if (_status == STATE.FAILED)
                Console.Out.WriteLine("Server at: " + _ownURL + "STATE.FAILED");
            else if (_status == STATE.FROZEN)
                Console.Out.WriteLine("Server at: " + _ownURL + "STATE.FROZEN");
            printPadints();
        }

        public void fail() 
        {
            _prevStatus = STATE.ALIVE;
            _status = STATE.FAILED;
        }

        public void freeze() 
        {
            _prevStatus = _status;
            _status = STATE.FROZEN;
        }

        public void recover() 
        {
            _master.safeServ(_ownURL);
            _status = _prevStatus;
        }
    }
}
