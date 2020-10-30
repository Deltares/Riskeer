﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// Your custom recording code should go in this file.
// The designer will only add methods to this file, so your custom code won't be overwritten.
// http://www.ranorex.com
//
///////////////////////////////////////////////////////////////////////////////

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
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Selection
{
    public partial class SelectGenericItemInProjectExplorer
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void SelectTreeItemInProjectExplorerGivenPath(string pathItem, RepoItemInfo rootNodeInfo)
        	{
        	var stepsPathItem = pathItem.Split('>').ToList();
        	var children = rootNodeInfo.FindAdapter<TreeItem>().Children;
        	// start up variable stepChild
        	var stepChild = children[0].As<TreeItem>();
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
        			stepChild.Focus();
        			if (i != stepsPathItem.Count - 1)
        				{
        				stepChild.Expand();
        				}
        			else {
        				stepChild.Click(Location.CenterLeft);
        			}
        			// Update the children
        			children = stepChild.Children;
        			}
        	return;
        }
        
        private string NameOfTreeItem(object treeItemInfo)
        {
        	return treeItemInfo.ToString().Substring(10, treeItemInfo.ToString().Length-11);
        }
    }
}
