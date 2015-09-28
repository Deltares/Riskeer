namespace DelftTools.Utils.FormulaParser
{
    /// <summary>
    /// Tokens used in ExcelFormula class to parse equations
    /// </summary>
    public class ExcelFormulaToken
    {
        private string value;
        private ExcelFormulaTokenType type;
        private ExcelFormulaTokenSubtype subtype;

        internal ExcelFormulaToken(string value, ExcelFormulaTokenType type)
            : this(value, type, ExcelFormulaTokenSubtype.Nothing)
        {
        }

        internal ExcelFormulaToken(string value, ExcelFormulaTokenType type, ExcelFormulaTokenSubtype subtype)
        {
            this.value = value;
            this.type = type;
            this.subtype = subtype;
        }

        public string Value
        {
            get { return value; }
            internal set { this.value = value; }
        }

        public ExcelFormulaTokenType Type
        {
            get { return type; }
            internal set { type = value; }
        }

        public ExcelFormulaTokenSubtype Subtype
        {
            get { return subtype; }
            internal set { subtype = value; }
        }
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