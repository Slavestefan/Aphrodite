using Discord.Commands;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Slavestefan.Aphrodite.Model;
using Slavestefan.Aphrodite.Web.Logger;
using Slavestefan.Aphrodite.Web.Services;

namespace Slavestefan.Aphrodite.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PicAutoPostContext>(options => options.UseSqlServer(Configuration.GetConnectionString("PicAutoPost")));
            services.AddSingleton<RandomService>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<Bot>(sp => new Bot(sp, Configuration.GetSection("DiscordBotToken").GetValue<string>("Aphrodite")));
            services.AddSingleton<IHostedService>(sp => sp.GetService<Bot>());
            services.AddSingleton<PostingServiceHost>();
            services.AddScoped<PostingService>();
            services.AddScoped<BotConfigService>();
            services.AddControllersWithViews();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            loggerFactory.AddProvider(new DiscordLoggerProvider(new DiscordLoggerConfiguration
            {
                ChannelId = Configuration.GetSection("Logging").GetValue<ulong>("DiscordLogChannel"),
                LogLevel = LogLevel.Warning
            }, app.ApplicationServices));
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
