using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestIot.Model
{
    class Payload
    {
        [JsonProperty("meterId")]
        public double MeterId { get; set; }
        [JsonProperty("billingFromDate")]
        public DateTime BillingFromDate { get; set; }
        [JsonProperty("billingToDate")]
        public DateTime BillingToDate { get; set; }
        [JsonProperty("lastMeterReading")]
        public double LastMeterReading { get; set; }
        [JsonProperty("currentMeterReading")]
        public double CurrentMeterReading { get; set; }
    }
}
