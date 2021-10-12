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
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 0;
            Delay.SpeedFactor = 0.0;
            var stepsPathItem = pathToFolder.Split('>').ToList();
            
            AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var rootNodeInfo = myRepository.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.SelfInfo;
            
            //var children = rootNode.Children;
            IList<Ranorex.Unknown> children = (new List<Ranorex.Unknown>(){rootNodeInfo.FindAdapter<Ranorex.Unknown>()});
            
            var stepChild = rootNodeInfo.FindAdapter<TreeItem>();
        	var nameStepChild = NameOfTreeItem(stepChild);

        	for (int i=0; i < stepsPathItem.Count; i++) {
        			// Find the item corresponding to the step
        			var step = stepsPathItem[i];
        			if (children.Count(ch => ch.ToString().Contains(step))==1)
        				{
        				Report.Log(ReportLevel.Info, "Information", "Only one occurrence of '" + step + "' found: choosing item containing the string in its name.");
        				stepChild = children.FirstOrDefault(ch => ch.ToString().Contains(step)).As<TreeItem>();
        			} else	{
        				Report.Log(ReportLevel.Info, "Information", "Multiple occurrences of '" + step + "' found: choosing item with this exact name.");
        				stepChild = children.FirstOrDefault(ch => NameOfTreeItem(ch.As<TreeItem>())==step).As<TreeItem>();
        			}
        			// Update the children
        			children = stepChild.Children;
        			// Expand if intermediate node is collased
                    stepChild.Focus();
                    stepChild.Expand();
        			}
        	int numberOfChildrenWithNodeName = children.Where(ch => NameOfTreeItem(ch.As<TreeItem>())==nameOfNode).ToList().Count;
        	if (nodeIsExpectedToBeContained=="true") {
        	    Report.Log(ReportLevel.Info, "Validating that folder '" + pathToFolder + "' contains one node with name '" + nameOfNode + "'.");
        	    Validate.IsTrue(numberOfChildrenWithNodeName==1);
        	    stepChild = children.FirstOrDefault(ch => ch.ToString().Contains(nameOfNode)).As<TreeItem>();
        	    Report.Screenshot(stepChild.Element);
        	    } else {
        	    Report.Log(ReportLevel.Info, "Validating that folder '" + pathToFolder + "' contains no node with name '" + nameOfNode + "'.");
        	    Validate.IsTrue(numberOfChildrenWithNodeName==0);
        	}
        }
        
        private string NameOfTreeItem(object treeItemInfo)
        {
        	return treeItemInfo.ToString().Substring(10, treeItemInfo.ToString().Length-11);
        }
    }
}
