namespace DelftTools.TestUtils
{
    /// <summary>
    /// [Uncategorized] .............. unit tests (less than 500ms), or marked with some custom categories<br/>
    ///<br/>
    /// The following categories started with prefix "Build." are used for integration tests which are handled <br/>
    /// in a special way on a build server:<br/>
    /// <br/>
    /// Build.Integration ................ general integration tests (many classes, almost no mocks, mix of windows.forms, data access)<br/>
    ///     Build.Performance. ........... integration tests which assert execution time using <see cref="TestHelper.AssertIsFasterThan(float,System.Action)"/><br/>
    /// <br/>
    /// These categories must be always used as mutually exclusive: NEVER USE 2 OF THESE CATEGORIES AT THE SAME TIME!<br/>
    ///<br/>
    /// Exceptions from the rule above (use of 2 categories at the same time):<br/>
    /// <br/>
    /// Build.WorkInProgress ......... tests which are currently in development<br/>
    /// Build.Slow ................... tests which take more than 500 ms but less than 20 s to run<br/>
    /// </summary>
    public class TestCategory
    {
        /// <summary>
        /// Checks how fast specific code runs. 
        /// Speed requirements: faster than 20000 ms
        /// <seealso cref="TestHelper.AssertIsFasterThan(float,System.Action)"/>
        /// </summary>
        public const string Performance = "Build.Performance";

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