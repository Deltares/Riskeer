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
    ///The UpdateSubsurfaceSchematization recording.
    /// </summary>
    [TestModule("32dc32fd-5a31-4bd3-96eb-7c8d70401ccb", ModuleType.Recording, 1)]
    public partial class UpdateSubsurfaceSchematization : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static UpdateSubsurfaceSchematization instance = new UpdateSubsurfaceSchematization();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public UpdateSubsurfaceSchematization()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static UpdateSubsurfaceSchematization Instance
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
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 20;
            Delay.SpeedFactor = 1.00;

            Init();

            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.StochasticSoilModelCollection'.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.StochasticSoilModelCollection.SelfInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.StochasticSoilModelCollection.Self.Focus();
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Select() on item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.StochasticSoilModelCollection'.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.StochasticSoilModelCollection.SelfInfo, new RecordItemIndex(1));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.StochasticSoilModelCollection.Self.Select();
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Right Click item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.StochasticSoilModelCollection' at Center.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.StochasticSoilModelCollection.SelfInfo, new RecordItemIndex(2));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.StochasticSoilModelCollection.Self.Click(System.Windows.Forms.MouseButtons.Right);
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'ContextMenu.Bijwerken' at Center.", repo.ContextMenu.BijwerkenInfo, new RecordItemIndex(3));
            repo.ContextMenu.Bijwerken.Click();
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Wait", "Waiting 5s to not exist. Associated repository item: 'ActivityProgressDialog.ProgressBar'", repo.ActivityProgressDialog.ProgressBarInfo, new ActionTimeout(5000), new RecordItemIndex(4));
            repo.ActivityProgressDialog.ProgressBarInfo.WaitForNotExists(5000);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
