using System;
using Wti.Data;

namespace Wti.Forms.Test.Helper
{
    /// <summary>
    /// Helper class to create simple PipingOutput.
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