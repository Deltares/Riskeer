﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// DO NOT MODIFY THIS FILE! It is regenerated by the designer.
// All your modifications will be lost!
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;
using Ranorex.Core.Repository;

namespace AutomatedSystemTests
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The OpenProject recording.
    /// </summary>
    [TestModule("3f746bb2-6845-45b4-9b28-b7fd605378f3", ModuleType.Recording, 1)]
    public partial class OpenProject : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static OpenProject instance = new OpenProject();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public OpenProject()
        {
            fileNameToOpen = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static OpenProject Instance
        {
            get { return instance; }
        }

#region Variables

        string _fileNameToOpen;

        /// <summary>
        /// Gets or sets the value of variable fileNameToOpen.
        /// </summary>
        [TestVariable("3c870d6e-0774-4179-b1f4-65350b234510")]
        public string fileNameToOpen
        {
            get { return _fileNameToOpen; }
            set { _fileNameToOpen = value; }
        }

#endregion

        /// <summary>
        /// Starts the replay of the static recording <see cref="Instance"/>.
        /// </summary>
        [System.CodeDom.Compiler.GeneratedCode("Ranorex", global::Ranorex.Core.Constants.CodeGenVersion)]
        public static void Start()
        {
            TestModuleRunner.Run(Instance);
        }

        /// <summary>
        /// Performs the playback of actions in this recording.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        [System.CodeDom.Compiler.GeneratedCode("Ranorex", global::Ranorex.Core.Constants.CodeGenVersion)]
        void ITestModule.Run()
        {
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 20;
            Delay.SpeedFactor = 0.00;

            Init();

            // Click on shortcut Open file
            Report.Log(ReportLevel.Info, "Mouse", "Click on shortcut Open file\r\nMouse Left Click item 'RiskeerMainWindow.Ribbon.UpperButtonsContainer.OpenProjectButton' at Center.", repo.RiskeerMainWindow.Ribbon.UpperButtonsContainer.OpenProjectButtonInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.Ribbon.UpperButtonsContainer.OpenProjectButton.Click();
            
            // Assign file name to open
            Report.Log(ReportLevel.Info, "Set value", "Assign file name to open\r\nSetting attribute Text to '$fileNameToOpen' on item 'OpenDialog.FileNameField'.", repo.OpenDialog.FileNameFieldInfo, new RecordItemIndex(1));
            repo.OpenDialog.FileNameField.Element.SetAttributeValue("Text", fileNameToOpen);
            
            // Click on open button
            Report.Log(ReportLevel.Info, "Mouse", "Click on open button\r\nMouse Left Click item 'OpenDialog.ButtonOpen' at Center.", repo.OpenDialog.ButtonOpenInfo, new RecordItemIndex(2));
            repo.OpenDialog.ButtonOpen.Click();
            
            // Wait time (300ms) so that dialog is started up
            Report.Log(ReportLevel.Info, "Delay", "Wait time (300ms) so that dialog is started up\r\nWaiting for 300ms.", new RecordItemIndex(3));
            Delay.Duration(300, false);
            
            // Wait until file has been loaded and open dialog has been closed
            Report.Log(ReportLevel.Info, "Wait", "Wait until file has been loaded and open dialog has been closed\r\nWaiting 30s to not exist. Associated repository item: 'ActivityProgressDialog.ProgressBar'", repo.ActivityProgressDialog.ProgressBarInfo, new ActionTimeout(30000), new RecordItemIndex(4));
            repo.ActivityProgressDialog.ProgressBarInfo.WaitForNotExists(30000);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
