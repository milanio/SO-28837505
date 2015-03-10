using System;
using System.ServiceModel;

namespace ConsoleServer
{
   

    
    class Program
    {
        static void Main(string[] args)
        {
           
            using (var host = new ServiceHost(typeof(SchedulerService)))
            {
                //Based on MSDN - config in app.config

                // Open the ServiceHost to start listening for messages. Since
                // no endpoints are explicitly configured, the runtime will create
                // one endpoint per base address for each service contract implemented
                // by the service.
                host.Open();

                Console.WriteLine("The service is ready at {0}", string.Join(",", host.BaseAddresses));
                Console.WriteLine("Press <Enter> to stop the service.");
                Console.ReadLine();

                // Close the ServiceHost.
                host.Close();
            }

            ;
        }
    }
}
