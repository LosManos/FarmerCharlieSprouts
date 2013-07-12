using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SproutWebServer
{
	public class MainModule : Nancy.NancyModule
	{
		public MainModule()
		{
			Get["/"] = x =>
				{
					//return "hello world";
					return View["Index", new Models.IndexModel{Name="from model"}];
				};
		}
	}
}
