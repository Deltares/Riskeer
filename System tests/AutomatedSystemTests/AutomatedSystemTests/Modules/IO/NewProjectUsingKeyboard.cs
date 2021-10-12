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

namespace AutomatedSystemTests.Modules.IO
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The NewProjectUsingKeyboard recording.
    /// </summary>
    [TestModule("f0658d7f-65e3-4f38-afe8-97fd942c1478", ModuleType.Recording, 1)]
    public partial class NewProjectUsingKeyboard : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static NewProjectUsingKeyboard instance = new NewProjectUsingKeyboard();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public NewProjectUsingKeyboard()
        {
            signallingValue = "";
            lowLimitValue = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static NewProjectUsingKeyboard Instance
        {
            get { return instance; }
        }

#region Variables

        string _signallingValue;

        /// <summary>
        /// Gets or sets the value of variable signallingValue.
        /// </summary>
        [TestVariable("5cbfe7da-3ce3-4a31-baa8-dcc95a2c80d7")]
        public string signallingValue
        {
            get { return _signallingValue; }
            set { _signallingValue = value; }
        }

        string _lowLimitValue;

        /// <summary>
        /// Gets or sets the value of variable lowLimitValue.
        /// </summary>
        [TestVariable("3624ac3a-3bbc-43cf-8c5b-c6a96b97116f")]
        public string lowLimitValue
        {
            get { return _lowLimitValue; }
            set { _lowLimitValue = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable trajectID.
        /// </summary>
        [TestVariable("171a542e-eb85-404b-a2fe-920346f601f6")]
        public string trajectID
        {
            get { return repo.trajectID; }
            set { repo.trajectID = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable normType.
        /// </summary>
        [TestVariable("929a9a29-2c5b-41dc-93ed-daff0f25d734")]
        public string normType
        {
            get { return repo.normType; }
            set { repo.normType = value; }
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

            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'RiskeerMainWindow' at UpperCenter.", repo.RiskeerMainWindow.SelfInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.Self.Click(Location.UpperCenter);
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key 'Ctrl+N' Press.", new RecordItemIndex(1));
            Keyboard.Press(System.Windows.Forms.Keys.N | System.Windows.Forms.Keys.Control, 49, Keyboard.DefaultKeyPressTime, 1, true);
            
            Mouse_Click_ButtonNoIfConformationDialogAppears(repo.ConfirmSaveProjectDialogWhenClosing.ButtonNoInfo);
            
            DoNotSaveIfAsked(repo.ConfirmSaveProjectDialogWhenClosing.ButtonNoInfo);
            
            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellTrajectId'.", repo.DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellTrajectIdInfo, new RecordItemIndex(4));
            repo.DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellTrajectId.Focus();
            
            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Select() on item 'DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellTrajectId'.", repo.DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellTrajectIdInfo, new RecordItemIndex(5));
            repo.DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellTrajectId.Select();
            
            Report.Log(ReportLevel.Info, "Get Value", "Getting attribute 'Text' from item 'DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellSignallingValue' and assigning its value to variable 'signallingValue'.", repo.DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellSignallingValueInfo, new RecordItemIndex(6));
            signallingValue = repo.DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellSignallingValue.Element.GetAttributeValueText("Text");
            
            Report.Log(ReportLevel.Info, "Get Value", "Getting attribute 'Text' from item 'DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellLowLimitValue' and assigning its value to variable 'lowLimitValue'.", repo.DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellLowLimitValueInfo, new RecordItemIndex(7));
            lowLimitValue = repo.DialogNewTraject.TableTrajects.GenericTrajectIDRow.CellLowLimitValue.Element.GetAttributeValueText("Text");
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'DialogNewTraject.NormRadioButton' at CenterLeft.", repo.DialogNewTraject.NormRadioButtonInfo, new RecordItemIndex(8));
            repo.DialogNewTraject.NormRadioButton.Click(Location.CenterLeft);
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'DialogNewTraject.ButtonOk' at Center.", repo.DialogNewTraject.ButtonOkInfo, new RecordItemIndex(9));
            repo.DialogNewTraject.ButtonOk.Click();
            
            Report.Log(ReportLevel.Info, "Wait", "Waiting 5s to exist. Associated repository item: 'RiskeerMainWindow.ProjectExplorerPanel.TrajectNode'", repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.SelfInfo, new ActionTimeout(5000), new RecordItemIndex(10));
            repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.SelfInfo.WaitForExists(5000);
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 150ms.", new RecordItemIndex(11));
            Delay.Duration(150, false);
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.Self, false, new RecordItemIndex(12));
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
