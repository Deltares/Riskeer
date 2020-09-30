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
    ///The SelectAssessmentForSection recording.
    /// </summary>
    [TestModule("e0d85a11-9622-402c-937d-fefa962c3394", ModuleType.Recording, 1)]
    public partial class SelectAssessmentForSection : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static SelectAssessmentForSection instance = new SelectAssessmentForSection();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SelectAssessmentForSection()
        {
            rowIndex = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static SelectAssessmentForSection Instance
        {
            get { return instance; }
        }

#region Variables

        /// <summary>
        /// Gets or sets the value of variable rowIndex.
        /// </summary>
        [TestVariable("d576037a-032a-4fa3-b9a8-9944503b34d3")]
        public string rowIndex
        {
            get { return repo.rowIndex; }
            set { repo.rowIndex = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable textItemDropDownMenu.
        /// </summary>
        [TestVariable("d82001f1-20c0-4080-a06d-9f1f8ee3a56d")]
        public string textItemDropDownMenu
        {
            get { return repo.textItemDropDownMenu; }
            set { repo.textItemDropDownMenu = value; }
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

            //Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsVak12_2_00000'.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsVak12_2_00000Info, new RecordItemIndex(0));
            //repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsVak12_2_00000.Focus();
            
            //Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsVak12_2_00000' at CenterRight.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsVak12_2_00000Info, new RecordItemIndex(1));
            //repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsVak12_2_00000.Click(Location.CenterRight);
            
            //Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'DropDownMenuItemList.ListItemVB' at Center.", repo.DropDownMenuItemList.ListItemVBInfo, new RecordItemIndex(2));
            //repo.DropDownMenuItemList.ListItemVB.Click();
            
            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsRow0'.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsRow0Info, new RecordItemIndex(3));
            repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsRow0.Focus();
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsRow0' at CenterRight.", repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsRow0Info, new RecordItemIndex(4));
            repo.RiskeerMainWindow.DocumentViewContainer.FailureMechanismResultView.TableDataGridView.EenvoudigeToetsRow0.Click(Location.CenterRight);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'DropDownMenuItemList.GenericDropDownItem' at Center.", repo.DropDownMenuItemList.GenericDropDownItemInfo, new RecordItemIndex(5));
            repo.DropDownMenuItemList.GenericDropDownItem.Click();
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
