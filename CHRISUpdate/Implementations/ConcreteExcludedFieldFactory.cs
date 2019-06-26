using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HRUpdate.Interfaces;

namespace HRUpdate.Implementations
{
    internal static class ConcreteExcludedFieldFactory
    {
        public static T GetExcludedField<T>(string major, string minor) where T : IExcludedField, new()
        {
            return new T {ExcludedFieldMajor = major, ExcludedFieldMinor = minor};
        }
    }
}
