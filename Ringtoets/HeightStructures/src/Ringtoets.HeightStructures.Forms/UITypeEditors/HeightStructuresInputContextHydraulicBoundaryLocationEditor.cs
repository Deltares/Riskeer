using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Forms.UITypeEditors;
using Ringtoets.HeightStructures.Forms.PropertyClasses;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HeightStructures.Forms.UITypeEditors
{
    /// <summary>
    /// This class defines a drop down list edit-control from which the user can select a
    /// <see cref="HydraulicBoundaryLocation"/> from a collection.
    /// </summary>
    public class HeightStructuresInputContextHydraulicBoundaryLocationEditor :
        SelectionEditor<HeightStructuresInputContextProperties, HydraulicBoundaryLocation>
    {
        /// <summary>
        /// Creates a new instance of <see cref="HeightStructuresInputContextHydraulicBoundaryLocationEditor"/>.
        /// </summary>
        public HeightStructuresInputContextHydraulicBoundaryLocationEditor()
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