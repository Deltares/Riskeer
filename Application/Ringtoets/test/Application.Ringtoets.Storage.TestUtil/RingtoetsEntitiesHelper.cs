using System.Collections.ObjectModel;
using Application.Ringtoets.Storage.DbContext;
using Rhino.Mocks;

namespace Application.Ringtoets.Storage.TestUtil
{
    static public class RingtoetsEntitiesHelper {
        public static IRingtoetsEntities Create(MockRepository mockRepository)
        {
            var ringtoetsEntities = mockRepository.Stub<IRingtoetsEntities>();
            var pSet = DbTestSet.GetDbTestSet(new ObservableCollection<ProjectEntity>());
            var hlSet = DbTestSet.GetDbTestSet(new ObservableCollection<HydraulicLocationEntity>());
            var fmSet = DbTestSet.GetDbTestSet(new ObservableCollection<FailureMechanismEntity>());
            var dasSet = DbTestSet.GetDbTestSet(new ObservableCollection<DikeAssessmentSectionEntity>());
            ringtoetsEntities.Stub(r => r.ProjectEntities).Return(pSet);
            ringtoetsEntities.Stub(r => r.Set<HydraulicLocationEntity>()).Return(hlSet);
            ringtoetsEntities.Stub(r => r.FailureMechanismEntities).Return(fmSet);
            ringtoetsEntities.Stub(r => r.Set<DikeAssessmentSectionEntity>()).Return(dasSet);
            ringtoetsEntities.Stub(r => r.DikeAssessmentSectionEntities).Return(dasSet);
            return ringtoetsEntities;
        }
    }
}