using System;

namespace App.Api.Entities
{
    public class Todo : BaseEntityAudit
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateModified { get; set; }
    }
}
