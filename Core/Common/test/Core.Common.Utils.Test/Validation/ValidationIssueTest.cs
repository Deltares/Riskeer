using System;

using Core.Common.Utils.Validation;

using NUnit.Framework;

namespace Core.Common.Utils.Test.Validation
{
    [TestFixture]
    public class ValidationIssueTest
    {
        [Test]
        public void ParameteredConstructor_WithViewData_InitializeProperties()
        {
            // Setup
            var subject = new object();
            var subjectName = "<subjectName>";
            var severity = ValidationSeverity.Error;
            var validationMessage = "<message>";
            var viewData = new object();

            // Call
            var issue = new ValidationIssue(subject, subjectName, severity, validationMessage, viewData);

            // Assert
            Assert.IsInstanceOf<IEquatable<ValidationIssue>>(issue);
            Assert.AreEqual(subject, issue.Subject);
            Assert.AreEqual(subjectName, issue.SubjectName);
            Assert.AreEqual(severity, issue.Severity);
            Assert.AreEqual(validationMessage, issue.Message);
            Assert.AreEqual(viewData, issue.ViewData);
        }

        [Test]
        public void ParameteredConstructor_WithoutViewData_InitializePropertiesExceptViewData()
        {
            // Setup
            var subject = new object();
            var subjectName = "<subjectName>";
            var severity = ValidationSeverity.Error;
            var validationMessage = "<message>";

            // Call
            var issue = new ValidationIssue(subject, subjectName, severity, validationMessage);

            // Assert
            Assert.IsInstanceOf<IEquatable<ValidationIssue>>(issue);
            Assert.AreEqual(subject, issue.Subject);
            Assert.AreEqual(subjectName, issue.SubjectName);
            Assert.AreEqual(severity, issue.Severity);
            Assert.AreEqual(validationMessage, issue.Message);
            Assert.AreEqual(subject, issue.ViewData);
        }

        [Test]
        public void ToString_ReturnInformativeText()
        {
            // Setup
            var issue = new ValidationIssue("<subject>", "subjectName", ValidationSeverity.Info, 
                "<message>");

            // Call
            var text = issue.ToString();

            // Assert
            Assert.AreEqual("[Info] <subject>: <message>", text);
        }

        [Test]
        public void Equals_ToItselfWithoutViewData_ReturnTrue()
        {
            // Setup
            var issue = new ValidationIssue("<subject>", "<subjectName>", ValidationSeverity.Info, "<message>");

            // Call
            var isEqual = issue.Equals(issue);

            // Assert
            Assert.True(isEqual);
        }

        [Test]
        public void Equals_ToItselfWithViewData_ReturnTrue()
        {
            // Setup
            var issue = new ValidationIssue("<subject>", "<subjectName>", ValidationSeverity.Info, "<message>", "<viewdata>");

            // Call
            var isEqual = issue.Equals(issue);

            // Assert
            Assert.True(isEqual);
        }

        [Test]
        public void Equals_ToNullWithoutViewData_ReturnFalse()
        {
            // Setup
            var issue = new ValidationIssue("<subject>", "<subjectName>", ValidationSeverity.Info, "<message>");

            // Call
            var isEqual = issue.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToNullWithViewData_ReturnFalse()
        {
            // Setup
            var issue = new ValidationIssue("<subject>", "<subjectName>", ValidationSeverity.Info, "<message>", "<viewdata>");

            // Call
            var isEqual = issue.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToOtherObjectTypeWithoutViewData_ReturnFalse()
        {
            // Setup
            var issue = new ValidationIssue("<subject>", "<subjectName>", ValidationSeverity.Info, "<message>");

            // Call
            var isEqual = issue.Equals(new object());

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToOtherObjectTypeWithViewData_ReturnFalse()
        {
            // Setup
            var issue = new ValidationIssue("<subject>", "<subjectName>", ValidationSeverity.Info, "<message>", "<viewdata>");

            // Call
            var isEqual = issue.Equals(new object());

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        [TestCase("<something completely different>", "<subjectName>", ValidationSeverity.Info, "<message>", "<viewdata>")]
        [TestCase("<subject>", "<subjectName>", ValidationSeverity.Warning, "<message>", "<viewdata>")]
        [TestCase("<subject>", "<subjectName>", ValidationSeverity.Info, "<different message>", "<viewdata>")]
        public void Equals_ToOtherIssueNotSimilar_ReturnFalse(object subject, string subjectName, ValidationSeverity severity,
                                                              string message, object viewData)
        {
            // Setup
            var issue = new ValidationIssue("<subject>", "<subjectName>", ValidationSeverity.Info, "<message>", "<viewdata>");
            var otherIssue = new ValidationIssue(subject, subjectName, severity, message, viewData);

            // Call
            var isEqual1 = issue.Equals(otherIssue);
            var isEqual2 = otherIssue.Equals(issue);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_SameSubjectSeverityAndMessage_ReturnTrue()
        {
            // Setup
            var subject = "<subject>";
            var severity = ValidationSeverity.None;
            var message = "<message>";

            var issue1 = new ValidationIssue(subject, "<subjectName>", severity, message, "<viewdata>");
            var issue2 = new ValidationIssue(subject, "different", severity, message, "also different");

            // Call
            var isEqual1 = issue1.Equals(issue2);
            var isEqual2 = issue2.Equals(issue1);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void GetHashCode_TwoEqualIssues_ShouldReturnSameHashCodes()
        {
            // Setup
            var subject = "<subject>";
            var severity = ValidationSeverity.Warning;
            var message = "<message>";

            var issue1 = new ValidationIssue(subject, "<subjectName>", severity, message, "<viewdata>");
            var issue2 = new ValidationIssue(subject, "different", severity, message, "also different");

            // Call
            var hashCode1 = issue1.GetHashCode();
            var hashCode2 = issue2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);
        }
    }
}