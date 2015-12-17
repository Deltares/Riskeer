﻿using Core.Common.Controls.Charting;

namespace Core.Plugins.CommonTools.Gui.Commands.Charting
{
    public class IncreaseFontSizeCommand : ChartViewCommandBase
    {
        /// <summary>
        /// Increases the fonts of the <see cref="Chart"/> on the active <see cref="ChartView"/>
        /// by one point.
        /// </summary>
        /// <param name="arguments">No arguments are needed when calling this method.</param>
        public override void Execute(params object[] arguments)
        {
            var view = View;
            if (view != null)
            {
                view.Chart.Font = GetChangedFontSize(view.Chart.Font, 1);
                view.Chart.Legend.Font = GetChangedFontSize(view.Chart.Legend.Font, 1);
                view.Chart.LeftAxis.LabelsFont = GetChangedFontSize(view.Chart.LeftAxis.LabelsFont, 1);
                view.Chart.LeftAxis.TitleFont = GetChangedFontSize(view.Chart.LeftAxis.TitleFont, 1);
                view.Chart.BottomAxis.LabelsFont = GetChangedFontSize(view.Chart.BottomAxis.LabelsFont, 1);
                view.Chart.BottomAxis.TitleFont = GetChangedFontSize(view.Chart.BottomAxis.TitleFont, 1);
                view.Chart.RightAxis.LabelsFont = GetChangedFontSize(view.Chart.RightAxis.LabelsFont, 1);
                view.Chart.RightAxis.TitleFont = GetChangedFontSize(view.Chart.RightAxis.TitleFont, 1);
            }
        }
    }
}