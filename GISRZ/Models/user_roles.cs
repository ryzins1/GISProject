//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

namespace GISRZ.Models
{
	public partial class user_roles
	{
		public int user_id { get; set; }
		public int role_id { get; set; }
		public Nullable<DateTime> action_datetime { get; set; }
		public Nullable<int> action_user { get; set; }
		public string action_type { get; set; }

		public virtual user user { get; set; }
		public virtual role role { get; set; }
	}
}