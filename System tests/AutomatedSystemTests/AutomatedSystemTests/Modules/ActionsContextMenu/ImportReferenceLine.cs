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
    ///The ImportReferenceLine recording.
    /// </summary>
    [TestModule("c5c8249d-a582-4230-ba22-0b7657e78ecd", ModuleType.Recording, 1)]
    public partial class ImportReferenceLine : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ImportReferenceLine instance = new ImportReferenceLine();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ImportReferenceLine()
        {
            pathReferenceLineFile = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ImportReferenceLine Instance
        {
            get { return instance; }
        }

#region Variables

        string _pathReferenceLineFile;

        /// <summary>
        /// Gets or sets the value of variable pathReferenceLineFile.
        /// </summary>
        [TestVariable("d2f452f1-239b-4d2c-a492-f7f177add580")]
        public string pathReferenceLineFile
        {
            get { return _pathReferenceLineFile; }
            set { _pathReferenceLineFile = value; }
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
            
            Report.Log(ReportLevel.Info, "Set value", "Setting attribute Text to '$pathReferenceLineFile' on item 'Openen.FileNameField'.", repo.Openen.FileNameFieldInfo, new RecordItemIndex(2));
            repo.Openen.FileNameField.Element.SetAttributeValue("Text", pathReferenceLineFile);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'Openen.ButtonOpen' at Center.", repo.Openen.ButtonOpenInfo, new RecordItemIndex(3));
            repo.Openen.ButtonOpen.Click();
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'Bevestigen.ButtonOK' at Center.", repo.Bevestigen.ButtonOKInfo, new RecordItemIndex(4));
            repo.Bevestigen.ButtonOK.Click();
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 500ms.", new RecordItemIndex(5));
            Delay.Duration(500, false);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}