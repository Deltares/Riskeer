using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;

namespace Core.Common.TestUtil
{
    public static class TestHelper
    {
        private static string solutionRoot;

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
            return Path.Combine(Path.Combine(TestDataDirectory, testDataPath.Path), "test-data", path);
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
        /// Checks whether the file pointed at by <paramref name="pathToFile"/> can be opened
        /// for writing.
        /// </summary>
        /// <param name="pathToFile">The location of the file to open for writing.</param>
        /// <returns><c>true</c> if the file could be opened with write permissions. <c>false</c> otherwise.</returns>
        public static bool CanOpenFileForWrite(string pathToFile)
        {
            FileStream file = null;
            try
            {
                file = File.OpenWrite(pathToFile);
                return true;
            }
            catch (IOException)
            {
                return false;
            }
            finally
            {
                if (file != null)
                {
                    file.Close();
                }
            }
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
            var actualMillisecond = default(double);

            stopwatch.Start();
            action();
            stopwatch.Stop();

            actualMillisecond = Math.Abs(actualMillisecond - default(double)) > 1e-5
                                    ? Math.Min(stopwatch.ElapsedMilliseconds, actualMillisecond)
                                    : stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();

            var machineHddPerformanceRank = GetMachineHddPerformanceRank();
            var rank = machineHddPerformanceRank;

            if (rankHddAccess) // when test relies a lot on HDD - multiply rank by hdd speed factor
            {
                rank *= machineHddPerformanceRank;
            }

            var userMessage = String.IsNullOrEmpty(message) ? "" : message + ". ";
            if (!rank.Equals(1.0f))
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
            assertLogMessages(renderedMessages.Select(rm => rm.Item1));
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

        /// <summary>
        /// Asserts that two bitmap images are equal.
        /// </summary>
        /// <param name="expectedImage">The expected image.</param>
        /// <param name="actualImage">The actual image.</param>
        /// <exception cref="AssertionException">When <paramref name="actualImage"/> is not
        /// equal to <paramref name="expectedImage"/>.</exception>
        public static void AssertImagesAreEqual(Image expectedImage, Image actualImage)
        {
            if (expectedImage == null)
            {
                Assert.IsNull(actualImage);
                return;
            }
            Assert.IsNotNull(actualImage);

            Assert.AreEqual(expectedImage.Size, actualImage.Size);
            IEnumerable<Color> expectedImageBytes = GetImageAsColorArray(expectedImage);
            IEnumerable<Color> actualImageBytes = GetImageAsColorArray(actualImage);
            CollectionAssert.AreEqual(expectedImageBytes, actualImageBytes);
        }

        /// <summary>
        /// Asserts that a <see cref="ContextMenuStrip"/> contains an item at the given <see cref="position"/>
        /// with the correct properties.
        /// </summary>
        /// <param name="menu">The <see cref="ContextMenuStrip"/> containing an item at position <paramref name="position"/>.</param>
        /// <param name="position">The position of the menu item in <paramref name="menu"/>.</param>
        /// <param name="text">The text expected for the menu item.</param>
        /// <param name="toolTip">The tooltip expected for the menu item.</param>
        /// <param name="icon">The image expected for the menu item.</param>
        /// <param name="enabled">Optional: the expected enabled state of the menu item. Default: <c>true</c>.</param>
        /// <exception cref="AssertionException">When <paramref name="menu"/> does not contain a menu item at
        /// position with the right <paramref name="text"/>, <paramref name="toolTip"/> or <paramref name="icon"/>.
        /// </exception>
        public static void AssertContextMenuStripContainsItem(ContextMenuStrip menu, int position, string text, string toolTip, Image icon, bool enabled = true)
        {
            Assert.IsNotNull(menu);
            AssertContextMenuStripContainsItem(menu.Items, position, text, toolTip, icon, enabled);
        }

        /// <summary>
        /// Asserts that a <see cref="ToolStripDropDownItem"/> contains an item at the given <see cref="position"/>
        /// with the correct properties.
        /// </summary>
        /// <param name="menu">The <see cref="ToolStripDropDownItem"/> containing an item at position <paramref name="position"/>.</param>
        /// <param name="position">The position of the menu item in <paramref name="menu"/>.</param>
        /// <param name="text">The text expected for the menu item.</param>
        /// <param name="toolTip">The tooltip expected for the menu item.</param>
        /// <param name="icon">The image expected for the menu item.</param>
        /// <param name="enabled">Optional: the expected enabled state of the menu item. Default: <c>true</c>.</param>
        /// <exception cref="AssertionException">When <paramref name="menu"/> does not contain a menu item at
        /// position with the right <paramref name="text"/>, <paramref name="toolTip"/> or <paramref name="icon"/>.
        /// </exception>
        public static void AssertDropDownItemContainsItem(ToolStripDropDownItem menu, int position, string text, string toolTip, Image icon, bool enabled = true)
        {
            Assert.IsNotNull(menu);
            AssertContextMenuStripContainsItem(menu.DropDownItems, position, text, toolTip, icon, enabled);
        }

        /// <summary>
        /// Asserts that the exception is of type <typeparamref name="T"/> and that the custom part of <see cref="Exception.Message"/> 
        /// is equal to <paramref name="expectedCustomMessage"/>.
        /// </summary>
        /// <typeparam name="T">The type of the expected exception.</typeparam>
        /// <param name="test">The test to execute and should throw exception of type <typeparamref name="T"/>.</param>
        /// <param name="expectedCustomMessage">The expected custom part of the exception message.</param>
        public static T AssertThrowsArgumentExceptionAndTestMessage<T>(TestDelegate test, string expectedCustomMessage) where T : ArgumentException
        {
            var exception = Assert.Throws<T>(test);
            var message = exception.Message;
            if (exception.ParamName != null)
            {
                var customMessageParts = message.Split(new[]
                {
                    Environment.NewLine
                }, StringSplitOptions.None).ToList();
                customMessageParts.RemoveAt(customMessageParts.Count - 1);

                message = String.Join(Environment.NewLine, customMessageParts.ToArray());
            }
            Assert.AreEqual(expectedCustomMessage, message);
            return exception;
        }

        /// <summary>
        /// Method used to check if a collection of messages have occured in the log.
        /// This function allowed checking log messages being generated like <see cref="AssertLogMessageIsGenerated"/>
        /// without having to rerun the action for every single message. Fails the test when any of checks fail.
        /// </summary>
        /// <param name="action">Action to be performed while recording the log.</param>
        /// <param name="message">The message that should occur in the log with a certain log level.</param>
        /// <param name="expectedLogMessageCount">Optional: assert that log has this number of messages.</param>
        /// <seealso cref="AssertLogMessageIsGenerated"/>
        public static void AssertLogMessageWithLevelIsGenerated(Action action, Tuple<string, LogLevelConstant> message, int? expectedLogMessageCount = null)
        {
            AssertLogMessagesWithLevelAreGenerated(action, new[]
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
        public static void AssertLogMessagesWithLevelAreGenerated(Action action, IEnumerable<Tuple<string, LogLevelConstant>> messages, int? expectedLogMessageCount = null)
        {
            var renderedMessages = GetAllRenderedMessages(action);

            AssertExpectedMessagesInRenderedMessages(messages, renderedMessages);
            if (expectedLogMessageCount != null)
            {
                Assert.AreEqual((int) expectedLogMessageCount, renderedMessages.Count());
            }
        }

        private static void AssertExpectedMessagesInRenderedMessages(IEnumerable<string> messages, IEnumerable<Tuple<string, Level>> renderedMessages)
        {
            foreach (string message in messages)
            {
                CollectionAssert.Contains(renderedMessages.Select(rm => rm.Item1), message);
            }
        }

        /// <summary>
        /// Checks if all messages from <paramref name="messages"/> occur in <paramref name="renderedMessages"/>
        /// </summary>
        /// <param name="messages">The collection of expected messages</param>
        /// <param name="renderedMessages">The collection of messages in the log</param>
        private static void AssertExpectedMessagesInRenderedMessages(IEnumerable<Tuple<string, LogLevelConstant>> messages, IEnumerable<Tuple<string, Level>> renderedMessages)
        {
            var messagesWithLog4NetLevel = messages.Select(m => Tuple.Create(m.Item1, m.Item2.ToLog4NetLevel()));
            foreach (Tuple<string, Level> message in messagesWithLog4NetLevel)
            {
                CollectionAssert.Contains(renderedMessages, message);
            }
        }

        private static void AssertContextMenuStripContainsItem(ToolStripItemCollection items, int position, string text, string toolTip, Image icon, bool enabled = true)
        {
            Assert.Less(position, items.Count);

            var item = items[position];

            Assert.AreEqual(text, item.Text);
            Assert.AreEqual(toolTip, item.ToolTipText);
            Assert.AreEqual(enabled, item.Enabled);
            AssertImagesAreEqual(icon, item.Image);
        }

        /// <summary>
        /// Create dir if not exists.
        /// </summary>
        /// <param name="path">File path to a directory.</param>
        /// <exception cref="IOException"> When:
        ///   The directory specified by <paramref name="path"/> is read-only
        /// </exception>
        /// <exception cref="UnauthorizedAccessException"> When: The caller does not have the required permission.</exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="path"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.GetInvalidPathChars"/>. -or- 
        ///   <paramref name="path"/> is prefixed with, or contains only a colon character (:).</exception>
        /// <exception cref="ArgumentNullException"><paramref name="path"/> is null.</exception>
        /// <exception cref="PathTooLongException">
        ///   The specified path, file name, or both exceed the system-defined maximum length. 
        ///   For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters. </exception>
        /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
        /// <exception cref="NotSupportedException"><paramref name="path"/> contains a colon character (:) that is not part of a drive label ("C:\").</exception>
        private static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
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

        private static IEnumerable<Tuple<string, Level>> GetAllRenderedMessages(Action action)
        {
            var memoryAppender = new MemoryAppender();
            BasicConfigurator.Configure(memoryAppender);
            LogHelper.SetLoggingLevel(Level.All);

            action();

            var renderedMessages = memoryAppender.GetEvents().Select(le => Tuple.Create(le.RenderedMessage, le.Level)).ToList();

            memoryAppender.Close();
            LogHelper.ResetLogging();

            return renderedMessages;
        }

        private static Color[] GetImageAsColorArray(Image image)
        {
            // Convert image to ARGB bitmap using 8bits/channel:
            var bitmap = new Bitmap(image).Clone(new Rectangle(0, 0, image.Size.Width, image.Size.Height), PixelFormat.Format32bppArgb);

            var index = 0;
            var imageColors = new Color[image.Size.Width*image.Size.Height];
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    imageColors[index++] = bitmap.GetPixel(i, j);
                }
            }
            return imageColors;
        }
    }
}