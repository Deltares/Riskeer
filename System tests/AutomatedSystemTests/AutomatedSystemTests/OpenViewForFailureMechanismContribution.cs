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
    ///The OpenViewForFailureMechanismContribution recording.
    /// </summary>
    [TestModule("5e81345e-66bc-4e08-8ade-03d386d824fd", ModuleType.Recording, 1)]
    public partial class OpenViewForFailureMechanismContribution : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static OpenViewForFailureMechanismContribution instance = new OpenViewForFailureMechanismContribution();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public OpenViewForFailureMechanismContribution()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static OpenViewForFailureMechanismContribution Instance
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

            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.Faalkansbegroting'.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.FaalkansbegrotingInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.Faalkansbegroting.Focus();
            
            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Select() on item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.Faalkansbegroting'.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.FaalkansbegrotingInfo, new RecordItemIndex(1));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.Faalkansbegroting.Select();
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left DoubleClick item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.Faalkansbegroting' at Center.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.FaalkansbegrotingInfo, new RecordItemIndex(2));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.Faalkansbegroting.DoubleClick();
            
            // Screenshot of FailureMechanismContributionView
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.FailureMechanismContributionView.Self, false, new RecordItemIndex(3));
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
