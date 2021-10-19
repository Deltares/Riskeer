/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 05/11/2020
 * Time: 14:43
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
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Calculation
{
    /// <summary>
    /// Description of CalculateStructureProjectExplorerNode.
    /// </summary>
    [TestModule("A3E08090-A06F-4108-B55C-DE95E3E51CC4", ModuleType.UserCode, 1)]
    public class CalculateStructureProjectExplorerNode : ITestModule
    {
        
        string _pathToNodeInProjectExplorer = "";
        [TestVariable("08426158-665a-4b60-bf86-6b1f45d00494")]
        public string pathToNodeInProjectExplorer
        {
            get { return _pathToNodeInProjectExplorer; }
            set { _pathToNodeInProjectExplorer = value; }
        }
        
        
        string _structureNode = "";
        [TestVariable("7b7c74c0-9847-4740-b48b-2ca96ae3f3df")]
        public string structureNode
        {
            get { return _structureNode; }
            set { _structureNode = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public CalculateStructureProjectExplorerNode()
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
            
            AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            
            RepoItemInfo rootNodeProject = myRepository.RiskeerMainWindow.ProjectExplorerPanel.TrajectNode.SelfInfo;
            //TreeItem nodeTreeItem = GetNodeInProjectExplorerGivenPath(pathToNodeInProjectExplorer, rootNodeProject);
            TreeItem nodeTreeItem = TreeItemHelpers.FindNodeInTree(pathToNodeInProjectExplorer, rootNodeProject, (ti)=>{});
            structureNode = GetStructureTreeItem(nodeTreeItem);
        }
        
        private TreeItem GetNodeInProjectExplorerGivenPath(string pathItem, RepoItemInfo rootNodeInfo)
        	{
        	var stepsPathItem = pathItem.Split('>').ToList();
        	var children = rootNodeInfo.FindAdapter<TreeItem>().Children;
        	// start up variable stepChild
        	TreeItem stepChild = children[0].As<TreeItem>();
        	
        	
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
        			if (i != stepsPathItem.Count - 1)
        				{
        				// Update the children
        				children = stepChild.Children;
        	            }
        	}
        	return stepChild;
        }
        
        private string NameOfTreeItem(object treeItemInfo)
        {
        	return treeItemInfo.ToString().Substring(10, treeItemInfo.ToString().Length-11);
        }
        
        private string GetStructureTreeItem(TreeItem treeitem)
        {
            var children = treeitem.Children;
            if (children.Count==0) {
                return "[" + NameOfTreeItem(treeitem) + "]";
            } else{
                string structureChildren = "[" + NameOfTreeItem(treeitem) + "->";
                foreach (var child in children) {
                    structureChildren += GetStructureTreeItem(child.As<TreeItem>());
                }
                structureChildren += "]";
                return structureChildren;
            }
        }
    }
}
