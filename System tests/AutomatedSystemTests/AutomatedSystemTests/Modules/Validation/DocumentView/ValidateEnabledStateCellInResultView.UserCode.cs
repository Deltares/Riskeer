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

namespace AutomatedSystemTests.Modules.Validation.DocumentView
{
    public partial class ValidateEnabledStateCellInResultView
    {
        /// <summary>
        /// This method gets called right after the recording has been started.
        /// It can be used to execute recording specific initialization code.
        /// </summary>
        private void Init()
        {
            // Your recording specific initialization code goes here.
        }

        public void ValidateEnabledGenericCellResultsView(RepoItemInfo cellInfo, string IsEnabledExpected)
        {
            int startX = isEnabledExpected=="true"?5:0;
            CompressedImage genericCellPatch = repo.RiskeerMainWindow.ContainerMultipleViews.DocumentViewContainer.FM_ResultView.TableFMResultView.Row.genericCellInfo.GetGreyWhitePatch(new Rectangle(startX, 0, 3, 17));
            Imaging.FindOptions genericCellPatchOptions = Imaging.FindOptions.Default;
            Report.Log(ReportLevel.Info, "Validation", "Validating ContainsImage (Screenshot: 'greyPatch1' with region {X=0,Y=1,Width=3,Height=17}) on item 'cellInfo'.", cellInfo);
            Validate.ContainsImage(cellInfo, genericCellPatch, genericCellPatchOptions);
        }

    }
}