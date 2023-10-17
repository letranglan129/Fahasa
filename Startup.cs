using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Fahasa.Startup))]
namespace Fahasa
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            
        }
    }
}
