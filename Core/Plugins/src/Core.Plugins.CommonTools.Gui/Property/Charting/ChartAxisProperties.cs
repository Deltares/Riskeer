using System;
using System.ComponentModel;
using System.Drawing;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Gui.Properties;

namespace Core.Plugins.CommonTools.Gui.Property.Charting
{
    [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_DisplayName")]
    public abstract class ChartAxisProperties
    {
        protected readonly IChartAxis chartAxis;

        protected ChartAxisProperties(IChartAxis chartAxis)
        {
            this.chartAxis = chartAxis;
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartingProperties_Visible_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Visible_Description")]
        public bool Visible
        {
            get
            {
                return chartAxis.Visible;
            }
            set
            {
                chartAxis.Visible = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Automatic_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Automatic_Description")]
        public bool Automatic
        {
            get
            {
                return chartAxis.Automatic;
            }
            set
            {
                chartAxis.Automatic = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Title_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Title_Description")]
        public string Title
        {
            get
            {
                return chartAxis.Title;
            }
            set
            {
                chartAxis.Title = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Labels_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Labels_Description")]
        public bool Labels
        {
            get
            {
                return chartAxis.Labels;
            }
            set
            {
                chartAxis.Labels = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_LabelFont_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_LabelFont_Description")]
        [TypeConverter(typeof(ChartFontPropertiesConverter))]
        public Font LabelsFont
        {
            get
            {
                return chartAxis.LabelsFont;
            }
            set
            {
                chartAxis.LabelsFont = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_TitleFont_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_TitleFont_Description")]
        [TypeConverter(typeof(ChartFontPropertiesConverter))]
        public Font TitleFont
        {
            get
            {
                return chartAxis.TitleFont;
            }
            set
            {
                chartAxis.TitleFont = value;
            }
        }

        public override string ToString()
        {
            return Title;
        }
    }

    public class ChartAxisDoubleProperties : ChartAxisProperties
    {
        public ChartAxisDoubleProperties(IChartAxis chartAxis) : base(chartAxis) {}

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Maximum_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Maximum_Description")]
        public double Maximum
        {
            get
            {
                return chartAxis.Maximum;
            }
            set
            {
                chartAxis.Maximum = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Minimum_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Minimum_Description")]
        public double Minimum
        {
            get
            {
                return chartAxis.Minimum;
            }
            set
            {
                chartAxis.Minimum = value;
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Logarithmic_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Logarithmic_Description")]
        public bool Logaritmic
        {
            get
            {
                return chartAxis.Logaritmic;
            }
            set
            {
                chartAxis.Logaritmic = value;
            }
        }
    }

    public class ChartAxisDateTimeProperties : ChartAxisProperties
    {
        public ChartAxisDateTimeProperties(IChartAxis chartAxis)
            : base(chartAxis) {}

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Maximum_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Maximum_Description")]
        public DateTime Maximum
        {
            get
            {
                return DateTime.FromOADate(chartAxis.Maximum);
            }
            set
            {
                chartAxis.Maximum = value.ToOADate();
            }
        }

        [ResourcesCategory(typeof(Resources), "Categories_General")]
        [ResourcesDisplayName(typeof(Resources), "ChartAxisProperties_Minimum_DisplayName")]
        [ResourcesDescription(typeof(Resources), "ChartAxisProperties_Minimum_Description")]
        public DateTime Minimum
        {
            get
            {
                return DateTime.FromOADate(chartAxis.Minimum);
            }
            set
            {
                chartAxis.Minimum = value.ToOADate();
            }
        }
    }
}