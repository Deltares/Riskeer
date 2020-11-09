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

namespace AutomatedSystemTests.Modules.Set_Assign.Assembly
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The GetRandomNorm recording.
    /// </summary>
    [TestModule("3f50af80-7a9e-4428-b269-4f7f8984a5c6", ModuleType.Recording, 1)]
    public partial class GetRandomNorm : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static GetRandomNorm instance = new GetRandomNorm();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GetRandomNorm()
        {
            signallingValue = "";
            lowerLimitValue = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static GetRandomNorm Instance
        {
            get { return instance; }
        }

#region Variables

        string _signallingValue;

        /// <summary>
        /// Gets or sets the value of variable signallingValue.
        /// </summary>
        [TestVariable("c2e11629-322d-4b63-afe5-6fef0cc6581e")]
        public string signallingValue
        {
            get { return _signallingValue; }
            set { _signallingValue = value; }
        }

        string _lowerLimitValue;

        /// <summary>
        /// Gets or sets the value of variable lowerLimitValue.
        /// </summary>
        [TestVariable("e01fd9fd-aa65-4395-8eb5-c1f689b71205")]
        public string lowerLimitValue
        {
            get { return _lowerLimitValue; }
            set { _lowerLimitValue = value; }
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

            signallingValue = GetRandomSignallingValue();
            Delay.Milliseconds(0);
            
            lowerLimitValue = GetRandomLowerLimitValue();
            Delay.Milliseconds(0);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
