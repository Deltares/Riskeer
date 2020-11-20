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

namespace AutomatedSystemTests.Modules.ActionsContextMenu
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The GenerateCalculations recording.
    /// </summary>
    [TestModule("8513f955-1cc5-4468-875c-d51ce8a545ba", ModuleType.Recording, 1)]
    public partial class GenerateCalculations : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static GenerateCalculations instance = new GenerateCalculations();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public GenerateCalculations()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static GenerateCalculations Instance
        {
            get { return instance; }
        }

#region Variables

        /// <summary>
        /// Gets or sets the value of variable indexRow.
        /// </summary>
        [TestVariable("201b1d21-866e-4c9e-bf76-93ca440565ea")]
        public string indexRow
        {
            get { return repo.indexRow; }
            set { repo.indexRow = value; }
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

            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{Apps}'.", new RecordItemIndex(0));
            Keyboard.Press("{Apps}");
            
            // Select Genereer scenario's from context menu
            Report.Log(ReportLevel.Info, "Mouse", "Select Genereer scenario's from context menu\r\nMouse Left Click item 'ContextMenu.GenereerScenarios' at Center.", repo.ContextMenu.GenereerScenariosInfo, new RecordItemIndex(1));
            repo.ContextMenu.GenereerScenarios.Click();
            
            // Select the Indexth row, cell Use
            Report.Log(ReportLevel.Info, "Mouse", "Select the Indexth row, cell Use\r\nMouse Left Click item 'DialogGenerateCalculations.UseCellRowIndexth' at Center.", repo.DialogGenerateCalculations.UseCellRowIndexthInfo, new RecordItemIndex(2));
            repo.DialogGenerateCalculations.UseCellRowIndexth.Click();
            
            // Click on Generate button to generate all desired calculations
            Report.Log(ReportLevel.Info, "Mouse", "Click on Generate button to generate all desired calculations\r\nMouse Left Click item 'DialogGenerateCalculations.GenerateButton' at Center.", repo.DialogGenerateCalculations.GenerateButtonInfo, new RecordItemIndex(3));
            repo.DialogGenerateCalculations.GenerateButton.Click();
            
            // Wait until all calculations have been generated
            Report.Log(ReportLevel.Info, "Delay", "Wait until all calculations have been generated\r\nWaiting for 1s.", new RecordItemIndex(4));
            Delay.Duration(1000, false);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
