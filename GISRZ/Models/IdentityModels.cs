using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GISRZ.Models
{
	// You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
	public class ApplicationUser : IdentityUser
	{
		public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
		{
			// Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
			var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
			// Add custom user claims here
			return userIdentity;
		}
	}

	//public class ApplicationRole : IdentityRole
	//{
	//	public ApplicationRole() : base()
	//	{
	//	}
	//	public ApplicationRole(string name) : base(name)
	//	{
	//	}
	//	public string Name
	//	{
	//		get;
	//		set;
	//	}
	//}

	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext()
			: base("DefaultConnection", false)
		{
		}

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);

			//modelBuilder.Entity<IdentityUser>().ToTable("users").Property(p => p.Id).HasColumnName("user_id");
			modelBuilder.Entity<ApplicationUser>().ToTable("users").Property(p => p.Id).HasColumnName("user_id");
			Database.SetInitializer<ApplicationDbContext>(null);
			//modelBuilder.Entity<IdentityUserRole>().ToTable("MyUserRoles");
			//modelBuilder.Entity<IdentityUserLogin>().ToTable("MyUserLogins");
			//modelBuilder.Entity<IdentityUserClaim>().ToTable("MyUserClaims");
			//modelBuilder.Entity<IdentityRole>().ToTable("MyRoles");
		}


		public static ApplicationDbContext Create()
		{
			return new ApplicationDbContext();
		}
	}
}