namespace DelftTools.Controls.Swf.Charting.Series
{
    public interface ISeriesValueList
    {
        double this[int index] { get; }
        int Count { get; }

        string DataMember { get; set; }

        bool DateTime { get; set; }
    }
}