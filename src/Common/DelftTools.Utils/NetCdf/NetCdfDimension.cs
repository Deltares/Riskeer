namespace DelftTools.Utils.NetCdf
{
    public class NetCdfDimension
    {
        private int Id { get; set; }
        
        public NetCdfDimension(int id)
        {
            Id = id;
        }
        
        public static implicit operator int(NetCdfDimension ncDim)
        {
            return ncDim.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is NetCdfDimension && ((NetCdfDimension) obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}