/*
 * Created by Ranorex
 * User: rodriqu_dd
 * Date: 27/11/2020
 * Time: 09:56
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

namespace AutomatedSystemTests.Modules.Calculation
{
    /// <summary>
    /// Description of DetermineFMSubstring.
    /// </summary>
    [TestModule("AF24474B-66CD-4C26-8686-3330DE69A789", ModuleType.UserCode, 1)]
    public class DetermineFMSubstring : ITestModule
    {
        
        
        string _indecesFMsSelected = "";
        [TestVariable("18483c93-2abb-4e08-a63b-fa2798ff5e0a")]
        public string indecesFMsSelected
        {
            get { return _indecesFMsSelected; }
            set { _indecesFMsSelected = value; }
        }
        
        string _indexOrderMessage = "";
        [TestVariable("7e8acb99-c918-4e36-8340-db04a45b62f8")]
        public string indexOrderMessage
        {
            get { return _indexOrderMessage; }
            set { _indexOrderMessage = value; }
        }
        
        
        string _fmSubstring = "";
        [TestVariable("a7042996-7d10-4ac0-bcdf-abe588f4ce90")]
        public string fmSubstring
        {
            get { return _fmSubstring; }
            set { _fmSubstring = value; }
        }
        
        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public DetermineFMSubstring()
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
            if (indexOrderMessage=="1" || indexOrderMessage=="2") {
                // Current iteration involves an FM
                List<string> listIndeces = indecesFMsSelected.Split(',').ToList();
                listIndeces.Reverse();
                int indexFM = Int32.Parse(listIndeces[Int32.Parse(indexOrderMessage)-1]);
                var listFMS = new List<string>{
                    "Dijken en dammen - Piping",
                    "Dijken en dammen - Grasbekleding erosie kruin en binnentalud",
                    "Dijken en dammen - Stabiliteit steenzetting",
                    "Kunstwerken - Betrouwbaarheid sluiting kunstwerk"
                };
                fmSubstring = listFMS[indexFM-1];
            } else {
                // Current iteration does not involve an FM
                fmSubstring = "";
            }
        }
    }
}
