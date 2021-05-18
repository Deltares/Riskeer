using System;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Base.Test.Data
{
    [TestFixture]
    public class ProjectFactoryExceptionTest :
        CustomExceptionDesignGuidelinesTestFixture<ProjectFactoryException, Exception> {}
}