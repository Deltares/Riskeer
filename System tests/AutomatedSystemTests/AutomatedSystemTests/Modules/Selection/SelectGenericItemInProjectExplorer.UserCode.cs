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
        	Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 0;
            Delay.SpeedFactor = 0.0;
            
            var stepsPathItem = pathItem.Split('>').ToList();
            IList<Ranorex.Unknown> children = (new List<Ranorex.Unknown>(){rootNodeInfo.FindAdapter<Ranorex.Unknown>()});
            var stepChild = rootNodeInfo.FindAdapter<TreeItem>();

        	for (int i=0; i < stepsPathItem.Count; i++) {
        			// Find the item corresponding to the step
        			var step = stepsPathItem[i];
        			var childrenWithStepInName = children.Where(ch => ch.ToString().Contains(step));
        			int amountChildrenWithStepInName = childrenWithStepInName.Count();
        			if (amountChildrenWithStepInName==1)
        				{
        			    stepChild = childrenWithStepInName.FirstOrDefault().As<TreeItem>();
        			} else if (amountChildrenWithStepInName>1){
        				Report.Info("Information", "Multiple occurrences of '" + step + "' found: choosing first item with this exact name.");
        				stepChild = childrenWithStepInName.FirstOrDefault(ch => NameOfTreeItem(ch.As<TreeItem>())==step).As<TreeItem>();
        			} else {
        			    Report.Error("Error", "No occurrences of '" + step + "' found.");
        			}
        			if (i != stepsPathItem.Count - 1)
        				{
        				// Update the children
        				children = stepChild.Children;
        				} else {
        			    // child is last one in path
        			    stepChild.Focus();
        			    stepChild.ClickWithoutBoundsCheck(new Location(-0.02, 0.5));
        			    }
        			}
        	return;
        }
        
        private string NameOfTreeItem(object treeItemInfo)
        {
        	return treeItemInfo.ToString().Substring(10, treeItemInfo.ToString().Length-11);
        }
    }
}
