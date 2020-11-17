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
    ///The AddBackgroundLayer recording.
    /// </summary>
    [TestModule("46407bea-c0f2-409e-8831-e2e674d7fad1", ModuleType.Recording, 1)]
    public partial class AddBackgroundLayer : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static AddBackgroundLayer instance = new AddBackgroundLayer();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public AddBackgroundLayer()
        {
            backgroundFileNameToAdd = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static AddBackgroundLayer Instance
        {
            get { return instance; }
        }

#region Variables

        string _backgroundFileNameToAdd;

        /// <summary>
        /// Gets or sets the value of variable backgroundFileNameToAdd.
        /// </summary>
        [TestVariable("3ba5677d-0f8b-401a-bdbe-3f131194f585")]
        public string backgroundFileNameToAdd
        {
            get { return _backgroundFileNameToAdd; }
            set { _backgroundFileNameToAdd = value; }
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
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 20;
            Delay.SpeedFactor = 1.00;

            Init();

            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'RiskeerMainWindow.MapLegendPanel.RootNode' at Center.", repo.RiskeerMainWindow.MapLegendPanel.RootNode.SelfInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.MapLegendPanel.RootNode.Self.Click();
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{Apps}'.", new RecordItemIndex(1));
            Keyboard.Press("{Apps}");
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'ContextMenu.VoegKaartlaagToe' at Center.", repo.ContextMenu.VoegKaartlaagToeInfo, new RecordItemIndex(2));
            repo.ContextMenu.VoegKaartlaagToe.Click();
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Set value", "Setting attribute Text to '$backgroundFileNameToAdd' on item 'Openen.FileNameField'.", repo.Openen.FileNameFieldInfo, new RecordItemIndex(3));
            repo.Openen.FileNameField.Element.SetAttributeValue("Text", backgroundFileNameToAdd);
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'Openen.ButtonOpen' at Center.", repo.Openen.ButtonOpenInfo, new RecordItemIndex(4));
            repo.Openen.ButtonOpen.Click();
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 300ms.", new RecordItemIndex(5));
            Delay.Duration(300, false);
            
            Report.Log(ReportLevel.Info, "Wait", "Waiting 5m to not exist. Associated repository item: 'ActivityProgressDialog.ButtonCancel'", repo.ActivityProgressDialog.ButtonCancelInfo, new ActionTimeout(300000), new RecordItemIndex(6));
            repo.ActivityProgressDialog.ButtonCancelInfo.WaitForNotExists(300000);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
