using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using System.Reflection;
using Autofac.Integration.Web;
using Autofac.Integration.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;

namespace Cedts_Test
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication, IContainerProviderAccessor
    {
        static IContainerProvider _containerProvider;
        public IContainerProvider ContainerProvider
        {
            get { return _containerProvider; }
        }


        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Account", action = "LogOn", id = UrlParameter.Optional } // 参数默认值
            );

        }


        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            if (HttpContext.Current.User != null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        //取得窗口认证当前这位用户的身份
                        FormsIdentity id = (FormsIdentity)HttpContext.Current.User.Identity;
                        //取得FormsAuthenticationTicket对象
                        FormsAuthenticationTicket ticket = id.Ticket;
                        //取得UserData字段数据（这里我们存储的是的角色）
                        string userData = ticket.UserData;
                        //如果有多个角色，可以用逗号分隔
                        string[] roles = userData.Split(',');
                        //赋值该用户新的身份（含角色信息）
                        HttpContext.Current.User = new GenericPrincipal(id, roles);
                    }
                }
            }
        }


        private void SetupResolveRules(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .Where(t => t.Name.EndsWith("Repository"))
                .AsImplementedInterfaces();
        }


        protected void Application_Start()
        {

            //加上此行可隐藏MVC的版本信息
            MvcHandler.DisableMvcResponseHeader = true;

            var builder = new ContainerBuilder();

            SetupResolveRules(builder);

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            _containerProvider = new ContainerProvider(builder.Build());

            ControllerBuilder.Current.SetControllerFactory(
                new AutofacControllerFactory(_containerProvider));

            AreaRegistration.RegisterAllAreas();

            RegisterRoutes(RouteTable.Routes);
        }
    }
}