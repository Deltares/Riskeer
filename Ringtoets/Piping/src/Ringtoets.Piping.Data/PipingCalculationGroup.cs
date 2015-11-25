using System.Collections.Generic;
using System.Linq;

using Core.Common.Base;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Class to allow grouping one or multiple <see cref="PipingCalculation"/> instances.
    /// </summary>
    public class PipingCalculationGroup : Observable, IPipingCalculationItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PipingCalculationGroup"/> class.
        /// </summary>
        public PipingCalculationGroup()
        {
            Name = "Berekening groep";
            Children = new List<IPipingCalculationItem>();
        }

        /// <summary>
        /// Gets the children that define this group.
        /// </summary>
        public ICollection<IPipingCalculationItem> Children { get; private set; }

        public string Name { get; set; }

        public bool HasOutput
        {
            get
            {
                return Children.Any(c => c.HasOutput);
            }
        }
    }
}