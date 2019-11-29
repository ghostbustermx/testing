using System;
using System.IO;
using System.ServiceProcess;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net;
using System.Net.Configuration;
using LocustSwarm.DTO;

namespace LocustSwarm
{
    public partial class LocustSwarmService : ServiceBase
    {

        private Timer _timer = null;
        static BackupDTO generatedBackup = new BackupDTO();
        static HttpClient client = new HttpClient();
        static HttpClient client2 = new HttpClient();
        static string baseAddressValue = ConfigurationManager.AppSettings["BaseAddress"];
        static string accessToken = ConfigurationManager.AppSettings["AccessToken"];
        static int scheduledTime = Convert.ToInt32(ConfigurationManager.AppSettings["ScheduledTime"]);
        static int interval = Convert.ToInt32(ConfigurationManager.AppSettings["Interval"]);
        static SmtpSection smtpSection = ConfigurationManager.GetSection("system.net/mailSettings/smtp") as SmtpSection;
        static DateTime localCreationDate;
        static bool isGenerated = false;
        static bool isDownloaded = false;

        public LocustSwarmService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            WriteToFile(DateTime.Now + "    INFO   : Service is started.");
            // Pass in the time you want to start and the interval
            //StartTimer(new TimeSpan(scheduledTime, 0, 0), new TimeSpan(interval, 0, 0));
            RunAsync().GetAwaiter().GetResult();
        }

        protected override void OnStop()
        {
            WriteToFile(DateTime.Now + "    INFO   : Service is stopped.");
        }

        // Timer method
        public void StartTimer(TimeSpan scheduledRunTime, TimeSpan timeBetweenEachRun)
        {
            // Initialize timer
            double current = DateTime.Now.TimeOfDay.TotalMilliseconds;
            double scheduledTime = scheduledRunTime.TotalMilliseconds;
            double intervalPeriod = timeBetweenEachRun.TotalMilliseconds;
            // calculates the first execution of the method, either its today at the scheduled time or tomorrow (if scheduled time has already occurred today)
            double firstExecution = current > scheduledTime ? intervalPeriod - (current - scheduledTime) : scheduledTime - current;

            // create callback - this is the method that is called on every interval
            TimerCallback callback = new TimerCallback(TimerCallback);

            // create timer
            _timer = new Timer(callback, null, Convert.ToInt32(firstExecution), Convert.ToInt32(intervalPeriod));
        }

        // Start async method
        public void TimerCallback(Object e)
        {
            RunAsync().GetAwaiter().GetResult();
        }

        static async Task RunAsync()
        {
            client.BaseAddress = new Uri(baseAddressValue);
            client.DefaultRequestHeaders.Accept.Clear();

            // Add an Accept header for JSON format
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Token", accessToken);

            try
            {
                var url = await GenerateBackupAsync();
                var urlDownload = await DownloadBackup();
                createMail(generatedBackup.Name, localCreationDate);
            }
            catch (Exception e)
            {
                WriteToFile(e.Message);
            }
        }

        // Generate Backup method
        static async Task<Uri> GenerateBackupAsync()
        {
            HttpResponseMessage response = await client.PostAsJsonAsync("/api/Locust/GenerateBackup", generatedBackup);
            if (response.IsSuccessStatusCode)
            {
                var result = response.Content.ReadAsStringAsync().Result;
                generatedBackup = Newtonsoft.Json.JsonConvert.DeserializeObject<BackupDTO>(result);

                DateTime utcCreationDate = DateTime.SpecifyKind(generatedBackup.Creation_Date, DateTimeKind.Utc);
                localCreationDate = utcCreationDate.ToLocalTime();
                // Create success log
                WriteToFile(localCreationDate + "    EVENT  : Backup " + generatedBackup.Name + " successfully created");

                isGenerated = true;
            }
            else
            {
                DateTime errorDate = DateTime.Now;
                WriteToFile(errorDate + "    WARNING: " + response.StatusCode.ToString());

                // Send error email
                isGenerated = false;
                var responseMessage = response.Content.ToString();
                createMail(responseMessage, errorDate);

                return response.Headers.Location;
            }

            // return URI of the created resource.
            return response.Headers.Location;
        }

        // Download Backups
        static async Task<Uri> DownloadBackup()
        {
            String generatedbackupName = generatedBackup.Name;
            client2.BaseAddress = new Uri(baseAddressValue);
            client2.DefaultRequestHeaders.Accept.Clear();

            // Add an Accept header for JSON format
            client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client2.DefaultRequestHeaders.Add("Token", accessToken);
            client2.DefaultRequestHeaders.Add("Name", generatedbackupName);

            HttpResponseMessage backupResponse = await client.GetAsync("/api/Locust/DownloadBackup?Name=" + generatedbackupName);

            if (backupResponse.IsSuccessStatusCode)
            {
                var path = "C://Backups//" + generatedbackupName;
                var byteArray = backupResponse.Content.ReadAsByteArrayAsync().Result;
                File.WriteAllBytes(path, byteArray);
                return backupResponse.Headers.Location;
            }
            else
            {
                DateTime errorDate = DateTime.Now;
                // Send error email
                isDownloaded = false;
                var responseMessage = backupResponse.Content.ToString();
                createMail(responseMessage, errorDate);

                WriteToFile(DateTime.Now + "    WARNING: " + backupResponse.StatusCode.ToString());
                return backupResponse.Headers.Location;
            }
        }

        // Send Email method
        public static void sendMail(string subject, string body)
        {
            string to = ConfigurationManager.AppSettings["To"];
            string from = smtpSection.From;

            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;

            var host = smtpSection.Network.Host;
            var port = smtpSection.Network.Port;

            SmtpClient smtpclient = new SmtpClient(host, port);
            smtpclient.EnableSsl = smtpSection.Network.EnableSsl;
            smtpclient.Timeout = 10000;
            smtpclient.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtpclient.UseDefaultCredentials = smtpSection.Network.DefaultCredentials;
            smtpclient.Credentials = new NetworkCredential(smtpSection.Network.UserName, smtpSection.Network.Password);


            try
            {
                smtpclient.Send(message);
                message.Dispose();
                WriteToFile(DateTime.Now + "    INFO   : Notification email sent to " + from);
            }
            catch (Exception ex)
            {
                WriteToFile(ex.Message);
            }

        }

        // Create emails content method 
        public static void createMail(String name, DateTime date)
        {
            if (isGenerated)
            {
                var subject = "Locust Swarm - System Backup " + date;
                var generatedBody = @"Backup " + name + " created at " + date;
                var downloadedBody = "";
                if (isDownloaded)
                {
                    downloadedBody = @"Backup is stored in this URL: " + name;
                }
                var body = generatedBody + " " + downloadedBody;
                sendMail(subject, body);
            }
            else
            {
                var subject = "Locust Swarm - System Backup Failed";
                var body = @"Backup creation failed at " + date + " providing the following information: " + name;
                sendMail(subject, body);
            }
        }

        // Write to Log file method
        public static void WriteToFile(string Message)
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
