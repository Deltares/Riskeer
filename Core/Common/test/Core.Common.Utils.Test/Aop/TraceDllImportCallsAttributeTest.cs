using System;
using System.IO;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Test.Aop.TestClasses;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Aop
{
    [TestFixture]
    public class TraceDllImportCallsAttributeTest
    {
        [Test]
        public void TraceDllCalls()
        {
            var logPath = "native.xml";

            // without logging:
            DllImports.GetCurrentThreadId();
            DllImports.GetWindowThreadProcessId(DllImports.GetForegroundWindow(), IntPtr.Zero);

            Assert.IsFalse(TraceDllImportCallsConfig.IsLoggingEnabled(typeof(DllImports)));

            TraceDllImportCallsConfig.StartLogging(typeof(DllImports), logPath);

            // with logging:
            DllImports.GetCurrentThreadId();
            DllImports.GetWindowThreadProcessId(DllImports.GetForegroundWindow(), IntPtr.Zero);

            TraceDllImportCallsConfig.StopLogging(typeof(DllImports));

            // without logging:
            DllImports.GetCurrentThreadId();

            var expectedLineStarts = new[]
            {
                "<?xml version=\"1.0\" encoding=\"utf-8\"?>",
                "<Run type=\"DllImports\" time=",
                "  <GetCurrentThreadId />",
                "  <GetForegroundWindow />",
                "  <GetWindowThreadProcessId hWnd=",
                "</Run>"
            };

            var actualLines = File.ReadAllLines(logPath);

            Assert.AreEqual(expectedLineStarts.Length, actualLines.Length, "#num lines");
            for (int i = 0; i < expectedLineStarts.Length; i++)
            {
                var expectedStart = expectedLineStarts[i];
                var actualLine = actualLines[i];

                Assert.IsTrue(actualLine.StartsWith(expectedStart),
                              string.Format("Expected: {0}, got: {1}", expectedStart, actualLine));
            }
        }
    }
}