﻿using System;
using System.ComponentModel;
using Core.Common.Utils;
using Core.Common.Utils.ComponentModel;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Styles;
using Core.Plugins.SharpMapGis.Gui.Properties;

namespace Core.Plugins.SharpMapGis.Gui.Forms.GridProperties
{
    [ResourcesDisplayName(typeof(Resources), "VectorLayerLineProperties_DisplayName")]
    public class VectorLayerLineProperties : LineStylePropertiesBase<VectorLayer>
    {
        [DynamicReadOnly]
        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "Common_Name_DisplayName")]
        [ResourcesDescription(typeof(Resources), "VectorLayerProperties_Name_Description")]
        public string Name
        {
            get
            {
                return data.Name;
            }
            set
            {
                data.Name = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [DisplayName("Opacity")]
        [Description("Defines the layer opacity, expressed as a value between 0.0 and 1.0. A value of 0.0 indicates fully transparent, whereas a value of 1.0 indicates fully opaque.")]
        public float Opacity
        {
            get
            {
                return data.Opacity;
            }
            set
            {
                data.Opacity = (float) Math.Min(1.0, Math.Max(0.0, value));
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Coordinates")]
        [DisplayName("Map coordinate system")]
        [Description("Coordinate system (geographic or projected) on which the map is represented.")]
        public string MapCoordinateSystem
        {
            get
            {
                return data.CoordinateTransformation == null || data.CoordinateTransformation.TargetCS == null ? "<empty>" : data.CoordinateTransformation.TargetCS.Name;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_Coordinates")]
        [DisplayName("Layer contents coordinate system")]
        [Description("Coordinate system (geographic or projected) in which the objects contained in the selected layer are declared.")]
        public string LayerCoordinateSystem
        {
            get
            {
                return data.CoordinateSystem == null ? "<empty>" : data.CoordinateSystem.Name;
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool OnIsLayerNameReadOnly(string propertyName)
        {
            return data.NameIsReadOnly;
        }

        [DynamicVisibleValidationMethod]
        public override bool IsPropertyVisible(string propertyName)
        {
            return data.Theme == null;
        }

        [Browsable(false)]
        protected override VectorStyle Style
        {
            get
            {
                return data.Style;
            }
        }
    }
}