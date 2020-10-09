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
    ///The ValidateDetailedAssessmentTypeAtRowCategoryIVv recording.
    /// </summary>
    [TestModule("42cf05e3-77be-41b2-bb8e-2d6b14e70872", ModuleType.Recording, 1)]
    public partial class ValidateDetailedAssessmentTypeAtRowCategoryIVv : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static ValidateDetailedAssessmentTypeAtRowCategoryIVv instance = new ValidateDetailedAssessmentTypeAtRowCategoryIVv();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateDetailedAssessmentTypeAtRowCategoryIVv()
        {
            expectedDetailedAssessmentCellType = "yourtext";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidateDetailedAssessmentTypeAtRowCategoryIVv Instance
        {
            get { return instance; }
        }

#region Variables

        string _expectedDetailedAssessmentCellType;

        /// <summary>
        /// Gets or sets the value of variable expectedDetailedAssessmentCellType.
        /// </summary>
        [TestVariable("5f9740ff-8d23-4a68-9028-29ad1795735a")]
        public string expectedDetailedAssessmentCellType
        {
            get { return _expectedDetailedAssessmentCellType; }
            set { _expectedDetailedAssessmentCellType = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable rowIndex.
        /// </summary>
        [TestVariable("3191cedb-5ef9-4f31-85c1-c8b9bb6a46f0")]
        public string rowIndex
        {
            get { return repo.rowIndex; }
            set { repo.rowIndex = value; }
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

            Report.Log(ReportLevel.Info, "Validation", "Validating AttributeEqual (AccessibleValue=$expectedDetailedAssessmentCellType) on item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentTypeCategoryIVv'.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentTypeCategoryIVvInfo, new RecordItemIndex(0));
            Validate.AttributeEqual(repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentTypeCategoryIVvInfo, "AccessibleValue", expectedDetailedAssessmentCellType);
            Delay.Milliseconds(0);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
