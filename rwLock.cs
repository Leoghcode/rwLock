using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RWLock
{
    class RWTask
    {
        static int rcount = 0;
        static int wcount = 0;
        static int w_wcount = 0;
        static SortedSet<String> waitQueue=new SortedSet<String>();
        public static void printWaitThread()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("WaitQueue contains:");
            if (waitQueue.Count() != 0)
                foreach (String wthread in waitQueue)
                {
                    Console.WriteLine(wthread);
                }
            else
                Console.WriteLine("empty!");
            Console.ForegroundColor = ConsoleColor.Gray;

        }
        public void read()
        {
            lock (this)
            {
                if (w_wcount != 0)
                {
                    waitQueue.Add("ReadThread:" + Thread.CurrentThread.ManagedThreadId);
                    printWaitThread();
                    while (!Interlocked.Equals(w_wcount, 0))
                    {
                        Monitor.Pulse(this);
                        Monitor.Wait(this);
                    }
                }
                //2.no writer and no waiting writer
                rcount++;
                Console.WriteLine("{2}\tThread:{0} is Reading...\t\treader:{1}", Thread.CurrentThread.ManagedThreadId, rcount,DateTime.Now.ToString("yy-MM-dd hh:mm:ss.fffffff"));

                if (waitQueue.Contains("ReadThread:" + Thread.CurrentThread.ManagedThreadId))
                    waitQueue.Remove("ReadThread:" + Thread.CurrentThread.ManagedThreadId);
                printWaitThread();

                Monitor.Pulse(this);

                Monitor.Wait(this);

                Thread.Sleep(1000);
                Interlocked.Decrement(ref rcount);
                Console.WriteLine("{2}\tThread:{0} finished Reading...\treader:{1}", Thread.CurrentThread.ManagedThreadId,rcount, DateTime.Now.ToString("yy-MM-dd hh:mm:ss.fffffff"));
                Monitor.Pulse(this);
            }     
            

        }
        public void write()
        {
            lock (this)
            {
                if(rcount!=0)
                {
                    waitQueue.Add("WriteThread:" + Thread.CurrentThread.ManagedThreadId);
                    w_wcount++;
                    while (!Interlocked.Equals(rcount, 0))
                    {
                        Monitor.Pulse(this);
                        Monitor.Wait(this);
                    }
                }
                if (wcount != 0)
                {
                    w_wcount++;
                    if(!waitQueue.Contains("WriteThread:" + Thread.CurrentThread.ManagedThreadId))
                        waitQueue.Add("WriteThread:" + Thread.CurrentThread.ManagedThreadId);
                    while (!Interlocked.Equals(wcount, 0))
                    {
                        Monitor.Pulse(this);
                        Monitor.Wait(this);
                    }
                }
                wcount++;
                if (w_wcount != 0)
                    w_wcount--;
                Console.WriteLine("{2}\tThread:{0} is Writing...\t\twriter:{1}", Thread.CurrentThread.ManagedThreadId, wcount, DateTime.Now.ToString("yy-MM-dd hh:mm:ss.fffffff"));
                if (waitQueue.Contains("WriteThread:" + Thread.CurrentThread.ManagedThreadId))
                    waitQueue.Remove("WriteThread:" + Thread.CurrentThread.ManagedThreadId);
                printWaitThread();
                Thread.Sleep(1000);
                Interlocked.Decrement(ref wcount);
                Console.WriteLine("{2}\tThread:{0} finished Writing...\twriter:{1}", Thread.CurrentThread.ManagedThreadId, wcount, DateTime.Now.ToString("yy-MM-dd hh:mm:ss.fffffff"));
                Monitor.Pulse(this);
            }
        }
        public void action(Object type)
        {
            if (type.ToString().Equals("read"))
                read();
            else if (type.ToString().Equals("write"))
                write();
        }
        public void WriteFirstTest(ref RWTask rw)
        {
           
        }
        public static void Main(String[] args)
        {
            Console.WriteLine("Please input the thread number:\t");
            int Tnumber = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine(Tnumber);

            RWTask t = new RWTask();
            string[] s = new string[]{ "write","read", "write", "read","write" };

            Thread[] tlist = new Thread[Tnumber];
            //Console.WriteLine("{0}", s[0]);
            Random ro = new Random();

            int rwtype = 0;
            for (int i = 0; i < Tnumber; i++)
            {
                rwtype = ro.Next(5);
                Thread t1 = new Thread(t.action);
                Console.WriteLine("new Thread:{0},type:{1}", t1.ManagedThreadId, rwtype);
                tlist[i] = t1;
                t1.Start(s[rwtype]);
            }
            for(int j=0;j<Tnumber;j++)
            {
                tlist[j].Join();
            }
            Console.WriteLine("End");
            Console.ReadKey();
        }
    }
}
