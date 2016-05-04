using System.Globalization;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="GrassCoverErosionInwardsInputContext"/> for properties panel.
    /// </summary>
    public class GrassCoverErosionInwardsInputContextProperties : ObjectProperties<GrassCoverErosionInwardsInputContext>
    {
        private const int dikeGeometryPropertyIndex = 1;
        private const int dikeHeightPropertyIndex = 2;
        private const int foreshorePropertyIndex = 3;
        private const int orientationPropertyIndex = 4;
        private const int breakWaterPropertyIndex = 5;

        [PropertyOrder(dikeGeometryPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "DikeGeometryProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeGeometryProperties_Description")]
        public DikeGeometryProperties DikeGeometry
        {
            get
            {
                return new DikeGeometryProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(dikeHeightPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "DikeHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "DikeHeight_Description")]
        public string DikeHeight
        {
            get
            {
                return new RoundedDouble(2, data.WrappedData.DikeHeight).Value.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                data.WrappedData.DikeHeight = new RoundedDouble(2, double.Parse(value));
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(foreshorePropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "ForeshoreProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ForeshoreProperties_Description")]
        public ForeshoreProperties Foreshore
        {
            get
            {
                return new ForeshoreProperties
                {
                    Data = data
                };
            }
        }

        [PropertyOrder(orientationPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "Orientation_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Orientation_Description")]
        public string Orientation
        {
            get
            {
                return new RoundedDouble(2, data.WrappedData.Orientation).Value.ToString(CultureInfo.InvariantCulture);
            }
            set
            {
                data.WrappedData.Orientation = new RoundedDouble(2, double.Parse(value));
                data.WrappedData.NotifyObservers();
            }
        }

        [PropertyOrder(breakWaterPropertyIndex)]
        [ResourcesCategory(typeof(Resources), "Categories_Schematisation")]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterProperties_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterProperties_Description")]
        public BreakWaterProperties BreakWater
        {
            get
            {
                return new BreakWaterProperties
                {
                    Data = data
                };
            }
        }
    }
}