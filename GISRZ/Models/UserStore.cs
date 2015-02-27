using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using GISRZ.DAL;
using Microsoft.AspNet.Identity;

namespace GISRZ.Models
{
	public class UserStore : IUserStore<user>, IUserLoginStore<user>, IUserPasswordStore<user>, IUserSecurityStampStore<user>, IUserRoleStore<user>
	{
		GISPortalContext _context;
		private IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
		private bool Disposed;
		private readonly string connectionString;
		
		public UserStore(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException("connectionString");

			this.connectionString = connectionString;
		}

		public UserStore()
		{
			connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
		}

		#region IUserStore
		public Task CreateAsync(user user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.Factory.StartNew(() =>
			{
				user.user_id = new int();
				db.Execute("insert into Users(User_Id, network_id, first_name, last_name) values(@user_Id, @network_id, @first_name, @last_name)", user);
			});
		}

		public Task DeleteAsync(user user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.Factory.StartNew(() =>
			{
				db.Execute("delete from User where User_Id = @userId", new { user.user_id });
			});
		}

		public Task<user> FindByIdAsync(string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException("userId");

			var parsedUserId = Convert.ToInt32(userId);
			return Task.Factory.StartNew(() => db.Query<user>("select * from Users where User_Id = @userId", new { userId = parsedUserId }).SingleOrDefault());
		}

		public Task<user> FindByNameAsync(string network_id)
		{
			if (string.IsNullOrWhiteSpace(network_id))
				throw new ArgumentNullException("network_id");

			return Task.Factory.StartNew(() => db.Query<user>("select * from Users where network_Id = @network_id", new { network_id }).SingleOrDefault());
		}

		public Task UpdateAsync(user user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.Factory.StartNew(() =>
			{
				db.Execute("update User set network_id = @network_id where network_id = @network_id", user);
			});
		}
		#endregion

		public void Dispose()
		{

		}

		#region IUserLoginStore
		public Task AddLoginAsync(user user, UserLoginInfo login)
		{
			throw new NotImplementedException();
		}

		public Task<user> FindAsync(UserLoginInfo login)
		{
			if (login == null)
				throw new ArgumentNullException("login");

			return Task.Factory.StartNew(() => db.Query<user>("select * from Users where User_Id = @user_id",
				login).SingleOrDefault());
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync(user user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.Factory.StartNew(() => (IList<UserLoginInfo>)db.Query<UserLoginInfo>("select * from Users where User_Id = @user_id", new
			{
				user.user_id
			}).ToList());
		}

		public Task RemoveLoginAsync(user user, UserLoginInfo login)
		{
			throw new NotImplementedException();
		}

		#endregion



		#region IUserPasswordStore
		public Task<string> GetPasswordHashAsync(user user)
		{
			throw new NotImplementedException();
		}

		public Task<bool> HasPasswordAsync(user user)
		{
			throw new NotImplementedException();
		}

		public Task SetPasswordHashAsync(user user, string passwordHash)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region IUserSecurityStampStore
		public virtual Task<string> GetSecurityStampAsync(user user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.FromResult(user.SecurityStamp);
		}

		public virtual Task SetSecurityStampAsync(user user, string stamp)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			user.SecurityStamp = stamp;
			return Task.FromResult(0);
		}

		#endregion

		protected void Dispose(bool disposing)
		{
			Disposed = true;
		}

		private void ThrowIfDisposed()
		{
			if (Disposed)
			{
				throw new ObjectDisposedException(GetType().Name);
			}
		}

		#region IUserRoleStore Members
		// check if user_role exists for user
		public async Task AddToRoleAsync(user user, string roleName)
		{
			ThrowIfDisposed();

			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			if (String.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}

			//get the role we're trying to add to the user
			RoleStore<role> roleStore = new RoleStore<role>(connectionString);
			role thisRole = new role();
			thisRole = await roleStore.FindByNameAsync(roleName);
			
			//find the user/role in the user_role table
			user_roles ur = new user_roles();
			ur.user_id = user.user_id;
			ur.role_id = thisRole.role_id;

			//if we find the user/role in the table, return false
			if (user.roles.Contains(ur))
				throw new Exception("user/role exists");

			//add the user/role to the user_role table and return true
			user.roles.Add(ur);

			await Task.Factory.StartNew(() =>
			{
				db.Query<user_roles>("INSERT INTO user_roles (user_id, role_id) VALUES(@user_id, @role_id)", ur);
			});
			await Task.FromResult(true);
		}

