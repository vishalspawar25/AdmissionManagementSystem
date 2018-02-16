using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdmissionManagementSystem.Startup))]
namespace AdmissionManagementSystem
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
