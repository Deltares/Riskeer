using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls.Swf.DataEditorGenerator;
using DelftTools.Controls.Swf.DataEditorGenerator.FromType;
using DelftTools.Tests.Controls.Swf.DataEditorGenerator.Domain;
using DelftTools.TestUtils;
using NUnit.Framework;

namespace DelftTools.Tests.Controls.Swf.DataEditorGenerator
{
    [TestFixture]
    [Category(TestCategory.WindowsForms)]
    public class DataEditorGeneratorSwfTest
    {
        [Test]
        public void GenerateEditorForCar()
        {
            var car = new Car();

            var control =
                DataEditorGeneratorSwf.GenerateView(
                    ObjectDescriptionFromTypeExtractor.ExtractObjectDescription(car.GetType()));

            control.Dock = DockStyle.Fill;
            control.Data = car;

            WindowsFormsTestHelper.ShowModal(control);
        }

        [Test]
        public void GenerateEditorForCarWithManualValidation()
        {
            var car = new Car();

            var description = ObjectDescriptionFromTypeExtractor.ExtractObjectDescription(car.GetType());
            description.FieldDescriptions.First(f => f.Name == "MilesPerGallon").ValidationMethod =
                (c, v) =>
                ((Car) c).CarType == Car.CarTypes.FourWheelDrive && (double) v >= 0.0
                    ? ""
                    : "Miles per gallon must be positive";
            var control = DataEditorGeneratorSwf.GenerateView(description);

            control.Dock = DockStyle.Fill;
            control.Data = car;

            WindowsFormsTestHelper.ShowModal(control);
        }

        [Test]
        public void GenerateEditorForModel()
        {
            var model = new BindModel();

            var control = DataEditorGeneratorSwf.GenerateView(
                ObjectDescriptionFromTypeExtractor.ExtractObjectDescription(model.GetType()));

            control.Dock = DockStyle.Fill;
            control.Data = model;

            WindowsFormsTestHelper.ShowModal(control);
        }
    }
}


