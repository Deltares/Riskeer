namespace DelftTools.Controls.Swf.Charting.Series
{
    public interface ISeriesValueList
    {
        int Count { get; }

        string DataMember { get; set; }

        bool DateTime { get; set; }

        double this[int index] { get; }
    }
}