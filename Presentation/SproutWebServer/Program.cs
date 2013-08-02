using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Nancy;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SproutWebServer
{
	class Program
	{
		static void Main(string[] args)
		{
			const string webUrl = "http://localhost:666";
			const string signalrUrl = "http://localhost:667";

			using (var webHost = new Nancy.Hosting.Self.NancyHost(
				new Uri(webUrl)
				))
			{

				using (WebApp.Start<Startup>(signalrUrl))
				{

					webHost.Start();

					Console.WriteLine("Surf to " + webUrl);
					Console.Write("Press any key");
					Console.ReadKey();
					webHost.Stop();
				
				}
			}
		}
	}

	class Startup
	{
		public void Configuration(Owin.IAppBuilder app)
		{
			//var config = new HubConfiguration { EnableCrossDomain = true };
			//app.MapHubs(config);
			app.MapHubs(new HubConfiguration() { EnableCrossDomain = true });
			app.UseNancy(new ApplicationBootstrapper());
		}
	}

	public class ApplicationBootstrapper : DefaultNancyBootstrapper
	{
		protected override void ConfigureConventions(Nancy.Conventions.NancyConventions nancyConventions)
		{
			nancyConventions.StaticContentsConventions.Add(
				Nancy.Conventions.StaticContentConventionBuilder.AddDirectory("Scripts", @"/Scripts")
				);
			base.ConfigureConventions(nancyConventions);
		}
	}

	public class Chat : Hub
	{
		public void Send(string message)
		{
			Clients.All.addMessage(message);
		}
	}
}
