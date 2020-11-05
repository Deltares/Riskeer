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
    ///The ImportConfigurationCalculationsFromXMLFile recording.
    /// </summary>
    [TestModule("65d26ded-850f-4a04-a479-9d3870513676", ModuleType.Recording, 1)]
    public partial class ImportConfigurationCalculationsFromXMLFile : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ImportConfigurationCalculationsFromXMLFile instance = new ImportConfigurationCalculationsFromXMLFile();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ImportConfigurationCalculationsFromXMLFile()
        {
            fileName = "";
            fileNameToImport = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ImportConfigurationCalculationsFromXMLFile Instance
        {
            get { return instance; }
        }

#region Variables

        string _fileName;

        /// <summary>
        /// Gets or sets the value of variable fileName.
        /// </summary>
        [TestVariable("30c8409f-c8d5-4b13-a879-95c2344511b7")]
        public string fileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        string _fileNameToImport;

        /// <summary>
        /// Gets or sets the value of variable fileNameToImport.
        /// </summary>
        [TestVariable("a6dd79d6-ee66-4b01-aba9-406f62250bb0")]
        public string fileNameToImport
        {
            get { return _fileNameToImport; }
            set { _fileNameToImport = value; }
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
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'ContextMenu.Importeren' at Center.", repo.ContextMenu.ImporterenInfo, new RecordItemIndex(1));
            repo.ContextMenu.Importeren.Click();
            
            AddWorkingDirectoryToFileNameIfRelativeFileName();
            
            Report.Log(ReportLevel.Info, "User", "Name of XLM file to export to:", new RecordItemIndex(3));
            
            Report.Log(ReportLevel.Info, "Set value", "Setting attribute Text to '$fileNameToImport' on item 'Openen.FileNameField'.", repo.Openen.FileNameFieldInfo, new RecordItemIndex(4));
            repo.Openen.FileNameField.Element.SetAttributeValue("Text", fileNameToImport);
            
            Report.Log(ReportLevel.Info, "User", fileNameToImport, new RecordItemIndex(5));
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'Openen.ButtonOpen' at Center.", repo.Openen.ButtonOpenInfo, new RecordItemIndex(6));
            repo.Openen.ButtonOpen.Click();
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 100ms.", new RecordItemIndex(7));
            Delay.Duration(100, false);
            
            Report.Log(ReportLevel.Info, "Wait", "Waiting 5m to not exist. Associated repository item: 'ActivityProgressDialog.ButtonCancel'", repo.ActivityProgressDialog.ButtonCancelInfo, new ActionTimeout(300000), new RecordItemIndex(8));
            repo.ActivityProgressDialog.ButtonCancelInfo.WaitForNotExists(300000);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
