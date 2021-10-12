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

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The SetContributionValueInScenariosView recording.
    /// </summary>
    [TestModule("a0d27b4d-f989-42ce-afab-76a62ad61956", ModuleType.Recording, 1)]
    public partial class SetContributionValueInScenariosView : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static SetContributionValueInScenariosView instance = new SetContributionValueInScenariosView();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SetContributionValueInScenariosView()
        {
            newValue = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static SetContributionValueInScenariosView Instance
        {
            get { return instance; }
        }

#region Variables

        /// <summary>
        /// Gets or sets the value of variable newValue.
        /// </summary>
        [TestVariable("fa77a94b-5e57-4ae0-959e-21197789d225")]
        public string newValue
        {
            get { return repo.newValue; }
            set { repo.newValue = value; }
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

            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContribution' at Center.", repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContributionInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContribution.Click();
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence from variable '$newValue' with focus on 'RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContribution'.", repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContributionInfo, new RecordItemIndex(1));
            repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContribution.PressKeys(newValue);
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{Return}' with focus on 'RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContribution'.", repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContributionInfo, new RecordItemIndex(2));
            repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContribution.PressKeys("{Return}");
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
