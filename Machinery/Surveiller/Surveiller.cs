using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FarmerCharlieSprouts.Machinery.Surveiller.Extensions;

namespace FarmerCharlieSprouts.Machinery.Surveiller
{
	/// <summary>
	/// http://social.msdn.microsoft.com/Forums/vstudio/en-US/6ec4b09e-da13-4aea-95db-b3e822fc6b5b/how-uses-c-to-monitor-under-some-directory-some-file-change-
	/// </summary>
	public class Surveiller
	{
		private Action<string> _changeMethod;
		private Action<string> _initCallMethod;
		private static object _lock = new Object();
		private string _pathAndFilename;
		private static List<long> _sizes = new List<long>();
		private FileSystemWatcher _watch = new FileSystemWatcher();

		/// <summary>This method is called to set the callback methods.
		/// </summary>
		/// <param name="initCallMethod">The frist method that is called back, used for showing init info.</param>
		/// <param name="actionMethod">This method is called once for every change.</param>
		public void SetChange(Action<string> initCallMethod, Action<string> actionMethod)
		{
			_changeMethod = actionMethod;
			_initCallMethod = initCallMethod;
			_watch.NotifyFilter = NotifyFilters.LastWrite;
			_watch.Changed += new FileSystemEventHandler(watch_Changed);
			_watch.EnableRaisingEvents = true;

			//DEBUG: Don't call the change method when setting up the watcher.
			_initCallMethod("Starts at " + _sizes.Single() + " characters.");
		}

		public void Watch(string pathAndFilename)
		{
			_pathAndFilename = pathAndFilename;
			_sizes.Add(GetFileSize(pathAndFilename));
			_watch.Path = System.IO.Path.GetDirectoryName(pathAndFilename);
			_watch.Filter = System.IO.Path.GetFileName(pathAndFilename);
		}

		/// <summary>This method is called whenever a change is done to the file watched.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		 void watch_Changed(object sender, FileSystemEventArgs e)
		{
			lock (_lock)
			{
				var fileinfo = new FileInfo(_pathAndFilename);
				var presentFileSize = fileinfo.Length;
				if (0 == presentFileSize) { return; }	//	Sometimes the file length is 0; bail. Why 0 - dunno.

				if (_sizes.AddLastIfDifferent(presentFileSize))
				{
					//_changeMethod("_sizes:" + string.Join(",", _sizes));
					//_changeMethod("Size:" + presentFileSize);
					try
					{
						using (var sr = new FileStream(_pathAndFilename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
						{
							SkipToPosition(sr, _sizes.First());
							while (_sizes.Count() >= 2)
							{
								var presentPosition = _sizes.First();
								var nextPosition = _sizes.Skip(1).First();
								var s = ReadToPosition(sr, presentPosition, nextPosition);
								//_changeMethod("Pos:" + nextPosition + ", Text:" + s);
								_changeMethod(s);
								_sizes.RemoveAt(0);	//	Remove only when we know we have printed.								
							}
						}
					}
					catch (IOException exc)
					{
						_changeMethod("Exception:" + exc.Message);
						if (exc.HResult != -2147024864)
						{
							throw;
						}
					}
				}
			}
		}

		//private static DateTime GetFileModifiedDatetime(string pathAndFilename)
		//{
		//	var fileinfo = new FileInfo(pathAndFilename);
		//	return fileinfo.LastWriteTime;
		//	fileinfo.Length
		//}

		/// <summary>This method returns the number of characters in a file.
		/// It is not the same as fileinfo.length. For instace create a text document with two characters
		/// in it - (saved as unicode?/standard at win8 x64) results in a file size of 5
		/// </summary>
		/// <param name="pathAndFilename"></param>
		/// <returns></returns>
		private static long GetFileSize(string pathAndFilename)
		{
			//var fileinfo = new FileInfo(pathAndFilename);
			//return fileinfo.Length;
			using (var sr = new StreamReader(pathAndFilename))
			{
				int length = -1;
				int c;
				do
				{
					c = sr.Read();
					++length;
				} while (c != -1);
				sr.Close();
				return length;
			}
		}

		/// <summary>This method reads a character from a stream
		/// and appends it to a  string.
		/// It also returns the character.
		/// </summary>
		/// <param name="res"></param>
		/// <param name="sr"></param>
		/// <returns></returns>
		private static int ReadCharacter(ref string res, FileStream sr)
		{
			int c;
			c = sr.ReadByte();
			if (c != -1)
			{
				res += (char)c;
			}
			return c;
		}

		private static string ReadToPosition( 
			FileStream sr, 
			long formerPosition, 
			long newPosition )
		{
			var ret = "";
			for (var i = formerPosition; i < newPosition; ++i)
			{
				ret += (char)sr.ReadByte();
			}
			return ret;
		}

		/// <summary>This method forwads the stream pointer to a certain place.
		/// The caller can then continue reading the file.
		/// 
		/// The algorithm is rather crude - there are probably faster ways fo winding a stream.
		/// I ahve tested but not succeded. /OF
		/// </summary>
		/// <param name="sr"></param>
		/// <param name="position"></param>
		private static void SkipToPosition(FileStream sr, long position)
		{
			for (long i = 0; i < position; ++i)
			{
				sr.ReadByte();
			}
		}

	}
}
