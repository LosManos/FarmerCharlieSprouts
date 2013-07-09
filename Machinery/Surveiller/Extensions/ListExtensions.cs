using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmerCharlieSprouts.Machinery.Surveiller.Extensions
{
	internal static class ListExtensions
	{
		internal static bool AddLastIfDifferent<T>( this List<T> me, T itemToAdd){
			if (me.Any() && me.Last().Equals(itemToAdd))
			{
				return false;
			}
			else
			{
				me.Add(itemToAdd);
				return true;
			}
		}
	}
}
