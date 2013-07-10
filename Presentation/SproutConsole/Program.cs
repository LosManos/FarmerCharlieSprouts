using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarmerCharlieSprouts.Machinery.Surveiller;
using FarmerCharlieSprouts.Presentation.SproutConsole.Extensions;

namespace FarmerCharlieSprouts.Presentation.SproutConsole
{
	class Program
	{
		static ConsoleColor[] colours = new ConsoleColor[] { ConsoleColor.Red, ConsoleColor.Green, ConsoleColor.Yellow };

		static void Main(string[] args)
		{
			if (null == args || args.Count()==0)
			{
				const string failMessage =
					@"FarmerCharlieSprouts - Console
Argument missing, please provide a file name to surveil.
E.g.: SproutConsole.exe mylogfile.log
Exiting.";
				Console.WriteLine(failMessage);
				return;
			}
			var pathfilename = args[0];

			var successMessage = string.Format(
					@"Starting surveillance on file {0}.", 
					pathfilename
				);
			Console.WriteLine(successMessage);

			Console.ForegroundColor = colours[0];

			var surveiller = new Surveiller();
			surveiller.Watch(pathfilename);
			surveiller.SetChange(InitCall, FileChanged);

			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}

		public static void FileChanged(long lineNumber, string message)
		{
			Console.ForegroundColor = colours.NextOrFirstItem(Console.ForegroundColor);

			Console.WriteLine(
				"[" + lineNumber + "]" +
				message ?? "null");
		}

		public static void InitCall(string message)
		{
			Console.WriteLine(message);
		}
	}
}