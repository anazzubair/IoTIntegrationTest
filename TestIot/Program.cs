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
            var nancyHost = new Nancy.Hosting.Self.NancyHost(hostConfiguration, new Uri(@"http://localhost:9001"));
            nancyHost.Start();
            Console.WriteLine("Web server running...");

            Console.ReadLine();
            nancyHost.Stop();
        }
    }
}
