using System.ComponentModel;
using DelftTools.Utils.ComponentModel;
using SharpMap.Styles;

namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    public class PolygonStyleProperties : PolygonStylePropertiesBase<VectorStyle>
    {
        [Browsable(false)]
        protected override VectorStyle Style
        {
            get { return data; }
        }

        [DynamicVisibleValidationMethod]
        public override bool IsPropertyVisible(string propertyName)
        {
            return true;
        }
    }
}