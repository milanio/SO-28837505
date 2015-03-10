using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ConsoleClient.ServiceReference1;

namespace ConsoleClient
{
    class Program
    {

        class SchedulerServiceCallback : ISchedulerServiceCallback
        {
            public void StatusUpdate(SchedulerStatus status)
            {
                Thread.Sleep(2000);
                Console.WriteLine("Thread {0} received callback at {2}- Status {1}", Thread.CurrentThread.ManagedThreadId, status, GetCurrentTime());
            }
        }
        [STAThread]
        static void Main(string[] args)
        {

            Console.WriteLine("Client app. Press any key to continue...");
            Console.ReadKey();
            var callback = new SchedulerServiceCallback();
            var ssc = new SchedulerServiceClient(new InstanceContext(callback));

            Console.WriteLine("Thread {0} - Calling SubscribeStatusUpdate at {1}", Thread.CurrentThread.ManagedThreadId, GetCurrentTime());

            ssc.SubscribeStatusUpdate();
            Console.WriteLine("Thread {0} - Calling Stop at {1}", Thread.CurrentThread.ManagedThreadId, GetCurrentTime());
            ssc.Stop();

            Console.WriteLine("Thread {0} - Stop processing has finished at {1}. Press enter to exit", Thread.CurrentThread.ManagedThreadId, GetCurrentTime());

            Console.ReadLine();
            ssc.Close();

        }

        static string GetCurrentTime()
        {
           return DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");                        
        }
    }
}
