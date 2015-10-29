using Core.Common.Utils.FormulaParser;
using NUnit.Framework;

namespace Core.Common.Utils.Tests
{
    [TestFixture]
    public class ExcelFormulaTest
    {
        [Test]
        public void SimpleFunction()
        {
            ExcelFormula excelFormula = new ExcelFormula("=SimpleFunction()");
            Assert.AreEqual(excelFormula.Count, 2);
            ExcelFormulaToken token = excelFormula[0];
            Assert.IsTrue(string.Compare(token.Value, "SimpleFunction") == 0);
        }

        [Test]
        public void FunctionWithParameters()
        {
            ExcelFormula excelFormula = new ExcelFormula("=Function(parameter,parameter1,10.0)");
            Assert.AreEqual(excelFormula.Count, 7);
            ExcelFormulaToken token = excelFormula[0];
            Assert.IsTrue(string.Compare(token.Value, "Function") == 0);
            Assert.IsTrue(token.Type.Equals(ExcelFormulaTokenType.Function));
            token = excelFormula[1];
            Assert.IsTrue(string.Compare(token.Value, "parameter") == 0);
            Assert.IsTrue(token.Type.Equals(ExcelFormulaTokenType.Operand));
            token = excelFormula[2];
            Assert.IsTrue(string.Compare(token.Value, ",") == 0);
            Assert.IsTrue(token.Type.Equals(ExcelFormulaTokenType.Argument));
            token = excelFormula[3];
            Assert.IsTrue(string.Compare(token.Value, "parameter1") == 0);
            Assert.IsTrue(token.Type.Equals(ExcelFormulaTokenType.Operand));
            token = excelFormula[5];
            Assert.IsTrue(string.Compare(token.Value, "10.0") == 0);
            Assert.IsTrue(token.Type.Equals(ExcelFormulaTokenType.Operand));
        }

        [Test]
        public void FormulaNameWithSpace()
        {
            ExcelFormula excelFormula = new ExcelFormula("=FormulaWithSpace(parameter 1,parameter 2)");
            Assert.IsNotNull(excelFormula);
        }

        [Test]
        public void Test1()
        {
            ExcelFormula excelFormula =
                new ExcelFormula(
                    @"=if(percentage>75,distinction,if(percentage>60,first,if(percentage>50,second,if(percentage>35,pass,fail))))");
            Assert.IsTrue(excelFormula.Count > 0);
        }
    }
}