using Assembly.Kernel.Model;
using NUnit.Framework;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil
{
    /// <summary>
    /// Class for asserting probabilities
    /// </summary>
    public static class ProbabilityAssert
    {
        /// <summary>
        /// Asserts whether <paramref name="actual"/> represents the same probability as <paramref name="expected"/>.
        /// </summary>
        /// <param name="expected">The expected probability.</param>
        /// <param name="actual">The actual probability</param>
        public static void AreEqual(double expected, Probability actual)
        {
            Assert.AreEqual(expected, actual.Value);
        }
    }
}