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
    ///The ValidateDetailedAssessmentValueAtRowPerCategoryGroup3 recording.
    /// </summary>
    [TestModule("d39d077e-c625-4d57-a6e7-a73a70d77e0d", ModuleType.Recording, 1)]
    public partial class ValidateDetailedAssessmentValueAtRowPerCategoryGroup3 : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static ValidateDetailedAssessmentValueAtRowPerCategoryGroup3 instance = new ValidateDetailedAssessmentValueAtRowPerCategoryGroup3();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateDetailedAssessmentValueAtRowPerCategoryGroup3()
        {
            expectedDetailedAssessmentValuePerCategory = "yourtext";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidateDetailedAssessmentValueAtRowPerCategoryGroup3 Instance
        {
            get { return instance; }
        }

#region Variables

        string _expectedDetailedAssessmentValuePerCategory;

        /// <summary>
        /// Gets or sets the value of variable expectedDetailedAssessmentValuePerCategory.
        /// </summary>
        [TestVariable("9422b68a-5397-40fc-bc4d-01db687e6cd3")]
        public string expectedDetailedAssessmentValuePerCategory
        {
            get { return _expectedDetailedAssessmentValuePerCategory; }
            set { _expectedDetailedAssessmentValuePerCategory = value; }
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

            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentValuePerSection'.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentValuePerSectionInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentValuePerSection.Focus();
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Validation", "Validating AttributeEqual (Text=$expectedDetailedAssessmentValuePerCategory) on item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentValuePerSection'.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentValuePerSectionInfo, new RecordItemIndex(1));
            Validate.AttributeEqual(repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentValuePerSectionInfo, "Text", expectedDetailedAssessmentValuePerCategory);
            Delay.Milliseconds(0);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
