using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Integration.Service.Exceptions;

namespace Ringtoets.Integration.Service.Test.Exceptions
{
    [TestFixture]
    public class AssessmentSectionProviderExceptionTest :
        CustomExceptionDesignGuidelinesTestFixture<AssessmentSectionProviderException, Exception> {}
}