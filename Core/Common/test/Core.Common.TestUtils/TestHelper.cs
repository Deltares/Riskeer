using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Core.Common.Utils.IO;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Core.Common.TestUtils
{
    public class TestHelper
    {
        private static string solutionRoot;
        private static int assertInTestMethod;
        private static string lastTestName;
        private static Color[] colors;

        public static string SolutionRoot
        {
            get
            {
                if (solutionRoot == null)
                {
                    solutionRoot = GetSolutionRoot();
                }
                return solutionRoot;
            }
        }

        //TODO: Replace this property
        public static string TestDataDirectory
        {
            get
            {
                return Path.GetDirectoryName(SolutionRoot);
            }
        }

        public static string GetCurrentMethodName()
        {
            MethodBase callingMethod = new StackFrame(1, false).GetMethod();
            return callingMethod.DeclaringType.Name + "." + callingMethod.Name;
        }

        /// <summary>
        /// Returns full path to the file or directory in "test-data"
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetTestDataPath(TestDataPath path)
        {
            return Path.Combine(TestDataDirectory, path.Path);
        }

        /// <summary>
        /// Returns full path to the file or directory in "test-data"
        /// </summary>
        /// <param name="testDataPath"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetTestDataPath(TestDataPath testDataPath, string path)
        {
            return Path.Combine(Path.Combine(TestDataDirectory, testDataPath.Path),"test-data", path);
        }

        public static string GetTestProjectDirectory()
        {
            var stackFrames = new StackTrace().GetFrames();
            if (stackFrames == null)
            {
                throw new Exception("Could not get stacktrace.");
            }

            var testMethod = stackFrames.FirstOrDefault(f => f.GetMethod().GetCustomAttributes(typeof(TestAttribute), true).Any() ||
                                                             f.GetMethod().GetCustomAttributes(typeof(SetUpAttribute), true).Any() ||
                                                             f.GetMethod().GetCustomAttributes(typeof(TestFixtureSetUpAttribute), true).Any());

            if (testMethod == null)
            {
                throw new Exception("Could not determine the test method.");
            }

            var testClassType = testMethod.GetMethod().DeclaringType;
            if (testClassType == null)
            {
                throw new Exception("Could not find test class type.");
            }

            return Path.GetDirectoryName((new Uri(testClassType.Assembly.CodeBase)).AbsolutePath);
        }

        /// <summary>
        /// Gets the test-data directory for the current test project.
        /// </summary>
        public static string GetDataDir()
        {
            var testProjectDirectory = GetTestProjectDirectory();
            var rootedTestProjectFolderPath = Path.GetFullPath(Path.Combine(testProjectDirectory, "..", ".."));

            return Path.GetFullPath(Path.Combine(rootedTestProjectFolderPath, "test-data") + Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Get's the path in test-data tree section
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetTestFilePath(string filename)
        {
            var path = Path.Combine(GetDataDir(), filename);
            var uri = new UriBuilder(path);

            path = Uri.UnescapeDataString(uri.Path);
            if (File.Exists(path))
            {
                return path;
            }

            // file not found..exception
            throw new FileNotFoundException(String.Format("File not found: {0}", path), path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxMilliseconds"></param>
        /// <param name="action"></param>
        /// <param name="rankHddAccess">Take HDD speed into account, makes sure that test timing is divided by MACHINE_HDD_PERFORMANCE_RANK environmental variable.</param>
        /// <returns></returns>
        public static double AssertIsFasterThan(float maxMilliseconds, Action action, bool rankHddAccess = false)
        {
            return AssertIsFasterThan(maxMilliseconds, null, action, rankHddAccess);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxMilliseconds"></param>
        /// <param name="message"></param>
        /// <param name="action"></param>
        /// <param name="rankHddAccess">Take HDD speed into account, makes sure that test timing is divided by MACHINE_HDD_PERFORMANCE_RANK environmental variable.</param>
        /// <returns></returns>
        public static double AssertIsFasterThan(float maxMilliseconds, string message, Action action, bool rankHddAccess)
        {
            var stopwatch = new Stopwatch();
            double actualMillisecond = default(double);

            stopwatch.Start();
            action();
            stopwatch.Stop();

            actualMillisecond = Math.Abs(actualMillisecond - default(double)) > 1e-5
                ? Math.Min(stopwatch.ElapsedMilliseconds, actualMillisecond)
                : stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();

            string testName = GetCurrentTestClassMethodName();

            if (testName == lastTestName) // check if there are more than one assert in a single test
            {
                assertInTestMethod++;
                testName += assertInTestMethod;
            }
            else
            {
                lastTestName = testName;
                assertInTestMethod = 1; // reset
            }

            float machinePerformanceRank = GetMachinePerformanceRank();

            float machineHddPerformanceRank = GetMachineHddPerformanceRank();

            var reportDirectory = GetSolutionRoot() + Path.DirectorySeparatorChar + "target/";
            FileUtils.CreateDirectoryIfNotExists(reportDirectory);

            var path = reportDirectory + "performance-times.html";
            WriteTimesToLogFile(maxMilliseconds, (int) actualMillisecond, machinePerformanceRank,
                                machineHddPerformanceRank, rankHddAccess, testName, false, path);

            path = reportDirectory + "performance-times-charts.html";
            WriteTimesToLogFile(maxMilliseconds, (int) actualMillisecond, machinePerformanceRank,
                                machineHddPerformanceRank, rankHddAccess, testName, true, path);

            float rank = machineHddPerformanceRank;

            if (rankHddAccess) // when test relies a lot on HDD - multiply rank by hdd speed factor
            {
                rank *= machineHddPerformanceRank;
            }

            var userMessage = String.IsNullOrEmpty(message) ? "" : message + ". ";
            if (rank != 1.0f)
            {
                Assert.IsTrue(rank*actualMillisecond < maxMilliseconds, userMessage + "Maximum of {0} milliseconds exceeded. Actual was {1}, machine performance weighted actual was {2}",
                              maxMilliseconds, actualMillisecond, actualMillisecond*rank);
                Console.WriteLine(userMessage + String.Format("Test took {1} milliseconds (machine performance weighted {2}). Maximum was {0}",
                                                              maxMilliseconds, actualMillisecond, actualMillisecond*rank));
            }
            else
            {
                Assert.IsTrue(actualMillisecond < maxMilliseconds, userMessage + "Maximum of {0} milliseconds exceeded. Actual was {1}", maxMilliseconds,
                              actualMillisecond);
                Console.WriteLine(userMessage + String.Format("Test took {1} milliseconds. Maximum was {0}", maxMilliseconds, actualMillisecond));
            }

            return actualMillisecond;
        }

        /// <summary>
        /// Checks if the given messages occurs in the log.
        /// </summary>
        /// <param name="action">Action to be performed while recording the log</param>
        /// <param name="message">The message that should occur in the log</param>
        /// <param name="expectedLogMessageCount">Optional: assert that log has this number of messages.</param>
        public static void AssertLogMessageIsGenerated(Action action, string message, int? expectedLogMessageCount = null)
        {
            AssertLogMessagesAreGenerated(action, new[]
            {
                message
            }, expectedLogMessageCount);
        }

        /// <summary>
        /// Method used to check if a collection of messages have occured in the log.
        /// This function allowed checking log messages being generated like <see cref="AssertLogMessageIsGenerated"/>
        /// without having to rerun the action for every single message. Fails the test when any of checks fail.
        /// </summary>
        /// <param name="action">Action to be performed while recording the log</param>
        /// <param name="messages">The collection of messages that should occur in the log</param>
        /// <param name="expectedLogMessageCount">Optional: assert that log has this number of messages.</param>
        /// <seealso cref="AssertLogMessageIsGenerated"/>
        public static void AssertLogMessagesAreGenerated(Action action, IEnumerable<string> messages, int? expectedLogMessageCount = null)
        {
            var renderedMessages = GetAllRenderedMessages(action);

            AssertExpectedMessagesInRenderedMessages(messages, renderedMessages);
            if (expectedLogMessageCount != null)
            {
                Assert.AreEqual((int) expectedLogMessageCount, renderedMessages.Count());
            }
        }

        /// <summary>
        /// Method use to perform any type of assertion on the generated log while performing
        /// a particular action.
        /// </summary>
        /// <param name="action">Action to be performed while recording the log.</param>
        /// <param name="assertLogMessages">The assertion logic performed on the generated log-messages.</param>
        public static void AssertLogMessages(Action action, Action<IEnumerable<string>> assertLogMessages)
        {
            var renderedMessages = GetAllRenderedMessages(action);
            assertLogMessages(renderedMessages);
        }

        /// <summary>
        /// Method used to check if a specific message is not in the log.
        /// This function allowed checking log messages being generated like <see cref="AssertLogMessageIsGenerated"/>
        /// without having to rerun the action for every single message.
        /// </summary>
        /// <param name="action">The action to be performed that should not generate <paramref name="unwantedMessage"/></param>
        /// <param name="unwantedMessage">The log message that should not occur in the log</param>
        /// <param name="expectedLogMessageCount">Optional: assert that log has this number of messages.</param>
        /// <seealso cref="AssertLogMessagesAreNotGenerated"/>
        [Obsolete("Please keep in mind that asserting unwanted messages are 'very easy to pass' asserts and can pass by a random change of 1 character.")]
        public static void AssertLogMessageIsNotGenerated(Action action, string unwantedMessage, int? expectedLogMessageCount = null)
        {
            AssertLogMessagesAreNotGenerated(action, new[]
            {
                unwantedMessage
            }, expectedLogMessageCount);
        }

        /// <summary>
        /// Method used to check if a collection of messages have not occurred in the log.
        /// This function allowed checking log messages being generated like <see cref="AssertLogMessageIsGenerated"/>
        /// without having to rerun the action for every single message. Fails the test when any of checks fail.
        /// </summary>
        /// <param name="action">The action to be performed that should not generate any of the messages in <paramref name="unwantedMessages"/></param>
        /// <param name="unwantedMessages">The log messages that should not occur in the log</param>
        /// <param name="expectedLogMessageCount">Optional: assert that log has this number of messages.</param>
        /// <seealso cref="AssertLogMessageIsGenerated"/>
        [Obsolete("Please keep in mind that asserting unwanted messages are 'very easy to pass' asserts and can pass by a random change of 1 character.")]
        public static void AssertLogMessagesAreNotGenerated(Action action, IEnumerable<string> unwantedMessages, int? expectedLogMessageCount = null)
        {
            var renderedMessages = GetAllRenderedMessages(action);

            AssertUnwantedMessagesNotInRenderedMessages(unwantedMessages, renderedMessages);
            if (expectedLogMessageCount != null)
            {
                Assert.AreEqual((int) expectedLogMessageCount, renderedMessages.Count());
            }
        }

        /// <summary>
        /// Checks if all expected messages occur in the log, 
        /// while none of the unwanted messages should occur in the log.
        /// </summary>
        /// <param name="action">Action to be performed while recording the log</param>
        /// <param name="expectedMessages">Collection of expected messages that should be in the log</param>
        /// <param name="unwantedMessages">Collection of messages of which none should be in the log</param>
        /// <param name="expectedLogMessageCount">Optional: assert that log has this number of messages.</param>
        [Obsolete("Please keep in mind that asserting unwanted messages are 'very easy to pass' asserts and can pass by a random change of 1 character.")]
        public static void AssertLogExpectedAndUnwantedMessages(Action action, IEnumerable<string> expectedMessages, IEnumerable<string> unwantedMessages, int? expectedLogMessageCount = null)
        {
            var renderedMessages = GetAllRenderedMessages(action);

            AssertUnwantedMessagesNotInRenderedMessages(unwantedMessages, renderedMessages);
            AssertExpectedMessagesInRenderedMessages(expectedMessages, renderedMessages);
            if (expectedLogMessageCount != null)
            {
                Assert.AreEqual((int) expectedLogMessageCount, renderedMessages.Count());
            }
        }

        /// <summary>
        /// Checks the number of messages in the log.
        /// </summary>
        /// <param name="action">Action to be performed while recording the log</param>
        /// <param name="count">The expected number of messages</param>
        public static void AssertLogMessagesCount(Action action, int count)
        {
            var renderedMessages = GetAllRenderedMessages(action);

            Assert.AreEqual(count, renderedMessages.Count());
        }

        private static string GetCurrentTestClassMethodName()
        {
            var stackTrace = new StackTrace(false);

            for (int i = 1; i < stackTrace.FrameCount; i++)
            {
                StackFrame stackFrame = stackTrace.GetFrame(i);
                if (stackFrame.GetMethod().GetCustomAttributes(true).OfType<TestAttribute>().Count() != 0)
                {
                    MethodBase method = stackFrame.GetMethod();
                    return method.DeclaringType.Name + "." + method.Name;
                }
            }

            return "<unknown test method>";
        }

        private static string GetSolutionRoot()
        {
            const string solutionName = "Ringtoets.sln";
            //get the current directory and scope up
            //TODO find a faster safer method 
            string curDir = ".";
            while (Directory.Exists(curDir) && !File.Exists(curDir + @"\" + solutionName))
            {
                curDir += "/../";
            }

            if (!File.Exists(Path.Combine(curDir, solutionName)))
            {
                throw new InvalidOperationException("Solution file not found.");
            }

            return Path.GetFullPath(curDir);
        }

        private static float GetMachineHddPerformanceRank()
        {
            string rank = Environment.GetEnvironmentVariable("MACHINE_HDD_PERFORMANCE_RANK");
            if (!String.IsNullOrEmpty(rank))
            {
                return Single.Parse(rank);
            }

            return 1.0f;
        }

        private static float GetMachinePerformanceRank()
        {
            string rank = Environment.GetEnvironmentVariable("MACHINE_PERFORMANCE_RANK");
            if (!String.IsNullOrEmpty(rank))
            {
                return Single.Parse(rank);
            }

            return 1.0f;
        }

        private static void WriteTimesToLogFile(float maxMilliseconds, float actualMilliseconds, float machineRank, float machineHddRank, bool useHddAccessRank, string testName, bool includeCharts, string path)
        {
            if (!File.Exists(path))
            {
                if (!includeCharts)
                {
                    File.AppendAllText(path, @"<a href=""performance-times-charts.html"" target=""_blank"">View with charts</a><br />");
                }

                if (machineRank != 1.0f)
                {
                    File.AppendAllText(path, String.Format("Machine performance rank (multiplier):{0}<br />", machineRank));
                    File.AppendAllText(path, @"Time is in milliseconds<br /><br />");
                    File.AppendAllText(path, String.Format("<table border=\"1\">\n<tr><th>Time</th><th>Name</th>{0}<th>MaxTime</th><th>ActualTime</th><th>RankedActualTime</th><th>Percentage</th></tr>", includeCharts ? "<th>Chart</th>" : ""));
                }
                else
                {
                    File.AppendAllText(path, @"Time is in milliseconds<br /><br />");
                    File.AppendAllText(path, String.Format("<table border=\"1\">\n<tr><th>Time</th><th>Name</th>{0}<th>MaxTime</th><th>ActualTime</th><th>Percentage</th></tr>", includeCharts ? "<th>Chart</th>" : ""));
                }
            }

            string contents;

            float rank = machineRank*(useHddAccessRank ? machineHddRank : 1.0f);

            var chartContent = includeCharts ? String.Format("<td><iframe style=\"width:950px;height:350px;border:none\" src=\"performace-test-reports\\{0}.json.html\"></iframe></td>", testName) : "";

            float fraction;
            if (machineRank != 1.0f)
            {
                contents = String.Format(CultureInfo.InvariantCulture,
                                         "<tr><td>{0}</td><td><a href=\"performace-test-reports/{1}.json.html\">{1}</a></td>{2}<td>{3:G5}</td><td>{4:G5}</td><td>{5:G5}</td>",
                                         DateTime.Now, testName, chartContent, maxMilliseconds, actualMilliseconds, actualMilliseconds*rank);
                fraction = (maxMilliseconds - actualMilliseconds*rank)/maxMilliseconds;
            }
            else
            {
                contents = String.Format(CultureInfo.InvariantCulture,
                                         "<tr><td>{0}</td><td><a href=\"performace-test-reports/{1}.json.html\">{1}</a></td>{2}<td>{3:G5}</td><td>{4:G5}</td>", DateTime.Now, testName, chartContent,
                                         maxMilliseconds, actualMilliseconds);
                fraction = (maxMilliseconds - actualMilliseconds)/maxMilliseconds;
            }

            string color = ColorTranslator.ToHtml(GetPerformanceColor(fraction));
            contents += String.Format(CultureInfo.InvariantCulture, "<td bgcolor=\"{0}\">{1:G5}%</td>", color, (100 - fraction*100));

            contents += "</tr>\n";
            File.AppendAllText(path, contents);

            // update test reports in JSON files on build server (tests statistics)
            // TODO: find way to write it somewhere so that it will be shared between build agents

            int buildNumber = 0;
            string s = Environment.GetEnvironmentVariable("BUILD_NUMBER");
            if (!String.IsNullOrEmpty(s))
            {
                buildNumber = Int32.Parse(s); // defined on build server
            }

            // generate JSON files locally
            string testHistoryDirectoryPath = GetSolutionRoot() + "/target/performace-test-reports";
            FileUtils.CreateDirectoryIfNotExists(testHistoryDirectoryPath);

            string testHistoryFilePath = testHistoryDirectoryPath + Path.DirectorySeparatorChar + testName + ".json";

            var testInfos = new List<TestRunInfo>();

            if (File.Exists(testHistoryFilePath))
            {
                testInfos = JsonConvert.DeserializeObject<List<TestRunInfo>>(File.ReadAllText(testHistoryFilePath));
            }

            if (buildNumber == 0)
            {
                if (testInfos == null || testInfos.Count == 0)
                {
                    testInfos = new List<TestRunInfo>();
                }
                else
                {
                    var maxBuildNumber = testInfos.Select(i => i.BuildNumber).Max();

                    // reset build numbers if they were not set
                    foreach (var testInfo in testInfos)
                    {
                        if (testInfo.BuildNumber == 0)
                        {
                            testInfo.BuildNumber = buildNumber;
                            buildNumber++;
                        }
                    }

                    buildNumber = maxBuildNumber + 1;
                }
            }

            int maxTestInfoCount = 100; // max number of tests locally

            while (testInfos.Count >= maxTestInfoCount)
            {
                testInfos.RemoveAt(0);
            }

            testInfos.Add(new TestRunInfo
            {
                TestName = testName,
                Actual = actualMilliseconds,
                ActualWeighted =
                    (int) (actualMilliseconds*machineRank*(useHddAccessRank ? machineHddRank : 1.0)),
                Max = maxMilliseconds,
                MachineHddRank = machineHddRank,
                MachineRank = machineRank,
                Time = DateTime.Now.ToString(CultureInfo.InvariantCulture),
                BuildNumber = buildNumber,
                UseMachineHddRank = useHddAccessRank
            });

            CreateChart(testInfos, testHistoryFilePath + ".html");

            string json = JsonConvert.SerializeObject(testInfos, Formatting.Indented);
            File.WriteAllText(testHistoryFilePath, json);
        }

        private static void CreateChart(List<TestRunInfo> testInfos, string filePath)
        {
            var content = File.ReadAllText(SolutionRoot + "/Core/Common/test/Core.Common.TestUtils/test-data/chart.template.html");

            var seriesPassed = "";
            var seriesFailed = "";
            var seriesThreshold = "";
            for (var i = 0; i < testInfos.Count; i++)
            {
                var info = testInfos[i];
                if (info.Actual > info.Max)
                {
                    seriesFailed += "[" + info.BuildNumber + ", " + info.ActualWeighted + "], ";
                }
                else
                {
                    seriesPassed += "[" + info.BuildNumber + ", " + info.ActualWeighted + "], ";
                }

                seriesThreshold += "[" + info.BuildNumber + ", " + info.Max + "], ";
            }

            content = content.Replace("$$SERIES_PASSED$$", seriesPassed);
            content = content.Replace("$$SERIES_FAILED$$", seriesFailed);
            content = content.Replace("$$SERIES_THRESHOLD$$", seriesThreshold);

            File.WriteAllText(filePath, content);
        }

        private static Color GetPerformanceColor(double fraction)
        {
            if (fraction < 0)
            {
                return Color.Red;
            }

            if (colors == null)
            {
                var bitmap = new Bitmap(101, 1);
                Graphics graphics = Graphics.FromImage(bitmap);

                var rectangle = new Rectangle(0, 0, 101, 1);

                var brush = new LinearGradientBrush(rectangle, Color.Green, Color.Yellow, 0.0f);

                graphics.FillRectangle(brush, rectangle);

                colors = new Color[101];
                for (int i = 0; i < 101; i++)
                {
                    colors[i] = bitmap.GetPixel(i, 0);
                }
            }

            // 25% is the best result GREEN, less or greater than goes to yellow
            var localValue = fraction >= 0.25 ? Math.Min(1, (fraction - 0.25)/0.75) : Math.Max(0, (0.25 - fraction)/0.25);

            return colors[(int) (localValue*100.0)];
        }

        /// <summary>
        /// Checks if all messages from <paramref name="messages"/> occur in <paramref name="renderedMessages"/>
        /// </summary>
        /// <param name="messages">The collection of expected messages</param>
        /// <param name="renderedMessages">The collection of messages in the log</param>
        private static void AssertExpectedMessagesInRenderedMessages(IEnumerable<string> messages, IEnumerable<string> renderedMessages)
        {
            foreach (string message in messages)
            {
                if (!renderedMessages.Contains(message))
                {
                    Assert.Fail("Message \"{0}\" not found in messages of log4net", message);
                }
            }
        }

        /// <summary>
        /// Checks if none of the messages from <paramref name="messages"/> occurs in <paramref name="renderedMessages"/>
        /// </summary>
        /// <param name="messages">The collection of unwanted messages</param>
        /// <param name="renderedMessages">The collection of log messages in the log</param>
        private static void AssertUnwantedMessagesNotInRenderedMessages(IEnumerable<string> messages, IEnumerable<string> renderedMessages)
        {
            foreach (var renderedMessage in renderedMessages)
            {
                if (messages.Contains(renderedMessage))
                {
                    Assert.Fail("Message \"{0}\" found in messages of log4net", renderedMessage);
                }
            }
        }

        private static IEnumerable<string> GetAllRenderedMessages(Action action)
        {
            var memoryAppender = new MemoryAppender();
            BasicConfigurator.Configure(memoryAppender);
            LogHelper.SetLoggingLevel(Level.All);

            action();

            var renderedMessages = memoryAppender.GetEvents().Select(le => le.RenderedMessage).ToList();

            memoryAppender.Close();
            LogHelper.ResetLogging();

            return renderedMessages;
        }

        #region Nested type: TestRunInfo

        internal class TestRunInfo
        {
            public string Time;
            public string TestName;
            public float Actual; // millis
            public float ActualWeighted; // millis
            public float Max; // millis
            public int BuildNumber;
            public float MachineHddRank;
            public float MachineRank;
            public bool UseMachineHddRank;
        }

        #endregion
    }
}