using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Utils.Reflection;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.UITypeEditors
{
    /// <summary>
    /// This class defines a drop down list edit-control from which the user can select a
    /// <see cref="PipingSoilProfile"/> from a collection.
    /// </summary>
    public class PipingInputParametersContextSoilProfileSelectionEditor : PipingInputParametersContextSelectionEditor<PipingSoilProfile>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingInputParametersContextSoilProfileSelectionEditor"/>.
        /// </summary>
        public PipingInputParametersContextSoilProfileSelectionEditor()
        {
            DisplayMember = TypeUtils.GetMemberName<PipingSoilProfile>(sl => sl.Name);
        }

        protected override IEnumerable<PipingSoilProfile> GetAvailableOptions(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).GetAvailableSoilProfiles();
        }

        protected override PipingSoilProfile GetCurrentOption(ITypeDescriptorContext context)
        {
            return GetPropertiesObject(context).SoilProfile;
        }
    }
}