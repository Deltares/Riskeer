using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Properties;
using Steema.TeeChart.Drawing;
using Steema.TeeChart.Styles;

namespace Core.Common.Controls.Swf.Charting.Series
{
    public abstract class ChartSeries : IChartSeries
    {
        protected const int MaximumAllowedSize = 999999;
        protected const int MinimumAllowedSize = 0;
        internal readonly Steema.TeeChart.Styles.Series series;

        private string dataMember;
        private object dataSource;
        private PropertyDescriptor xPropertyDescriptor;
        private PropertyDescriptor yPropertyDescriptor;
        private BindingContext bindingContext;

        private IList<double> noDataValues = new List<double>();

        protected ChartSeries(CustomPoint series) : this((Steema.TeeChart.Styles.Series) series)
        {
            series.LinePen.Width = 1;

            SetDefaultPointerStyle(series.Pointer);
            series.TreatNulls = TreatNullsStyle.DoNotPaint;
        }

        protected ChartSeries(Steema.TeeChart.Styles.Series series)
        {
            this.series = series;

            series.bBrush = new ChartBrush();
            series.ColorEach = false;
            series.XValues.Order = ValueListOrder.None;

            //synchronized is the default
            UpdateASynchronously = false;
        }

        /// <summary>
        /// Set to force the x-axis to use date-time formatting, even if no data is present.
        /// </summary>
        public bool XAxisIsDateTime
        {
            get
            {
                return series.XValues.DateTime;
            }
            set
            {
                series.XValues.DateTime = value;
            }
        }

        /// <summary>
        /// The title of the series
        /// </summary>
        /// <remarks>Changes null to an empty string to avoid TeeChart problems (TeeChart cannot cope with null titles)</remarks>
        public string Title
        {
            get
            {
                return series.Title;
            }
            set
            {
                series.Title = value ?? "";
            }
        }

        public object DataSource
        {
            get
            {
                return dataSource;
            }
            set
            {
                Unbind();
                dataSource = value;

                series.DataSource = series; // set TeeChart specific data source to call us to get values
                try
                {
                    Bind();
                }
                catch (InvalidCastException ex)
                {
                    throw new ArgumentException(Resources.ChartSeries_DataSource_Invalid_argument_for_series_datasource__Are_you_passing_IEnumerable__IList_and_IListSource_are_supported, ex);
                }
            }
        }

        public string XValuesDataMember
        {
            get
            {
                return series.XValues.DataMember;
            }
            set
            {
                series.XValues.DataMember = value;
                Unbind();
                Bind();
            }
        }

        public string YValuesDataMember
        {
            get
            {
                return series.YValues.DataMember;
            }
            set
            {
                series.YValues.DataMember = value;
                Unbind();
                Bind();
            }
        }

        public VerticalAxis VertAxis
        {
            get
            {
                string verticalAxisName = Enum.GetName(typeof(Steema.TeeChart.Styles.VerticalAxis), series.VertAxis);
                return (VerticalAxis) Enum.Parse(typeof(VerticalAxis), verticalAxisName);
            }
            set
            {
                string verticalAxisName = Enum.GetName(typeof(VerticalAxis), value);
                series.VertAxis = (Steema.TeeChart.Styles.VerticalAxis) Enum.Parse(typeof(Steema.TeeChart.Styles.VerticalAxis), verticalAxisName);
            }
        }

        public bool Visible
        {
            get
            {
                return series.Visible;
            }
            set
            {
                series.Visible = value;
            }
        }

        public bool ShowInLegend
        {
            get
            {
                return series.ShowInLegend;
            }
            set
            {
                series.ShowInLegend = value;
            }
        }

        public double DefaultNullValue
        {
            get
            {
                return series.DefaultNullValue;
            }
            set
            {
                series.DefaultNullValue = value;
            }
        }

        public IList<double> NoDataValues
        {
            get
            {
                return noDataValues;
            }
            set
            {
                noDataValues = value;
            }
        }

        public bool UpdateASynchronously { get; set; }

        public object Tag { get; set; }

        public bool RefreshRequired { get; private set; }

        public IChart Chart { get; set; }

        public abstract Color Color { get; set; }

        public void CheckDataSource()
        {
            if (DataSource != null && (CurrencyManager == null || CurrencyManager.Count == 0))
            {
                return;
            }

            series.CheckDataSource();
        }

        public double XScreenToValue(int x)
        {
            return series.XScreenToValue(x);
        }

        public void Refresh()
        {
            FillValues();
            CheckDataSource();
            RefreshRequired = false;
        }

        public void Add(DateTime dateTime, double value)
        {
            series.Add(dateTime, value);
        }

        public void Add(double? x, double? y)
        {
            series.Add(x, y);
        }

        public void Add(double?[] xValues, double?[] yValues)
        {
            series.Add(xValues, yValues);
        }

        public void Clear()
        {
            series.Clear();
        }

