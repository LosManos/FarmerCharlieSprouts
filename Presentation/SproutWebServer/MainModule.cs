using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SproutWebServer
{
	public class MyModel
	{
		public string Name { get; set; }
	}

	public class MainModule : Nancy.NancyModule
	{
		public MainModule()
		{
			Get["/"] = x =>
				{
					//return "hello world";
					return View["Index", new MyModel{Name="from model"}];
				};
		}
	}
}
