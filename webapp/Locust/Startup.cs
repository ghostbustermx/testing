
using Locus.Core.Repositories;
using Locus.Core.Services;
using Autofac;
using Autofac.Integration.Mvc;
using Autofac.Integration.WebApi;
using Microsoft.Owin;
using Owin;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;

[assembly: OwinStartupAttribute(typeof(Locust.Startup))]
namespace Locust
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureDependencyResolver(app);
        }

        private void ConfigureDependencyResolver(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterControllers(assembly);

            // Register API controllers using assembly scanning.
            builder.RegisterApiControllers(assembly);
            builder.RegisterFilterProvider();
            RegisterDependencies(builder, app);
            var container = builder.Build();
            var resolver = new AutofacDependencyResolver(container);
            DependencyResolver.SetResolver(resolver);

            // This override is needed because Web API is not using DependencyResolver to build controllers
            var apiResolver = new AutofacWebApiDependencyResolver(container);
            GlobalConfiguration.Configuration.DependencyResolver = apiResolver;
            app.UseAutofacMvc();
            app.UseAutofacMiddleware(container);
            app.UseAutofacWebApi(GlobalConfiguration.Configuration);
            
            //   app.UseWebApi(GlobalConfiguration.Configuration);
        }

        private void RegisterDependencies(ContainerBuilder builder, IAppBuilder app)
        {
            var assembly = Assembly.GetExecutingAssembly();
            //Type of object
            var coreAssembly = Assembly.GetAssembly(typeof(IProjectService));
            //Identify all the classes that ends with "Service"
            builder.RegisterAssemblyTypes(coreAssembly).Where(t => t.Name.EndsWith("Service")).AsImplementedInterfaces();

            coreAssembly = Assembly.GetAssembly(typeof(IProjectRepository));

            builder.RegisterAssemblyTypes(coreAssembly).Where(t => t.Name.EndsWith("Repository")).AsImplementedInterfaces();

        }
    }
}