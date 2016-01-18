using System;
using System.Linq;

using Core.Common.TestUtil;
using Core.Common.Utils.Validation;
using NUnit.Framework;

namespace Core.Common.Utils.Test.Validation
{
    [TestFixture]
    public class ValidationReportTest
    {
        [Test]
        public void ParameteredConstructor_AllArgumentsProvided_ExpectedValues()
        {
            // Setup
            var category = "<Category>";
            var validationIssues = new[]
            {
                new ValidationIssue("subject", "subjectName", ValidationSeverity.Error, "message")
            };
            var subReports = new[]
            {
                new ValidationReport("<Child>", null)
            };

            // Call
            var report = new ValidationReport(category, validationIssues, subReports);

            // Assert
            Assert.IsInstanceOf<IEquatable<ValidationReport>>(report);
            Assert.AreEqual(category, report.Category);
            CollectionAssert.AreEqual(validationIssues, report.Issues);
            CollectionAssert.AreEqual(subReports, report.SubReports);
            Assert.AreEqual(ValidationSeverity.Error, report.Severity);
        }

        [Test]
        public void ParameteredConstructor_OnlyCategoryProvided_ExpectedValues()
        {
            // Setup
            var category = "<Nice stuff>";

            // Call
            var report = new ValidationReport(category, null);

            // Assert
            Assert.AreEqual(category, report.Category);
            CollectionAssert.IsEmpty(report.Issues);
            CollectionAssert.IsEmpty(report.SubReports);
            Assert.AreEqual(ValidationSeverity.None, report.Severity);
        }

        [Test]
        public void ParameteredConstructor_CategoryIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ValidationReport(null, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, 
                "Category/Concept moet gespecificeerd worden om een validatie rapport te maken.");
        }

        [Test]
        public void Severity_ReportHighestIssueSeverityIsWarning_ReturnWarning()
        {
            // Setup
            var report = CreateValidationReport();

            var subReport = report.SubReports.First();

            // Call
            var validationSeverity = subReport.Severity;

            // Assert
            
            Assert.AreEqual(ValidationSeverity.Warning, validationSeverity);
        }

        [Test]
        public void Severity_ReportHasSubReportWithErrorSeverity_ReturnError()
        {
            // Setup
            var report = CreateValidationReport();

            // Call
            var validationSeverity = report.Severity;

            // Assert
            Assert.AreEqual(ValidationSeverity.Error, validationSeverity);
        }

        [Test]
        public void SeverityCount_ReportWithVariousChildReports_RecursivelyCountAllSeverities()
        {
            // Setup
            var report = CreateValidationReport();

            // Call
            var errorCount = report.ErrorCount;
            var warningCount = report.WarningCount;
            var infoCount = report.InfoCount;

            // Assert
            Assert.AreEqual(1, errorCount);
            Assert.AreEqual(4, warningCount);
            Assert.AreEqual(2, infoCount);
        }

        [Test]
        public void GetAllIssuesRecursive_ReportWithVariousChildReports_ReturnAllValidationIssues()
        {
            // Setup
            var report = CreateValidationReport();

            // Call
            var allIssues = report.GetAllIssuesRecursive();

            // Assert
            var expectedCount = report.Issues.Count() + report.SubReports.Sum(sr => sr.Issues.Count() + sr.SubReports.Sum(sr2=>sr2.Issues.Count()));
            Assert.AreEqual(expectedCount, allIssues.Count);
            Assert.AreEqual(report.ErrorCount, allIssues.Count(i => i.Severity == ValidationSeverity.Error));
            Assert.AreEqual(report.WarningCount, allIssues.Count(i => i.Severity == ValidationSeverity.Warning));
            Assert.AreEqual(report.InfoCount, allIssues.Count(i => i.Severity == ValidationSeverity.Info));
            Assert.AreEqual(3, allIssues.Count(i => i.Severity == ValidationSeverity.None));
            foreach (ValidationIssue validationIssue in report.SubReports.SelectMany(sr => sr.Issues))
            {
                CollectionAssert.Contains(allIssues, validationIssue);
            }
            foreach (ValidationIssue validationIssue in report.Issues)
            {
                CollectionAssert.Contains(allIssues, validationIssue);
            }
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var report = CreateValidationReport();

            // Call
            var isEqual = report.Equals(report);

            // Assert
            Assert.True(isEqual);
        }

