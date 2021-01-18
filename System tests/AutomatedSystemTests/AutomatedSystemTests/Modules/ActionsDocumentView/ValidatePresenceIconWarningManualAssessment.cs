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
    ///The ValidatePresenceIconWarningManualAssessment recording.
    /// </summary>
    [TestModule("eda96359-8672-43bd-b08e-6668567acb0a", ModuleType.Recording, 1)]
    public partial class ValidatePresenceIconWarningManualAssessment : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ValidatePresenceIconWarningManualAssessment instance = new ValidatePresenceIconWarningManualAssessment();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidatePresenceIconWarningManualAssessment()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidatePresenceIconWarningManualAssessment Instance
        {
            get { return instance; }
        }

#region Variables

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

            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.DocumentViewContainerUncached.FM_ResultView.WarningIconManualAssessment, false, new RecordItemIndex(0));
            
            Report.Log(ReportLevel.Info, "Validation", "Validating Exists on item 'RiskeerMainWindow.DocumentViewContainerUncached.FM_ResultView.WarningIconManualAssessment'.", repo.RiskeerMainWindow.DocumentViewContainerUncached.FM_ResultView.WarningIconManualAssessmentInfo, new RecordItemIndex(1));
            Validate.Exists(repo.RiskeerMainWindow.DocumentViewContainerUncached.FM_ResultView.WarningIconManualAssessmentInfo);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
