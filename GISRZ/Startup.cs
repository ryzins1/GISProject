using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GISRZ.Startup))]
namespace GISRZ
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
