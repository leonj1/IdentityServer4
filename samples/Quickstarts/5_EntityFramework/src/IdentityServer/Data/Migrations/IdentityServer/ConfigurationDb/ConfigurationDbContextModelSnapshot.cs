using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Metadata;

namespace YourNamespace // Replace with your actual namespace
{
    public class ConfigurationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<YourEntity>(entity =>
            {
                entity.ToTable("YourTableName");

                entity.Property(e => e.YourProperty).IsRequired();
                // Add other properties and configurations here
            });
        }
    }
}
