namespace App.Api.Entities
{
    // [Table("Customers")]
    public class Customer : BaseEntityAudit
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }
}
