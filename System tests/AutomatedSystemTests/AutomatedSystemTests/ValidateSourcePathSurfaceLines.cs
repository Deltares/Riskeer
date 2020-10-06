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
    ///The ValidateSourcePathSurfaceLines recording.
    /// </summary>
    [TestModule("cb7662f3-2819-4dda-b3ec-c0e6a585160f", ModuleType.Recording, 1)]
    public partial class ValidateSourcePathSurfaceLines : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static ValidateSourcePathSurfaceLines instance = new ValidateSourcePathSurfaceLines();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateSourcePathSurfaceLines()
        {
            pathSurfaceLines = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidateSourcePathSurfaceLines Instance
        {
            get { return instance; }
        }

#region Variables

        string _pathSurfaceLines;

        /// <summary>
        /// Gets or sets the value of variable pathSurfaceLines.
        /// </summary>
        [TestVariable("4bd72c19-90ee-4d4d-8396-3588c63d074c")]
        public string pathSurfaceLines
        {
            get { return _pathSurfaceLines; }
            set { _pathSurfaceLines = value; }
        }

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

            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Select() on item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.SurfaceLinesCollectionNode'.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.SurfaceLinesCollectionNode.SelfInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.SurfaceLinesCollectionNode.Self.Select();
            
            Report.Log(ReportLevel.Info, "Validation", "Validating AttributeEqual (AccessibleValue=$pathSurfaceLines) on item 'RiskeerMainWindow.PropertiesPanelContainer.Table.SourcePath'.", repo.RiskeerMainWindow.PropertiesPanelContainer.Table.SourcePathInfo, new RecordItemIndex(1));
            Validate.AttributeEqual(repo.RiskeerMainWindow.PropertiesPanelContainer.Table.SourcePathInfo, "AccessibleValue", pathSurfaceLines);
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.PropertiesPanelContainer.Table.SourcePath, false, new RecordItemIndex(2));
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.SurfaceLinesCollectionNode' at Center.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.SurfaceLinesCollectionNode.SelfInfo, new RecordItemIndex(3));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericFMItemWithSubstringInName.InputFM.SurfaceLinesCollectionNode.Self.Click();
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{Right}'.", new RecordItemIndex(4));
            Keyboard.Press("{Right}");
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
