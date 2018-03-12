using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestIot.Data;

namespace TestIot.Business
{
    class InvoiceManager
    {

        public void CreateInvoice(string meterId, DateTime billingFromDate, DateTime billingToDate,
                                  double lastMeterReading, double currentMeterReading
                                 )
        {
            using (var db = new ESuiteEntities())
            {
                var lastInvoiceEntry = db.IotInvoices.OrderByDescending(i => i.INTERFACE_LINE_ID)
                                                     .FirstOrDefault();
                var invoice = new IotInvoice
                {
                    INTERFACE_LINE_ID = lastInvoiceEntry == null ? 1 : lastInvoiceEntry.INTERFACE_LINE_ID + 1,
                    BATCH_SOURCE_NAME = "ELECTRICITY Invoicing Source",
                    SET_OF_BOOKS_ID = 124,
                    LINE_TYPE = "LINE",
                    DESCRIPTION = "Frais d'electricite",
                    CURRENCY_CODE = "EUR",
                    AMOUNT = 0,
                    CUST_TRX_TYPE_ID = 6436,
                    CUST_TRX_TYPE_NAME = "Facture Electricite",
                    TERM_NAME = "IMMEDIATE",
                    TERM_ID = 5,
                    ORIG_SYSTEM_BILL_CUSTOMER_ID = 3347,
                    METER_ID = meterId,
                    ORIG_SYSTEM_BILL_ADDRESS_REF = null,
                    ORIG_SYSTEM_BILL_ADDRESS_ID = null,
                    CONVERSION_TYPE = "User",
                    CONVERSION_RATE = 1,
                    CONVERSION_DATE = null,
                    CUSTOMER_TRX_ID = null,
                    TRX_DATE = DateTime.Now,
                    GL_DATE = DateTime.Now,
                    //BILLING_FROM = lastInvoiceEntry != null ? lastInvoiceEntry.BILLING_FROM.Value.AddMonths(1) : DateTime.Now,
                    //BILLING_TO = lastInvoiceEntry != null ? lastInvoiceEntry.BILLING_FROM.Value.AddMonths(2) : DateTime.Now.AddMonths(1),
                    //LAST_METER_READING = lastInvoiceEntry == null ? 1000 : lastInvoiceEntry.CURRENT_METER_READING,
                    //CURRENT_METER_READING = lastInvoiceEntry == null ? 1200 : lastInvoiceEntry.CURRENT_METER_READING + 200,
                    BILLING_FROM = billingFromDate,
                    BILLING_TO = billingToDate,
                    LAST_METER_READING = (decimal)lastMeterReading,
                    CURRENT_METER_READING = (decimal)currentMeterReading,
                    DOCUMENT_NUMBER = null,
                    TRX_NUMBER = null,
                    LINE_NUMBER = null,
                    UNITS_CONSUMED = (decimal)currentMeterReading - (decimal)lastMeterReading,
                    UNIT_SELLING_PRICE = 10,
                    UNIT_STANDARD_PRICE = null,
                    TAX_CODE = null,
                    UOM_CODE = "KWH",
                    LEGAL_ENTITY_ID = "888"
                };

                db.IotInvoices.Add(invoice);
                db.SaveChanges();
            }
        }
    }
}
