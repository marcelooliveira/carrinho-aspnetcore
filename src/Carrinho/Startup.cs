using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Carrinho.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using React.AspNet;

namespace Carrinho
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddAntiforgery(op => op.HeaderName = "X-XSRF-TOKEN");
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddReact();
            services.AddMvc();
            //services.Configure<ConnectionStrings>(Configuration.GetSection("ConnectionStrings"));
            //services.AddDbContext<Context>(options =>
            //     options.UseSqlServer(Configuration.GetConnectionString("Carrinho")));
            services.AddTransient<ICheckoutManager, CheckoutManager>();

            var options = new DbContextOptionsBuilder<Context>();
            options.UseSqlServer(Configuration.GetConnectionString("Carrinho"));
            var dbOptions = options.Options;
            services.AddSingleton<DbContextOptions<Context>>(dbOptions);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseApplicationInsightsExceptionTelemetry();

            // Initialise ReactJS.NET. Must be before static files.
            app.UseReact(config =>
            {
                config
                .AddScript("~/js/showdown.js")
                .AddScript("~/js/react-bootstrap.js")
                .AddScript("~/js/Components.jsx")
                .AddScript("~/js/Cart.jsx")
                .AddScript("~/js/CheckoutSuccess.jsx");

                //config
                //.SetBabelConfig(new React.BabelConfig())
                //.AddScriptWithoutTransform("~/build/bundle.js");
            });

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
