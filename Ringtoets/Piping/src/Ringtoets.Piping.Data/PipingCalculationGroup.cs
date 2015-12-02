using System;
using System.Collections.Generic;
using System.Linq;

using Core.Common.Base;

using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Class to allow grouping one or multiple <see cref="PipingCalculation"/> instances.
    /// </summary>
    public class PipingCalculationGroup : Observable, IPipingCalculationItem
    {
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationGroup"/> class
        /// with an editable name.
        /// </summary>
        public PipingCalculationGroup() : this(Resources.PipingCalculationGroup_DefaultName, true)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationGroup"/> class.
        /// </summary>
        /// <param name="newName">The name of the group.</param>
        /// <param name="canEditName">Determines if the name of the group is editable (true) or not.</param>
        public PipingCalculationGroup(string newName, bool canEditName)
        {
            name = newName;
            NameIsEditable = canEditName;
            Children = new List<IPipingCalculationItem>();
        }

        /// <summary>
        /// Gets the children that define this group.
        /// </summary>
        public ICollection<IPipingCalculationItem> Children { get; private set; }

        /// <summary>
        /// Gets or sets the name of this calculation grouping object.
        /// </summary>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (!NameIsEditable)
                {
                    throw new InvalidOperationException(Resources.PipingCalculationGroup_Setting_readonly_name_error_message);
                }
                name = value;
            }
        }

        public bool HasOutput
        {
            get
            {
                return Children.Any(c => c.HasOutput);
            }
        }

        /// <summary>
        /// Gets a value indicating whether <see cref="Name"/> is editable or not.
        /// </summary>
        public bool NameIsEditable { get; private set; }
    }
}