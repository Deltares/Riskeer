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

        public double this[int index]
        {
            get
            {
                return valuelist[index];
            }
        }

        public int Count
        {
            get
            {
                return valuelist.Count;
            }
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

        public bool DateTime
        {
            get
            {
                return valuelist.DateTime;
            }
            set
            {
                valuelist.DateTime = value;
            }
        }
    }
}