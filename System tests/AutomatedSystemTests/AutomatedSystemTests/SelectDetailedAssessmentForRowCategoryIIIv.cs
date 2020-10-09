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
    ///The SelectDetailedAssessmentForRowCategoryIIIv recording.
    /// </summary>
    [TestModule("a7eb8c7e-6846-46ff-b2ed-97d624f39d39", ModuleType.Recording, 1)]
    public partial class SelectDetailedAssessmentForRowCategoryIIIv : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static SelectDetailedAssessmentForRowCategoryIIIv instance = new SelectDetailedAssessmentForRowCategoryIIIv();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SelectDetailedAssessmentForRowCategoryIIIv()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static SelectDetailedAssessmentForRowCategoryIIIv Instance
        {
            get { return instance; }
        }

#region Variables

        /// <summary>
        /// Gets or sets the value of variable rowIndex.
        /// </summary>
        [TestVariable("3191cedb-5ef9-4f31-85c1-c8b9bb6a46f0")]
        public string rowIndex
        {
            get { return repo.rowIndex; }
            set { repo.rowIndex = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable textDropDownMenu.
        /// </summary>
        [TestVariable("4f707b64-d756-4bb7-a45a-f34af402b280")]
        public string textDropDownMenu
        {
            get { return repo.textDropDownMenu; }
            set { repo.textDropDownMenu = value; }
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

            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentTypeCategoryIIIv'.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentTypeCategoryIIIvInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentTypeCategoryIIIv.Focus();
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentTypeCategoryIIIv' at CenterRight.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentTypeCategoryIIIvInfo, new RecordItemIndex(1));
            repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.DetailedAssessmentTypeCategoryIIIv.Click(Location.CenterRight);
            Delay.Milliseconds(0);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'DropDownMenuItemList.DropDownItem' at Center.", repo.DropDownMenuItemList.DropDownItemInfo, new RecordItemIndex(2));
            repo.DropDownMenuItemList.DropDownItem.Click();
            Delay.Milliseconds(0);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
