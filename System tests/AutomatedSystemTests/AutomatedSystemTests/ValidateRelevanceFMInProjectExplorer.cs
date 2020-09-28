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
    ///The ValidateRelevanceFMInProjectExplorer recording.
    /// </summary>
    [TestModule("f42a383d-8a05-4e55-bddb-48e5a69b14e6", ModuleType.Recording, 1)]
    public partial class ValidateRelevanceFMInProjectExplorer : ITestModule
    {
        /// <summary>
        /// Holds an instance of the AutomatedSystemTestsRepository repository.
        /// </summary>
        public static AutomatedSystemTestsRepository repo = AutomatedSystemTestsRepository.Instance;

        static ValidateRelevanceFMInProjectExplorer instance = new ValidateRelevanceFMInProjectExplorer();

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateRelevanceFMInProjectExplorer()
        {
            fmExpectedRelevance = "";
        }

        /// <summary>
        /// Gets a static instance of this recording.
        /// </summary>
        public static ValidateRelevanceFMInProjectExplorer Instance
        {
            get { return instance; }
        }

#region Variables

        string _fmExpectedRelevance;

        /// <summary>
        /// Gets or sets the value of variable fmExpectedRelevance.
        /// </summary>
        [TestVariable("e71d741d-fe4d-41b8-ba2d-00fc7872d25f")]
        public string fmExpectedRelevance
        {
            get { return _fmExpectedRelevance; }
            set { _fmExpectedRelevance = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable substringTrajectName.
        /// </summary>
        [TestVariable("77ae6c27-603e-4704-add9-e1249169f0e5")]
        public string substringTrajectName
        {
            get { return repo.substringTrajectName; }
            set { repo.substringTrajectName = value; }
        }

        /// <summary>
        /// Gets or sets the value of variable substringItemName.
        /// </summary>
        [TestVariable("3a7276c1-fca1-4026-9d2e-5bac10651a47")]
        public string substringItemName
        {
            get { return repo.substringItemName; }
            set { repo.substringItemName = value; }
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

            Report.Log(ReportLevel.Info, "User", "Validating number of subnodes", new RecordItemIndex(0));
            
            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Focus() on item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInName'.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInNameInfo, new RecordItemIndex(1));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInName.Focus();
            
            Report.Log(ReportLevel.Info, "Invoke action", "Invoking Select() on item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInName'.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInNameInfo, new RecordItemIndex(2));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInName.Select();
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{Right}'.", new RecordItemIndex(3));
            Keyboard.Press("{Right}");
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.ProjectExplorer.Self, false, new RecordItemIndex(4));
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{Left}'.", new RecordItemIndex(5));
            Keyboard.Press("{Left}");
            
            try {
                ValidateNumberOfNodesInFM(repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInNameInfo, fmExpectedRelevance);
            } catch(Exception ex) { Report.Log(ReportLevel.Warn, "Module", "(Optional Action) " + ex.Message, new RecordItemIndex(6)); }
            
            Report.Log(ReportLevel.Info, "User", "Validating context menu", new RecordItemIndex(7));
            
            Report.Log(ReportLevel.Info, "Mouse", "Mouse Right Click item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInName' at Center.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInNameInfo, new RecordItemIndex(8));
            repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInName.Click(System.Windows.Forms.MouseButtons.Right);
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.ContextMenu.Self, false, new RecordItemIndex(9));
            
            try {
                ValidateNumberOfItemsInContextMenu(repo.ContextMenu.SelfInfo, fmExpectedRelevance);
            } catch(Exception ex) { Report.Log(ReportLevel.Warn, "Module", "(Optional Action) " + ex.Message, new RecordItemIndex(10)); }
            
            Report.Log(ReportLevel.Info, "Keyboard", "Key sequence '{Escape}'.", new RecordItemIndex(11));
            Keyboard.Press("{Escape}");
            
            Report.Log(ReportLevel.Info, "User", "Validating property panel when selected", new RecordItemIndex(12));
            
            try {
                Report.Log(ReportLevel.Info, "Mouse", "(Optional Action)\r\nMouse Left Click item 'RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInName' at Center.", repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInNameInfo, new RecordItemIndex(13));
                repo.RiskeerMainWindow.ProjectExplorer.ProjectRootNode.TrajectWithSubstringInName.GenericItemWithSubstringInName.Click();
            } catch(Exception ex) { Report.Log(ReportLevel.Warn, "Module", "(Optional Action) " + ex.Message, new RecordItemIndex(13)); }
            
            Report.Screenshot(ReportLevel.Info, "User", "", repo.RiskeerMainWindow.PropertiesPanelContainer.Table.IsRelevant, false, new RecordItemIndex(14));
            
            try {
                Report.Log(ReportLevel.Info, "Validation", "(Optional Action)\r\nValidating AttributeEqual (AccessibleValue=$fmExpectedRelevance) on item 'RiskeerMainWindow.PropertiesPanelContainer.Table.IsRelevant'.", repo.RiskeerMainWindow.PropertiesPanelContainer.Table.IsRelevantInfo, new RecordItemIndex(15));
                Validate.AttributeEqual(repo.RiskeerMainWindow.PropertiesPanelContainer.Table.IsRelevantInfo, "AccessibleValue", fmExpectedRelevance, null, false);
            } catch(Exception ex) { Report.Log(ReportLevel.Warn, "Module", "(Optional Action) " + ex.Message, new RecordItemIndex(15)); }
            
        }

#region Image Feature Data
#endregion
    }
#pragma warning restore 0436
}
