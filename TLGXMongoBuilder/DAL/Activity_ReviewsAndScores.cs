//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Activity_ReviewsAndScores
    {
        public System.Guid Activity_ReviewsAndScores_Id { get; set; }
        public Nullable<System.Guid> Activity_Flavour_Id { get; set; }
        public Nullable<System.Guid> Activity_Id { get; set; }
        public string Review_Source { get; set; }
        public string Review_Type { get; set; }
        public Nullable<decimal> Review_Score { get; set; }
        public Nullable<bool> IsCustomerReview { get; set; }
        public string Review_Author { get; set; }
        public Nullable<System.DateTime> Review_PostedDate { get; set; }
        public string Review_Title { get; set; }
        public string Review_Description { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Create_User { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Edit_User { get; set; }
        public string Review_Status { get; set; }
    }
}
