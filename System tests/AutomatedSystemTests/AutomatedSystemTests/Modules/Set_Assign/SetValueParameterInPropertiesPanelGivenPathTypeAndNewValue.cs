﻿/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 30/10/2020
 * Time: 12:12
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using WinForms = System.Windows.Forms;
using Ranorex;
using Ranorex.Core;
using Ranorex.Core.Repository;
using Ranorex.Core.Testing;

namespace AutomatedSystemTests.Modules.Set_Assign
{
    /// <summary>
    /// Description of SetValueParameterInPropertiesPanelGivenPathTypeAndNewValue.
    /// </summary>
    [TestModule("52D4C162-4D43-4281-9B87-3CF96D0FA894", ModuleType.UserCode, 1)]
    public class SetValueParameterInPropertiesPanelGivenPathTypeAndNewValue : ITestModule
    {
        
        string _pathToElementInPropertiesPanel = "";
        [TestVariable("a6c6752c-53a8-4e28-a7d5-e369855d34cb")]
        public string pathToElementInPropertiesPanel
        {
        	get { return _pathToElementInPropertiesPanel; }
        	set { _pathToElementInPropertiesPanel = value; }
        }
        
        
        string _typeParameter = "";
        [TestVariable("f6cc0a7d-f65c-4a45-8975-405678527d6a")]
        public string typeParameter
        {
        	get { return _typeParameter; }
        	set { _typeParameter = value; }
        }
        
        
        string _newValueForParameter = "";
        [TestVariable("57b81fcc-fdb5-4138-9a1d-0f85b3b8c8b6")]
        public string newValueForParameter
        {
        	get { return _newValueForParameter; }
        	set { _newValueForParameter = value; }
        }
        
        public Ranorex.Row GetRowInPropertiesPanelGivenPath(Adapter argumentAdapter, string pathItem)
        	{
        	int minimumIndex = 0;
        	var stepsPathItem = pathItem.Split('>').ToList();
        	Ranorex.Row stepRow = argumentAdapter.As<Table>().Rows.ToList()[1];
        	for (int i=0; i < stepsPathItem.Count; i++) {
        			// Find the item corresponding to the step
        			var step = stepsPathItem[i];
        			var completeList = argumentAdapter.As<Table>().Rows.ToList();
        			var searchList = completeList.GetRange(minimumIndex, completeList.Count-minimumIndex);
        			var indexStepRow = searchList.FindIndex(rw => rw.GetAttributeValue<string>("AccessibleName").Contains(step));
        			stepRow = searchList[indexStepRow];
        			// Select (and expand) the item
        			stepRow.Focus();
        			stepRow.Select();
        			if (i != stepsPathItem.Count - 1)
        				{
        					stepRow.PressKeys("{Right}");
        				}
        			
        			// Update the minimum index administration (only search forward)
        			minimumIndex += 1 + indexStepRow;
        			}
        	return stepRow;
        	}
        
        
        public void SetDoubleParameterInPropertiesPanel(Ranorex.Row row, string newValue)
        {
        	//Convert string number from fixed culture of tables in Ranorex to double
        	// and then back to string using current culture
        	System.Globalization.CultureInfo fixedDataSourceCulture = new CultureInfo("en-US");
        	fixedDataSourceCulture.NumberFormat.NumberDecimalSeparator = ".";
        	fixedDataSourceCulture.NumberFormat.NumberGroupSeparator = "";
        	System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
        	newValue = Double.Parse(newValue, fixedDataSourceCulture).ToString(currentCulture);
        	// Assign converted value
        	row.Element.SetAttributeValue("AccessibleValue", newValue);
        }
        
        public void SetTextParameterInPropertiesPanel(Ranorex.Row row, string newValue)
        {
        	row.Element.SetAttributeValue("AccessibleValue", newValue);
        }
        
