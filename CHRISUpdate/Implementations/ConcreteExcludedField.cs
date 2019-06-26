using System;
using System.Linq;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    internal class ConcreteExcludedField : IExcludedField
    {
        public ConcreteExcludedField()
        {
        }

        public string ExcludedFieldMajor{ get; set;}

        public string ExcludedFieldMinor{ get; set;}
        public object GetValue(Employee source)
        {
            var majorFieldPropertyInfo = typeof(Employee)
                .GetProperties()
                .FirstOrDefault(prop => prop.CanRead && prop.CanWrite && prop.Name == ExcludedFieldMajor);

            var minorFieldPropertyInfo = majorFieldPropertyInfo?
                .GetType()
                .GetProperties()
                .FirstOrDefault(prop => prop.CanRead && prop.CanWrite && prop.Name == ExcludedFieldMinor);

            var sourceValue = minorFieldPropertyInfo?
                .GetValue(majorFieldPropertyInfo.GetValue(source, null), null);

            return sourceValue;
        }
    }
}
