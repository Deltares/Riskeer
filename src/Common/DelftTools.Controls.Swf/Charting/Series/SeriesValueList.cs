using Steema.TeeChart.Styles;

namespace DelftTools.Controls.Swf.Charting.Series
{
    internal class SeriesValueList : ISeriesValueList
    {
        private readonly ValueList valuelist;

        public SeriesValueList(ValueList list)
        {
            valuelist = list;
        }

        public string DataMember
        {
            get
            {
                return valuelist.DataMember;
            }
            set
            {
                valuelist.DataMember = value;
            }
        }
    }
}