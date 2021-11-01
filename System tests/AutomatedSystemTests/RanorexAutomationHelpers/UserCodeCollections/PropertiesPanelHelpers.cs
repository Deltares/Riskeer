/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 01/11/2021
 * Time: 09:11
 * 
 * To change this template use Tools > Options > Coding > Edit standard headers.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Threading;
using System.Linq;
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
    public static class PropertiesPanelHelpers
    {
        // You can use the "Insert New User Code Method" functionality from the context menu,
        // to add a new method with the attribute [UserCodeMethod].
        
        /// <summary>
        /// This static method returns a Row object given the path in the properties panel.
        /// </summary>
        [UserCodeMethod]
        public static Ranorex.Row GetRowInPropertiesPanelGivenPath(string pathItem, Adapter argumentAdapter)
        {
            Mouse.DefaultMoveTime = 0;
            Keyboard.DefaultKeyPressTime = 0;
            Delay.SpeedFactor = 0.0;
            int minimumIndex = 0;
            var stepsPathItem = pathItem.Split('>').ToList();
            Ranorex.Row stepRow = argumentAdapter.As<Table>().Rows.ToList()[1];
            for (int i=0; i < stepsPathItem.Count; i++)
            {
                // Find the item corresponding to the step
                var step = stepsPathItem[i];
                var completeList = argumentAdapter.As<Table>().Rows.ToList();
                var searchList = completeList.GetRange(minimumIndex, completeList.Count-minimumIndex);
                var indexStepRow = searchList.FindIndex(rw => rw.GetAttributeValue<string>("AccessibleName").Contains(step));
                stepRow = searchList[indexStepRow];
                stepRow.Focus();
                stepRow.Select();
                if (i != stepsPathItem.Count - 1 && stepRow.Element.GetAttributeValueText("AccessibleState").Contains("Collapsed"))
                {
                    stepRow.PressKeys("{Right}");
                }
                minimumIndex += 1 + indexStepRow;
            }
            return stepRow;
        }
        
        
        /// <summary>
        /// This method sets a new value to a parameter in a field in properties panel given its path.
        /// </summary>
        [UserCodeMethod]
        public static void SetValue(this Ranorex.Row row, string newValue, string parameterType)
        {
            switch (parameterType) {
                case "Text":
                    SetTextParameterInPropertiesPanel(row, newValue);
                    break;
                case "Double":
                    SetDoubleParameterInPropertiesPanel(row, newValue);
                    break;
                case "DropDown":
                    //RepoItemInfo listItemInfo = new RepoItemInfo(myRepository.DropDownMenuInRowPropertiesPanel.List, "genericItemInDropDownList", "listitem[@accessiblename~" + newValueForParameter + "]", 30000, null);
                    //SetDynamicDropDownParameterInPropertiesPanel(row, newValueForParameter, listItemInfo);
                    break;
                default:
                    Report.Log(ReportLevel.Error, "Type of parameter " + parameterType + " not implemented!");
                    break;
            }
        }
        
        private static void SetDoubleParameterInPropertiesPanel(Ranorex.Row row, string newValue)
        {
            row.Element.SetAttributeValue("AccessibleValue", newValue.ToCurrentCulture());
        }
        
        private static void SetTextParameterInPropertiesPanel(Ranorex.Row row, string newValue)
        {
            row.Element.SetAttributeValue("AccessibleValue", newValue);
        }
        
        private static void SetDynamicDropDownParameterInPropertiesPanel(Ranorex.Row row, string newValueForParameter, RepoItemInfo listItemInfo)
        {
            row.Click();
            row.Click(".98;.5");
            listItemInfo.FindAdapter<ListItem>().Focus();
            listItemInfo.FindAdapter<ListItem>().Click();
        }

    }
}
