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
    ///The ValidateWaterLevelInDocumentView recording.
    /// </summary>
    [TestModule("c93bb1b9-c14c-4a0a-ae72-fe034593361b", ModuleType.Recording, 1)]
    public partial class ValidateWaterLevelInDocumentView : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static ValidateWaterLevelInDocumentView instance = new ValidateWaterLevelInDocumentView();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateWaterLevelInDocumentView()
        {
            expectedValue = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidateWaterLevelInDocumentView Instance
        {
            get { return instance; }
        }

#region Variables

        string _expectedValue;

        /// <summary>
        /// Gets or sets the value of variable expectedValue.
        /// </summary>
        [TestVariable("ac93a9b5-24d0-453b-8057-a697e7574717")]
        public string expectedValue
        {
            get { return _expectedValue; }
            set { _expectedValue = value; }
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
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 20;
            Delay.SpeedFactor = 0.00;

            Init();

            Report.Log(ReportLevel.Info, "Validation", "Validating AttributeEqual (Text=$expectedValue) on item 'RiskeerMainWindow.DocumentViewContainer.DesignWaterLevelCalculationsViewCached.LeftSide.Table.WaterLevelNthRow'.", repo.RiskeerMainWindow.DocumentViewContainer.DesignWaterLevelCalculationsViewCached.LeftSide.Table.WaterLevelNthRowInfo, new RecordItemIndex(0));
            Validate.AttributeEqual(repo.RiskeerMainWindow.DocumentViewContainer.DesignWaterLevelCalculationsViewCached.LeftSide.Table.WaterLevelNthRowInfo, "Text", expectedValue);
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.DocumentViewContainer.DesignWaterLevelCalculationsViewCached.LeftSide.Table.WaterLevelNthRow, false, new RecordItemIndex(1));
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
