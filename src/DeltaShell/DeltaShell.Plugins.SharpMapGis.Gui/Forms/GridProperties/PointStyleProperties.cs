using System.ComponentModel;
using DelftTools.Utils.ComponentModel;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    public class PointStyleProperties : PointStylePropertiesBase<VectorStyle>
    {
        [DynamicVisibleValidationMethod]
        public override bool IsPropertyVisible(string propertyName)
        {
            return true;
        }

        [Browsable(false)]
        protected override VectorStyle Style
        {
            get { return data; }
        }
    }
}