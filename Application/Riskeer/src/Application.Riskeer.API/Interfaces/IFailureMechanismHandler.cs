using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Riskeer.API.Interfaces
{
    public interface IFailureMechanismHandler<TMechanism> where TMechanism : IFailureMechanismApi
    {
        TMechanism FindFailureMechanism(IProjectApi projectApi);

        bool ImportAssessmentSections(TMechanism mechanism);
    }
}
