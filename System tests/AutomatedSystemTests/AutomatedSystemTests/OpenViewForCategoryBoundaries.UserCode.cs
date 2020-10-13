﻿///////////////////////////////////////////////////////////////////////////////
//
// This file was automatically generated by RANOREX.
// Your custom recording code should go in this file.
// The designer will only add methods to this file, so your custom code won't be overwritten.
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
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests
{
    public partial class OpenViewForCategoryBoundaries
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void OpenCategoryBoundariesView(RepoItemInfo nodeItemInfo, string nameSubnode)
        {
            string nameSubnodeLevel1 = "Oordeel";
            string nameSubnodeLevel2 = "Categoriegrenzen";
        	var nodeChildren = nodeItemInfo.FindAdapter<TreeItem>().Children;
            foreach (var childNode in nodeChildren) {
            	string childNodeName = NameOfTreeItem(childNode);
            	if (childNodeName == nameSubnodeLevel1) {
            		childNode.As<TreeItem>().Expand();
            		var grandchildren = childNode.As<TreeItem>().Children;
            		foreach (var grandchild in grandchildren) {
            			string grandchildName = NameOfTreeItem(grandchild);
            			if (grandchildName == nameSubnodeLevel2) {
            				grandchild.As<TreeItem>().Focus();
            				grandchild.As<TreeItem>().DoubleClick();
            			}
            		}
            	}
            }
        }
		
        private string NameOfTreeItem(object treeItemInfo)
        {
        	return treeItemInfo.ToString().Substring(10, treeItemInfo.ToString().Length-11);
        }
        
        public void ExpandNode(RepoItemInfo nodeItemInfo)
        {
        	try {
        		nodeItemInfo.CreateAdapter<TreeItem>(true).Expand();
        	} catch (Exception) {
        	}
        }

    }
}
