using System;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Forms.Test.Helper
{
    /// <summary>
    /// Helper class to create simple <see cref="PipingOutput"/>.
    /// </summary>
    public static class PipingOutputCreator
    {
        public static PipingOutput Create()
        {
            Random random = new Random(22);
            return new PipingOutput(
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble()
                );
        }
    }
}