using System;
using System.Collections.Generic;
using System.Linq;

namespace Dotjosh.DayZCommander.Core
{
	public static class ExtensionMethods
	{
		public static int TryInt(this string val)
		{
			int result;
			if(int.TryParse(val, out result))
			{
				return result;
			}
			return 0;
		}

		public static List<T> ToList<T>(this IEnumerable<T> items, Action<T> action)
		{
			List<T> list = items.ToList();
			foreach(var item in list)
			{
				action(item);
			}
			return list;
		}

		public static bool None<T>(this IEnumerable<T> items, Func<T, bool> predicate)
		{
			return !items.Any(predicate);
		}
	}
}