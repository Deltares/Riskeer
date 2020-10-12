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
using System.Globalization;
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
    public partial class CalculateNFormula3
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public string CalculateNGroup3Formula()
        {
            System.Globalization.CultureInfo currentCulture = CultureInfo.CurrentCulture;
            double cValue = Double.Parse(cParameter, currentCulture);
            double twoNAValue = Double.Parse(twoNAParameter, currentCulture);
            double calculatedNValue = Math.Max(twoNAValue*cValue, 1);
            return calculatedNValue.ToString("N2", currentCulture).Replace(currentCulture.NumberFormat.NumberGroupSeparator, "");
        }

    }
}
