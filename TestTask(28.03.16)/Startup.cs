using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestTask_28._03._16_.Startup))]
namespace TestTask_28._03._16_
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
