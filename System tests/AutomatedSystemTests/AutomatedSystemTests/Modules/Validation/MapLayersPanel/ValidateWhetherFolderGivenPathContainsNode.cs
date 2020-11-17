/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 17/11/2020
 * Time: 18:10
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

namespace AutomatedSystemTests.Modules.Validation.MapLayersPanel
{
    /// <summary>
    /// Description of ValidateWhetherFolderGivenPathContainsNode.
    /// </summary>
    [TestModule("D902C15B-F5AA-4DB3-A0A1-E4AF2B09C2CA", ModuleType.UserCode, 1)]
    public class ValidateWhetherFolderGivenPathContainsNode : ITestModule
    {
        
        
        string _pathToFolder = "";
        [TestVariable("78cde0a3-4042-453e-a0dd-656345445114")]
        public string pathToFolder
        {
            get { return _pathToFolder; }
            set { _pathToFolder = value; }
        }
        
        
        string _nameOfNode = "";
        [TestVariable("a99747df-1774-42b3-9164-2861023d359c")]
        public string nameOfNode
        {
            get { return _nameOfNode; }
            set { _nameOfNode = value; }
        }
        
        string _nodeIsExpectedToBeContained = "";
        [TestVariable("6549233a-4e6c-4e40-96d1-5453ee5f08ec")]
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
            
            AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            
            var rootNode = myRepository.RiskeerMainWindow.MapLegendPanel.RootNode.Self;
            TreeItem stepChild = rootNode.As<TreeItem>();
            var children = rootNode.Children;
            
            if (pathToFolder!="") {
                var stepsPathItem = pathToFolder.Split('>').ToList();
                stepChild = children[0].As<TreeItem>();
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
            }
             int numberOfChildrenWithNodeName = children.Where(ch => NameOfTreeItem(ch.As<TreeItem>())==nameOfNode).ToList().Count;
            if (nodeIsExpectedToBeContained=="true") {
                Report.Log(ReportLevel.Info, "Validating that folder '" + pathToFolder + "' contains one node with name '" + nameOfNode + "'.");
                Validate.IsTrue(numberOfChildrenWithNodeName==1);
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