        protected void CopySettings(IChartSeries chartSeries)
        {
            Title = chartSeries.Title;

            if (chartSeries.DataSource == null)
            {
                var teeChartSeries = ((ChartSeries) chartSeries).series;

                for (int i = 0; i < teeChartSeries.XValues.Count; i++)
                {
                    series.Add(teeChartSeries.XValues[i], teeChartSeries.YValues[i]);
                }
            }
            else
            {
                DataSource = chartSeries.DataSource;
                XValuesDataMember = chartSeries.XValuesDataMember;
                YValuesDataMember = chartSeries.YValuesDataMember;
                RefreshRequired = true;
            }

            VertAxis = chartSeries.VertAxis;
            Visible = chartSeries.Visible;
            ShowInLegend = chartSeries.ShowInLegend;
            DefaultNullValue = chartSeries.DefaultNullValue;
            noDataValues = new List<double>(chartSeries.NoDataValues);
            UpdateASynchronously = chartSeries.UpdateASynchronously;
            Color = chartSeries.Color;

            if (series is CustomPoint && chartSeries.Chart != null && chartSeries.Chart.StackSeries)
            {
                ((CustomPoint) series).Stacked = CustomStack.Stack;
            }
        }

        private BindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                {
                    bindingContext = new BindingContext();
                }

                return bindingContext;
            }
        }

        private CurrencyManager CurrencyManager
        {
            get
            {
                if (dataMember == null)
                {
                    dataMember = String.Empty;
                }

                if (DataSource == null)
                {
                    return null;
                }

                return (CurrencyManager) BindingContext[DataSource, dataMember];
            }
        }

        private void SetDefaultPointerStyle(SeriesPointer seriesPointer)
        {
            seriesPointer.Pen.Color = Color.LimeGreen;
            seriesPointer.Brush = new ChartBrush(series.Chart, Color.White);
            seriesPointer.Pen.Width = 1;
            seriesPointer.Style = Steema.TeeChart.Styles.PointerStyles.Circle;
            seriesPointer.VertSize = 1;
            seriesPointer.HorizSize = 1;
            seriesPointer.Visible = true;
        }

        private void Bind()
        {
            if (DataSource == null || CurrencyManager == null)
            {
                return; // no binding
            }

            foreach (PropertyDescriptor property in CurrencyManager.GetItemProperties())
            {
                if (!property.IsBrowsable)
                {
                    continue;
                }

                if (property.DisplayName == XValuesDataMember)
                {
                    xPropertyDescriptor = property;

                    if (property.PropertyType == typeof(DateTime))
                    {
                        //log.Warn("TODO: set axis type to date time");
                    }
                }

                if (property.DisplayName == YValuesDataMember)
                {
                    yPropertyDescriptor = property;
                }
            }

            if (xPropertyDescriptor == null || yPropertyDescriptor == null)
            {
                return; // nothing to bind to yet
            }

            FillValues();

            //synchronize the changed code since it should run in the UI thread.
            CurrencyManager.ListChanged += CurrencyManagerListChanged; //OnListChanged;

            CheckDataSource();
        }

        private void CurrencyManagerListChanged(object sender, ListChangedEventArgs e)
        {
            OnListChanged();
        }

        private void FillValues()
        {
            series.Clear();
            foreach (object element in CurrencyManager.List)
            {
                Add(xPropertyDescriptor.GetValue(element), yPropertyDescriptor.GetValue(element));
            }
        }

        private void Add(object x, object y)
        {
            if (x == null || y == null)
            {
                return;
            }

            int addedIndex;

            bool xIsNumeric = xPropertyDescriptor.PropertyType == typeof(double) ||
                              xPropertyDescriptor.PropertyType == typeof(float) ||
                              xPropertyDescriptor.PropertyType == typeof(short) ||
                              xPropertyDescriptor.PropertyType == typeof(int) ||
                              xPropertyDescriptor.PropertyType == typeof(bool);

            bool yIsNumeric = yPropertyDescriptor.PropertyType == typeof(double) ||
                              yPropertyDescriptor.PropertyType == typeof(float) ||
                              yPropertyDescriptor.PropertyType == typeof(short) ||
                              yPropertyDescriptor.PropertyType == typeof(int) ||
                              yPropertyDescriptor.PropertyType == typeof(bool);

            bool xIsDateTime = xPropertyDescriptor.PropertyType == typeof(DateTime);

            if (!yIsNumeric)
            {
                throw new NotSupportedException(String.Format(Resources.ChartSeries_Add_Input_format_not_supported__y_must_be_numeric_but_is_of_type___0__, y.GetType()));
            }

            double yValue = Convert.ToDouble(y);
            double yValueToSet = Double.IsNaN(yValue) ? 0.0 : yValue;

            if (xIsNumeric)
            {
                double xValue = Convert.ToDouble(x);
                addedIndex = series.Add(xValue, yValueToSet);
            }
            else if (xIsDateTime)
            {
                addedIndex = series.Add((DateTime) x, yValueToSet);
            }
            else //x is something non-numeric, so use the index here
            {
                addedIndex = series.Add((double) series.XValues.Count - 1, yValueToSet, x.ToString());
            }

            if (NoDataValues.Contains(yValue) || Double.IsNaN(yValue))
            {
                series.SetNull(addedIndex); //make sure we don't actually display the added point
            }
        }

        private void Unbind()
        {
            if (DataSource == null)
            {
                return;
            }

            CurrencyManager.ListChanged -= CurrencyManagerListChanged;

            xPropertyDescriptor = null;
            yPropertyDescriptor = null;
        }

        private void OnListChanged()
        {
            if (UpdateASynchronously)
            {
                RefreshRequired = true;
            }
            else
            {
                //synchronous update immediately
                FillValues();
                CheckDataSource();
            }
        }
    }
}