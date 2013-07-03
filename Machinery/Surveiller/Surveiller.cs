using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FarmerCharlieSprouts.Machinery.Surveiller
{
	/// <summary>
	/// http://social.msdn.microsoft.com/Forums/vstudio/en-US/6ec4b09e-da13-4aea-95db-b3e822fc6b5b/how-uses-c-to-monitor-under-some-directory-some-file-change-
	/// </summary>
	public class Surveiller
	{
		private string _pathAndFilename;
		private FileSystemWatcher _watch = new FileSystemWatcher();
		private Action<string> _changeMethod;
		private long _lastPosition = 0;
		private DateTime? _lastModifiedDatetime;
		private static object _fileLock = new Object();

		public void SetChange(Action<string> actionMethod)
		{
			_changeMethod = actionMethod;
			_watch.NotifyFilter = NotifyFilters.LastWrite;
			_watch.Changed += new FileSystemEventHandler(watch_Changed);
			_watch.EnableRaisingEvents = true;

			//DEBUG: Don't call the change method when setting up the watcher.
			_changeMethod("starts at " + _lastPosition + " characters.");
		}

		public void Watch(string pathAndFilename)
		{
			_pathAndFilename = pathAndFilename;
			_lastPosition = GetFileSize(pathAndFilename);
			_watch.Path = System.IO.Path.GetDirectoryName(pathAndFilename);
			_watch.Filter = System.IO.Path.GetFileName(pathAndFilename);
		}

		/// <summary>This method is called whenever a change is done to the file watched.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void watch_Changed(object sender, FileSystemEventArgs e)
		{
			//_changeMethod(GetFileModifiedDatetime(_pathAndFilename).ToLongTimeString());
			var changeDatetime = GetFileModifiedDatetime(_pathAndFilename);
			if (null == _lastModifiedDatetime || _lastModifiedDatetime.Value != changeDatetime)
			{
				_lastModifiedDatetime = changeDatetime;

				//	res is a reference to the new data in the file.
				//	There are probably faster ways than to append character by character but it works for now.
				var res = "";

				//Thread.Sleep(1000);
				var lastPositionOutsideTryCatch = _lastPosition;

				lock (_fileLock)
				{
					try
					{
						using (var sr = new StreamReader(_pathAndFilename))
						{
							ReadToPosition(sr, lastPositionOutsideTryCatch);
							int c;
							do
							{
								c = ReadCharacter(ref res, sr);
							} while (c != -1);
							sr.Close();
							lastPositionOutsideTryCatch += res.Count();
							_changeMethod("lastPositionOutsideTryCatch:" + lastPositionOutsideTryCatch);
						}
					}
					catch (IOException exc)
					{
						_changeMethod("lastPositionOutsideTryCatch in exception:" + lastPositionOutsideTryCatch);

						if (exc.HResult != -2147024864)
						{
							throw;
						}
					}
				}
				_lastPosition = lastPositionOutsideTryCatch;

				_changeMethod(res);
			}
		}

		private static DateTime GetFileModifiedDatetime(string pathAndFilename)
		{
			var fileinfo = new FileInfo(pathAndFilename);
			return fileinfo.LastWriteTime;
		}

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
		private static int ReadCharacter(ref string res, StreamReader sr)
		{
			int c;
			c = sr.Read();
			if (c != -1)
			{
				res += (char)c;
			}
			return c;
		}

		/// <summary>This method forwads the stream pointer to a certain place.
		/// The caller can then continue reading the file.
		/// 
		/// The algorithm is rather crude - there are probably faster ways fo winding a stream.
		/// I ahve tested but not succeded. /OF
		/// </summary>
		/// <param name="sr"></param>
		/// <param name="lastPosition"></param>
		private static void ReadToPosition(StreamReader sr, long lastPosition)
		{
			for (long i = 0; i < lastPosition; ++i)
			{
				sr.Read();
			}
		}

	}
}
