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

namespace AutomatedSystemTests.Modules.Set_Assign
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The SetParameterInCalculationsView recording.
    /// </summary>
    [TestModule("7413cc50-50d0-4361-8163-7d8d3b57b81d", ModuleType.Recording, 1)]
    public partial class SetParameterInCalculationsView : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static SetParameterInCalculationsView instance = new SetParameterInCalculationsView();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SetParameterInCalculationsView()
        {
            valueParameter = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static SetParameterInCalculationsView Instance
        {
            get { return instance; }
        }

#region Variables

        string _valueParameter;

        /// <summary>
        /// Gets or sets the value of variable valueParameter.
        /// </summary>
        [TestVariable("33a3a4d5-6b9a-420a-b2c8-cdb117a312f4")]
        public string valueParameter
        {
            get { return _valueParameter; }
            set { _valueParameter = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable indexRowCalculation.
        /// </summary>
        [TestVariable("22cf54d8-7caa-49ab-8e5b-1c14fb3ba580")]
        public string indexRowCalculation
        {
            get { return repo.indexRowCalculation; }
            set { repo.indexRowCalculation = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable nameColumn.
        /// </summary>
        [TestVariable("0654e545-9710-4c76-b4ae-a428f59a6303")]
        public string nameColumn
        {
            get { return repo.nameColumn; }
            set { repo.nameColumn = value; }
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

            Report.Log(ReportLevel.Info, "Set value", "Setting attribute AccessibleValue to '$valueParameter' on item 'RiskeerMainWindow.DocumentViewContainerUncached.CalculationsView.TableSelectedSection.GenericRow.GenericCell'.", repo.RiskeerMainWindow.DocumentViewContainerUncached.CalculationsView.TableSelectedSection.GenericRow.GenericCellInfo, new RecordItemIndex(0));
            repo.RiskeerMainWindow.DocumentViewContainerUncached.CalculationsView.TableSelectedSection.GenericRow.GenericCell.Element.SetAttributeValue("AccessibleValue", valueParameter);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
