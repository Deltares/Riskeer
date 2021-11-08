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
    ///The SelectOptionFromContextMenu recording.
    /// </summary>
    [TestModule("b649f262-04cb-457f-b71d-d4572aeaf8bb", ModuleType.Recording, 1)]
    public partial class SelectOptionFromContextMenu : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static SelectOptionFromContextMenu instance = new SelectOptionFromContextMenu();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SelectOptionFromContextMenu()
        {
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static SelectOptionFromContextMenu Instance
        {
            get { return instance; }
        }

#region Variables

        /// <summary>
        /// Gets or sets the value of variable nameOfItem.
        /// </summary>
        [TestVariable("9ce62106-6b73-4d5b-8b48-7398d9284c21")]
        public string nameOfItem
        {
            get { return repo.nameOfItem; }
            set { repo.nameOfItem = value; }
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
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Left Click item 'ContextMenu.GenericItemInContextMenu' at Center.", repo.ContextMenu.GenericItemInContextMenuInfo, new RecordItemIndex(1));
            repo.ContextMenu.GenericItemInContextMenu.Click();
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}