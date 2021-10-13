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

namespace AutomatedSystemTests.Modules.OpenViews
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The OpenChartCalculation recording.
    /// </summary>
    [TestModule("2161f947-3dda-4270-9a96-0d33afc50b6c", ModuleType.Recording, 1)]
    public partial class OpenChartCalculation : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static OpenChartCalculation instance = new OpenChartCalculation();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public OpenChartCalculation()
        {
            logMessageChartScreenshot = "";
            singleCalculationName = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static OpenChartCalculation Instance
        {
            get { return instance; }
        }

#region Variables

        string _logMessageChartScreenshot;

        /// <summary>
        /// Gets or sets the value of variable logMessageChartScreenshot.
        /// </summary>
        [TestVariable("3c63428f-921e-4ed8-96c7-a91856d049c7")]
        public string logMessageChartScreenshot
        {
            get { return _logMessageChartScreenshot; }
            set { _logMessageChartScreenshot = value; }
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

        /// <summary>
        /// Gets or sets the value of variable singleCalculationName.
        /// </summary>
        [TestVariable("4f964365-5470-426e-9e06-2f54c57565cb")]
        public string singleCalculationName
        {
            get { return repo.singleCalculationName; }
            set { repo.singleCalculationName = value; }
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

            Report.Log(ReportLevel.Info, "Mouse", "Mouse Right Click item 'RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.GenericFMItemWithSubstringInName.Calculations.InputSingleCalculation' at Center.", repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.GenericFMItemWithSubstringInName.Calculations.InputSingleCalculationInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.GenericFMItemWithSubstringInName.Calculations.InputSingleCalculation.Click(System.Windows.Forms.MouseButtons.Right);
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'ContextMenu.Openen' at Center.", repo.ContextMenu.OpenenInfo, new RecordItemIndex(1));
            repo.ContextMenu.Openen.Click();
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Wait", "Waiting 5s to not exist. Associated repository item: 'ContextMenu'", repo.ContextMenu.SelfInfo, new ActionTimeout(5000), new RecordItemIndex(2));
            repo.ContextMenu.SelfInfo.WaitForNotExists(5000);
            
            Report.Log(ReportLevel.Info, "User", logMessageChartScreenshot, new RecordItemIndex(3));
            
            Report.Log(ReportLevel.Info, "User", singleCalculationName, new RecordItemIndex(4));
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.DocumentViewContainer.Self, false, new RecordItemIndex(5));
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
