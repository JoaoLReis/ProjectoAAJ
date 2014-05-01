using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PADI_DSTM;

namespace TestApp
{
    class Program
    {
        static void test1(string[] args)
        {
            bool res;

            PadiDstm.Init();

            res = PadiDstm.TxBegin();
            PadInt pi_a = PadiDstm.CreatePadInt(0);
            PadInt pi_b = PadiDstm.CreatePadInt(1);
            res = PadiDstm.TxCommit();

            res = PadiDstm.TxBegin();
            pi_a = PadiDstm.AccessPadInt(0);
            pi_b = PadiDstm.AccessPadInt(1);
            pi_a.Write(36);
            pi_b.Write(37);
            Console.WriteLine("a = " + pi_a.Read());
            Console.WriteLine("b = " + pi_b.Read());
            PadiDstm.Status();
            // The following 3 lines assume we have 2 servers: one at port 2001 and another at port 2002
            res = PadiDstm.Freeze("tcp://localhost:2001/Server");
            res = PadiDstm.Recover("tcp://localhost:2001/Server");
            res = PadiDstm.Fail("tcp://localhost:2002/Server");
            res = PadiDstm.TxCommit();
        }

        static void test2(string[] args)
        {
            bool res = false;
            PadInt pi_a, pi_b;
            PadiDstm.Init();

            // Create 2 PadInts
            if ((args.Length > 0) && (args[0].Equals("C")))
            {
                try
                {
                    res = PadiDstm.TxBegin();
                    pi_a = PadiDstm.CreatePadInt(1);
                    pi_b = PadiDstm.CreatePadInt(2000000000);
                    Console.WriteLine("####################################################################");
                    Console.WriteLine("BEFORE create commit. Press enter for commit.");
                    Console.WriteLine("####################################################################");
                    PadiDstm.Status();
                    Console.ReadLine();
                    res = PadiDstm.TxCommit();
                    Console.WriteLine("####################################################################");
                    Console.WriteLine("AFTER create commit returned " + res + " . Press enter for next transaction.");
                    Console.WriteLine("####################################################################");
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    Console.WriteLine("####################################################################");
                    Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for next transaction.");
                    Console.WriteLine("####################################################################");
                    Console.ReadLine();
                    PadiDstm.TxAbort();
                }
            }

            try
            {
                res = PadiDstm.TxBegin();
                pi_a = PadiDstm.AccessPadInt(1);
                pi_b = PadiDstm.AccessPadInt(2000000000);
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status after AccessPadint");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                if ((args.Length > 0) && ((args[0].Equals("C")) || (args[0].Equals("A"))))
                {
                    pi_a.Write(11);
                    pi_b.Write(12);
                }
                else
                {
                    pi_a.Write(21);
                    pi_b.Write(22);
                }
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status after write. Press enter for read.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                Console.WriteLine("1 = " + pi_a.Read());
                Console.WriteLine("2000000000 = " + pi_b.Read());
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status after read. Press enter for commit.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                Console.ReadLine();
                res = PadiDstm.TxCommit();
                Console.WriteLine("####################################################################");
                Console.WriteLine("Status after commit. commit = " + res + "Press enter for verification transaction.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER r/w ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                PadiDstm.TxAbort();
            }

            try
            {
                res = PadiDstm.TxBegin();
                PadInt pi_c = PadiDstm.AccessPadInt(1);
                PadInt pi_d = PadiDstm.AccessPadInt(2000000000);
                Console.WriteLine("####################################################################");
                Console.WriteLine("1 = " + pi_c.Read());
                Console.WriteLine("2000000000 = " + pi_d.Read());
                Console.WriteLine("Status after verification read. Press enter for commit and exit.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                Console.ReadLine();
                res = PadiDstm.TxCommit();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER verification ABORT. Commit returned " + res + " . Press enter for abort and exit.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                PadiDstm.TxAbort();
            }
        }

        static void test3(string[] args)
        {
            bool res = false; int aborted = 0, committed = 0;

            PadiDstm.Init();
            try
            {
                if ((args.Length > 0) && (args[0].Equals("C")))
                {
                    res = PadiDstm.TxBegin();
                    PadInt pi_a = PadiDstm.CreatePadInt(2);
                    PadInt pi_b = PadiDstm.CreatePadInt(2000000001);
                    PadInt pi_c = PadiDstm.CreatePadInt(1000000000);
                    pi_a.Write(0);
                    pi_b.Write(0);
                    res = PadiDstm.TxCommit();
                }
                Console.WriteLine("####################################################################");
                Console.WriteLine("Finished creating PadInts. Press enter for 300 R/W transaction cycle.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                PadiDstm.TxAbort();
            }
            for (int i = 0; i < 300; i++)
            {
                try
                {
                    res = PadiDstm.TxBegin();
                    PadInt pi_d = PadiDstm.AccessPadInt(2);
                    PadInt pi_e = PadiDstm.AccessPadInt(2000000001);
                    PadInt pi_f = PadiDstm.AccessPadInt(1000000000);
                    int d = pi_d.Read();
                    d++;
                    pi_d.Write(d);
                    int e = pi_e.Read();
                    e++;
                    pi_e.Write(e);
                    int f = pi_f.Read();
                    f++;
                    pi_f.Write(f);
                    Console.Write(".");
                    res = PadiDstm.TxCommit();
                    if (res) { committed++; Console.Write("."); }
                    else
                    {
                        aborted++;
                        Console.WriteLine("$$$$$$$$$$$$$$ ABORT $$$$$$$$$$$$$$$$$");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception: " + e.Message);
                    Console.WriteLine("####################################################################");
                    Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
                    Console.WriteLine("####################################################################");
                    Console.ReadLine();
                    PadiDstm.TxAbort();
                    aborted++;
                }

            }
            Console.WriteLine("####################################################################");
            Console.WriteLine("committed = " + committed + " ; aborted = " + aborted);
            Console.WriteLine("Status after cycle. Press enter for verification transaction.");
            Console.WriteLine("####################################################################");
            PadiDstm.Status();
            Console.ReadLine();

            try
            {
                res = PadiDstm.TxBegin();
                PadInt pi_g = PadiDstm.AccessPadInt(2);
                PadInt pi_h = PadiDstm.AccessPadInt(2000000001);
                PadInt pi_j = PadiDstm.AccessPadInt(1000000000);
                int g = pi_g.Read();
                int h = pi_h.Read();
                int j = pi_j.Read();
                res = PadiDstm.TxCommit();
                Console.WriteLine("####################################################################");
                Console.WriteLine("2 = " + g);
                Console.WriteLine("2000000001 = " + h);
                Console.WriteLine("1000000000 = " + j);
                Console.WriteLine("Status post verification transaction. Press enter for exit.");
                Console.WriteLine("####################################################################");
                PadiDstm.Status();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
                Console.WriteLine("####################################################################");
                Console.WriteLine("AFTER create ABORT. Commit returned " + res + " . Press enter for abort and next transaction.");
                Console.WriteLine("####################################################################");
                Console.ReadLine();
                PadiDstm.TxAbort();
            }
        }

        static void Main(string[] args)
        {
            //test1(args);
            //test2(args);
            //test3(args);
            PadiDstm.Init();
            PadiDstm.Status();
        }
    }
}
