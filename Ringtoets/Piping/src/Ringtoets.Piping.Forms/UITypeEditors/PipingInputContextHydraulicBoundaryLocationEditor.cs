using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Utils.Reflection;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.Piping.Forms.UITypeEditors
{
    /// <summary>
    /// This class defines a drop down list edit-control from which the user can select a
    /// <see cref="HydraulicBoundaryLocation"/> from a collection.
    /// </summary>
    public class PipingInputContextHydraulicBoundaryLocationEditor : PipingInputContextSelectionEditor<HydraulicBoundaryLocation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingInputContextHydraulicBoundaryLocationEditor"/>.
        /// </summary>
        public PipingInputContextHydraulicBoundaryLocationEditor()
        {
            DisplayMember = TypeUtils.GetMemberName<HydraulicBoundaryLocation>(hbl => hbl.Name);
        }

        protected override IEnumerable<HydraulicBoundaryLocation> GetAvailableOptions(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).GetAvailableHydraulicBoundaryLocations();
        }

        protected override HydraulicBoundaryLocation GetCurrentOption(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).HydraulicBoundaryLocation;
        }
    }
}