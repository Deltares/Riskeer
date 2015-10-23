using System.Collections.Generic;

namespace Wti.Data
{
    public class PipingSoilLayer
    {
        public PipingSoilLayer(double top)
        {
            Top = top;
        }

        public double Top { get; private set; }
    }
}