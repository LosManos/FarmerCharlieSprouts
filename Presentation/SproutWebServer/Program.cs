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
			using (var host = new Nancy.Hosting.Self.NancyHost(
				new Uri("http://localhost:666")
				))
			{
				host.Start();

				Console.WriteLine("Surf to http://localhost:666");
				Console.Write("Press any key");
				Console.ReadKey();
				host.Stop();
			}
		}
	}
}
