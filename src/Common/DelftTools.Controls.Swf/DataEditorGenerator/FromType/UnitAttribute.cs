using System;

namespace DelftTools.Controls.Swf.DataEditorGenerator.FromType
{
    public class UnitAttribute : Attribute
    {
        public string Symbol { get; set; }

        public UnitAttribute(string symbol)
        {
            Symbol = symbol;
        }
    }
}