        [Test]
        public void Equals_ToOtherObjectType_ReturnFalse()
        {
            // Setup
            var report = CreateValidationReport();

            // Call
            var isEqual = report.Equals(new object());

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var report = CreateValidationReport();

            // Call
            var isEqual = report.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToSimilarObject_ReturnTrue()
        {
            // Setup
            var report1 = CreateValidationReport();
            var report2 = CreateValidationReport();

            // Call
            var isEqual1 = report1.Equals(report2);
            var isEqual2 = report2.Equals(report1);

            // Assert
            Assert.True(isEqual1);
            Assert.True(isEqual2);
        }

        [Test]
        public void Equals_DifferentCategory_ReturnFalse()
        {
            // Setup
            var report1 = new ValidationReport("A", null);
            var report2 = new ValidationReport("B", null);

            // Call
            var isEqual1 = report1.Equals(report2);
            var isEqual2 = report2.Equals(report1);

            // Assert
            Assert.False(isEqual1);
            Assert.False(isEqual2);
        }

        [Test]
        public void Equals_DifferentNumberOfIssues_ReturnFalse()
        {
            // Setup
            var category = "A";

            var report1 = new ValidationReport(category, null);
            var report2 = new ValidationReport(category, new[]{new ValidationIssue("", "", ValidationSeverity.None, "") });

            // Call
            var isEqual1 = report1.Equals(report2);
            var isEqual2 = report2.Equals(report1);

            // Assert
            Assert.False(isEqual1);
            Assert.False(isEqual2);
        }

        [Test]
        public void Equals_NotEqualIssues_ReturnFalse()
        {
            // Setup
            var category = "A";

            var report1 = new ValidationReport(category, new[] { new ValidationIssue("A", "", ValidationSeverity.None, "") });
            var report2 = new ValidationReport(category, new[] { new ValidationIssue("B", "", ValidationSeverity.None, "") });

            // Call
            var isEqual1 = report1.Equals(report2);
            var isEqual2 = report2.Equals(report1);

            // Assert
            Assert.False(isEqual1);
            Assert.False(isEqual2);
        }

        [Test]
        public void Equals_DifferentSubReportCount_ReturnFalse()
        {
            // Setup
            var report1 = new ValidationReport("A", null);
            var report2 = new ValidationReport("A", null, new []{ new ValidationReport("A", null) });

            // Call
            var isEqual1 = report1.Equals(report2);
            var isEqual2 = report2.Equals(report1);

            // Assert
            Assert.False(isEqual1);
            Assert.False(isEqual2);
        }

        [Test]
        public void Equals_DifferentSubReports_ReturnFalse()
        {
            // Setup
            var report1 = new ValidationReport("A", null, new[] { new ValidationReport("B", null) });
            var report2 = new ValidationReport("A", null, new[] { new ValidationReport("A", null) });

            // Call
            var isEqual1 = report1.Equals(report2);
            var isEqual2 = report2.Equals(report1);

            // Assert
            Assert.False(isEqual1);
            Assert.False(isEqual2);
        }

        [Test]
        public void GetHashCode_TwoEqualReports_ReturnEqualHashCodes()
        {
            // Setup
            var report1 = CreateValidationReport();
            var report2 = CreateValidationReport();

            // Call
            var hashCode1 = report1.GetHashCode();
            var hashCode2 = report2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
        }

        [Test]
        public void ToString_ForValidationReport_ReturnText()
        {
            // Setup
            var report = CreateValidationReport();

            // Call
            var text = report.ToString();

            // Assert
            Assert.AreEqual("q, ernst: Error (1 fout(en), 4 waarschuwing(en), 2 info).", text);
        }

        private static ValidationReport CreateValidationReport()
        {
            var issue1 = new ValidationIssue("", "somename", ValidationSeverity.Warning, "");
            var issue2 = new ValidationIssue("", "somename", ValidationSeverity.Info, "");
            var issue3 = new ValidationIssue("", "somename", ValidationSeverity.None, "");
            var issue4 = new ValidationIssue("", "somename", ValidationSeverity.Warning, "");
            var issue5 = new ValidationIssue("", "somename", ValidationSeverity.Error, "");
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