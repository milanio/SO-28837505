using System.ServiceModel;

namespace ConsoleServer
{
    [ServiceContract(CallbackContract = typeof(ISchedulerServiceCallback))]
    public interface ISchedulerService
    {
        [OperationContract]
        void Stop();

        [OperationContract]
        void SubscribeStatusUpdate();
    }

    public interface ISchedulerServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void StatusUpdate(SchedulerStatus status);
    }

    public enum SchedulerStatus
    {
        Stopped,
        Running
    }

}
