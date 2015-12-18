using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    static class Objects
     {
        public static void Copy(object src, object dest)
        {
            if (src != null)
            {
                foreach (PropertyDescriptor item in TypeDescriptor.GetProperties(dest))
                {
                    item.SetValue(dest, item.GetValue(src));
                }
            }
        }

        public static object MapObjects(object source, object target)
        {
            foreach (PropertyInfo sourceProp in source.GetType().GetProperties())
            {
                PropertyInfo targetProp = target.GetType().GetProperties().Where(p => p.Name == sourceProp.Name).FirstOrDefault();
                if (targetProp != null && targetProp.GetType().Name == sourceProp.GetType().Name)
                {
                    targetProp.SetValue(target, sourceProp.GetValue(source));
                }
            }
            return target;
        }

    }
}
