using System.Collections.Generic;
using HRUpdate.Implementations;
using HRUpdate.Interfaces;
using HRUpdate.Models;

namespace HRUpdate.Utilities
{
    /// <summary>
    /// Tool to handle special cases where the default copy logic is not appropriate. Utilizes state design pattern
    /// to determine the action of a group of excluded columns. When the tool finishes with a group of excluded
    /// columns the proper values are in the Employee object and these columns should be ignored in the copy function.
    /// </summary>
    internal class ExcludedFieldTool
    {
        private readonly IExcludedFieldGroup excludedColumnGroupType;

        /// <summary>
        /// Set the type of the column group by passing in a value and creating the object that will handle the group
        /// </summary>
        /// <param name="major"></param>
        public ExcludedFieldTool(string major)
        {
            switch (major)
            {
                case "Phone": excludedColumnGroupType = new PersonalPhoneGroup();
                    break;
                case "Emergency": excludedColumnGroupType = new EmergencyContactPhoneGroup();
                    break;
            }
        }

        /// <summary>
        /// Create the list of excluded columns and passes in the context that holds the values for the columns
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <param name="hr"></param>
        public void Create(string major, string[] minor, Employee hr)
        {
            //create excluded field objects
            excludedColumnGroupType.Context = hr;
            excludedColumnGroupType.ExcludedFieldGroup = new List<IExcludedField>();

            foreach (var itm in minor)
            {
                excludedColumnGroupType
                    .ExcludedFieldGroup
                    .Add(ConcreteExcludedFieldFactory.GetExcludedField<ConcreteExcludedField>(major, itm));
            }
        }

        /// <summary>
        /// Calls the function that will process the column group, the type of group will determine which
        /// version of the function is executed
        /// </summary>
        /// <param name="hr"></param>
        /// <param name="db"></param>
        public void Process(Employee hr, Employee db)
        {
            excludedColumnGroupType.ProcessGroup(hr, db);
        }
    }
}
