namespace DelftTools.Utils.FormulaParser
{
    /// <summary>
    /// Tokens used in ExcelFormula class to parse equations
    /// </summary>
    public class ExcelFormulaToken
    {
        internal ExcelFormulaToken(string value, ExcelFormulaTokenType type)
            : this(value, type, ExcelFormulaTokenSubtype.Nothing) {}

        internal ExcelFormulaToken(string value, ExcelFormulaTokenType type, ExcelFormulaTokenSubtype subtype)
        {
            this.Value = value;
            this.Type = type;
            this.Subtype = subtype;
        }

        public string Value { get; internal set; }

        public ExcelFormulaTokenType Type { get; internal set; }

        public ExcelFormulaTokenSubtype Subtype { get; internal set; }
    }

    public enum ExcelFormulaTokenType
    {
        Noop,
        Operand,
        Function,
        Subexpression,
        Argument,
        OperatorPrefix,
        OperatorInfix,
        OperatorPostfix,
        WhiteSpace,
        Unknown
    }

    public enum ExcelFormulaTokenSubtype
    {
        Nothing,
        Start,
        Stop,
        Text,
        Number,
        Logical,
        Error,
        Range,
        Math,
        Concatenation,
        Intersection,
        Union
    }
}