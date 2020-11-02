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

namespace AutomatedSystemTests.Modules.Selection
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The SelectItemFromDynamicDropDownMenuInPropertiesPanelRow recording.
    /// </summary>
    [TestModule("feb22e37-85f1-4e45-95cc-1cebb52e7292", ModuleType.Recording, 1)]
    public partial class SelectItemFromDynamicDropDownMenuInPropertiesPanelRow : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static SelectItemFromDynamicDropDownMenuInPropertiesPanelRow instance = new SelectItemFromDynamicDropDownMenuInPropertiesPanelRow();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SelectItemFromDynamicDropDownMenuInPropertiesPanelRow()
        {
            newValueForParameter = "";
            pathToItemInPropertiesPanel = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static SelectItemFromDynamicDropDownMenuInPropertiesPanelRow Instance
        {
            get { return instance; }
        }

#region Variables

        string _pathToItemInPropertiesPanel;

        /// <summary>
        /// Gets or sets the value of variable pathToItemInPropertiesPanel.
        /// </summary>
        [TestVariable("be2f1205-1a50-44d6-a190-fa98d028c783")]
        public string pathToItemInPropertiesPanel
        {
            get { return _pathToItemInPropertiesPanel; }
            set { _pathToItemInPropertiesPanel = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable newValueForParameter.
        /// </summary>
        [TestVariable("3ed76e3f-c80c-42a1-9230-37334bd6b5c8")]
        public string newValueForParameter
        {
            get { return repo.newValueForParameter; }
            set { repo.newValueForParameter = value; }
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

            Report.Log(ReportLevel.Info, "User", newValueForParameter, new RecordItemIndex(0));
            
            SelectItemFromDynamicDropDownMenuInRowPropertiesPanel(repo.DropDownMenuInRowPropertiesPanel.List.genericItemInDropDownListInfo, pathToItemInPropertiesPanel);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
