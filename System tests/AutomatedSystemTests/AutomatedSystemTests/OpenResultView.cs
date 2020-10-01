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
    ///The OpenResultView recording.
    /// </summary>
    [TestModule("14b19856-facd-4b69-82b3-b7f678560774", ModuleType.Recording, 1)]
    public partial class OpenResultView : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static OpenResultView instance = new OpenResultView();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public OpenResultView()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static OpenResultView Instance
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
        /// Gets or sets the value of variable substringItemName.
        /// </summary>
        [TestVariable("3a7276c1-fca1-4026-9d2e-5bac10651a47")]
        public string substringItemName
        {
            get { return repo.substringItemName; }
            set { repo.substringItemName = value; }
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

            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInName'.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInNameInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInName.Focus();
            Delay.Milliseconds(0);
            
            ExpandNode(repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInNameInfo);
            Delay.Milliseconds(0);
            
            try {
                OpenResultViewIfFMIsRelevant(repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInNameInfo, "Oordeel");
                Delay.Milliseconds(0);
            } catch(Exception ex) { Report.Log(ReportLevel.Warn, "Module", "(Optional Action) " + ex.Message, new RecordItemIndex(2)); }
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.DocumentViewContainer.Self, false, new RecordItemIndex(3));
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
