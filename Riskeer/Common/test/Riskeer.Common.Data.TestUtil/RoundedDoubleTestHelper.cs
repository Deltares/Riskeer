using Core.Common.Base.Data;
using NUnit.Framework;

namespace Riskeer.Common.Data.TestUtil
{
    /// <summary>
    /// Class for asserting <see cref="RoundedDouble"/> instances.
    /// </summary>
    public static class RoundedDoubleTestHelper
    {
        /// <summary>
        /// Asserts whether <paramref name="expectedValue"/> matches <paramref name="actualValue"/>.
        /// </summary>
        /// <param name="expectedValue">The expected <c>double</c> value.</param>
        /// <param name="actualValue">The actual <see cref="RoundedDouble"/> instance.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="expectedValue"/> doesn't match
        /// <paramref name="actualValue"/>.</exception>
        private static void AssertRoundedDouble(double? expectedValue, RoundedDouble actualValue)
        {
            Assert.IsTrue(expectedValue.HasValue);
            Assert.AreEqual(expectedValue.Value, actualValue, actualValue.GetAccuracy());
        }

        /// <summary>
        /// Asserts whether <paramref name="expectedValue"/> matches <paramref name="actualValue"/>.
        /// </summary>
        /// <param name="expectedValue">The expected <c>double</c> value.</param>
        /// <param name="actualValue">The actual <see cref="RoundedDouble"/> instance.</param>
        /// <exception cref="AssertionException">Thrown when <paramref name="expectedValue"/> doesn't match
        /// <paramref name="actualValue"/>.</exception>
        private static void AssertRoundedDouble(double expectedValue, RoundedDouble actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, actualValue.GetAccuracy());
        }
    }
}