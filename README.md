AspNet.Identity.Oracle
======================

AspNet.Identity.Oracle for ASP.NET Identity 2.0 with ODP.NET  
forked from AspNet.Identity.MySQL provided at codeplex.

https://aspnet.codeplex.com/SourceControl/latest#Samples/Identity/AspNet.Identity.MySQL/AspNet.Identity.MySQL.csproj  

How To Use
==========
(from ReadMe.txt)

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

Notice
======

Please change to the table name you want.  
By default, "ANID2{Purpose}" (e.g. "ANID2USERS"), which means that "AspNetUsers" ASP.NET Identity is created automatically.  

See Also
========

AspNet.Identity.MySQL(CodePlex)  
https://aspnet.codeplex.com/SourceControl/latest#Samples/Identity/AspNet.Identity.MySQL/AspNet.Identity.MySQL.csproj  

Implementing a Custom MySQL ASP.NET Identity Storage Provider 
http://www.asp.net/identity/overview/extensibility/implementing-a-custom-mysql-aspnet-identity-storage-provider  

Announcing RTM of ASP.NET Identity 2.0.0  
http://blogs.msdn.com/b/webdev/archive/2014/03/20/test-announcing-rtm-of-asp-net-identity-2-0-0.aspx  
//
