// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Core.Common.TestUtil
{
    public static class TestHelper
    {
        private static string solutionRoot;

        public static string SolutionRoot
        {
            get
            {
                return solutionRoot ?? (solutionRoot = GetSolutionRoot());
            }
        }

        /// <summary>
        /// Returns a full path to a test data directory given the <paramref name="testDataPath"/>.
        /// </summary>
        /// <param name="testDataPath">The path to construct a full test data path for.</param>
        /// <returns>A full path to the test data.</returns>
        public static string GetTestDataPath(TestDataPath testDataPath)
        {
            return Path.Combine(Path.GetDirectoryName(SolutionRoot), testDataPath.Path, "test-data");
        }

        /// <summary>
        /// Returns a full path to a file or directory in the test data directory.
        /// </summary>
        /// <param name="testDataPath">The path to construct a full test data path for.</param>
        /// <param name="path">The path to a file or directory in the test data directory.</param>
        /// <returns>A full path to the file or directory from the test data directory.</returns>
        public static string GetTestDataPath(TestDataPath testDataPath, string path)
        {
            return Path.Combine(GetTestDataPath(testDataPath.Path), path);
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
        /// Asserts that the execution of some implementation of a functionality runs faster than a given allowed time.
        /// </summary>
        /// <param name="maxMilliseconds">The maximum time in milliseconds that the functionality is allowed to run.</param>
        /// <param name="action">The functionality to execute.</param>
        /// <param name="rankHddAccess">Take HDD speed into account, makes sure that test timing is divided by MACHINE_HDD_PERFORMANCE_RANK environmental variable.</param>
        public static void AssertIsFasterThan(float maxMilliseconds, Action action, bool rankHddAccess = false)
        {
            AssertIsFasterThan(maxMilliseconds, null, action, rankHddAccess);
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
        /// Method used to check if a collection of messages have occurred in the log.
        /// This function allowed checking log messages being generated like <see cref="AssertLogMessageIsGenerated"/>
        /// without having to rerun the action for every single message. Fails the test when any of checks fail.
        /// </summary>
        /// <param name="action">Action to be performed while recording the log</param>
        /// <param name="messages">The collection of messages that should occur in the log</param>
        /// <param name="expectedLogMessageCount">Optional: assert that log has this number of messages.</param>
        /// <seealso cref="AssertLogMessageIsGenerated"/>
        public static void AssertLogMessagesAreGenerated(Action action, IEnumerable<string> messages, int? expectedLogMessageCount = null)
        {
            Tuple<string, Level>[] renderedMessages = GetAllRenderedMessages(action).ToArray();

            AssertExpectedMessagesInRenderedMessages(messages, renderedMessages);
            if (expectedLogMessageCount != null)
            {
                Assert.AreEqual((int) expectedLogMessageCount, renderedMessages.Length);
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
        /// <exception cref="AssertionException">Thrown when <paramref name="actualImage"/> is not
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
        /// <exception cref="AssertionException">Thrown when <paramref name="menu"/> does not contain a menu item at
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
        /// <exception cref="AssertionException">Thrown when <paramref name="menu"/> does not contain a menu item at
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
        /// <typeparam name="T">The type of the expected <see cref="ArgumentException"/>.</typeparam>
        /// <param name="test">The test to execute and should throw <see cref="ArgumentException"/> of type <typeparamref name="T"/>.</param>
        /// <param name="expectedCustomMessage">The expected custom part of the <see cref="ArgumentException.Message"/>.</param>
        /// <return>The <see cref="ArgumentException"/> that was thrown while executing <paramref name="test"/>.</return>
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

                message = string.Join(Environment.NewLine, customMessageParts.ToArray());
            }
            Assert.AreEqual(expectedCustomMessage, message);
            return exception;
        }

        /// <summary>
        /// Method used to check if a collection of messages have occurred in the log.
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
        /// Method used to check if a collection of messages have occurred in the log.
        /// This function allowed checking log messages being generated like <see cref="AssertLogMessageIsGenerated"/>
        /// without having to rerun the action for every single message. Fails the test when any of checks fail.
        /// </summary>
        /// <param name="action">Action to be performed while recording the log</param>
        /// <param name="messages">The collection of messages that should occur in the log</param>
        /// <param name="expectedLogMessageCount">Optional: assert that log has this number of messages.</param>
        /// <seealso cref="AssertLogMessageIsGenerated"/>
        public static void AssertLogMessagesWithLevelAreGenerated(Action action, Tuple<string, LogLevelConstant>[] messages, int? expectedLogMessageCount = null)
        {
            Tuple<string, Level>[] renderedMessages = GetAllRenderedMessages(action).ToArray();

            AssertExpectedMessagesInRenderedMessages(messages, renderedMessages);
            if (expectedLogMessageCount != null)
            {
                Assert.AreEqual((int) expectedLogMessageCount, renderedMessages.Length);
            }
        }

        private static void AssertIsFasterThan(float maxMilliseconds, string message, Action action, bool rankHddAccess)
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

            var userMessage = string.IsNullOrEmpty(message) ? "" : message + ". ";
            if (!rank.Equals(1.0f))
            {
                Assert.IsTrue(rank * actualMillisecond < maxMilliseconds, userMessage + "Maximum of {0} milliseconds exceeded. Actual was {1}, machine performance weighted actual was {2}",
                              maxMilliseconds, actualMillisecond, actualMillisecond * rank);
                Console.WriteLine(userMessage + string.Format("Test took {1} milliseconds (machine performance weighted {2}). Maximum was {0}",
                                                              maxMilliseconds, actualMillisecond, actualMillisecond * rank));
            }
            else
            {
                Assert.IsTrue(actualMillisecond < maxMilliseconds, userMessage + "Maximum of {0} milliseconds exceeded. Actual was {1}", maxMilliseconds,
                              actualMillisecond);
                Console.WriteLine(userMessage + string.Format("Test took {1} milliseconds. Maximum was {0}", maxMilliseconds, actualMillisecond));
            }
        }

        private static void AssertExpectedMessagesInRenderedMessages(IEnumerable<string> messages, Tuple<string, Level>[] renderedMessages)
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
        private static void AssertExpectedMessagesInRenderedMessages(Tuple<string, LogLevelConstant>[] messages, Tuple<string, Level>[] renderedMessages)
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

        private static string GetSolutionRoot()
        {
            const string solutionName = "Ringtoets.sln";
            //get the current directory and scope up
            //TODO find a faster safer method 
            var testContext = new TestContext(TestExecutionContext.CurrentContext);
            string curDir = testContext.TestDirectory;
            while (Directory.Exists(curDir) && !File.Exists(curDir + @"\" + solutionName))
            {
                curDir += "/../";
            }

            if (!File.Exists(Path.Combine(curDir, solutionName)))
            {
                throw new InvalidOperationException(string.Format("Solution file '{0}' not found in any folder of '{1}'.",
                                                                  solutionName, 
                                                                  Directory.GetCurrentDirectory()));
            }

            return Path.GetFullPath(curDir);
        }

        private static float GetMachineHddPerformanceRank()
        {
            string rank = Environment.GetEnvironmentVariable("MACHINE_HDD_PERFORMANCE_RANK");
            if (!string.IsNullOrEmpty(rank))
            {
                return float.Parse(rank);
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
            var imageColors = new Color[image.Size.Width * image.Size.Height];
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    imageColors[index++] = bitmap.GetPixel(j, i);
                }
            }
            return imageColors;
        }
    }
}