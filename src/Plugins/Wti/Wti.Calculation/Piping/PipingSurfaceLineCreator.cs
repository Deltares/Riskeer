using Deltares.WTIPiping;

namespace Wti.Calculation.Piping
{
    /// <summary>
    /// Creates <see cref="PipingSurfaceLine"/> instances which are required by the <see cref="PipingCalculation"/>.
    /// </summary>
    internal class PipingSurfaceLineCreator
    {
        public PipingSurfaceLine Create()
        {
            var surfaceLine = new PipingSurfaceLine();
            surfaceLine.Points.Add(new PipingPoint(0,0,0));

            return surfaceLine;
        }
    }
}