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

namespace AutomatedSystemTests.Modules.ActionsPropertiesPanel
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The ExpandDamInPropertiesPanel recording.
    /// </summary>
    [TestModule("6e208fe1-d9b3-477a-8c12-d78d705a4908", ModuleType.Recording, 1)]
    public partial class ExpandDamInPropertiesPanel : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static ExpandDamInPropertiesPanel instance = new ExpandDamInPropertiesPanel();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ExpandDamInPropertiesPanel()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ExpandDamInPropertiesPanel Instance
        {
            get { return instance; }
        }

#region Variables

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

            ExpandDamInPropertiesPanelMethod(repo.RiskeerMainWindow.PropertiesPanelContainer.Table.GenericParameterVisibleInPropertiesPanelInfo);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
