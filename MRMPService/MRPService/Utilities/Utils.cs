using MRMPService;
using MRMPService.Exceptions;
using MRMPService.LocalDatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MRMPService.Utilities
{
    public static class StringExtensionMethods
    {
        public static IEnumerable<string> GetLines(this string str, bool removeEmptyLines = false)
        {
            return str.Split(new[] { "\r\n", "\r", "\n" },
                removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

		/// <summary>
		/// Escapes and return the given value.
		/// </summary>
		/// <param name="valueToEscape">
		/// The value to escape.
		/// </param>
		/// <returns>
		/// The escaped value.
		/// </returns>
		public static string Escape(this string valueToEscape)
		{
			ExceptionFactory.CheckArgumentIsNullOrEmpty(valueToEscape);

			return Uri.EscapeDataString(valueToEscape);
		}
    }
    static class Objects
     {
        public static string RamdomGuid()
        {
            return (Guid.NewGuid().ToString("N") + (new Random().NextDouble()) + DateTime.UtcNow.ToString("hh.mm.ss.ffffff")).GetHashString();
        }
        //public static void Copy(object src, object dest)
        //{
        //    if (src != null)
        //    {
        //        foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(dest))
        //        {
        //            item.SetValue(dest, item.GetValue(src));
        //        }
        //    }
        //}

        //public static object Copy(object source, object target)
        //{
        //    foreach (PropertyInfo sourceProp in source.GetType().GetProperties())
        //    {
        //        PropertyInfo targetProp = target.GetType().GetProperties().Where(p => p.Name == sourceProp.Name).FirstOrDefault();
        //        if (targetProp != null && targetProp.GetType().Name == sourceProp.GetType().Name)
        //        {
        //            targetProp.SetValue(target, sourceProp.GetValue(source));
        //        }
        //    }
        //    return target;
        //}
    }
}
