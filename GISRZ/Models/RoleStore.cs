using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNet.Identity;

namespace GISRZ.Models
{
	
	public class RoleStore<TRole> : IQueryableRoleStore<TRole, int> where TRole : IRole<int>
	{
		private IDbConnection db = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
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
		public async Task CreateAsync(TRole role)
		{
			ThrowIfDisposed();

			if (role == null)
			{
				throw new ArgumentNullException("role");
			}

			await Task.Factory.StartNew(() =>
			{
				db.Execute("insert into Role(role_id,name) values(@role_id, @name)", role);
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
				db.Execute("delete from role where role_id = @role_id", new { role.role_id });
			});
		}

		Task<TRole> IRoleStore<TRole, int>.FindByIdAsync(int roleId)
		{
			ThrowIfDisposed();

			return Task.Factory.StartNew(() => db.Query<TRole>("select * from role where role_id = @role_id", new { roleId }).SingleOrDefault());
		}

		public Task<TRole> FindByNameAsync(string roleName)
		{
			ThrowIfDisposed();

			return Task<TRole>.Factory.StartNew(() => db.Query<TRole>("select * from role where name = @name", new { roleName }).SingleOrDefault());
		}

		#endregion

		#region IDisposable Members
		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

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
		#endregion


		public Task UpdateAsync(TRole role)
		{
			throw new NotImplementedException();
		}

		public Task DeleteAsync(TRole role)
		{
			throw new NotImplementedException();
		}
	}
}