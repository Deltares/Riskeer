using System.ComponentModel;

using Core.Common.Gui.Attributes;
using Core.Common.Utils.Attributes;
using Core.GIS.SharpMap.Styles;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    public class LineStyleProperties : LineStylePropertiesBase<VectorStyle>
    {
        [DynamicVisibleValidationMethod]
        public override bool IsPropertyVisible(string propertyName)
        {
            return true;
        }

        [Browsable(false)]
        protected override VectorStyle Style
        {
            get
            {
                return data;
            }
        }
    }
}