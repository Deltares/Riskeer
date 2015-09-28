namespace DelftTools.Utils.NetCdf
{
    public class NetCdfAttribute
    {
        public NetCdfAttribute(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public object Value
        {
            get; private set;
        }

        public string Name
        {
            get; private set;
        }
    }
}