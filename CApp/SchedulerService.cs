using System;
using System.ServiceModel;
using System.Threading;

namespace ConsoleServer
{
    [CallbackBehavior(UseSynchronizationContext = false, ConcurrencyMode = ConcurrencyMode.Multiple)] // Tried Reentrant as well.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)] // Single due to a timer in the service that must keep time across calls.
    public class SchedulerService : ISchedulerService
    {
        public SchedulerService()
        {
            

        }

         static string GetCurrentTime()
        {
           return DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt");                        
        }

        private static Action<SchedulerStatus> statusUpdate = delegate { };

        public void Stop()
        {
            Console.WriteLine("Service: Thread {0} - Stop called at {1} ", Thread.CurrentThread.ManagedThreadId, GetCurrentTime());
            Status = SchedulerStatus.Stopped;
            statusUpdate(Status);
            Console.WriteLine("Service: Thread {0} - Stop Finished {1}", Thread.CurrentThread.ManagedThreadId, GetCurrentTime());
        }

        private SchedulerStatus Status { get; set; }

        public void SubscribeStatusUpdate()
        {
            Console.WriteLine("Service: Thread {0} - SubscribeStatusUpdate called at {1}", Thread.CurrentThread.ManagedThreadId, GetCurrentTime());

            var sub = OperationContext.Current.GetCallbackChannel<ISchedulerServiceCallback>();

            EventHandler channelClosed =null;
            channelClosed=new EventHandler(delegate
            {
                Console.WriteLine("Service: Thread {0} - Channel Closed called at {1}", Thread.CurrentThread.ManagedThreadId, GetCurrentTime());
                statusUpdate -= sub.StatusUpdate;
                
            });
            OperationContext.Current.Channel.Closed += channelClosed;
            OperationContext.Current.Channel.Faulted += channelClosed;
            statusUpdate += sub.StatusUpdate;
            Console.WriteLine("Service: Thread {0} - SubscribeStatusUpdate finished at {1}", Thread.CurrentThread.ManagedThreadId, GetCurrentTime());
        }

        
    }

}
