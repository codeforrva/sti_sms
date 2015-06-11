using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Cfrva.Sti.Sms.Models;
using Twilio;
using Twilio.TwiML.Mvc;

namespace Cfrva.Sti.Sms.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(string from, string body)
        {
            if (string.IsNullOrEmpty(from))
            {
                return SendJsonOk();
            }

            if (string.IsNullOrEmpty(body))
            {
                SendErrorSms(from);
                return SendJsonOk();
            }

            int validatedZipCode;
            if (!int.TryParse(body, out validatedZipCode))
            {
                SendErrorSms(from);
                return SendJsonOk();
            }

            if (body.Length != 5)
            {
                SendErrorSms(from);
                return SendJsonOk();
            }

            var messages = GetMessagesForZipCode(validatedZipCode);

            if (messages == null || !messages.Any())
            {
                SendNotFoundMessage(from);
                return SendJsonOk();
            }

            SendMessages(messages, from);

            return SendJsonOk();
        }

        private JsonResult SendJsonOk()
        {
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        private void SendNotFoundMessage(string smsNumber)
        {
            SendMessage(smsNumber, "No clinics were found in your area");
        }

        private void SendMessages(IList<string> messages, string smsNumber)
        {
            foreach (var message in messages)
            {
                SendMessage(smsNumber, message);
            }
        }
        
        private IList<string> GetMessagesForZipCode(int validatedZipCode)
        {
            var messages = new List<string>();

            try
            {
                CDCServices.GetWidgetDataResponse response;

                using (var client = new CDCServices.NPINDataWebservicesSoapClient("NPINDataWebservicesSoap12"))
                {
                    var request = new CDCServices.GetWidgetDataRequest(validatedZipCode.ToString(), "20",
                        "https://npin.cdc.gov/HIVWidgets/hivwidget.aspx", "HIVTest_Graphic");
                    response = client.GetWidgetData(request);
                }

                var organizationsNode = response.GetWidgetDataResult;
                var xmlDoc = XDocument.Parse(organizationsNode.OuterXml);

                var clinics = from org in xmlDoc.Descendants("Organization")
                                    select new Clinic
                                    {
                                        Name = org.Element("OrgName").Value,
                                        Address = string.Format("{0} {1}", org.Element("Street1").Value, (org.Element("Street2").Value == "NULL") ? "" : org.Element("Street2").Value).Trim(),
                                        City = org.Element("City").Value,
                                        State = org.Element("StateName").Value,
                                        Zip = org.Element("ZipCode").Value,
                                        Phone = org.Element("MainPhone").Value,
                                        Categories = org.Element("CategoryAbbrev").Value,
                                        Distance = org.Element("Distance").Value
                                    };

                //taking the top 3 results from the cdc site (using their order)
                var topThreeClinics = clinics
                    .Take(3)
                    .ToList();

                //build message of max 160 characters - prioritize name and phone number
                foreach (var org in topThreeClinics)
                {
                    var message = string.Format("{0}\n{1}\n{2}\n{3}, {4} {5}", 
                        org.Name,org.Phone, org.Address, org.City, org.State, org.Zip);
                    if (message.Length > 160)
                    {
                        message = message.Substring(0,160);
                    }
                    messages.Add(message);
                }

                return messages;
            }
            catch (Exception exc)
            {
                //TODO: add logging
                return null;
            }
        }

        private void SendErrorSms(string smsNumber)
        {
            SendMessage(smsNumber, "Please make sure you are using a valid 5 digit zip code and try again");
        }

        private void SendMessage(string smsNumber, string message)
        {
            var client = new TwilioRestClient(ConfigurationManager.AppSettings["TwilioAccountSid"], ConfigurationManager.AppSettings["TwilioAuthToken"]);
            client.SendMessage("+1" + ConfigurationManager.AppSettings["TwilioSystemSmsNumber"], smsNumber, message);
        }
    }
}