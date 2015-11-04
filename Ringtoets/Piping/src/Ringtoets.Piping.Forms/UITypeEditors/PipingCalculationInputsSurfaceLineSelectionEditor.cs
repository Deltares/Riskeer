using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Utils.Reflection;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.UITypeEditors
{
    /// <summary>
    /// This class defines a drop down list edit-control from which the user can select a
    /// <see cref="RingtoetsPipingSurfaceLine"/> from a collection.
    /// </summary>
    public class PipingCalculationInputsSurfaceLineSelectionEditor : PipingCalculationInputSelectionEditor<RingtoetsPipingSurfaceLine>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationInputsSurfaceLineSelectionEditor"/>.
        /// </summary>
        public PipingCalculationInputsSurfaceLineSelectionEditor()
        {
            DisplayMember = TypeUtils.GetMemberName<RingtoetsPipingSurfaceLine>(sl => sl.Name);
        }

        protected override IEnumerable<RingtoetsPipingSurfaceLine> GetAvailableOptions(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).GetAvailableSurfaceLines();
        }

        protected override RingtoetsPipingSurfaceLine GetCurrentOption(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).SurfaceLine;
        }
    }
}