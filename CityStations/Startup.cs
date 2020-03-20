using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CityStations.Startup))]
namespace CityStations
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
