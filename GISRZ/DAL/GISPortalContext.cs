using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using GISRZ.Models;

namespace GISRZ.DAL
{
	public class GISPortalContext : DbContext
	{
		public GISPortalContext() : base("DefaultConnection")
		{
		
		}

		public virtual DbSet<role> roles { get; set; }
		public virtual DbSet<security> security { get; set; }
		public virtual DbSet<security_roles> security_roles { get; set; }
		public virtual DbSet<user_roles> user_roles { get; set; }
		public virtual DbSet<user> users { get; set; }
	}
}