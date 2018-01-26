using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FourthCoffeeWeb.Startup))]
namespace FourthCoffeeWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
