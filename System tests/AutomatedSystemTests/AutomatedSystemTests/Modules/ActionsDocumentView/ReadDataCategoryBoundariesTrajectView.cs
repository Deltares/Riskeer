/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 07/12/2020
 * Time: 09:16
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

namespace AutomatedSystemTests.Modules.ActionsDocumentView
{
    /// <summary>
    /// Description of ReadDataCategoryBoundariesTrajectView.
    /// </summary>
    [TestModule("2078E4E4-9E0D-4396-A14A-8D19B9465BCA", ModuleType.UserCode, 1)]
    public class ReadDataCategoryBoundariesTrajectView : ITestModule
    {
        
        
        string _categoryBoundariesTraject = "";
        [TestVariable("3848b12b-0450-4651-8089-385b98ccb5db")]
        public string categoryBoundariesTraject
        {
            get { return _categoryBoundariesTraject; }
            set { _categoryBoundariesTraject = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public ReadDataCategoryBoundariesTrajectView()
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
            
            var repo = global::AutomatedSystemTests.AutomatedSystemTestsRepository.Instance;
            var tableCategoryBoundariesTraject = repo.RiskeerMainWindow.DocumentViewContainerUncached.AssemblyResultsCategoryView.Table;
            
            int idxColumn = 3;
            for (int indexRow = 2; indexRow < 7; indexRow++) {
                string fraction = tableCategoryBoundariesTraject.Rows[indexRow].Cells[idxColumn].Element.GetAttributeValueText("AccessibleValue");
                categoryBoundariesTraject += fraction + ";";
            }
            categoryBoundariesTraject = categoryBoundariesTraject.TrimEnd(';');
        }
        
        private string GetBackgroundColorOfCell(Table table, int rowIndex, int columnIndex)
        {
            Ranorex.Control tableCtrl = (Ranorex.Control) table.Element;

            // Invoke Remotely  
            string colorOfCell = (string)tableCtrl.InvokeRemotely( delegate(System.Windows.Forms.Control control, object input)  
                    {
                        System.Windows.Forms.DataGridView dataGrid = (System.Windows.Forms.DataGridView) control;  
                        // Now you can access each cell:  
                        Color color = dataGrid.Rows[rowIndex].Cells[columnIndex].Style.BackColor;
                        Console.WriteLine("Color: "+color);  
                        return color.ToString();  
                     }
                     );
                  
            Report.Info("Color of Cell: "+colorOfCell);
            return colorOfCell;
        }
    }
}
