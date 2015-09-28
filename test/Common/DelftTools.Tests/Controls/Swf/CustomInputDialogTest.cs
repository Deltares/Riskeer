using System.Collections.Generic;
using System.Windows.Forms;
using DelftTools.Controls.Swf;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf
{
    [TestFixture]
    public class CustomInputDialogTest
    {
        [Test]
        public void CreateCustomDialog()
        {
            var dialog = new CustomInputDialog();
            dialog.AddInput<string>("Name");
            dialog.AddInput<string>("Lastname");
            dialog.AddInput<double>("Age").ValidationMethod = (o, v) => ((double) v) < 0 ? "Must be positive" : "";
            dialog.AddInput<bool>("Is employee1");
            dialog.AddInput<bool>("Is employee2");
            dialog.AddInput<bool>("Is employee3");
            dialog.AddInput<bool>("Is employee4");
            dialog.AddInput("Result", DialogResult.Retry);
            var valueInput = dialog.AddInput("Value", 10.0);
            valueInput.ToolTip = "Value of item";
            valueInput.UnitSymbol = "m";

            Assert.AreEqual(10.0, dialog["Value"]);
            Assert.AreEqual(DialogResult.Retry, dialog["Result"]);
        }

        [Test]
        public void CreateCustomDialogWithDropDownBox()
        {
            var dialog = new CustomInputDialog();
            dialog.AddInput<string>("Name");
            dialog.AddChoice("Occupation", new List<string> {"Construction", "IT", "Management", "Finance"});
            dialog.AddChoice("Years of experience", new List<int> { 0, 1, 2, 3 }).ToolTip = "Number of years experience, choose 3 if 3 or more year";

            Assert.AreEqual("", dialog["Name"]);
            Assert.AreEqual("Construction", dialog["Occupation"]);
            Assert.AreEqual(0, dialog["Years of experience"]);
        }
    }
}