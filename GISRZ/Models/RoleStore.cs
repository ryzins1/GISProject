using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNet.Identity;

namespace GISRZ.Models
{
	
	public class RoleStore<TRole> : IQueryableRoleStore<TRole, int>, IDisposable where TRole : IRole<int>
	{
		private bool Disposed;
		private readonly string connectionString;

		public RoleStore(string connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
				throw new ArgumentNullException("connectionString");

			this.connectionString = connectionString;
		}

		#region IQueryableRoleStore Members
		public IQueryable<TRole> Roles
		{
			get
			{
				return Roles;
			}
		}
		#endregion

		#region IRoleStore Members
		public async Task CreateAsync(role role)
		{
			ThrowIfDisposed();

			if (role == null)
			{
				throw new ArgumentNullException("role");
			}

			await Task.Factory.StartNew(() =>
			{
				role.role_id = new int();
				using (SqlConnection connection = new SqlConnection(connectionString))
					connection.Execute("insert into Role(role_id,name) values(@role_id, @name)", role);
			});
		}

		public async Task DeleteAsync(role role)
		{
			ThrowIfDisposed();

			if (role == null)
			{
				throw new ArgumentNullException("role");
			}

			await Task.Factory.StartNew(() =>
			{
				using (var connection = new SqlConnection(connectionString))
					connection.Execute("delete from role where role_id = @role_id", new
					{
						role.role_id
					});
			});
		}

		public Task<role> FindByIdAsync(int roleId)
		{
			ThrowIfDisposed();

			return Task.Factory.StartNew(() =>
			{
				using (var connection = new SqlConnection(connectionString))
					return connection.Query<role>("select * from role where role_id = @role_id", new
					{
						roleId
					}).SingleOrDefault();
			});
		}

		public Task FindByNameAsync(string roleName)
		{
			ThrowIfDisposed();

			return Task.Factory.StartNew(() =>
			{
				using (var connection = new SqlConnection(connectionString))
					return connection.Query<role>("select * from role where name = @name", new
					{
						roleName
					}).SingleOrDefault();
			});
		}

		//public async Task UpdateAsync(role role)
		//{
		//	ThrowIfDisposed();

		//	if (role == null)
		//	{
		//		throw new ArgumentNullException("role");
		//	}

		//	await role.CommitAsync();
		//}
		#endregion

		#region IDisposable Members
		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

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
		#endregion


		Task<TRole> IRoleStore<TRole, int>.FindByIdAsync(int roleId)
		{
			throw new NotImplementedException();
		}

		Task<TRole> IRoleStore<TRole, int>.FindByNameAsync(string roleName)
		{
			throw new NotImplementedException();
		}

		public Task UpdateAsync(TRole role)
		{
			throw new NotImplementedException();
		}

		public Task CreateAsync(TRole role)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(TRole role)
		{
			throw new NotImplementedException();
		}
	}
}