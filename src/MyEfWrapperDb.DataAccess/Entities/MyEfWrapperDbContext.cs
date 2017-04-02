namespace MyEfWrapperDb.DataAccess.Entities
{
    using System.Data.Entity;

    public partial class MyEfWrapperDbContext : DbContext
    {
        public MyEfWrapperDbContext() : base("name=MyEfWrapperDbContext")
        {
            Database.SetInitializer<MyEfWrapperDbContext>(null);
        }

        public MyEfWrapperDbContext(string connectionString) : base(connectionString)
        {
            Database.SetInitializer<MyEfWrapperDbContext>(null);
        }

        public virtual DbSet<Customer> Customers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .Property(e => e.Name)
                .IsUnicode(false);
        }
    }
}
