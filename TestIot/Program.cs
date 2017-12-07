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

namespace TestIOTInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            var hostConfiguration = new HostConfiguration();
            hostConfiguration.RewriteLocalhost = true;
            var nancyHost = new Nancy.Hosting.Self.NancyHost(hostConfiguration, new Uri(@"http://localhost:9000"));
            nancyHost.Start();
            Console.WriteLine("Web server running...");

            Console.ReadLine();
            nancyHost.Stop();
        }

        private static void GetServerDetails()
        {
            var reader = new AppSettingsReader();
            
            var request = WebRequest.Create(reader.GetValue("endpoint", typeof(string)).ToString());
            var username = reader.GetValue("endpoint", typeof(string)).ToString();
            var password = reader.GetValue("endpoint", typeof(string)).ToString();
            var strResponseValue = String.Empty;

            String authHeaer = System.Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(username + ":" + password));
            request.Headers.Add("Authorization:Basic " + authHeaer);

            HttpWebResponse response = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();


                //Proecess the resppnse stream... (could be JSON, XML or HTML etc..._

                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                    {
                        using (var streamReader = new StreamReader(responseStream))
                        {
                            strResponseValue = streamReader.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                strResponseValue = "{\"errorMessages\":[\"" + ex.Message.ToString() + "\"],\"errors\":{}}";
            }
            finally
            {
                Console.WriteLine(strResponseValue);
                if (response != null)
                {
                    ((IDisposable)response).Dispose();
                }
            }
        }
    }

    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get["/"] = parameters => "Hello World";

            Post["/httpConnector"] = _ =>
            {
                dynamic model = Context.ToDynamic();
                var data = model[0].payload.data;
                Console.WriteLine($"WaterLevel: {data.Water_Level}  No. of cups: ${data.No_of_cups_brewed}");
                return Nancy.HttpStatusCode.OK;
            };
        }
    }

    public static class DynamicModelBinder
    {
        public static dynamic ToDynamic(this NancyContext context)
        {
            var serializer = new JsonSerializer();
            using (var sr = new StreamReader(context.Request.Body))
            {
                using (var jsonTextReader = new JsonTextReader(sr))
                {
                    return serializer.Deserialize(jsonTextReader);
                }
            }
        }
    }
}
