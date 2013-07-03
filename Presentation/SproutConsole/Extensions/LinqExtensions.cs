using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerCharlieSprouts.Presentation.SproutConsole.Extensions
{
	internal static class LinqExtensions
	{
		/// <summary>This method returns the index of an item in a list of items.
		/// 
		/// Right now it is limited to array. An IEnumerable[T] wouldn't be any problem
		/// but it isn't considered good coding standard to transparently loop an IEnumerable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		internal static int IndexOf<T>(this T[] items, T item)
		{
			return items
				.Select((o, index) => new { O = o, Index = index })
				.Where(c => Equals( c.O , item))
				.Single().Index;
		}

		/// <summary>This method finds the index of an item in an array but return the next index.
		/// If the end of the array is passed, there isn't any more items, the first index (0) is returned.
		/// 
		/// Right now it is limited to array. An IEnumerable[T] wouldn't be any problem
		/// but it isn't considered good coding standard to transparently loop an IEnumerable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		internal static int NextOrFirstIndexOf<T>(this T[] items, T item)
		{
			return (items.IndexOf(item) + 1) % items.Length;
		}

		/// <summary>This method finds the an item in an array of items and returns the next item.
		/// If there isn't any more item the first is returned.
		/// 
		/// Right now it is limited to array. An IEnumerable[T] wouldn't be any problem
		/// but it isn't considered good coding standard to transparently loop an IEnumerable.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		internal static T NextOrFirstItem<T>(this T[] items, T item)
		{
			return items[items.NextOrFirstIndexOf(item)];
		}
	}
}
