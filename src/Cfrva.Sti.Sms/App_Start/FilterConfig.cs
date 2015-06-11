using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Twilio.TwiML.Mvc;

namespace Cfrva.Sti.Sms
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new ValidateRequestAttribute(ConfigurationManager.AppSettings["TwilioAuthToken"]));
        }
    }
}
