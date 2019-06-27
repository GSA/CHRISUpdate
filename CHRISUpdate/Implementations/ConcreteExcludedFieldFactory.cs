using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRUpdate.Interfaces;

namespace HRUpdate.Implementations
{
    /// <summary>
    /// Factory to return fields of type IExcludedField
    /// </summary>
    internal static class ConcreteExcludedFieldFactory
    {
        public static T GetExcludedField<T>(string major, string minor) where T : IExcludedField, new()
        {
            return new T {ExcludedFieldMajor = major, ExcludedFieldMinor = minor};
        }
    }
}
