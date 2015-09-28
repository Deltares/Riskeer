using DelftTools.Shell.Gui.Swf.Validation;
using DelftTools.TestUtils;
using DelftTools.Utils.Validation;
using NUnit.Framework;
using Rhino.Mocks;

namespace DelftTools.Tests.Shell.Gui.Swf.Validation
{
    [TestFixture]
    [Category(TestCategory.WindowsForms)]
    public class ValidationReportControlTest
    {
        private static MockRepository mocks = new MockRepository();
        
        [Test]
        public void ShowEmpty()
        {
            var reportView = new ValidationReportControl {Data = null};
            WindowsFormsTestHelper.ShowModal(reportView);
        }

        [Test]
        public void ShowErrorless()
        {
            var reportView = new ValidationReportControl
                                 {
                                     Data =
                                         new ValidationReport("Main",
                                                              new[]
                                                                  {
                                                                      new ValidationReport("First", null),
                                                                      new ValidationReport("Second", null),
                                                                  })
                                 };
            WindowsFormsTestHelper.ShowModal(reportView);
        }

        [Test]
        public void ShowVeryWideForTruncation()
        {
            var veryLongCategoryName =
                "First very long name which is beyond the size of the validation report so should be truncated correctly etc, ";
            var reportView = new ValidationReportControl
            {
                Width = 1000,
                Data =
                    new ValidationReport("Main",
                                         new[]
                                                                  {
                                                                      new ValidationReport(veryLongCategoryName + veryLongCategoryName + veryLongCategoryName, null),
                                                                      new ValidationReport("Second", null),
                                                                  })
            };
            WindowsFormsTestHelper.ShowModal(reportView);
        }

        [Test]
        public void ShowReport()
        {
            var mixedReport = new ValidationReport("Network",
                                                   new[]
                                                       {
                                                           new ValidationIssue(null, ValidationSeverity.Error, "info message"),
                                                           new ValidationIssue(null, ValidationSeverity.Error, "info message"),
                                                           new ValidationIssue(null, ValidationSeverity.Warning, "info message"),
                                                           new ValidationIssue(null, ValidationSeverity.Warning, "info message"),
                                                           new ValidationIssue(null, ValidationSeverity.Info, "info message"),
                                                           new ValidationIssue(null, ValidationSeverity.Info, "info message"),
                                                           new ValidationIssue(null, ValidationSeverity.Info, "info message"),
                                                       });

            var data = new object();
            var noneReport = new ValidationReport("Input", null);
            var infoReport = new ValidationReport("Output", new[] { new ValidationIssue(data, ValidationSeverity.Info, "info message") });
            var warningReport = new ValidationReport("Discretization", new[] { new ValidationIssue(null, ValidationSeverity.Warning, "warning message") });
            var errorReport = new ValidationReport("Interpolation", new[] { new ValidationIssue(data, ValidationSeverity.Error, "error message") });
            var report = new ValidationReport("Model", null, new[] {noneReport, infoReport, warningReport, errorReport, mixedReport});

            var reportView = new ValidationReportControl();
            reportView.Data = report;

            WindowsFormsTestHelper.ShowModal(reportView);
        }
    }
}