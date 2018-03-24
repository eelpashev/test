using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TestOne.Model;

namespace TestOne
{
    partial class DataService : ServiceBase
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public DataService()
        {
            InitializeComponent();
        }

        Timer workingTimer = new Timer();

        protected override void OnStart(string[] args)
        {
            Init();
        }

        public double ScheduledInterval
        {
            get
            {
                return workingTimer.Interval;
            }
            set
            {
                workingTimer.Interval = value;
            }
        }

        public int DaysCount
        {
            get;
            set;
        }

        internal void Init()
        {
            workingTimer.Elapsed += WorkingTimer_Elapsed;
            StartWorkingThread();
        }

        object lockObj = new object();

        private void WorkingTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (lockObj)
            {
                var webClient = new WebClient();
                string json = webClient.DownloadString("http://api.eia.gov/series/?api_key=ec92aacd6947350dcb894062a4ad2d08&series_id=PET.EMD_EPD2D_PTE_NUS_DPG.W");

                var data = JsonConvert.DeserializeObject<JObject>(json);
                var firstSeriesData = data["series"].FirstOrDefault();

                if (firstSeriesData != null)
                {
                    foreach (var s in firstSeriesData["data"])
                    {
                        string dtStr = s[0].Value<string>();
                        DateTime dt;

                        if (DateTime.TryParseExact(dtStr, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                        {
                            double price = s[1].Value<double>();

                            if (DateTime.Today < dt.AddDays(DaysCount))
                            {
                                if (!db.FuelPrices.Any(x => x.Date == dt))
                                {
                                    db.FuelPrices.Add(new FuelPrice() { Date = dt, Price = price });
                                }
                            }
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        void StartWorkingThread()
        {
            workingTimer.Start();
        }

        void StopWorkingThread()
        {
            workingTimer.Stop();
        }

        protected override void OnStop()
        {
            StopWorkingThread();
        }
    }
}
