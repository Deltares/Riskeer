//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Application.Ringtoets.Storage.DbContext
{
    using System;
    using System.Collections.Generic;
    
    public partial class DuneAssessmentSectionEntity
    {
        public long DuneAssessmentSectionEntityId { get; set; }
        public long ProjectEntityId { get; set; }
        public string Name { get; set; }
        public int Norm { get; set; }
        public int Order { get; set; }
    
        public virtual ProjectEntity ProjectEntity { get; set; }
    }
}
