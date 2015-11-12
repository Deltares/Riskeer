using Core.Common.Gui;
using Core.Common.Utils;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="RingtoetsProject"/> for properties panel.
    /// </summary>
    [ResourcesDisplayName(typeof(Resources), "RingtoetsProjectProperties_DisplayName")]
    public class RingtoetsProjectProperties : ObjectProperties<RingtoetsProject>
    {
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "RingtoetsProject_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "RingtoetsProject_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
                data.NotifyObservers();
            }
        }
    }
}