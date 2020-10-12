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
    ///The CalculateLinearTransformationOfValue recording.
    /// </summary>
    [TestModule("ff1cb4d9-1388-49e6-bf2a-00ed354dca1e", ModuleType.Recording, 1)]
    public partial class CalculateLinearTransformationOfValue : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static CalculateLinearTransformationOfValue instance = new CalculateLinearTransformationOfValue();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CalculateLinearTransformationOfValue()
        {
            returnedValue = "";
            aParameter = "";
            bParameter = "";
            xVariable = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static CalculateLinearTransformationOfValue Instance
        {
            get { return instance; }
        }

#region Variables

        string _returnedValue;

        /// <summary>
        /// Gets or sets the value of variable returnedValue.
        /// </summary>
        [TestVariable("5c3fdb07-7073-459d-8c51-922caeb92a86")]
        public string returnedValue
        {
            get { return _returnedValue; }
            set { _returnedValue = value; }
        }

        string _aParameter;

        /// <summary>
        /// Gets or sets the value of variable aParameter.
        /// </summary>
        [TestVariable("96607dd0-1076-4d31-a7dd-d20dfee7c567")]
        public string aParameter
        {
            get { return _aParameter; }
            set { _aParameter = value; }
        }

        string _bParameter;

        /// <summary>
        /// Gets or sets the value of variable bParameter.
        /// </summary>
        [TestVariable("16f51e88-473c-4f0d-b4a9-810b68fea466")]
        public string bParameter
        {
            get { return _bParameter; }
            set { _bParameter = value; }
        }

        string _xVariable;

        /// <summary>
        /// Gets or sets the value of variable xVariable.
        /// </summary>
        [TestVariable("32b52b59-7056-4c43-85c8-445a27bd4e65")]
        public string xVariable
        {
            get { return _xVariable; }
            set { _xVariable = value; }
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

            returnedValue = LinearTransformation(aParameter, bParameter);
            Delay.Milliseconds(0);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
