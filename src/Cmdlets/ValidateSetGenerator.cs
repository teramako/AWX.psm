using System.Management.Automation;
using System.Reflection;
using AWX.Resources;

namespace AWX.Cmdlets
{
    public class EnumValidateSetGenerator<TEnum> : IValidateSetValuesGenerator
        where TEnum : Enum
    {
        public string[] GetValidValues()
        {
            return Enum.GetNames(typeof(TEnum))
                .Select(x => x.ToLowerInvariant())
                .ToArray();
        }
    }
    /*
    public class ResourceTypeValidateSetGenerator<TClass> : IValidateSetValuesGenerator
        where TClass : class
    {
        public string[] GetValidValues()
        {
            Type t = typeof(TClass);
            var attr = t.GetCustomAttributes<ResourceTypeAttribute>(false).First();
            return attr == null ? [] : attr.AssociateWith.Select(type => type.ToString()).ToArray();
        }
    }
    */

}

