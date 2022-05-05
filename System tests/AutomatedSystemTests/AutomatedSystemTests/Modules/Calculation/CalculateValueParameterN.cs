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
    ///The CalculateValueParameterN recording.
    /// </summary>
    [TestModule("2fdf315c-3bbf-4b7a-b901-e88b144bf94e", ModuleType.Recording, 1)]
    public partial class CalculateValueParameterN : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static CalculateValueParameterN instance = new CalculateValueParameterN();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CalculateValueParameterN()
        {
            valueOfParameterInPropertiesPanel = "";
            fmLabel = "";
            nameOfParameterInPropertiesPanel = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static CalculateValueParameterN Instance
        {
            get { return instance; }
        }

#region Variables

        string _valueOfParameterInPropertiesPanel;

        /// <summary>
        /// Gets or sets the value of variable valueOfParameterInPropertiesPanel.
        /// </summary>
        [TestVariable("36cb73b7-04f3-4751-8474-8aef02c92ab7")]
        public string valueOfParameterInPropertiesPanel
        {
            get { return _valueOfParameterInPropertiesPanel; }
            set { _valueOfParameterInPropertiesPanel = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable substringNameItemInTraject.
        /// </summary>
        [TestVariable("cb52c14e-9bef-4f4a-9d11-1758141c50cb")]
        public string substringNameItemInTraject
        {
            get { return repo.substringNameItemInTraject; }
            set { repo.substringNameItemInTraject = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable fmLabel.
        /// </summary>
        [TestVariable("e5da979c-abe6-4a71-b419-78bbd2a92489")]
        public string fmLabel
        {
            get { return repo.fmLabel; }
            set { repo.fmLabel = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable nameOfParameterInPropertiesPanel.
        /// </summary>
        [TestVariable("afe1ce8e-ff00-4307-a9d8-1f91fa796fad")]
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

            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.GenericItemInTraject'.", repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.GenericItemInTrajectInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.GenericItemInTraject.Focus();
            
            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Select() on item 'RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.GenericItemInTraject'.", repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.GenericItemInTrajectInfo, new RecordItemIndex(1));
            repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.GenericItemInTraject.Select();
            
            CalculateValueNFromFMParameters();
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
