using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Cfrva.Sti.Sms.Startup))]
namespace Cfrva.Sti.Sms
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
