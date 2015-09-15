This is an example to implement a OracleDatabase store for ASP.NET Identity 2.0

Steps to run project

- Open project in VS with Update 2 or later installed
- Build project to restore packages and build project
- In the solution, add a new one ASP.NET project with MVC and Individual Authentication
- Uninstall Microsoft.AspNet.Identity.EntityFramework package from the web application
- Update connection string to use the OracleDatabase database as needed
 - Make the file "MyConnectionStrings.config" into the project SampleWebSite's root folder
 - e.g.
 ```
<?xml version="1.0" encoding="utf-8"?>
<connectionStrings>
  <add name="DefaultConnection"
       connectionString="Data Source=OracleDbTnsName; User Id=scott;Password=tiger;"
       providerName="Oracle.DataAccess.Client" />
</connectionStrings>
 ```
- In the IdentityModel.cs (in the SampleWebProject),
 let ApplicationUser class extend from Identity user in AspNet.Identity.Oracle
 - e.g
 ```
 using AspNet.Identity.Oracle;
 // using Microsoft.AspNet.Identity.EntityFramework;
 ```
- ApplicationDbContext extend from OracleDatabase and the contructor take a single parameter with the connectionstring name
```
    public class ApplicationDbContext : OracleDatabase
    {
        public ApplicationDbContext()
            : base("DefaultConnection")
        {
        }
	~
	}
```
- In the ApplicationUserManager.Create method (in the IdentityConfig.cs), replace instantiating UserManager as shown below
```
var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>() as OracleDatabase));
```
- In the ApplicationRoleManager.Create method (in the IdentityConfig.cs), replace instantiating UserManager as shown below
```
var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>() as OracleDatabase));
```
- If you want to create initial Administrator account or modify tables,
  you can write some code block at the InitializeDb method or InitializeIdentity method
  in the ApplicationDbInitializer class (in the IdentityConfig.cs)
  - This SampleWebSite project enabled initial admin account as shoen below
  ```
const string name = "admin@admin.com";
const string password = "Admin@123456";
const string roleName = "Admin";  
  ```
- Before you do debugging, you must create the tables in the database.
 Please run DDL Script, "OracleIdentity.sql.txt" in the AspNet.Identity.Oracle project.


//