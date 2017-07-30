using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Session;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using JiraMonitor.Data;
using JiraMonitor.Models;
using JiraMonitor.Service.Services;
using JiraMonitor.Data.DbModel;
using JiraMonitor.Web.Models;

namespace JiraMonitor
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see https://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

         }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            //Configure BasicJiraRetriever and Email sender services

            services.AddTransient<IJqlExecutor, BasicJqlExecutor>();
            services.AddTransient<IEmailSender, GmailSender>();

            services.AddSingleton<IJiraPoller, JiraPoller>();

            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                // Set a short timeout for easy testing.
                options.IdleTimeout = TimeSpan.FromSeconds(10);
                options.CookieHttpOnly = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime appLifetime, IServiceProvider serviceProvider)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseIdentity();

            // Add external authentication middleware below. To configure them please see https://go.microsoft.com/fwlink/?LinkID=532715

            //TODO move to hangfire jobs or to windows service instead of this hacky solution
            appLifetime.ApplicationStarted.Register(new Action(() => StartService(serviceProvider)));


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=JqlQueries}/{action=Index}");
            });
        }

        private void StartService(IServiceProvider serviceProvider)
        {
         
            var jiraPoller = ActivatorUtilities.CreateInstance<JiraPoller>(serviceProvider);


            jiraPoller.Initialize(PrepareData(serviceProvider));

            Task.Run(new Action(() => jiraPoller.Start()));
        }

        private IEnumerable<UserData> PrepareData(IServiceProvider serviceProvider)
        {
            var usersData = new List<UserData>();
            ApplicationDbContext dbContext = serviceProvider.GetService<ApplicationDbContext>();

            if (dbContext != null)
            {
                foreach (var user in dbContext.Users)
                {

                    dbContext.Entry(user).Collection(u => u.JqlQueries).Load();
                    dbContext.Entry(user).Reference(u => u.EmailNotificaionSettings).Load();
                    dbContext.Entry(user).Reference(u => u.JiraSettings).Load();

                    usersData.Add(
                        new UserData
                        {
                            EmailNotificationSettings = user.EmailNotificaionSettings,
                            JiraSettings = user.JiraSettings,
                            JqlQueries = user.JqlQueries,
                            UserLogin = user.Email
                        }
                    );
                }

                return usersData;
            }           

            return null;
        }
    }
}
