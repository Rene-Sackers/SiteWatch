using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Web;
using LogLevel = NLog.LogLevel;

namespace SiteWatch
{
	public class Program
	{
		private static void SetUpNlog()
		{
			var loggingConfiguration = new LoggingConfiguration();

			const string nlogFormat = @"[${date:format=HH\:mm\:ss}] [${level}] ${message} ${exception:format=toString}";

			var coloredConsoleTarget = new ColoredConsoleTarget
			{
				Layout = nlogFormat
			};

			loggingConfiguration.AddTarget("console", coloredConsoleTarget);
			loggingConfiguration.AddRule(LogLevel.Trace, LogLevel.Fatal, coloredConsoleTarget);

			var logsFolder = Path.Join(Path.GetFullPath("."), "logs");

			var fileTarget = new FileTarget
			{
				FileName = logsFolder + "/${shortdate}.log",
				ArchiveAboveSize = 1024 * 1024 * 5, // 5 MB
				Layout = nlogFormat
			};

			loggingConfiguration.AddTarget("file", fileTarget);
			loggingConfiguration.AddRule(LogLevel.Info, LogLevel.Fatal, fileTarget);

			LogManager.Configuration = loggingConfiguration;
		}

		public static void Main(string[] args)
		{
			SetUpNlog();

			var logger = LogManager.GetCurrentClassLogger();

			try
			{
				logger.Info("Starting");

				CreateHostBuilder(args).Build().Run();
			}
			catch (Exception e)
			{
				logger.Fatal(e, "Failed to start app.");
				throw;
			}
			finally
			{
				LogManager.Shutdown();
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<Startup>();
				})
				.ConfigureLogging(logging =>
				{
					logging.ClearProviders();
					logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
				})
				.UseNLog();
	}
}
