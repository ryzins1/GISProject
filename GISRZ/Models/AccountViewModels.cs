using System.ComponentModel.DataAnnotations;

namespace GISRZ.Models
{
	public class LoginViewModel
	{
		[Required]
		[Display(Name = "Username")]
		public string Username
		{
			get;
			set;
		}

		[Required]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string Password
		{
			get;
			set;
		}
	}

	public class SelectRoleEditorViewModel
	{
		public SelectRoleEditorViewModel()
		{
		}
		public SelectRoleEditorViewModel(IdentityRole role)
		{
			RoleName = role.Name;
		}

		public bool Selected
		{
			get; set;
		}

		[Required]
		public string RoleName
		{
			get; set;
		}
	}


}
