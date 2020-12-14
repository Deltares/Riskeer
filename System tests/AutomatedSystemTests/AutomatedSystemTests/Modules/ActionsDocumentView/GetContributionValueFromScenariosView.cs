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
    ///The GetContributionValueFromScenariosView recording.
    /// </summary>
    [TestModule("3f07e952-3053-491e-9536-4986ced8e8c3", ModuleType.Recording, 1)]
    public partial class GetContributionValueFromScenariosView : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static GetContributionValueFromScenariosView instance = new GetContributionValueFromScenariosView();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GetContributionValueFromScenariosView()
        {
            valueContributionCell = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static GetContributionValueFromScenariosView Instance
        {
            get { return instance; }
        }

#region Variables

        string _valueContributionCell;

        /// <summary>
        /// Gets or sets the value of variable valueContributionCell.
        /// </summary>
        [TestVariable("5b699eef-7072-43c4-96e6-7592b8a103ae")]
        public string valueContributionCell
        {
            get { return _valueContributionCell; }
            set { _valueContributionCell = value; }
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

            Report.Log(ReportLevel.Info, "Get Value", "Getting attribute 'Text' from item 'RiskeerMainWindow.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContribution' and assigning its value to variable 'valueContributionCell'.", repo.RiskeerMainWindow.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContributionInfo, new RecordItemIndex(0));
            valueContributionCell = repo.RiskeerMainWindow.DocumentViewContainerUncached.ScenariosView.Table.GenericRowContribution.Element.GetAttributeValueText("Text");
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
