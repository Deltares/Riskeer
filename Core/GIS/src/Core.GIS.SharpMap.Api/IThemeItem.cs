using System;
using System.Drawing;
using Core.Common.Utils;

namespace Core.GIS.SharpMap.Api
{
    public interface IThemeItem : IComparable, INotifyPropertyChange
    {
        string Range { get; }
        string Label { get; set; }
        IStyle Style { get; set; }
        Bitmap Symbol { get; }
    }
}