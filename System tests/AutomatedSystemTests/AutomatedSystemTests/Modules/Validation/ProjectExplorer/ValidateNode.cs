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

namespace AutomatedSystemTests.Modules.Validation.ProjectExplorer
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The ValidateNode recording.
    /// </summary>
    [TestModule("906330ec-73fa-48a5-91c6-ed42eabd4771", ModuleType.Recording, 1)]
    public partial class ValidateNode : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ValidateNode instance = new ValidateNode();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateNode()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidateNode Instance
        {
            get { return instance; }
        }

#region Variables

        /// <summary>
        /// Gets or sets the value of variable substringTrajectName.
        /// </summary>
        [TestVariable("77ae6c27-603e-4704-add9-e1249169f0e5")]
        public string substringTrajectName
        {
            get { return repo.substringTrajectName; }
            set { repo.substringTrajectName = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable substringFMName.
        /// </summary>
        [TestVariable("3a7276c1-fca1-4026-9d2e-5bac10651a47")]
        public string substringFMName
        {
            get { return repo.substringFMName; }
            set { repo.substringFMName = value; }
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

            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Select() on item 'RiskeerMainWindow.ProjectExplorerPanel.TrajectNode'.", repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.SelfInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.Self.Select();
            
            // Validates that Item with Name containing $TextInItemName exists
            Report.Log(ReportLevel.Info, "Validation", "Validates that Item with Name containing $TextInItemName exists\r\nValidating Exists on item 'RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName'.", repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.SelfInfo, new RecordItemIndex(1));
            Validate.Exists(repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.SelfInfo);
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.Self, false, new RecordItemIndex(2));
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
