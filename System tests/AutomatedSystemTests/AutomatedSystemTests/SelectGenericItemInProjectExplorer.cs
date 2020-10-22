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
    ///The SelectGenericItemInProjectExplorer recording.
    /// </summary>
    [TestModule("ff04c0e9-5dfd-4aa7-b9fc-b8ec84f1399e", ModuleType.Recording, 1)]
    public partial class SelectGenericItemInProjectExplorer : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static SelectGenericItemInProjectExplorer instance = new SelectGenericItemInProjectExplorer();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SelectGenericItemInProjectExplorer()
        {
            pathToItemToOpenView = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static SelectGenericItemInProjectExplorer Instance
        {
            get { return instance; }
        }

#region Variables

        string _pathToItemToOpenView;

        /// <summary>
        /// Gets or sets the value of variable pathToItemToOpenView.
        /// </summary>
        [TestVariable("a31c9d25-98bf-4be6-a0e1-d56bcf8fe1d8")]
        public string pathToItemToOpenView
        {
            get { return _pathToItemToOpenView; }
            set { _pathToItemToOpenView = value; }
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

            SelectTreeItemInProjectExplorerGivenPath(pathToItemToOpenView, repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.SelfInfo);
            Delay.Milliseconds(0);
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
