using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace ExampleDbLib
{
    public interface IAuditable
    {
    }
    public interface IAuditableExplicit
    {
        DateTime Created { get; set; }
        DateTime Modified { get; set; }
        string CreatedBy { get; set; }
        string ModifiedBy { get; set; }
    }
    public interface IDefaultAndOrder
    {
        int OrderBy { get; set; }
        bool IsDefault { get; set; }
    }
    public class ExampleDbContext : DbContext
    {
        private readonly DbContextOptions<ExampleDbContext> _options;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _config;

        public DbSet<Person> People { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public ExampleDbContext(DbContextOptions<ExampleDbContext> options,
            IHttpContextAccessor httpContextAccessor, 
            IConfiguration config)
        : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            _options = options;
            _config = config;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            base.OnModelCreating(modelBuilder);
            // Create shadow properties
            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
                    .Where(e => typeof(IAuditable).IsAssignableFrom(e.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("Created");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<DateTime>("Modified");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>("CreatedBy");

                modelBuilder.Entity(entityType.ClrType)
                    .Property<string>("ModifiedBy");

                //       modelBuilder.Entity(entityType.ClrType)
                //           .Property<byte[]>("Version")
                //           .IsRowVersion();
            }

        }
        public string GetUserName()
        {
            string username = "";
            
            bool? authenticated = _httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated;
            if (authenticated.HasValue && authenticated == true)
            {
                var identity = _httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
                username = identity.Claims.FirstOrDefault(c => c.Type == "name")?.Value;
                if (username == null || username.Length == 0)
                {
                    username = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                }
            }
            if (username == null || username.Length == 0)
            {
                string authHeader = _httpContextAccessor?.HttpContext?.Request?.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    //Extract credentials
                    string encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    Encoding encoding = Encoding.GetEncoding("iso-8859-1");
                    string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    int seperatorIndex = usernamePassword.IndexOf(':');

                    username = usernamePassword.Substring(0, seperatorIndex);
                }
            }
            if (username == null || username.Length == 0)
            {
                username = "Not signed in";
            }
            
            return username;
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            string username = GetUserName();
            ApplyAuditInformation(username);
            return base.SaveChangesAsync(cancellationToken);
        }
        public override int SaveChanges()
        {
            string username = GetUserName();
            ApplyAuditInformation(username);
            return base.SaveChanges();
        }

        public void ApplyAuditInformation(string userName)
        {
            var aa = ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified).ToList();

            var modifiedEntities = ChangeTracker.Entries<IAuditable>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            foreach (var entity in modifiedEntities)
            {
                entity.Property("Modified").CurrentValue = DateTime.Now;
                if (userName != null)
                    entity.Property("ModifiedBy").CurrentValue = userName;
                if (entity.State == EntityState.Added)
                {
                    entity.Property("Created").CurrentValue = DateTime.Now;
                    if (userName != null)
                        entity.Property("CreatedBy").CurrentValue = userName;
                }
            }

            var modifiedEntities2 = ChangeTracker.Entries<IAuditableExplicit>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);
            foreach (var entity in modifiedEntities2)
            {
                entity.Property("Modified").CurrentValue = DateTime.Now;
                if (userName != null)
                    entity.Property("ModifiedBy").CurrentValue = userName;
                if (entity.State == EntityState.Added)
                {
                    entity.Property("Created").CurrentValue = DateTime.Now;
                    if (userName != null)
                        entity.Property("CreatedBy").CurrentValue = userName;
                }
            }
        }
    }
}


