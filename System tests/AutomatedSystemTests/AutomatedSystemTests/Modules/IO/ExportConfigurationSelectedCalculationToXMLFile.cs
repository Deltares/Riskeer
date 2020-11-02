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
    ///The ExportConfigurationSelectedCalculationToXMLFile recording.
    /// </summary>
    [TestModule("f9bf6401-43a0-4383-984d-457c6eee59b7", ModuleType.Recording, 1)]
    public partial class ExportConfigurationSelectedCalculationToXMLFile : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ExportConfigurationSelectedCalculationToXMLFile instance = new ExportConfigurationSelectedCalculationToXMLFile();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ExportConfigurationSelectedCalculationToXMLFile()
        {
            fileNameToSave = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ExportConfigurationSelectedCalculationToXMLFile Instance
        {
            get { return instance; }
        }

#region Variables

        string _fileNameToSave;

        /// <summary>
        /// Gets or sets the value of variable fileNameToSave.
        /// </summary>
        [TestVariable("56347b29-0010-4e17-9350-df3326ef24fd")]
        public string fileNameToSave
        {
            get { return _fileNameToSave; }
            set { _fileNameToSave = value; }
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

            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{Apps}'.", new RecordItemIndex(0));
            Keyboard.Press("{Apps}");
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'ContextMenu.Exporteren' at Center.", repo.ContextMenu.ExporterenInfo, new RecordItemIndex(1));
            repo.ContextMenu.Exporteren.Click();
            
            AddWorkingDirectoryToFileNameIfRelativeFileName();
            
            Report.Log(ReportLevel.Info, "User", "Name of XLM file to export to:", new RecordItemIndex(3));
            
            Report.Log(ReportLevel.Info, "Set value", "Setting attribute Text to '$fileNameToSave' on item 'OpslaanAls.SaveAsFileName'.", repo.OpslaanAls.SaveAsFileNameInfo, new RecordItemIndex(4));
            repo.OpslaanAls.SaveAsFileName.Element.SetAttributeValue("Text", fileNameToSave);
            
            Report.Log(ReportLevel.Info, "User", fileNameToSave, new RecordItemIndex(5));
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'OpslaanAls.SaveButton' at Center.", repo.OpslaanAls.SaveButtonInfo, new RecordItemIndex(6));
            repo.OpslaanAls.SaveButton.Click();
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 50ms.", new RecordItemIndex(7));
            Delay.Duration(50, false);
            
            ConfirmOverwrite(repo.ButtonYesInfo);
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 250ms.", new RecordItemIndex(9));
            Delay.Duration(250, false);
            
            Report.Log(ReportLevel.Info, "Wait", "Waiting 5m to not exist. Associated repository item: 'ActivityProgressDialog.ButtonCancel'", repo.ActivityProgressDialog.ButtonCancelInfo, new ActionTimeout(300000), new RecordItemIndex(10));
            repo.ActivityProgressDialog.ButtonCancelInfo.WaitForNotExists(300000);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
