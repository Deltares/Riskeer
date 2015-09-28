namespace DelftTools.Utils.NetCdf
{
    public class NetCdfVariable
    {
        private int Id { get; set; }
        
        public NetCdfVariable(int id)
        {
            Id = id;
        }
        
        public static implicit operator int(NetCdfVariable ncVar)
        {
            return ncVar.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is NetCdfVariable && ((NetCdfVariable)obj).Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}