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

namespace AutomatedSystemTests.Modules.IO
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The SaveAs recording.
    /// </summary>
    [TestModule("9195acd9-b037-4ace-80c3-0f7f64c35452", ModuleType.Recording, 1)]
    public partial class SaveAs : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static SaveAs instance = new SaveAs();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SaveAs()
        {
            fileName = "";
            scriptOutputPath = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static SaveAs Instance
        {
            get { return instance; }
        }

#region Variables

        string _fileName;

        /// <summary>
        /// Gets or sets the value of variable fileName.
        /// </summary>
        [TestVariable("d40271fe-a08b-44b0-bb48-a991f40e104b")]
        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        string _scriptOutputPath;

        /// <summary>
        /// Gets or sets the value of variable scriptOutputPath.
        /// </summary>
        [TestVariable("75a65d50-6a8f-4582-9f63-7c20d0f1ae9f")]
        public string scriptOutputPath
        {
            get { return _scriptOutputPath; }
            set { _scriptOutputPath = value; }
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

            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'RiskeerMainWindow' at UpperCenter.", repo.RiskeerMainWindow.SelfInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.Self.Click(Location.UpperCenter);
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key 'Ctrl+Shift+S' Press.", new RecordItemIndex(1));
            Keyboard.Press(System.Windows.Forms.Keys.S | System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.Control, 31, Keyboard.DefaultKeyPressTime, 1, true);
            
            //Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'RiskeerMainWindow.Ribbon.Bestand' at Center.", repo.RiskeerMainWindow.Ribbon.BestandInfo, new RecordItemIndex(2));
            //repo.RiskeerMainWindow.Ribbon.Bestand.Click();
            
            //Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'RiskeerMainWindow.Ribbon.ButtonMenuFileSaveProjectAs' at Center.", repo.RiskeerMainWindow.Ribbon.ButtonMenuFileSaveProjectAsInfo, new RecordItemIndex(3));
            //repo.RiskeerMainWindow.Ribbon.ButtonMenuFileSaveProjectAs.Click();
            
            AddWorkingDirectoryToFileNameIfRelativeFileName();
            
            Report.Log(ReportLevel.Info, "User", "Name of file to save:", new RecordItemIndex(5));
            
            Report.Log(ReportLevel.Info, "User", fileName, new RecordItemIndex(6));
            
            Report.Log(ReportLevel.Info, "Set value", "Setting attribute Text to '$fileName' on item 'OpslaanAls.SaveAsFileName'.", repo.OpslaanAls.SaveAsFileNameInfo, new RecordItemIndex(7));
            repo.OpslaanAls.SaveAsFileName.Element.SetAttributeValue("Text", fileName);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'OpslaanAls.SaveButton' at Center.", repo.OpslaanAls.SaveButtonInfo, new RecordItemIndex(8));
            repo.OpslaanAls.SaveButton.Click();
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 50ms.", new RecordItemIndex(9));
            Delay.Duration(50, false);
            
            ConfirmOverwrite(repo.ButtonYesInfo);
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 250ms.", new RecordItemIndex(11));
            Delay.Duration(250, false);
            
            Report.Log(ReportLevel.Info, "Wait", "Waiting 5m to not exist. Associated repository item: 'ActivityProgressDialog.ButtonCancel'", repo.ActivityProgressDialog.ButtonCancelInfo, new ActionTimeout(300000), new RecordItemIndex(12));
            repo.ActivityProgressDialog.ButtonCancelInfo.WaitForNotExists(300000);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
