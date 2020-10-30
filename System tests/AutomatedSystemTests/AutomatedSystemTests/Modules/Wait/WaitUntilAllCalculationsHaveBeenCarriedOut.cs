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

namespace AutomatedSystemTests.Modules.Wait
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The WaitUntilAllCalculationsHaveBeenCarriedOut recording.
    /// </summary>
    [TestModule("ada8c353-8b24-47e4-b6bf-adfb36229783", ModuleType.Recording, 1)]
    public partial class WaitUntilAllCalculationsHaveBeenCarriedOut : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static WaitUntilAllCalculationsHaveBeenCarriedOut instance = new WaitUntilAllCalculationsHaveBeenCarriedOut();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public WaitUntilAllCalculationsHaveBeenCarriedOut()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static WaitUntilAllCalculationsHaveBeenCarriedOut Instance
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
            Mouse.DefaultMoveTime = 300;
            Keyboard.DefaultKeyPressTime = 20;
            Delay.SpeedFactor = 1.00;

            Init();

            Report.Log(ReportLevel.Info, "Delay", "Waiting for 500ms.", new RecordItemIndex(0));
            Delay.Duration(500, false);
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.ActivityProgressDialog.Self, false, new RecordItemIndex(1));
            
            Report.Log(ReportLevel.Info, "Wait", "Waiting 2h to not exist. Associated repository item: 'ActivityProgressDialog'", repo.ActivityProgressDialog.SelfInfo, new ActionTimeout(7200000), new RecordItemIndex(2));
            repo.ActivityProgressDialog.SelfInfo.WaitForNotExists(7200000);
            
            Report.Log(ReportLevel.Info, "Delay", "Waiting for 300ms.", new RecordItemIndex(3));
            Delay.Duration(300, false);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
