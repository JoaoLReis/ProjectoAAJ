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
    public delegate void RemoteAsyncValidate(Transaction t);
    public delegate bool RemoteAsyncAbort(int ticket);

    //Commit local changes delegate.
    public delegate void RemoteAsyncCommitLocalChanges(int ticket);

    struct TransactionInfo
    {
        //List of all participants.
        public List<String> _participants;

        //Changes to be commited to the local database.
        public List<PadIntValue> _valuesToBeChanged;

        public List<Transaction> _transactionsUncommited;

        //Active transaction.
        public Transaction _transaction;

        //Handlers.
        public AutoResetEvent[] _handles;
        public AutoResetEvent[] _validatehandles;
        public AutoResetEvent[] _unComTransHandles;
        
        //Participants handlers.
        public Dictionary<String, int> _partHandlers;
        public int _lastTicketSeen;

        //Coordinator URL keeps being updated.
        public String _coordinatorURL;

        public int _handlerID;

        public TransactionInfo(Transaction t)
        {
            _transaction = t;
            _handles = null;
            _validatehandles = null;
            _unComTransHandles = null;
            _participants = new List<String>();
            _valuesToBeChanged = new List<PadIntValue>();
            _partHandlers = new Dictionary<string, int>();
            _coordinatorURL = "";
            _handlerID = 0;
            _lastTicketSeen = 0;
            _transactionsUncommited = new List<Transaction>();
        }
    }

    class ServerRemote : MarshalByRefObject, RemoteServerInterface
    {
        //URLs for future use.
        private string _ownURL;
        private string _replicaURL;
        private string _masterURL;

        //Padint Database.
        private Dictionary<int, PadIntValue> _padInts;

        //Last commited Transaction.
        private int _lastTicketTrans;
        private List<int> _tickets;

        //Master remote reference.
        private RemoteMasterInterface _master;

        //Previous and current state of the server.
        private STATE _status;
        private STATE _prevStatus;


        ////Active transaction.
        //private Transaction _activeTransaction;
        ////List of all participants.
        //List<String> _participants;
        ////Changes to be commited to the local database.
        //List<PadIntValue> _valuesToBeChanged;

        ////Handlers.
        //AutoResetEvent[] _handles;
        //AutoResetEvent[] _validatehandles;
        //AutoResetEvent[] _unComTransHandles;

        Dictionary<int, TransactionInfo> _transactions;


        ////Participants handlers.
        //Dictionary<String, int> _partHandlers;

        ////Coordinator URL keeps being updated.
        //String _coordinatorURL;

        //int _handlerID;

        List<Transaction> _transactionsUncommited;

        //Makes this remote objects lease indefinite.
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public ServerRemote()
        {
            
            _padInts = new Dictionary<int, PadIntValue>();
            _status = STATE.ALIVE;
            _prevStatus = STATE.ALIVE;
            _transactionsUncommited = new List<Transaction>();
            _tickets = new List<int>();
            _tickets.Add(0);
            _transactions = new Dictionary<int, TransactionInfo>();
            _lastTicketTrans = 0;
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


        /*################################################################################
         *                          TICKETS FUNCTIONS
         * ##############################################################################*/
        private void addTicketToList(int ticket)
        {
            System.Object obj = (System.Object)_tickets;
            System.Threading.Monitor.Enter(obj);
            try
            {

                if (ticket == _lastTicketTrans + 1)
                {
                    _tickets.Add(ticket);
                    _lastTicketTrans = ticket;
                    return;
                }

                int a = 0, b = 0;
                foreach (int i in _tickets)
                {
                    a = b;
                    b = i;
                    if (ticket > a && ticket < b)
                        _tickets.Add(ticket);
                }
                if (ticket > _tickets.Last())
                    _tickets.Add(ticket);
            }
            finally
            {
                System.Threading.Monitor.Exit(obj);
            }
        }

        private void removeTicketSequence()
        {   
            System.Object obj = (System.Object)_tickets;
            System.Threading.Monitor.Enter(obj);
            try
            {

                List<int> aux = new List<int>();
                aux.AddRange(_tickets);
                int k = 0, l = 0, m = 0;

                foreach (int i in aux)
                {
                    k = l;
                    l = i;
                    if (m == 0)
                    {
                        _tickets.Remove(_tickets.First());
                        m = 1;
                        continue;
                    }

                    if (l == k + 1)
                        _tickets.Remove(k);
                    else break;
                }
                _lastTicketTrans = _tickets.First();
            }
            finally
            {
                System.Threading.Monitor.Exit(obj);
            }
        }

        //function to receive tickets from broadcast
        public void getTicket(int ticket)
        {
            // WARNING!!!!!!!!!!!!!!!!!
            // need to set the uncomtranshandles but there are issues with the index
            if (_transactions.ContainsKey(ticket))
            {
                TransactionInfo tInfo = _transactions[ticket];
                if (tInfo._unComTransHandles != null)
                {
                    tInfo._unComTransHandles[ticket - tInfo._lastTicketSeen - 1].Set();
                    //Talvez ter uma lista de transactions -> lastTicketSeen aquando do seu início resolva.
                }
                _transactions[ticket] = tInfo;
            }

            // if new ticket seen is immediately after last seen than 
            addTicketToList(ticket);
            
            removeTicketSequence();
        }

        /*################################################################################
          ###############################################################################*/

        //Coordinator Function invoked by a participant to the coordinator after a prepare is done. 
        public void prepared(int ticket, string url, bool sucessfull)
        {
            TransactionInfo tInfo = _transactions[ticket];
            //Currently we compare only size.
            //And we store only.
            tInfo._handles[(tInfo._partHandlers[url])].Set();

            _transactions[ticket] = tInfo;
        }

        //Coordinator Function invoked by a participant to the coordinator after a commit is done. 
        public void commited(int ticket, string url, bool sucessfull)
        {
            TransactionInfo tInfo = _transactions[ticket];
            //Currently we compare only size.
            //And we store only.
            if (!sucessfull)
                abort(ticket);
            tInfo._handles[(tInfo._partHandlers[url])].Set();

            _transactions[ticket] = tInfo;
        }

        public void validated(int ticket, string url, bool sucessfull)
        {
            TransactionInfo tInfo = _transactions[ticket];
            //Currently we compare only size.
            //And we store only.
            if (!sucessfull)
                abort(ticket);
            tInfo._validatehandles[(tInfo._partHandlers[url])].Set();

            _transactions[ticket] = tInfo;
        }

        //Participant prepare function invoked by a coordinator.
        public void prepare(Transaction t, string coordinatorURL)
        {
            TransactionInfo tInfo = new TransactionInfo(t);
            _transactions.Add(t.getTicket(), tInfo);

            checkStatus();
            _prevStatus = _status;
            tInfo._coordinatorURL = coordinatorURL;

            bool selfinvolved = false;

            foreach (Request r in t.getRequests())
            {
                PadIntValue value;
                if (_padInts.TryGetValue(r.involved(), out value))
                {
                    selfinvolved = true;

                    if (r.isWrite())
                    {
                        value.setValue(r.getVal());
                        tInfo._valuesToBeChanged.Add(value);
                        Console.WriteLine("Preparing write to padint " + value.getId() + " the value " + value.getValue());
                    }
                    else
                    {
                        Console.WriteLine("Preparing Read of padint " + value.getId() + "it has the value " + value.getValue());
                    }
                }
            }

            if (selfinvolved)
            {
                Console.WriteLine("###############");
                Console.WriteLine("adding uncomm trans Prepare");
                Console.WriteLine("t Request Count: " + t.getRequests().Count());
                Console.WriteLine("t Ticket: " + t.getTicket());
                Console.WriteLine("t TimeStamp: " + t.getTimeStamp());
                Console.WriteLine("###############");
                Console.WriteLine("");
                //add transaction t to the list of uncommited transactions and the ticket to the tickets seen list
                if(!_transactionsUncommited.Contains(t))
                    _transactionsUncommited.Add(t);
            }

            addTicketToList(t.getTicket());
            removeTicketSequence();

            try
            {
                RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                typeof(RemoteServerInterface), coordinatorURL);
                serv.prepared(t.getTicket(), _ownURL, true);
            }
            catch(TxException e)
            {
                Console.WriteLine("Failed at remote prepare");
                throw e;
            }

            _transactions[t.getTicket()] = tInfo;

        }

        //Prepares the execution of a list of requests.
        private void prepExec(int ticket)
        {
            TransactionInfo tInfo = _transactions[ticket];
            Transaction t = tInfo._transaction;

            List<Request> requests = t.getRequests();

            bool selfinvolved = false;

            foreach (Request r in requests)
            {
                PadIntValue value;
                //Does the current server has the padint involved in the request?
                if (_padInts.TryGetValue(r.involved(), out value))
                {
                    selfinvolved = true;

                    //Is it a write?
                    if (r.isWrite())
                    {
                        value.setValue(r.getVal());
                        tInfo._valuesToBeChanged.Add(value);
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
                        if(!tInfo._participants.Contains(serverURL))
                        {
                            tInfo._participants.Add(serverURL);
                            tInfo._partHandlers.Add(serverURL, tInfo._handlerID);
                            tInfo._handlerID++;
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

            if (selfinvolved)
            {
                Console.WriteLine("###############");
                Console.WriteLine("adding uncom trans PrepExec");
                Console.WriteLine("t Request Count: " + t.getRequests().Count());
                Console.WriteLine("t Ticket: " + t.getTicket());
                Console.WriteLine("t TimeStamp: " + t.getTimeStamp());
                Console.WriteLine("###############");
                Console.WriteLine("");
                //add transaction t to the list of uncommited transactions and the ticket to the tickets seen list
                if (!_transactionsUncommited.Contains(t))
                    _transactionsUncommited.Add(t);
            }

            tInfo._handles = new AutoResetEvent[tInfo._participants.Count()];
            tInfo._validatehandles = new AutoResetEvent[tInfo._participants.Count()];

            for (int i = 0; i < tInfo._handlerID; i++)
            {
                tInfo._handles[i] = new AutoResetEvent(false);
                tInfo._validatehandles[i] = new AutoResetEvent(false);
            }

            _transactions[ticket] = tInfo;
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

        public void validateLocal(Transaction t)
        {
            TransactionInfo tInfo = _transactions[t.getTicket()];

            RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
            typeof(RemoteServerInterface), tInfo._coordinatorURL);


            tInfo._transactionsUncommited.Clear();
            tInfo._transactionsUncommited.AddRange(_transactionsUncommited);
            //Start the validating proccess
            if (!(t.getTicket() <= _lastTicketTrans))
            {
                tInfo._lastTicketSeen = _lastTicketTrans;
                tInfo._unComTransHandles = new AutoResetEvent[t.getTicket() - tInfo._lastTicketSeen];
                for (int i = 0; i < tInfo._handlerID; i++)
                {
                    tInfo._unComTransHandles[i] = new AutoResetEvent(false);
                }

                if (!WaitHandle.WaitAll(tInfo._unComTransHandles, 2000))
                    serv.validated(t.getTicket(), _ownURL, false);
            }

            // check if the transaction is in conflict with any uncommited transaction before me
            foreach (Transaction tran_last in tInfo._transactionsUncommited)
            {
                if (tran_last.getTicket() < t.getTicket())
                {
                    foreach (Request req_last in tran_last.getRequests())
                        foreach (Request req_act in t.getRequests())
                            if (req_last.involved() == req_act.involved() && req_last.isWrite() == req_act.isWrite())
                                serv.validated(t.getTicket(), _ownURL, false);
                }
            }
            serv.validated(t.getTicket(), _ownURL, true);

            _transactions[t.getTicket()] = tInfo;

        }

        //Validates the current transaction.
        public bool validate(int ticket)
        {
            TransactionInfo tInfo = _transactions[ticket];
            Transaction t = tInfo._transaction;

            //Start the validating proccess
            tInfo._transactionsUncommited.Clear();
            tInfo._transactionsUncommited.AddRange(_transactionsUncommited);
            if (!(t.getTicket() <= _lastTicketTrans))
            {
                tInfo._lastTicketSeen = _lastTicketTrans;
                int aux = t.getTicket() - tInfo._lastTicketSeen;
                tInfo._unComTransHandles = new AutoResetEvent[aux];
                for (int i = 0; i < aux; i++)
                {
                    tInfo._unComTransHandles[i] = new AutoResetEvent(false);
                }

                _transactions[ticket] = tInfo;

                if (!WaitHandle.WaitAll(tInfo._unComTransHandles, 2000))
                    return false;
            }

            // check if the transaction is in conflict with any uncommited transaction before me
            foreach (Transaction tran_last in tInfo._transactionsUncommited)
            {
                if (tran_last.getTicket() < t.getTicket())
                {
                    foreach (Request req_last in tran_last.getRequests())
                        foreach (Request req_act in t.getRequests())
                            if (req_last.involved() == req_act.involved() && req_last.isWrite() == req_act.isWrite())
                                return false;
                }
            }
            if (tInfo._participants.Count > 0)
            {
                foreach (String participant in tInfo._participants)
                {
                    try
                    {
                        Console.WriteLine("###############");
                        Console.WriteLine("Before Sending ValidateLocal");
                        Console.WriteLine("###############");
                        Console.WriteLine("");
                        RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                        typeof(RemoteServerInterface), participant);
                        RemoteAsyncValidate validate = new RemoteAsyncValidate(serv.validateLocal);
                        validate.BeginInvoke(t, null, null);
                    }
                    catch (TxException e)
                    {
                        throw e;
                    }
                }
                Console.WriteLine("###############");
                Console.WriteLine("Before Handles in Validate");
                Console.WriteLine("###############");
                Console.WriteLine("");
                if (!WaitHandle.WaitAll(tInfo._validatehandles, 2000))
                    return false;
            }

            _transactions[ticket] = tInfo;
            return true;

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
        public void commitLocalChanges(int ticket)
        {
            TransactionInfo tInfo = _transactions[ticket];
            Transaction t = tInfo._transaction;

            foreach (PadIntValue item in tInfo._valuesToBeChanged)
            {
                _padInts[(item.getId())].setValue(item.getValue());
            }
            RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
            typeof(RemoteServerInterface), tInfo._coordinatorURL);
            _status = STATE.ALIVE;


            //Console.WriteLine("###############");
            //Console.WriteLine("Before Removing from uncom trans Local");
            //Console.WriteLine("t Request Count: " + t.getRequests().Count());
            //Console.WriteLine("t Ticket: " + t.getTicket());
            //Console.WriteLine("t TimeStamp: " + t.getTimeStamp());
            //if(_transactionsUncommited.Count() > 0)
            //{
            //    Console.WriteLine("Trans Inside List Request Count: " + _transactionsUncommited.First().getRequests().Count());
            //    Console.WriteLine("Trans Inside List Ticket: " + _transactionsUncommited.First().getTicket());
            //    Console.WriteLine("Trans Inside List TimeStamp: " + _transactionsUncommited.First().getTimeStamp());
            //}
            //Console.WriteLine("###############");
            //Console.WriteLine("");
            if (_transactionsUncommited.Contains(t))
            {
                //Console.WriteLine("###############");
                //Console.WriteLine("Removing from uncom trans Local");
                //Console.WriteLine("###############");
                //Console.WriteLine("");
                _transactionsUncommited.Remove(t);
            }
            tInfo._transactionsUncommited.Clear();
            //Console.WriteLine("###############");
            //Console.WriteLine("After Removing from uncom trans Local");
            //Console.WriteLine("###############");
            //Console.WriteLine("");

            serv.commited(ticket, _ownURL, true);
            _transactions[ticket] = tInfo;

            
        }

        private void resetHandles(int ticket)
        {
            TransactionInfo tInfo = _transactions[ticket];
            tInfo._handlerID = 0;
            foreach(AutoResetEvent e in tInfo._handles)
            {
                e.Reset();
            }
            _transactions[tInfo._transaction.getTicket()] = tInfo;
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

            int ticket = t.getTicket();
            TransactionInfo tInfo = new TransactionInfo(t);

            //Updates current transaction.
            _transactions.Add(t.getTicket(), tInfo);

            //Sets coordinator STATE.
            _prevStatus = _status;

            //Writes to the _valuesToBeChanged list the changes to be executed on this server.
            //Determines who are the participants and stores them on _participants.
            prepExec(ticket);

            List<String> aux = new List<string>();
            aux.AddRange(tInfo._participants);
            aux.Add(_ownURL);

            tInfo = _transactions[ticket];

            broadcast(aux, ticket);
            
            addTicketToList(t.getTicket());
            removeTicketSequence();

            //Invokes prepare statement on all participants.
            foreach (String p in tInfo._participants)
            {
                try
                {
                    RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                    typeof(RemoteServerInterface), p);
                    RemoteAsyncPrepare prep = new RemoteAsyncPrepare(serv.prepare);
                    prep.BeginInvoke(t, _ownURL, null, null);
                }
                catch(Exception e)
                {
                    //TODO
                        //Console.WriteLine(e.Message);
                        throw e;
                }
            }

            if (tInfo._participants.Count() > 0)
            {
                if (!WaitHandle.WaitAll(tInfo._handles, 2000))
                    throw new TxException("Receiving Prepares failed");

                resetHandles(t.getTicket());
            }

            tInfo = _transactions[ticket];

            Console.WriteLine("###############");
            Console.WriteLine("Before Validate");
            Console.WriteLine("###############");
            Console.WriteLine("");

            if(validate(ticket))
            {
                foreach (String p in tInfo._participants)
                {
                    try
                    {
                        Console.WriteLine("###############");
                        Console.WriteLine("InsideForeachParticipant");
                        Console.WriteLine("###############");
                        Console.WriteLine("");
                        RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                        typeof(RemoteServerInterface), p);
                        RemoteAsyncCommitLocalChanges prep = new RemoteAsyncCommitLocalChanges(serv.commitLocalChanges);
                        prep.BeginInvoke(t.getTicket(), null, null);
                    }
                    catch(TxException e)
                    {
                        Console.WriteLine(e.Message);
                        throw e;
                    }
                }

                Console.WriteLine("###############");
                Console.WriteLine("Before HandleReception");
                Console.WriteLine("###############");
                Console.WriteLine("");
                if (tInfo._participants.Count() > 0)
                {
                    if (!WaitHandle.WaitAll(tInfo._handles, 2000))
                        throw new TxException("Receiving Commit failed");
                    resetHandles(t.getTicket());
                }

                tInfo = _transactions[ticket];

                //Commiting local changes
                foreach (PadIntValue item in tInfo._valuesToBeChanged)
                {
                    _padInts[(item.getId())].setValue(item.getValue());
                }


                Console.WriteLine("###############");
                Console.WriteLine("After Handle Reception");
                Console.WriteLine("###############");
                Console.WriteLine("");

                Console.WriteLine("###############");
                Console.WriteLine("Before Removing from uncom trans");
                Console.WriteLine("t Request Count: " + t.getRequests().Count());
                Console.WriteLine("t Ticket: " + t.getTicket());
                Console.WriteLine("t TimeStamp: " + t.getTimeStamp());
                Console.WriteLine("###############");
                Console.WriteLine("");
                if (_transactionsUncommited.Contains(t))
                {
                    Console.WriteLine("###############");
                    Console.WriteLine("Removing from uncom trans");
                    Console.WriteLine("###############");
                    Console.WriteLine("");
                    _transactionsUncommited.Remove(t);
                }
                tInfo._transactionsUncommited.Clear();
                _transactions[ticket] = tInfo;
                Console.WriteLine("###############");
                Console.WriteLine("After Removing from uncom trans");
                Console.WriteLine("###############");
                Console.WriteLine("");

                _prevStatus = STATE.ALIVE;
                _status = STATE.ALIVE;

                _transactions.Remove(tInfo._transaction.getTicket());

                //tInfo._participants.Clear();
                //tInfo._valuesToBeChanged.Clear();
                //tInfo._handles = null;
                //tInfo._validatehandles = null;
                //tInfo._unComTransHandles = null;
                //tInfo._partHandlers.Clear();
                //tInfo._Transaction = null;
                //tInfo._coordinatorURL = null;
                Console.WriteLine("Transaction Successfull.");
                return true;
            }
            else
            {
                Console.WriteLine("last ticket seen: " + _lastTicketTrans);
                Console.WriteLine("trans ticket " + t.getTicket());
                return abort(ticket);
            }
        }

        public bool abort(int ticket)
        { 
            TransactionInfo tInfo; 
        //abort a transaction
            if (_transactions.Count() > 0 && _transactions.ContainsKey(ticket))
            {
                tInfo = _transactions[ticket];

                //_valuesToBeChanged.Clear();     //Discutir esta parte, porque está-se a eliminar tudo mesmo o que pertence a outras transacções

                //if(t.getRequests() != null)
                //    t.getRequests().Clear();
                //_coordinatorURL = null;
                //_activeTransaction = null;
                //_partHandlers.Clear();
                //_handles = null;
                //_validatehandles = null;
                //_unComTransHandles = null;

                foreach (String p in tInfo._participants)
                {
                    try
                    {
                        RemoteServerInterface serv = (RemoteServerInterface)Activator.GetObject(
                        typeof(RemoteServerInterface), p);
                        RemoteAsyncAbort abort = new RemoteAsyncAbort(serv.abort);
                        abort.BeginInvoke(ticket, null, null);
                    }
                    catch (TxException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
                //_participants.Clear();
                _transactions.Remove(ticket);
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
            catch (Exception e)
           {
               Console.WriteLine("Error Creating Padint from master.");
                //throws either a new exception or the same returned from the master.
               //throw new TxException("Error Creating Padint from master.");
               throw e;
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
            foreach (int i in _tickets)
            {
                Console.WriteLine("Ticket list: " + i);
            }
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
