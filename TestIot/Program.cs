using Nancy;
using Nancy.Extensions;
using Nancy.Hosting.Self;
using Nancy.ModelBinding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net.Security;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json.Linq;
using TestIot.Business;
using TestIot.Model;
using System.Threading;

namespace TestIOTInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            Poll();
        }

        static void Receive()
        {
            var hostConfiguration = new HostConfiguration();
            hostConfiguration.RewriteLocalhost = true;
            var nancyHost = new Nancy.Hosting.Self.NancyHost(hostConfiguration, new Uri(@"http://localhost:9001"));
            nancyHost.Start();
            Console.WriteLine("Web server running...");

            Console.ReadLine();
            nancyHost.Stop();
        }

        static void Poll()
        {
            //Trust all certificates
            System.Net.ServicePointManager.ServerCertificateValidationCallback =
                ((sender, certificate, chain, sslPolicyErrors) => true);

            // trust sender
            System.Net.ServicePointManager.ServerCertificateValidationCallback
                            = ((sender, cert, chain, errors) => cert.Subject.Contains("YourServerName"));

            // validate cert by calling a function
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ValidateRemoteCertificate);

            var appSettingsReader = new AppSettingsReader();
            var username = appSettingsReader.GetValue("username", typeof(string)).ToString();
            var password = appSettingsReader.GetValue("password", typeof(string)).ToString();
            var url = appSettingsReader.GetValue("serverBase", typeof(string)).ToString() + appSettingsReader.GetValue("integrtionAlerts", typeof(string)).ToString();
            
            

            String responseContent;

            using (var httpClient = new HttpClient())
            {
                
                String authHeader = System.Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
                //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Authorization", "Basic " + authHeader);
                httpClient.DefaultRequestHeaders.Add("Authorization", "Basic " + authHeader);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                while (true)
                {
                    var lastSentTime = new InvoiceManager().GetLastSentTime();
                    var filter = $"?since={lastSentTime}";
                    var filterUrl = url + filter;
                    Console.WriteLine($"Messages URL {filterUrl}");
                    HttpResponseMessage response = httpClient.GetAsync(filterUrl).Result;


                    using (var responseStream = response.Content.ReadAsStreamAsync().Result)
                    {
                        if (responseStream == null) return;
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            responseContent = streamReader.ReadToEnd();
                            var json = JObject.Parse(responseContent);
                            var jsonArray = json.GetValue("items");
                            //Console.WriteLine(json);
                            dynamic model = JsonConvert.DeserializeObject<List<ExpandoObject>>(jsonArray.ToString());
                            //var data = model[0].payload.data;
                            //if (!((string)model[0].payload.format).Contains("pytdue")) return Nancy.HttpStatusCode.OK;

                            foreach (var item in model)
                            {
                                //DisplayModel(item);
                                var data = item.payload.data;
                                var meterId = data.meterId;
                                var billingFromDate = DateTime.ParseExact(data.billingFromDate, "dd/MM/yyyy", null);
                                var billingToDate = DateTime.ParseExact(data.billingToDate, "dd/MM/yyyy", null);
                                var lastMeterReading = double.Parse(data.lastMeterReading);
                                var currentMeterReading = double.Parse(data.currentMeterReading);
                                var sentTime = (double)item.sentTime;
                                var id = item.id;

                                Console.WriteLine($"{id}  {sentTime}");
                            }


                            var items = from item in json["items"]
                                        let data = item["payload"]["data"]
                                        select new Payload
                                        {
                                            Id = item["id"].ToString(),
                                            SentTime = (Double)item["sentTime"],
                                            MeterId = data["meterId"].ToString(),
                                            BillingToDate = DateTime.ParseExact(data["billingToDate"].ToString(), "dd/MM/yyyy", null),
                                            BillingFromDate = DateTime.ParseExact(data["billingFromDate"].ToString(), "dd/MM/yyyy", null),
                                            CurrentMeterReading = (Double)data["currentMeterReading"],
                                            LastMeterReading = (Double)data["lastMeterReading"],
                                        };

                            new InvoiceManager().InsertMessages(items.ToList());
                        }
                    }
                    Thread.Sleep(2000);
                }
            }
        }

        private static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            bool result = false;
            if (cert.Subject.ToUpper().Contains("129.150.66.155"))
            {
                result = true;
            }

            return result;
        }

        public static void DisplayModel(dynamic item)
        {
            foreach (KeyValuePair<string, object> kvp in item) // enumerating over it exposes the Properties and Values as a KeyValuePair
            {
                if (kvp.Value is ExpandoObject)
                {
                    DisplayModel(kvp.Value);
                }
                else
                {
                    Console.WriteLine("{0} = {1}", kvp.Key, kvp.Value);
                }

            }
        }
    }
}
