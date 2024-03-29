using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SiteWatch.Extensions;
using SiteWatch.Factories;
using SiteWatch.Factories.Interfaces;
using SiteWatch.Providers;
using SiteWatch.Providers.Interfaces;
using SiteWatch.Services;
using SiteWatch.Services.Interfaces;

namespace SiteWatch
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddRazorPages();
			services.AddServerSideBlazor();

			services.AddSingleton<ISettingsProvider, SettingsProvider>();
			services.AddSingleton<IWatchersSettingsProvider, WatchersSettingsProvider>();
			services.AddSingleton<IHttpClientProvider, HttpClientProvider>();
			services.AddSingleton<IWatchService, WatchService>();
			services.AddSingleton<ITelegramBotClientProvider, TelegramBotClientProvider>();

			services.AddTransient<IPageScrapeService, PageScrapeService>();
			services.AddTransient<IPageWatchServiceFactory, PageWatchServiceFactory>();
			services.AddTransient<INotificationService, TelegramNotificationService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseExceptionHandler("/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseIpWhitelist();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapBlazorHub();
				endpoints.MapFallbackToPage("/_Host");
			});

			app.ApplicationServices.GetService<IWatchService>().SetUpWatchers();
		}
	}
}
