using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Enums;

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
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<User> Users { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<KitchenOrder> KitchenOrders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<Ingredient> Ingredients { get; set; } = null!;
        public DbSet<RecipeDetail> RecipeDetails { get; set; } = null!;
        public DbSet<StockMovement> StockMovements { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
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
                entity.HasOne(p => p.Category)// cada producto tiene una categoria
                .WithMany(c => c.Products) //cada categoria puede tener muchos productos
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

            // Configuración de User
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);
                entity.Property(u => u.UserName)
                 .IsRequired()
                 .HasMaxLength(100);

                entity.Property(u => u.UserLastName)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(u => u.UserEmail)
                    .IsRequired()
                    .HasMaxLength(254);

                entity.HasIndex(u => u.UserEmail)
                    .IsUnique();
                entity.Property(u => u.UserPasswordHash)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(u => u.UserRole)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();
                entity.Property(u => u.IsActive)
                    .IsRequired();
            });
            // Configuración de Tables
            modelBuilder.Entity<Table>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.Number)
                    .IsRequired();
                entity.HasIndex(t => t.Number)
                    .IsUnique();
                entity.Property(t => t.Capacity)
                    .IsRequired();
                entity.Property(t => t.Status)
                    .IsRequired()
                    .HasDefaultValue(true);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Table_Number", "[Number] > 0");
                    t.HasCheckConstraint("CK_Table_Capacity", "[Capacity] > 0");
                });

            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(o => o.Id);

                entity.Property(o => o.DateTime)
                    .IsRequired();

                entity.Property(o => o.Status)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasDefaultValue("Activo");

                entity.Property(o => o.Total)
                    .HasPrecision(18, 2)
                    .IsRequired();

                // Relación Order - Table
                entity.HasOne(o => o.Table)
                    .WithMany(t => t.Orders)
                    .HasForeignKey(o => o.TableId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación Order - User
                entity.HasOne(o => o.User)
                    .WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Order_Status",
                        "[Status] IN ('Activo', 'Cerrado')"
                    );

                    t.HasCheckConstraint(
                        "CK_Order_Total",
                        "[Total] >= 0"
                    );
                });
            });

            modelBuilder.Entity<KitchenOrder>(entity =>
            {
                entity.HasKey(k => k.Id);

                entity.Property(k => k.SentDateTime)
                    .IsRequired(false);

                entity.Property(k => k.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                // Relación KitchenOrder - Order
                entity.HasOne(k => k.Order)
                    .WithMany(o => o.KitchenOrders)
                    .HasForeignKey(k => k.OrderId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(d => d.Id);

                entity.Property(d => d.Quantity)
                    .IsRequired();

                entity.Property(d => d.UnitPrice)
                    .HasPrecision(18, 2)
                    .IsRequired();

                entity.Property(d => d.SubTotal)
                    .HasPrecision(18, 2)
                    .IsRequired();

                // Relación OrderDetail - KitchenOrder
                entity.HasOne(d => d.KitchenOrder)
                    .WithMany(k => k.OrderDetails)
                    .HasForeignKey(d => d.KitchenOrderId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación OrderDetail - Product
                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_OrderDetail_Quantity",
                        "[Quantity] > 0"
                    );

                    t.HasCheckConstraint(
                        "CK_OrderDetail_UnitPrice",
                        "[UnitPrice] >= 0"
                    );

                    t.HasCheckConstraint(
                        "CK_OrderDetail_SubTotal",
                        "[SubTotal] >= 0"
                    );
                });
            });

            // Configuración de Ingredient
            modelBuilder.Entity<Ingredient>(entity =>
            {
                entity.HasKey(i => i.Id);

                entity.Property(i => i.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(i => i.UnitOfMeasure)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(i => i.QuantityStock)
                    .HasPrecision(18, 4);

                entity.Property(i => i.MinimumStock)
                    .HasPrecision(18, 4);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_Ingredient_QuantityStock",
                        "[QuantityStock] >= 0"
                    );

                    t.HasCheckConstraint(
                        "CK_Ingredient_MinimumStock",
                        "[MinimumStock] >= 0"
                    );
                });
            });

            // Configuración de RecipeDetail
            modelBuilder.Entity<RecipeDetail>(entity =>
            {
                entity.HasKey(r => r.Id);

                entity.Property(r => r.RequiredQuantity)
                    .HasPrecision(18, 4);

                entity.HasOne(r => r.Product)
                    .WithMany(p => p.RecipeDetails)
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(r => r.Ingredient)
                    .WithMany(i => i.RecipeDetails)
                    .HasForeignKey(r => r.IngredientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(r => new { r.ProductId, r.IngredientId })
                    .IsUnique();

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_RecipeDetail_RequiredQuantity",
                        "[RequiredQuantity] > 0"
                    );
                });

            });

            // Configuración de StockMovement
            modelBuilder.Entity<StockMovement>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.DateTime)
                    .IsRequired();

                entity.Property(m => m.Quantity)
                    .HasPrecision(18, 4);

                entity.Property(m => m.Reason)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(m => m.MovementType)
                    .HasConversion<string>()
                    .HasMaxLength(10)
                    .IsRequired();

                entity.HasOne(m => m.User)
                    .WithMany()
                    .HasForeignKey(m => m.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Product)
                    .WithMany()
                    .HasForeignKey(m => m.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.Ingredient)
                    .WithMany()
                    .HasForeignKey(m => m.IngredientId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(m => m.OrderDetail)
                    .WithMany()
                    .HasForeignKey(m => m.OrderDetailId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.ToTable(t =>
                {
                    t.HasCheckConstraint(
                        "CK_StockMovement_Quantity",
                        "[Quantity] > 0"
                    );

                    t.HasCheckConstraint(
                        "CK_StockMovement_Target",
                        "([ProductId] IS NOT NULL AND [IngredientId] IS NULL) OR " +
                        "([ProductId] IS NULL AND [IngredientId] IS NOT NULL)"
                    );

                    t.HasCheckConstraint(
                        "CK_StockMovement_TypeAndDetail",
                        "([MovementType] = 'In' AND [OrderDetailId] IS NULL) OR " +
                        "([MovementType] = 'Out' AND [OrderDetailId] IS NOT NULL)"
                    );
                });
            });

        }
    }
}
