namespace Ringtoets.Piping.Data
{
    public class PipingSoilLayer
    {
        public PipingSoilLayer(double top)
        {
            Top = top;
        }

        public double Top { get; private set; }
        public bool IsAquifer { get; set; }
    }
}