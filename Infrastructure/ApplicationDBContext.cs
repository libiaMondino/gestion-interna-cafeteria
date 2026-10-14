
using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure
{
    public class ApplicationDBContext : DbContext
    {   //DbContextOptions<ApplicationDBContext> contiene la configuración que necesita Entity Framework Core, por ejemplo, qué motor de base de datos utilizar.
        //options es el parámetro donde recibimos esa configuración.
        //: base(options) le pasa esa configuración al constructor de la clase padre, DbContext.
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
            : base(options)
            
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }        protected override void OnModelCreating(ModelBuilder modelBuilder)
{      
    //método para configurar como nuestras clases se represetan en la base de datos
    base.OnModelCreating(modelBuilder);

    modelBuilder.Entity<Category>(entity =>
    {   
        entity.HasKey(c => c.CategoryId);
          
        entity.Property(c => c.CategoryName)
              .IsRequired()
              .HasMaxLength(100);

        entity.HasIndex(c => c.CategoryName)
              .IsUnique();

        entity.Property(c => c.CategoryDescription)
              .HasMaxLength(255);
    });
    //Configuracion de product
    modelBuilder.Entity<Product>(entity =>
    {
        entity.HasKey(p => p.ProductId);
        entity.HasOne<Category>()// cada producto tiene una categoria
          .WithMany() //cada categoria puede tener muchos productos
          .HasForeignKey(p => p.CategoryId)
          .OnDelete(DeleteBehavior.Restrict);
        //nombre
        entity.Property(p => p.ProductName)
            .IsRequired()
            .HasMaxLength(100);
            //descipcion, opcional
        entity.Property(p => p.ProductDescription)
            .HasMaxLength(255);
        // Disponibilidad: obligatoria
        entity.Property(p => p.ProductIsAvailable)
            .IsRequired();
        entity.Property(p => p.ProductPrice)
          .HasPrecision(18, 2);
        entity.Property(p => p.ProductType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();// para guardar si es comprado o elaborado

        
        entity.ToTable(t =>
        {
            t.HasCheckConstraint(
                "CK_Product_QuantityStock",
                "[ProductQuantityStock] IS NULL OR [ProductQuantityStock] >= 0"
            );

            t.HasCheckConstraint(
                "CK_Product_MinimumStock",
                "[ProductMinimumStock] IS NULL OR [ProductMinimumStock] >= 0"
            );
            t.HasCheckConstraint( //no guardar rpreciosn negativos
           "CK_Product_Price",
            "[ProductPrice] >= 0"
            );
        });
   
    });
    

}
    }
}