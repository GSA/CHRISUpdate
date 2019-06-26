using HRUpdate.Models;

namespace HRUpdate.Interfaces
{
    public interface IExcludedField
    {
        string ExcludedFieldMajor { get; set; }
        string ExcludedFieldMinor { get; set; }

        object GetValue(Employee source);
    }
}
