using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNet.Identity.Oracle
{
    /// <summary>
    /// Class that implements the key ASP.NET Identity role store iterfaces
    /// </summary>
    public class RoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : IdentityRole
    {
        private RoleTable roleTable;
        public OracleDatabase Database { get; private set; }

        /// <summary>
        /// Get all Roles defined.
        /// This code is a loose implementation.
        /// An occurrence of a performance problem is when you get a large amount of data.
        /// </summary>
        public IQueryable<TRole> Roles
        {
            get
            {
                // If you have some performance issues, then you can implement the IQueryable.
                var x = roleTable.GetRoles() as List<TRole>;
                return x != null ? x.AsQueryable() : null;
            }
        }

        /// <summary>
        /// Default constructor that initializes a new Oracle Database
        /// instance using the Default Connection string
        /// </summary>
        public RoleStore()
        {
            new RoleStore<TRole>(new OracleDatabase());
        }

        /// <summary>
        /// Constructor that takes a Oracle Database as argument 
        /// </summary>
        /// <param name="database"></param>
        public RoleStore(OracleDatabase database)
        {
            Database = database;
            roleTable = new RoleTable(database);
        }

        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            roleTable.Insert(role);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            roleTable.Delete(role.Id);

            return Task.FromResult<Object>(null);
        }

        public Task<TRole> FindByIdAsync(string roleId)
        {
            var result = roleTable.GetRoleById(roleId) as TRole;

            return Task.FromResult(result);
        }

        public Task<TRole> FindByNameAsync(string roleName)
        {
            var result = roleTable.GetRoleByName(roleName) as TRole;

            return Task.FromResult(result);
        }

        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            roleTable.Update(role);

            return Task.FromResult<Object>(null);
        }

        public void Dispose()
        {
            if (Database == null) return;

            Database.Dispose();
            Database = null;
        }

    }
}
