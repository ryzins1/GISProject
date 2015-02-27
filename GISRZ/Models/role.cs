//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace GISRZ.Models
{
	public partial class role : IRole, IRole<int>
	{
		//public role()
		//{
		//	this.security_roles = new HashSet<security_roles>();
		//	this.user_roles = new HashSet<user_roles>();
		//}
	
		public static int role_id { get; set; }
		public string name { get; set; }
		public string disabled { get; set; }
		public Nullable<DateTime> disabled_date { get; set; }
		public Nullable<int> disabled_user_id { get; set; }
	
		public virtual ICollection<security_roles> securities { get; set; }
		public virtual ICollection<user_roles> users { get; set; }

		public string Id
		{
			get
			{
				return role_id.ToString();
			}
		}

		int IRole<int>.Id
		{
			get { return role_id; }
		}

		public string Name
		{
			get { return name; }
			set
			{
				this.Name = value;
			}
		}
	}
}
