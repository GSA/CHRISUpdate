using System;
using System.Linq;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Implementations
{
    /// <summary>
    /// Generic implementation of IExcludedField, If a more specific implementation of IExcludedField is more
    /// appropriate then it can still be generated using the supplied factory
    /// </summary>
    internal class ConcreteExcludedField : IExcludedField
    {
        public string ExcludedFieldMajor{ get; set;}

        public string ExcludedFieldMinor{ get; set;}

        /// <summary>
        /// The IExcludedField should be able to obtain its value from the supplied Employee context
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public object GetValue(Employee source)
        {
            var majorFieldPropertyInfo = typeof(Employee)
                .GetProperties()
                .FirstOrDefault(prop => prop.CanRead && prop.CanWrite && prop.Name == ExcludedFieldMajor);

            var majorType =  majorFieldPropertyInfo?.GetValue(source, null).GetType();

            var minorFieldPropertyInfo = majorType?
                .GetProperties()
                .FirstOrDefault(prop => prop.CanRead && prop.CanWrite && prop.Name == ExcludedFieldMinor);

            var sourceValue = minorFieldPropertyInfo?
                .GetValue(majorFieldPropertyInfo.GetValue(source, null), null);

            return sourceValue;
        }
    }
}
