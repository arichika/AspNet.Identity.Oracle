This is an example to implement a OracleDatabase store for ASP.NET Identity 2.0

Steps to run project

- Open project in VS with Update 2 or later installed
- Build project to restore packages and build project
- In the solution, add a new one ASP.NET project with MVC and Individual Authentication
- Uninstall Microsoft.AspNet.Identity.EntityFramework package from the web application
- Update connection string to use the OracleDatabase database as needed
- In the IdentityModel.cs, let ApplicationUser class extend from Identity user in AspNet.Identity.Oracle
- ApplicationDbContext extend from OracleDatabase and the contructor take a single parameter with the connectionstring name
- In the ApplicationManager.Create method, replace instantiating UserManager as shown below

    var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>() as OracleDatabase));

//