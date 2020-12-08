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

namespace AutomatedSystemTests.Modules.Validation.DocumentView
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The ValidateAssessmentLabelAndProbForFMInAssemblyTrajectView recording.
    /// </summary>
    [TestModule("0a4d8d5f-1427-4a9b-9ba1-ee5acbb844db", ModuleType.Recording, 1)]
    public partial class ValidateAssessmentLabelAndProbForFMInAssemblyTrajectView : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ValidateAssessmentLabelAndProbForFMInAssemblyTrajectView instance = new ValidateAssessmentLabelAndProbForFMInAssemblyTrajectView();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateAssessmentLabelAndProbForFMInAssemblyTrajectView()
        {
            indexRow = "";
            expectedAssessmentLabel = "";
            expectedAssessmentApproxProb = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidateAssessmentLabelAndProbForFMInAssemblyTrajectView Instance
        {
            get { return instance; }
        }

#region Variables

        string _expectedAssessmentLabel;

        /// <summary>
        /// Gets or sets the value of variable expectedAssessmentLabel.
        /// </summary>
        [TestVariable("63a214be-a9ec-4338-bca0-2d229c5bcf23")]
        public string expectedAssessmentLabel
        {
            get { return _expectedAssessmentLabel; }
            set { _expectedAssessmentLabel = value; }
        }

        string _expectedAssessmentApproxProb;

        /// <summary>
        /// Gets or sets the value of variable expectedAssessmentApproxProb.
        /// </summary>
        [TestVariable("e609f034-4a18-4251-9f83-dbee6f37fe3f")]
        public string expectedAssessmentApproxProb
        {
            get { return _expectedAssessmentApproxProb; }
            set { _expectedAssessmentApproxProb = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable indexRow.
        /// </summary>
        [TestVariable("949ae0e5-2a92-4a0a-a329-2c6c7394803c")]
        public string indexRow
        {
            get { return repo.indexRow; }
            set { repo.indexRow = value; }
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

            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Select() on item 'RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentLabelGenericRow'.", repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentLabelGenericRowInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentLabelGenericRow.Select();
            
            Report.Log(ReportLevel.Info, "Validation", "Validating AttributeEqual (Text=$expectedAssessmentLabel) on item 'RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentLabelGenericRow'.", repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentLabelGenericRowInfo, new RecordItemIndex(1));
            Validate.AttributeEqual(repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentLabelGenericRowInfo, "Text", expectedAssessmentLabel);
            
            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Select() on item 'RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentApproxProbGenericRow'.", repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentApproxProbGenericRowInfo, new RecordItemIndex(2));
            repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentApproxProbGenericRow.Select();
            
            Report.Log(ReportLevel.Info, "Validation", "Validating AttributeEqual (Text=$expectedAssessmentApproxProb) on item 'RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentApproxProbGenericRow'.", repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentApproxProbGenericRowInfo, new RecordItemIndex(3));
            Validate.AttributeEqual(repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResult.Table.AssessmentApproxProbGenericRowInfo, "Text", expectedAssessmentApproxProb);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
