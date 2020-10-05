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
    ///The UpdateSurfaceLines recording.
    /// </summary>
    [TestModule("18e39f84-9b42-4666-802c-2fecd9846123", ModuleType.Recording, 1)]
    public partial class UpdateSurfaceLines : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static UpdateSurfaceLines instance = new UpdateSurfaceLines();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public UpdateSurfaceLines()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static UpdateSurfaceLines Instance
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

            Report.Log(ReportLevel.Info, "Mouse", "Mouse Right Click item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.SurfaceLinesCollectionNode' at Center.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.SurfaceLinesCollectionNode.SelfInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.SurfaceLinesCollectionNode.Self.Click(System.Windows.Forms.MouseButtons.Right);
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'ContextMenu.Bijwerken' at Center.", repo.ContextMenu.BijwerkenInfo, new RecordItemIndex(1));
            repo.ContextMenu.Bijwerken.Click();
            Delay.Milliseconds(0);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
