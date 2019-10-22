using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TRS.Startup))]
namespace TRS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
