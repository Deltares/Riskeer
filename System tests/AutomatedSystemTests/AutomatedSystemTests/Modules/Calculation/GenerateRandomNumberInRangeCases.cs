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

namespace AutomatedSystemTests.Modules.Calculation
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The GenerateRandomNumberInRangeCases recording.
    /// </summary>
    [TestModule("6104469d-a535-47ca-b393-2256db12cca4", ModuleType.Recording, 1)]
    public partial class GenerateRandomNumberInRangeCases : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static GenerateRandomNumberInRangeCases instance = new GenerateRandomNumberInRangeCases();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GenerateRandomNumberInRangeCases()
        {
            nameOfFolderWithCases = "";
            generatedRandomNumber = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static GenerateRandomNumberInRangeCases Instance
        {
            get { return instance; }
        }

#region Variables

        string _nameOfFolderWithCases;

        /// <summary>
        /// Gets or sets the value of variable nameOfFolderWithCases.
        /// </summary>
        [TestVariable("14b8b92f-c5ac-45fe-9221-5b190e11f77f")]
        public string nameOfFolderWithCases
        {
            get { return _nameOfFolderWithCases; }
            set { _nameOfFolderWithCases = value; }
        }

        string _generatedRandomNumber;

        /// <summary>
        /// Gets or sets the value of variable generatedRandomNumber.
        /// </summary>
        [TestVariable("ff4e49ec-c5ac-454c-81f0-9c2b3d84516c")]
        public string generatedRandomNumber
        {
            get { return _generatedRandomNumber; }
            set { _generatedRandomNumber = value; }
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

            generateRandomNumberInRangeRowsCases();
            Delay.Milliseconds(0);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
