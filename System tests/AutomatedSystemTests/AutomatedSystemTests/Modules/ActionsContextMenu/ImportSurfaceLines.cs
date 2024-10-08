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

namespace AutomatedSystemTests.Modules.ActionsContextMenu
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The ImportSurfaceLines recording.
    /// </summary>
    [TestModule("ec62b093-3fff-4a7e-8468-0636f99e4153", ModuleType.Recording, 1)]
    public partial class ImportSurfaceLines : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ImportSurfaceLines instance = new ImportSurfaceLines();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ImportSurfaceLines()
        {
            SurfaceLineCollectionFileName = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ImportSurfaceLines Instance
        {
            get { return instance; }
        }

#region Variables

        string _SurfaceLineCollectionFileName;

        /// <summary>
        /// Gets or sets the value of variable SurfaceLineCollectionFileName.
        /// </summary>
        [TestVariable("1cec0b06-5aec-4724-a571-e2f4d523d773")]
        public string SurfaceLineCollectionFileName
        {
            get { return _SurfaceLineCollectionFileName; }
            set { _SurfaceLineCollectionFileName = value; }
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
            
            Report.Log(ReportLevel.Info, "Set value", "Setting attribute Text to '$SurfaceLineCollectionFileName' on item 'Openen.FileNameField'.", repo.Openen.FileNameFieldInfo, new RecordItemIndex(2));
            repo.Openen.FileNameField.Element.SetAttributeValue("Text", SurfaceLineCollectionFileName);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'Openen.ButtonOpen' at Center.", repo.Openen.ButtonOpenInfo, new RecordItemIndex(3));
            repo.Openen.ButtonOpen.Click();
            
            Report.Log(ReportLevel.Info, "Wait", "Waiting 5s to not exist. Associated repository item: 'ActivityProgressDialog.ButtonCancel'", repo.ActivityProgressDialog.ButtonCancelInfo, new ActionTimeout(5000), new RecordItemIndex(4));
            repo.ActivityProgressDialog.ButtonCancelInfo.WaitForNotExists(5000);
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 2s.", new RecordItemIndex(5));
            Delay.Duration(2000, false);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
