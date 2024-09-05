using System.Management.Automation;

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
}
