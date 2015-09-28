namespace DelftTools.TestUtils
{
    public class TestCategory
    {
        /// <summary>
        /// Test is incomplete and unfinished.
        /// </summary>
        public const string WorkInProgress = "Build.WorkInProgress";

        /// <summary>
        /// Takes more than 500 ms but less than 20 s to run
        /// </summary>
        public const string Slow = "Build.Slow";
    }
}