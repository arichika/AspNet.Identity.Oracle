using System;
using AspNet.Identity.Oracle;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;

namespace SampleWebSite.Models
{
    //This is useful if you do not want to tear down the database each time you run the application.
    public sealed class ApplicationDbInitializer : IDisposable
    {
        private static readonly object ThisLock = new object();
        private static volatile ApplicationDbInitializer _applicationDbInitializer;

        private bool _isInitialized;

        private ApplicationDbInitializer(IOwinContext context)
        {
            Seed(context);
        }

        public static ApplicationDbInitializer Create(IdentityFactoryOptions<ApplicationDbInitializer> options, IOwinContext context)
        {
            if (_applicationDbInitializer != null)
                return _applicationDbInitializer;

            lock (ThisLock)
            {
                if (_applicationDbInitializer != null) return _applicationDbInitializer;
                _applicationDbInitializer = new ApplicationDbInitializer(context);
            }
            return _applicationDbInitializer;
        }

        /// <summary>
        /// Run Once Initialize code blocks.
        /// </summary>
        /// <param name="context"></param>
        private void Seed(IOwinContext context)
        {
            if (_isInitialized) return;

            InitializeDb(context);
            InitializeIdentity(context);

            _isInitialized = true;
        }

        // Verify Db or Tables and Create it, If you need.
        private static void InitializeDb(IOwinContext context)
        {
            // var oracleDatabase = context.Get<ApplicationDbContext>() as OracleDatabase;
            // e.g. Run the DDL if Table is not.
        }

        // Create Sample Admin User: admin@admin.com with password: Admin@123456 in the Admin role
        private static void InitializeIdentity(IOwinContext context)
        {
            var userManager = context.GetUserManager<ApplicationUserManager>();
            var roleManager = context.Get<ApplicationRoleManager>();

            // TODO: !!ATTENTION!! Please change yours setting.
            const string name = "admin@admin.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //Create Role Admin if it does not exist
            var role = roleManager.FindByName(roleName);
            if (role == null) {
                role = new IdentityRole(roleName);
                roleManager.Create(role);
            }

            var user = userManager.FindByName(name);
            if (user == null) {
                user = new ApplicationUser { UserName = name, Email = name };
                userManager.Create(user, password);
                userManager.SetLockoutEnabled(user.Id, false);
            }

            // Add user admin to Role Admin if not already added
            var rolesForUser = userManager.GetRoles(user.Id);
            if (!rolesForUser.Contains(role.Name)) {
                userManager.AddToRole(user.Id, role.Name);
            }
        }

        bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            _disposed = true;
        }
    }
}