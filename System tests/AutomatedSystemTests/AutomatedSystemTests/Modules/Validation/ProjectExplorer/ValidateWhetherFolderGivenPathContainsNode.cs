/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 11/11/2020
 * Time: 11:30
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Ranorex_Automation_Helpers.UserCodeCollections;

using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Validation.ProjectExplorer
{
    /// <summary>
    /// Description of ValidateWhetherFolderGivenPathContainsNode.
    /// </summary>
    [TestModule("DC82B628-B8BE-49E4-9BB1-14D06F8FDDFD", ModuleType.UserCode, 1)]
    public class ValidateWhetherFolderGivenPathContainsNode : ITestModule
    {
        
        string _pathToFolder = "";
        [TestVariable("18dbb304-aeb2-4af5-a75e-da05307d5569")]
        public string pathToFolder
        {
            get { return _pathToFolder; }
            set { _pathToFolder = value; }
        }
        
        string _nameOfNode = "";
        [TestVariable("67b6bc6d-0da4-4cfb-b318-dee7e61facb8")]
        public string nameOfNode
        {
            get { return _nameOfNode; }
            set { _nameOfNode = value; }
        }
        
        string _nodeIsExpectedToBeContained = "";
        [TestVariable("7cec956b-d645-4283-82ca-087a2f1a38e7")]
        public string nodeIsExpectedToBeContained
        {
            get { return _nodeIsExpectedToBeContained; }
            set { _nodeIsExpectedToBeContained = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ValidateWhetherFolderGivenPathContainsNode()
        {
            // Do not delete - a parameterless constructor is required!
        }

        /// <summary>
        /// Performs the playback of actions in this module.
        /// </summary>
        /// <remarks>You should not call this method directly, instead pass the module
        /// instance to the <see cref="TestModuleRunner.Run(ITestModule)"/> method
        /// that will in turn invoke this method.</remarks>
        void ITestModule.Run()
        {
            string path = pathToFolder.ReplacePathAliases();
            AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var rootNodeInfo = myRepository.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.SelfInfo;
            Action<TreeItem> actions = (it=> (it as Adapter).Focus());
            actions += (it=>ValidateNodeContainsChildWithName(it, nameOfNode, nodeIsExpectedToBeContained=="true"));
            TreeItemHelpers.FindNodeInTree(path, rootNodeInfo, actions);
            return;
        }
        
        private void ValidateNodeContainsChildWithName(TreeItem node, string nameOfChild, bool childIsExpected)
        {
            var children = node.Children;
            int numberOfChildrenWithExpectedName = children.Where(ch => TreeItemHelpers.NameOfTreeItem(ch.As<TreeItem>())==nameOfChild).ToList().Count;
            if (childIsExpected) {
        	    Report.Log(ReportLevel.Info, "Validating that folder '" + pathToFolder + "' contains one node with name '" + nameOfNode + "'.");
        	    Validate.IsTrue(numberOfChildrenWithExpectedName==1);
        	    node = children.FirstOrDefault(ch => ch.ToString().Contains(nameOfNode)).As<TreeItem>();
        	    Report.Screenshot(node.Element);
        	    } else {
        	    Report.Log(ReportLevel.Info, "Validating that folder '" + pathToFolder + "' contains no node with name '" + nameOfNode + "'.");
        	    Validate.IsTrue(numberOfChildrenWithExpectedName==0);
        	}
        }
    }
}
