using System;

namespace DelftTools.Controls.Swf.DataEditorGenerator.FromType
{
    public class UnitAttribute : Attribute
    {
        public UnitAttribute(string symbol)
        {
            Symbol = symbol;
        }

        public string Symbol { get; set; }
    }
}