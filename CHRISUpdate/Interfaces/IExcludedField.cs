using HRUpdate.Models;

namespace HRUpdate.Interfaces
{
    /// <summary>
    /// Interface used to represent the excluded columns
    /// </summary>
    public interface IExcludedField
    {
        string ExcludedFieldMajor { get; set; }
        string ExcludedFieldMinor { get; set; }

        object GetValue(Employee source);
    }
}
