using Locus.Core.Context;
using Locus.Core.DTO;
using Locus.Core.Models;
using Locus.Core.Repositories;
using Locus.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace LocustRunnerService
{
    public partial class Service : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)  

        public Service()
        {
            InitializeComponent();
        }



        protected override void OnStart(string[] args)
        {

            WriteToFile("Service is started at " + DateTime.UtcNow);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 7000; //number in milisecinds  
            timer.Enabled = true;
        }


        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.UtcNow);
        }

        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            try
            {
                LocustDBContext context = new LocustDBContext();

                List<Runner> runners = context.Runners.Where(r => r.Status == true && r.IsConnected == true).ToList();
                foreach (var runner in runners)
                {
                    TimeSpan t = DateTime.UtcNow.Subtract(runner.Last_Connection_Date);
                    double time = t.Seconds;
                    if (time > 7)
                    {
                        runner.IsConnected = false;
                        context.Entry(runner).State = System.Data.Entity.EntityState.Modified;
                        context.SaveChanges();
                        WriteToFile("Runner Disconnected: Identifier-" + runner.Identifier + " IP-" + runner.IPAddress + " MAC-" + runner.MAC + " Time:" + DateTime.UtcNow);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteToFile("Error" + ex);
            }
        }

        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}