        public void OpenDropDownMenuParameterInPropertiesPanel(Ranorex.Row row, string newValueForParameter) //, RepoItemInfo itemToSelectInfo)
        {
			//AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
			row.Click();
			
			//var itemToSelectInfo = myRepository.DropDownMenuInRowPropertiesPanel.List.genericItemInDropDownListInfo;
			
			//row.Click(".98;.5");
            //itemToSelectInfo.FindAdapter<ListItem>().Focus();
            //itemToSelectInfo.FindAdapter<ListItem>().Click(); 
			//var childrenList = myRepository.DropDownMenuInRowPropertiesPanel.List.Self.Children;
			//var listAccessibleNames = new List<string>();
			
			//foreach (var ch in childrenList) {
			//	listAccessibleNames.Add(ch.Element.GetAttributeValueText("AccessibleName"));
			//}
			//var accName1 = myRepository.DropDownMenuInRowPropertiesPanel.List.Self.Children[1].Element.GetAttributeValueText("AccessibleName");
			//newValueForParameter = newValue;
			//itemToSelectInfo.FindAdapter<ListItem>().Focus();
			//itemToSelectInfo.FindAdapter<ListItem>().Click();
			
			
			//myRepository.DropDownMenuInRowPropertiesPanel.List.YM2122Dk0003524KmInfo;
			//var indexToClick = listAccessibleNames.IndexOf(listAccessibleNames.FirstOrDefault(it => it.Contains(newValue)));
			
			//row.Click(".98;.5");
			//myRepository.DropDownMenuInRowPropertiesPanel.List.Self.Children[indexToClick].As<ListItem>().Focus();
			//row.Click(".98;.5");
			//myRepository.DropDownMenuInRowPropertiesPanel.List.Self.Children[indexToClick].As<ListItem>().Select();
			//row.PressKeys("{Return}");
			
			//var childToSelect = childrenList[indexToClick];
			
			//var nameToSelect = childToSelect.Element.GetAttributeValueText("AccessibleName");
			//childToSelect.EnsureVisible();
			//childToSelect.Click();
			
			//var listItems = myRepository.DropDownMenuInRowPropertiesPanel.List.Self.Children.Select(it => it.Element.GetAttributeValueText("AccessibleName"));
			
			//myRepository.DropDownMenuInRowPropertiesPanel.List.Self.Children.FirstOrDefault(it => it.Element.GetAttributeValueText("AccessibleName").Contains(newValue)).EnsureVisible();
			//myRepository.DropDownMenuInRowPropertiesPanel.List.Self.Children.FirstOrDefault(it => it.Element.GetAttributeValueText("AccessibleName").Contains(newValue)).Click();
			//itemToChoose.Focus();
			//itemToChoose.Click();
			//var itemToChoose = listItems.FirstOrDefault(it => it.ToString().Contains(newValue));
            //itemToChoose.Focus();
            //itemToChoose.Click();
            //row.Element.SetAttributeValue("AccessibleValue", nameToSelect);
        }

        
    	/// <summary>
        /// Constructs a new instance.
        /// </summary>
        public SetValueParameterInPropertiesPanelGivenPathTypeAndNewValue()
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
            Delay.SpeedFactor = 1.0;
            
            
            AutomatedSystemTestsRepository myRepository = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            Adapter propertiesPanelAdapter = myRepository.RiskeerMainWindow.PropertiesPanelContainer.Table.Self;
            Ranorex.Row row = GetRowInPropertiesPanelGivenPath(propertiesPanelAdapter, pathToElementInPropertiesPanel);
            
            switch (typeParameter) {
            	case "Text":
            		SetTextParameterInPropertiesPanel(row, newValueForParameter);
            		break;
            	case "Double":
					SetDoubleParameterInPropertiesPanel(row, newValueForParameter);
					break;
				case "DropDown":
					// Specific module
					//OpenDropDownMenuParameterInPropertiesPanel(row, newValueForParameter); //, myRepository.DropDownMenuInRowPropertiesPanel.List.genericItemInDropDownListInfo);
					break;
            	default:
            		Report.Log(ReportLevel.Error, "Type of parameter " + typeParameter + " not implemented!");
            		break;
            }
        }
    }
}
