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
			Console.ForegroundColor = colours[0];

			var surveiller = new Surveiller();
			surveiller.Watch(@"C:\DATA\PROJEKT\FarmerCharlieSprouts\Presentation\SproutConsole\Data\test to change.txt");
			surveiller.SetChange(FileChanged);

			Console.WriteLine("Press any key...");
			Console.ReadKey();
		}

		public static void FileChanged(string message)
		{
			Console.ForegroundColor = colours.NextOrFirstItem(Console.ForegroundColor);

			Console.WriteLine(
				"Change:" + (message ?? "null"));
		}
	}
}