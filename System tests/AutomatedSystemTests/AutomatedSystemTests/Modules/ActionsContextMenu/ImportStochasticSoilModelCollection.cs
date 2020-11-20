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
    ///The ImportStochasticSoilModelCollection recording.
    /// </summary>
    [TestModule("288d66d7-3c19-43f3-bd1e-9523f15bb660", ModuleType.Recording, 1)]
    public partial class ImportStochasticSoilModelCollection : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ImportStochasticSoilModelCollection instance = new ImportStochasticSoilModelCollection();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ImportStochasticSoilModelCollection()
        {
            nameSoilFile = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ImportStochasticSoilModelCollection Instance
        {
            get { return instance; }
        }

#region Variables

        string _nameSoilFile;

        /// <summary>
        /// Gets or sets the value of variable nameSoilFile.
        /// </summary>
        [TestVariable("afc0a4c0-0d40-403c-b397-fb6a4c9112e8")]
        public string nameSoilFile
        {
            get { return _nameSoilFile; }
            set { _nameSoilFile = value; }
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
            
            Report.Log(ReportLevel.Info, "Set value", "Setting attribute Text to '$nameSoilFile' on item 'OpenDialog.FileNameField'.", repo.OpenDialog.FileNameFieldInfo, new RecordItemIndex(2));
            repo.OpenDialog.FileNameField.Element.SetAttributeValue("Text", nameSoilFile);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'OpenDialog.ButtonOpen' at Center.", repo.OpenDialog.ButtonOpenInfo, new RecordItemIndex(3));
            repo.OpenDialog.ButtonOpen.Click();
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 100ms.", new RecordItemIndex(4));
            Delay.Duration(100, false);
            
            Report.Log(ReportLevel.Info, "Wait", "Waiting 5s to not exist. Associated repository item: 'ActivityProgressDialog.ButtonCancel'", repo.ActivityProgressDialog.ButtonCancelInfo, new ActionTimeout(5000), new RecordItemIndex(5));
            repo.ActivityProgressDialog.ButtonCancelInfo.WaitForNotExists(5000);
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 1s.", new RecordItemIndex(6));
            Delay.Duration(1000, false);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
