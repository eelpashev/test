using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace TestOne
{
    class Program
    {
        static void Main(string[] args)
        {
            var scheduledInterval = 1000;
            var daysCount = 30;
            
            Int32.TryParse(ConfigurationManager.AppSettings["ScheduledInterval"], out scheduledInterval);
            Int32.TryParse(ConfigurationManager.AppSettings["DaysCount"], out daysCount);

            var svc = new DataService();

            svc.ScheduledInterval = scheduledInterval;
            svc.DaysCount = daysCount;

            if (!Environment.UserInteractive)
            {
                ServiceBase.Run(new[] { svc });
            }
            else
            {
                svc.Init();
                Console.WriteLine("Service is running");
                Console.ReadLine();
            }
        }
    }
}
