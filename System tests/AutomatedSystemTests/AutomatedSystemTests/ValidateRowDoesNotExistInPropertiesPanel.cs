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
    ///The ValidateRowDoesNotExistInPropertiesPanel recording.
    /// </summary>
    [TestModule("a91dc990-2aae-4244-86f4-1f59055da825", ModuleType.Recording, 1)]
    public partial class ValidateRowDoesNotExistInPropertiesPanel : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static ValidateRowDoesNotExistInPropertiesPanel instance = new ValidateRowDoesNotExistInPropertiesPanel();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateRowDoesNotExistInPropertiesPanel()
        {
            nameOfParameterInPropertiesPanel = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidateRowDoesNotExistInPropertiesPanel Instance
        {
            get { return instance; }
        }

#region Variables

        /// <summary>
        /// Gets or sets the value of variable nameOfParameterInPropertiesPanel.
        /// </summary>
        [TestVariable("b366a34a-f5ed-4519-a8e4-7450dafb84e4")]
        public string nameOfParameterInPropertiesPanel
        {
            get { return repo.nameOfParameterInPropertiesPanel; }
            set { repo.nameOfParameterInPropertiesPanel = value; }
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

            Report.Log(ReportLevel.Info, "User", "Counting occurrences of $nameOfParameterInPropertiesPanel  in Properties Panel", new RecordItemIndex(0));
            
            Validate_GenericParameterVisibleInProjectExplorer(repo.RiskeerMainWindow.PropertiesPanelContainer.Table.Self);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
