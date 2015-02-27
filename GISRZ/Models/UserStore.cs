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
	public class UserStore : IUserStore<user>, IUserLoginStore<user>, IUserPasswordStore<user>, IUserSecurityStampStore<user>
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
				using (SqlConnection connection = new SqlConnection(connectionString))
					connection.Execute("insert into Users(User_Id, network_id, first_name, last_name) values(@user_Id, @network_id, @first_name, @last_name)", user);
			});
		}

		public Task DeleteAsync(user user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.Factory.StartNew(() =>
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
					connection.Execute("delete from User where User_Id = @userId", new
					{
						user.user_id
					});
			});
		}

		public Task<user> FindByIdAsync(string userId)
		{
			if (string.IsNullOrWhiteSpace(userId))
				throw new ArgumentNullException("userId");

			int parsedUserId = Convert.ToInt32(userId);
			return Task.Factory.StartNew(() =>
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
					return connection.Query<user>("select * from Users where User_Id = @userId", new
					{
						userId = parsedUserId
					}).SingleOrDefault();
			});
		}

		public Task<user> FindByNameAsync(string network_id)
		{
			if (string.IsNullOrWhiteSpace(network_id))
				throw new ArgumentNullException("network_id");

			return Task.Factory.StartNew(() =>
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
					return connection.Query<user>("select * from Users where network_id = lower(@network_id)", new
					{
						network_id
					}).SingleOrDefault();
			});
		}

		public Task UpdateAsync(user user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.Factory.StartNew(() =>
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
					connection.Execute("update User set network_id = @network_id where network_id = @network_id", user);
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

			return Task.Factory.StartNew(() =>
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
					return connection.Query<user>("select * from Users where User_Id = @user_id",
						login).SingleOrDefault();
			});
		}

		public Task<IList<UserLoginInfo>> GetLoginsAsync(user user)
		{
			if (user == null)
				throw new ArgumentNullException("user");

			return Task.Factory.StartNew(() =>
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
					return (IList<UserLoginInfo>)connection.Query<UserLoginInfo>("select * from Users where User_Id = @user_id", new
					{
						user.user_id
					}).ToList();
			});
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

		protected void Dispose(
		bool disposing)
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
		public Task AddToRoleAsync(user user, string roleName)
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

			role thisRole = RoleStore<role>.FindByNameAsync(roleName);
			if (thisRole == null)
			{
				user.roles.Add(thisRole);
			}
			 
			return Task.FromResult(true);

			
		}

		public Task<IList<string>> GetRolesAsync(user user)
		{
			ThrowIfDisposed();

			if (user == null)
			{
				throw new ArgumentNullException("user");
			}

			//List<string> user_roles = new IList<user_roles>().Where(ur => ur.user_id == user.user_id).ToList();

			//return Task.FromResult<IList<string>>(new List<user_roles>().Where(ur => ur.user_id == user.user_id).ToList());
			return Task.Factory.StartNew(() =>
			{
				using (SqlConnection connection = new SqlConnection(connectionString))
					return connection.Query<user_roles>("select * from user_roles where User_Id = @user_id", new
					{
						user.user_id
					}).ToList() as IList<string>;
			});
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

			IList<role> foo = db.Query<role>("select * from role where name = @name", new
					{
						roleName
					}).ToList();

			return Task.FromResult(foo.Count > 0);
		}

		public Task RemoveFromRoleAsync(user user, string roleName)
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

			role role = this.RolesRepository.FindSingleOrDefault(
				r =>
					(r.Name == roleName));

			if (role == null)
			{
				throw new InvalidOperationException("Role is null");
			}

			user.roles.Remove(role);

			return Task.FromResult(0);
		}
		#endregion


	}
}