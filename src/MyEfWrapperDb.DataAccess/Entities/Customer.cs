namespace MyEfWrapperDb.DataAccess.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Customer")]
    public partial class Customer
    {
        public Customer()
        {
        }

        [Key]
        public Guid CustomerGuid { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
    }
}
