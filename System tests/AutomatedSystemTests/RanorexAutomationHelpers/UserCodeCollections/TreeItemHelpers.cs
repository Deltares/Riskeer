/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 18/10/2021
 * Time: 18:39
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
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace Ranorex_Automation_Helpers.UserCodeCollections
{
    /// <summary>
    /// Creates a Ranorex user code collection. A collection is used to publish user code methods to the user code library.
    /// </summary>
    [UserCodeCollection]
    public static class TreeItemHelpers
    {
        public static TreeItem FindNodeInTree(string pathItem, RepoItemInfo rootNodeInfo, Action<TreeItem> actionsOnFinalChild)
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
                if (step=="*" && children.Count==1) {
                    Report.Info("Information", "Using wildcard '*'. One single child found.");
                    stepChild = children.Single().As<TreeItem>();
                } else {
                    var childrenWithStepInName = children.Where(ch => ch.ToString().Contains(step));
                    int amountChildrenWithStepInName = childrenWithStepInName.Count();
                    if (amountChildrenWithStepInName==1)
                        {
                            stepChild = childrenWithStepInName.FirstOrDefault().As<TreeItem>();
                        } else if (amountChildrenWithStepInName>1){
                            Report.Info("Information", "Multiple occurrences of '" + step + "' found: choosing first item with this exact name.");
                            stepChild = childrenWithStepInName.FirstOrDefault(ch => NameOfTreeItem(ch.As<TreeItem>())==step).As<TreeItem>();
                        } else {
                            throw new Ranorex.RanorexException("No occurrences of '" + step + "' found.");
                        }
                }
                if (i != stepsPathItem.Count - 1)
                    {
                        // Update the children
                        children = stepChild.Children;
                    } else {
                        // child is last one in path
                        actionsOnFinalChild(stepChild);
                    }
                }
            return stepChild;
        }
        
        private static string NameOfTreeItem(object treeItemInfo)
        {
            return treeItemInfo.ToString().Substring(10, treeItemInfo.ToString().Length-11);
        }
        
    }
}
