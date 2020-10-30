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

namespace AutomatedSystemTests.Modules.Validation
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The ValidateDoubleValueParameterCurrentlyShownInPropertiesPanel recording.
    /// </summary>
    [TestModule("ee50ca81-702d-41bf-9d5a-c6659515e29c", ModuleType.Recording, 1)]
    public partial class ValidateDoubleValueParameterCurrentlyShownInPropertiesPanel : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ValidateDoubleValueParameterCurrentlyShownInPropertiesPanel instance = new ValidateDoubleValueParameterCurrentlyShownInPropertiesPanel();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateDoubleValueParameterCurrentlyShownInPropertiesPanel()
        {
            expectedValue = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidateDoubleValueParameterCurrentlyShownInPropertiesPanel Instance
        {
            get { return instance; }
        }

#region Variables

        string _expectedValue;

        /// <summary>
        /// Gets or sets the value of variable expectedValue.
        /// </summary>
        [TestVariable("c4454e97-5bc7-4a1c-9898-54fd594a5f41")]
        public string expectedValue
        {
            get { return _expectedValue; }
            set { _expectedValue = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable nameOfParameterInPropertiesPanel.
        /// </summary>
        [TestVariable("be55172b-156d-4b6c-a990-593bf6c5d6a2")]
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

            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer'.", repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorerInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer.Focus();
            
            Validate_GenericParameterVisibleInProjectExplorer(repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorerInfo);
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInProjectExplorer, false, new RecordItemIndex(2));
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