		Task<IList<string>> IUserRoleStore<user, string>.GetRolesAsync(user user)
		{
			ThrowIfDisposed();

			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			//return Task.Factory.StartNew(() => (IList<string>) db.Query<user_roles>("select * from user_roles where User_Id = @user_id", new { user.user_id }));
			//return Task.FromResult<IList<string>>(user.roles.Select(r=>r.role.name).ToList());
			return Task.Factory.StartNew(() => (IList<string>) (user.roles.Select(r => r.role.name).ToList()));
		}

		public Task<bool> IsInRoleAsync(user user, string roleName)
		{
			ThrowIfDisposed();

			if (user == null)
			{
				throw new ArgumentNullException("employee");
			}

			if (String.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}

			IList<role> thisRole = db.Query<role>("select * from role where name = @name", new { roleName }).ToList();
			return Task.FromResult(thisRole.Count > 0);
		}

		public Task RemoveFromRoleAsync(user user, string roleName)
		{
			ThrowIfDisposed();

			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			if (String.IsNullOrEmpty(roleName))
			{
				throw new ArgumentNullException("roleName");
			}

			var roleId = db.Query<role>("select role_id from role where name = @name", new { roleName });

			if (roleId == null)
			{
				throw new InvalidOperationException("Role is null");
			}
			
			db.Execute("DELETE from user_roles where user_id = @user_id and role_id = @role_id", new {user.user_id, ToInt32 = roleId});
			return Task.FromResult(true);
		}
		#endregion


		//Task IUserRoleStore<user, string>.AddToRoleAsync(user user, string roleName)
		//{
		//	ThrowIfDisposed();

		//	if (user == null)
		//	{
		//		throw new ArgumentNullException("user");
		//	}

		//	if (String.IsNullOrEmpty(roleName))
		//	{
		//		throw new ArgumentNullException("roleName");
		//	}

		//	//get the role we're trying to add to the user
		//	RoleStore<role> roleStore = new RoleStore<role>(connectionString);
		//	var thisRole = new Task<role>;
		//	thisRole = roleStore.FindByNameAsync(roleName);

		//	//find the user/role in the user_role table
		//	user_roles ur = new user_roles();
		//	ur.user_id = user.user_id;
		//	ur.role_id = thisRole.role_id;

		//	//if we find the user/role in the table, return false
		//	if (user.roles.Contains(ur))
		//		throw new Exception("user/role exists");

		//	//add the user/role to the user_role table and return true
		//	user.roles.Add(ur);
		//	return Task.FromResult(ur);
		//}

		//Task<IList<string>> IUserRoleStore<user, string>.GetRolesAsync(user user)
		//{
		//	throw new NotImplementedException();
		//}

		//Task<bool> IUserRoleStore<user, string>.IsInRoleAsync(user user, string roleName)
		//{
		//	throw new NotImplementedException();
		//}

		//Task IUserRoleStore<user, string>.RemoveFromRoleAsync(user user, string roleName)
		//{
		//	throw new NotImplementedException();
		//}

		//Task IUserStore<user, string>.CreateAsync(user user)
		//{
		//	throw new NotImplementedException();
		//}

		//Task IUserStore<user, string>.DeleteAsync(user user)
		//{
		//	throw new NotImplementedException();
		//}

		//Task<user> IUserStore<user, string>.FindByIdAsync(string userId)
		//{
		//	throw new NotImplementedException();
		//}

		//Task<user> IUserStore<user, string>.FindByNameAsync(string userName)
		//{
		//	throw new NotImplementedException();
		//}

		//Task IUserStore<user, string>.UpdateAsync(user user)
		//{
		//	throw new NotImplementedException();
		//}

		//void IDisposable.Dispose()
		//{
		//	throw new NotImplementedException();
		//}

		
	}
}