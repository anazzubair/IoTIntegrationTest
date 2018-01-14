using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestIot.Utilities;

namespace TestIot.Modules
{
    public class HelloModule : NancyModule
    {
        public HelloModule()
        {
            Get["/"] = parameters => "Hello World";

            Post["/httpConnector"] = _ =>
            {
                dynamic model = Context.ToDynamic();
                var data = model[0].payload.data;
                //if (((string)model[0].payload.format).Contains("coffee"))
                //{
                //    Console.WriteLine($"WaterLevel: {data.Water_Level}  No. of cups: ${data.No_of_cups_brewed}");
                //}
                //else
                //{
                //    if (((string)model[0].payload.format).Contains("ALERT"))
                //        Console.WriteLine(model);
                //}
                Console.WriteLine(model);
                return Nancy.HttpStatusCode.OK;
            };
        }
    }
}
