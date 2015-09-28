using System;
using DelftTools.TestUtils;
using log4net.Core;
using NUnit.Framework;

namespace DeltaShell.Tests.Gui.Forms.MessageWindow
{
    [TestFixture]
    public class MessageWindowTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        [Ignore("Hangs buildserver")]
        public void ShowMessageWindowWithMessages()
        {
            LogHelper.ConfigureLogging(Level.Debug);
            using (var messageWindow = new DeltaShell.Gui.Forms.MessageWindow.MessageWindow())
            {
                messageWindow.AddMessage(Level.Debug, DateTime.Now, "source", "Debug message", "");
                messageWindow.AddMessage(Level.Info, DateTime.Now, "source", "Info message", "");
                messageWindow.AddMessage(Level.Warn, DateTime.Now, "source", "Warn message", "");
                messageWindow.AddMessage(Level.Error, DateTime.Now, "source", "Error message", "Exception : Error");

                System.Console.WriteLine("UserName: {0}", Environment.UserName);

                WindowsFormsTestHelper.ShowModal(messageWindow);
            }
            LogHelper.ResetLogging();
        }
    }
}