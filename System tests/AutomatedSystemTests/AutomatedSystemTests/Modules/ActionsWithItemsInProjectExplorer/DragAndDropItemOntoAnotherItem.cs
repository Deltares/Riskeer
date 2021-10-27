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

namespace AutomatedSystemTests.Modules.ActionsWithItemsInProjectExplorer
{
#pragma warning disable 0436 //(CS0436) The type 'type' in 'assembly' conflicts with the imported type 'type2' in 'assembly'. Using the type defined in 'assembly'.
    /// <summary>
    ///The DragAndDropItemOntoAnotherItem recording.
    /// </summary>
    [TestModule("909ea538-c662-4239-a56a-672c6d28b694", ModuleType.Recording, 1)]
    public partial class DragAndDropItemOntoAnotherItem : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static DragAndDropItemOntoAnotherItem instance = new DragAndDropItemOntoAnotherItem();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public DragAndDropItemOntoAnotherItem()
        {
            pathItemToMove = "";
            pathItemDestination = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static DragAndDropItemOntoAnotherItem Instance
        {
            get { return instance; }
        }

#region Variables

        string _pathItemToMove;

        /// <summary>
        /// Gets or sets the value of variable pathItemToMove.
        /// </summary>
        [TestVariable("0b446edc-7bfd-4e53-b683-8c3dd2b46bb3")]
        public string pathItemToMove
        {
            get { return _pathItemToMove; }
            set { _pathItemToMove = value; }
        }

        string _pathItemDestination;

        /// <summary>
        /// Gets or sets the value of variable pathItemDestination.
        /// </summary>
        [TestVariable("f21e4405-5055-47d1-a985-93bb7d25200f")]
        public string pathItemDestination
        {
            get { return _pathItemDestination; }
            set { _pathItemDestination = value; }
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

            DragAndDropProjectExplorerItemOntoAnotherOne(pathItemToMove, pathItemDestination, repo.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.SelfInfo);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
