using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.Integration.Forms.PresentationObjects;

namespace Ringtoets.Integration.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="FailureMechanismPlaceholderContext"/> properties panel.
    /// </summary>
    public class FailureMechanismPlaceholderContextProperties : ObjectProperties<FailureMechanismPlaceholderContext> {

        #region General

        [PropertyOrder(1)]
        [ResourcesCategory(typeof(Common.Data.Properties.Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Common.Data.Properties.Resources), "FailureMechanism_Name_DisplayName")]
        [ResourcesDescription(typeof(Common.Data.Properties.Resources), "FailureMechanism_Name_Description")]
        public string Name
        {
            get
            {
                return data.WrappedData.Name;
            }
        }

        [PropertyOrder(2)]
        [ResourcesCategory(typeof(Common.Data.Properties.Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Common.Data.Properties.Resources), "FailureMechanism_Code_DisplayName")]
        [ResourcesDescription(typeof(Common.Data.Properties.Resources), "FailureMechanism_Code_Description")]
        public string Code
        {
            get
            {
                return data.WrappedData.Code;
            }
        }

        #endregion
    }
}