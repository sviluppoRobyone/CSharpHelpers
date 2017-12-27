using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace CSharpHelpers.Attributes
{
    public static class AttributesHelper
    {
        public static bool HasDescriptionAttribute(this PropertyInfo p)
        {
            return p.GetCustomAttributes(typeof(DescriptionAttribute), true).Any();
        }

        public static string GetDescriptionAttribute(this PropertyInfo p)
        {
            var a = p.GetCustomAttributes(typeof(DescriptionAttribute), true);
            return a.Length > 0 ? a.Select(x => x as DescriptionAttribute).First().Description : null;
        }
    }
}
