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
    ///The RenameCurrentlySelectedItem recording.
    /// </summary>
    [TestModule("30f153a9-9c98-400c-a020-34c10ac7af64", ModuleType.Recording, 1)]
    public partial class RenameCurrentlySelectedItem : ITestModule
    {
        /// <summary>
        /// Holds an instance of the global::AutomatedSystemTests.AutomatedSystemTestsRepository repository.
        /// </summary>
        public static global::AutomatedSystemTests.AutomatedSystemTestsRepository repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;

        static RenameCurrentlySelectedItem instance = new RenameCurrentlySelectedItem();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public RenameCurrentlySelectedItem()
        {
            newNameItem = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static RenameCurrentlySelectedItem Instance
        {
            get { return instance; }
        }

#region Variables

        string _newNameItem;

        /// <summary>
        /// Gets or sets the value of variable newNameItem.
        /// </summary>
        [TestVariable("1cd2936d-1a59-46ff-b9e0-89eedd021915")]
        public string newNameItem
        {
            get { return _newNameItem; }
            set { _newNameItem = value; }
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

            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{F2}'.", new RecordItemIndex(0));
            Keyboard.Press("{F2}");
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence from variable '$newNameItem'.", new RecordItemIndex(1));
            Keyboard.Press(newNameItem);
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{Return}'.", new RecordItemIndex(2));
            Keyboard.Press("{Return}");
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
