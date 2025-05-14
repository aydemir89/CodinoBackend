using System.Collections;
using System.Reflection;

namespace Codino_UserCredential.Core.Extentions;

public static class ObjectExtensions
{
    public static void TrimAllStrings<Tself>(this Tself obj)
    {
        if (obj == null)
        {
            return;
        }

        if (obj is IEnumerable)
        {
            foreach (var item in obj as IEnumerable)
            {
                item.TrimAllStrings();
            }
            return;
        }

        BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                    BindingFlags.FlattenHierarchy;
        PropertyInfo[] propertyInfos = obj.GetType().GetProperties(bindingFlags);
        foreach (var propertyInfo in propertyInfos)
        {
            Type propertiesType = propertyInfo.PropertyType;
            if (propertiesType == typeof(string))
            {
                string text = (string)propertyInfo.GetValue(obj, null);
                if (text != null)
                {
                    propertyInfo.SetValue(obj, text.Trim(), null);
                }
            }else if (propertiesType != typeof(object) && Type.GetTypeCode(propertiesType) == TypeCode.Object)
            {
                propertyInfo.GetValue(obj, null).TrimAllStrings();
            }
        }
    }
}