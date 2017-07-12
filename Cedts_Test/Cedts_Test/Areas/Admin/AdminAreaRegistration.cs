using System.Web.Mvc;
using Autofac.Integration.Web;
using Autofac;
using System.Reflection;

namespace Cedts_Test.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {     

        private void SetupResolveRules(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();
        }

        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
