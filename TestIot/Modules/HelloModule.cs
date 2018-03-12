using Nancy;
using Nancy.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestIot.Business;
using TestIot.Model;
using TestIot.Utilities;

namespace TestIot.Modules
{
    public class HelloModule : NancyModule
    {
        public void DisplayModel(dynamic item)
        {
            foreach (KeyValuePair<string, object> kvp in item) // enumerating over it exposes the Properties and Values as a KeyValuePair
            {
                if(kvp.Value is ExpandoObject)
                {
                    DisplayModel(kvp.Value);
                } else
                {
                    Console.WriteLine("{0} = {1}", kvp.Key, kvp.Value);
                }
                
            }
        }

        public HelloModule()
        {
            Get["/"] = parameters => "Hello World";

            Post["/httpConnector"] = _ =>
            {
                dynamic model = JsonConvert.DeserializeObject<List<ExpandoObject>>(Request.Body.AsString());
                //var data = model[0].payload.data;
                //if (!((string)model[0].payload.format).Contains("pytdue")) return Nancy.HttpStatusCode.OK;

                foreach (var item in model)
                {
                    DisplayModel(item);
                }
                

                //string dataString = model[0].payload.data;

                var data = model[0].payload.data;
                var meterId = data.meterId;
                var billingFromDate = DateTime.ParseExact(data.billingFromDate, "dd/MM/yyyy", null);
                var billingToDate = DateTime.ParseExact(data.billingToDate, "dd/MM/yyyy", null);
                var lastMeterReading = double.Parse(data.lastMeterReading);
                var currentMeterReading = double.Parse(data.currentMeterReading);

                var invoiceManager = new InvoiceManager();
                invoiceManager.CreateInvoice(meterId, billingFromDate, billingToDate, lastMeterReading, currentMeterReading);
                return Nancy.HttpStatusCode.OK;
            };
        }
    }
}
