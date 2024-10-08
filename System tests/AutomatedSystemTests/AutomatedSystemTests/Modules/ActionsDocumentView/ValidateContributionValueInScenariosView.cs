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
    ///The ValidateContributionValueInScenariosView recording.
    /// </summary>
    [TestModule("abc7c300-9a65-468b-a578-f8e564738f13", ModuleType.Recording, 1)]
    public partial class ValidateContributionValueInScenariosView : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ValidateContributionValueInScenariosView instance = new ValidateContributionValueInScenariosView();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateContributionValueInScenariosView()
        {
            expectedContribution = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidateContributionValueInScenariosView Instance
        {
            get { return instance; }
        }

#region Variables

        string _expectedContribution;

        /// <summary>
        /// Gets or sets the value of variable expectedContribution.
        /// </summary>
        [TestVariable("61018541-00b3-43aa-8932-ccff5ec68f08")]
        public string expectedContribution
        {
            get { return _expectedContribution; }
            set { _expectedContribution = value; }
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

            Validate_GenericRowContribution(repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.ScenariosView.Table.GenericRowContributionInfo);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
