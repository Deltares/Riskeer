using System.Linq;
using DelftTools.Utils.Validation;
using NUnit.Framework;

namespace DelftTools.Utils.Tests.Validation
{
    [TestFixture]
    public class ValidationReportTest
    {
        [Test]
        public void Empty()
        {
            var staticEmpty = ValidationReport.Empty("");
            Assert.IsTrue(staticEmpty.IsEmpty);

            var newObject = new ValidationReport("", null);
            Assert.IsTrue(newObject.IsEmpty);
        }

        [Test]
        public void SeverityIssues()
        {
            var report = CreateValidationReport();
            var subReport = report.SubReports.First();
            Assert.AreEqual(ValidationSeverity.Warning, subReport.Severity);
        }

        [Test]
        public void SeverityRecursive()
        {
            var report = CreateValidationReport();
            Assert.AreEqual(ValidationSeverity.Error, report.Severity);
        }

        [Test]
        public void SeverityCount()
        {
            var report = CreateValidationReport();
            Assert.AreEqual(1, report.ErrorCount);
            Assert.AreEqual(4, report.WarningCount);
            Assert.AreEqual(2, report.InfoCount);
        }

        private static ValidationReport CreateValidationReport()
        {
            var issue1 = new ValidationIssue("", ValidationSeverity.Warning, "");
            var issue2 = new ValidationIssue("", ValidationSeverity.Info, "");
            var issue3 = new ValidationIssue("", ValidationSeverity.None, "");
            var issue4 = new ValidationIssue("", ValidationSeverity.Warning, "");
            var issue5 = new ValidationIssue("", ValidationSeverity.Error, "");
            var subReport1 = new ValidationReport("q", new[]
            {
                issue1,
                issue3,
                issue4
            });
            var subReport2 = new ValidationReport("q", new[]
            {
                issue2,
                issue3,
                issue5
            });
            return new ValidationReport("q", new[]
            {
                issue1,
                issue2,
                issue3,
                issue4
            }, new[]
            {
                subReport1,
                subReport2
            });
        }
    }
}