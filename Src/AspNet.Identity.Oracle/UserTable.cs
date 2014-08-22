using System;
using System.Collections.Generic;
using Oracle.DataAccess.Client;

namespace AspNet.Identity.Oracle
{
    /// <summary>
    /// Class that represents the Users table in the Oracle Database
    /// </summary>
    public class UserTable<TUser>
        where TUser :IdentityUser
    {
        private OracleDatabase _database;

        /// <summary>
        /// Constructor that takes a Oracle Database instance 
        /// </summary>
        /// <param name="database"></param>
        public UserTable(OracleDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns the user's name given a user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetUserName(string userId)
        {
            const string commandText = @"SELECT NAME FROM ANID2USERS WHERE ID = :USERID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value = userId, OracleDbType = OracleDbType.Varchar2 }
            };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Returns a User ID given a user name
        /// </summary>
        /// <param name="userName">The user's name</param>
        /// <returns></returns>
        public string GetUserId(string userName)
        {
            const string commandText = @"SELECT ID FROM ANID2USERS WHERE USERNAME = :NAME";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "NAME", Value = userName, OracleDbType = OracleDbType.Varchar2 }
            };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Returns an TUser given the user's id
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public TUser GetUserById(string userId)
        {
            TUser user;
            const string commandText = @"SELECT * FROM ANID2USERS WHERE ID = :USERID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value = userId, OracleDbType = OracleDbType.Varchar2 }
            };

            var rows = _database.Query(commandText, parameters);
            if (rows == null || rows.Count != 1) return null;

            var row = rows[0];
            user = (TUser)Activator.CreateInstance(typeof(TUser));
            user.Id = row["ID"];
            user.UserName = row["USERNAME"];
            user.PasswordHash = string.IsNullOrEmpty(row["PASSWORDHASH"]) ? null : row["PASSWORDHASH"];
            user.SecurityStamp = string.IsNullOrEmpty(row["SECURITYSTAMP"]) ? null : row["SECURITYSTAMP"];
            user.Email = string.IsNullOrEmpty(row["EMAIL"]) ? null : row["EMAIL"];
            user.EmailConfirmed = (row["EMAILCONFIRMED"] == "1");
            user.PhoneNumber = string.IsNullOrEmpty(row["PHONENUMBER"]) ? null : row["PHONENUMBER"];
            user.PhoneNumberConfirmed = (row["PHONENUMBERCONFIRMED"] == "1");
            user.LockoutEnabled = (row["LOCKOUTENABLED"] == "1");
            user.LockoutEndDateUtc = string.IsNullOrEmpty(row["LOCKOUTENDDATEUTC"]) ? DateTime.Now : DateTime.Parse(row["LOCKOUTENDDATEUTC"]);
            user.AccessFailedCount = string.IsNullOrEmpty(row["ACCESSFAILEDCOUNT"]) ? 0 : int.Parse(row["ACCESSFAILEDCOUNT"]);
            user.TwoFactorEnabled = (row["TWOFACTORENABLED"] == "1");

            return user;
        }

        /// <summary>
        /// Returns a list of TUser instances given a user name
        /// </summary>
        /// <param name="userName">User's name</param>
        /// <returns></returns>
        public List<TUser> GetUserByName(string userName)
        {
            var users = new List<TUser>();
            const string commandText = @"SELECT * FROM ANID2USERS WHERE USERNAME = :NAME";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "NAME", Value = userName, OracleDbType = OracleDbType.Varchar2}
            };

            var rows = _database.Query(commandText, parameters);
            foreach(var row in rows)
            {
                var user = (TUser)Activator.CreateInstance(typeof(TUser));
                user.Id = row["ID"];
                user.UserName = row["USERNAME"];
                user.PasswordHash = string.IsNullOrEmpty(row["PASSWORDHASH"]) ? null : row["PASSWORDHASH"];
                user.SecurityStamp = string.IsNullOrEmpty(row["SECURITYSTAMP"]) ? null : row["SECURITYSTAMP"];
                user.Email = string.IsNullOrEmpty(row["EMAIL"]) ? null : row["EMAIL"];
                user.EmailConfirmed = (row["EMAILCONFIRMED"] == "1");
                user.PhoneNumber = string.IsNullOrEmpty(row["PHONENUMBER"]) ? null : row["PHONENUMBER"];
                user.PhoneNumberConfirmed = (row["PHONENUMBERCONFIRMED"] == "1");
                user.LockoutEnabled = (row["LOCKOUTENABLED"] == "1");
                user.LockoutEndDateUtc = string.IsNullOrEmpty(row["LOCKOUTENDDATEUTC"]) ? DateTime.Now : DateTime.Parse(row["LOCKOUTENDDATEUTC"]);
                user.AccessFailedCount = string.IsNullOrEmpty(row["ACCESSFAILEDCOUNT"]) ? 0 : int.Parse(row["ACCESSFAILEDCOUNT"]);
                user.TwoFactorEnabled = (row["TWOFACTORENABLED"] == "1");
                users.Add(user);
            }

            return users;
        }

        /// <summary>
        /// Get Users by email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public List<TUser> GetUserByEmail(string email)
        {
            var users = new List<TUser>();
            // throw new NotImplementedException();
            return users;
        }

        /// <summary>
        /// Return the user's password hash
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public string GetPasswordHash(string userId)
        {
            const string commandText = @"SELECT PASSWORDHASH FROM ANID2USERS WHERE ID = :USERID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value = userId, OracleDbType = OracleDbType.Varchar2 }
            };

            var passHash = _database.GetStrValue(commandText, parameters);
            return string.IsNullOrEmpty(passHash) ? null : passHash;
        }

        /// <summary>
        /// Sets the user's password hash
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="passwordHash"></param>
        /// <returns></returns>
        public int SetPasswordHash(string userId, string passwordHash)
        {
            const string commandText = @"UPDATE ANID2USERS SET PASSWORDHASH = :PWHASH WHERE ID = :USERID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "PWHASH", Value = passwordHash, OracleDbType = OracleDbType.Clob },
                new OracleParameter{ ParameterName = "USERID", Value = userId, OracleDbType = OracleDbType.Varchar2 }
            };

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Returns the user's security stamp
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetSecurityStamp(string userId)
        {
            const string commandText = @"SELECT SECURITYSTAMP FROM ANID2USERS WHERE ID = :USERID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value =  userId, OracleDbType = OracleDbType.Varchar2 }
            };

            return _database.GetStrValue(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Insert(TUser user)
        {
            const string commandText = @"INSERT INTO ANID2USERS (USERNAME, ID, PASSWORDHASH, SECURITYSTAMP,EMAIL,EMAILCONFIRMED,PHONENUMBER,PHONENUMBERCONFIRMED, ACCESSFAILEDCOUNT,LOCKOUTENABLED,LOCKOUTENDDATEUTC,TWOFACTORENABLED)
                VALUES (:NAME, :USERID, :PWHASH, :SECSTAMP,:EMAIL,:EMAILCONFIRMED,:PHONENUMBER,:PHONENUMBERCONFIRMED,:ACCESSFAILEDCOUNT,:LOCKOUTENABLED,:LOCKOUTENDDATEUTC,:TWOFACTORENABLED)";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "NAME", Value = user.UserName, OracleDbType = OracleDbType.Varchar2 },
                new OracleParameter{ ParameterName = "USERID", Value = user.Id, OracleDbType = OracleDbType.Varchar2 },
                new OracleParameter{ ParameterName = "PWHASH", Value = user.PasswordHash, OracleDbType = OracleDbType.Clob },
                new OracleParameter{ ParameterName = "SECSTAMP", Value = user.SecurityStamp, OracleDbType = OracleDbType.Clob },
                new OracleParameter{ ParameterName = "EMAIL", Value = user.Email, OracleDbType = OracleDbType.Varchar2 },
                new OracleParameter{ ParameterName = "EMAILCONFIRMED", Value = user.EmailConfirmed.ToDecimal(), OracleDbType = OracleDbType.Decimal },
                new OracleParameter{ ParameterName = "PHONENUMBER", Value = user.PhoneNumber, OracleDbType = OracleDbType.Clob },
                new OracleParameter{ ParameterName = "PHONENUMBERCONFIRMED", Value = user.PhoneNumberConfirmed.ToDecimal(), OracleDbType = OracleDbType.Decimal },
                new OracleParameter{ ParameterName = "ACCESSFAILEDCOUNT", Value = user.AccessFailedCount, OracleDbType = OracleDbType.Decimal },
                new OracleParameter{ ParameterName = "LOCKOUTENABLED", Value = user.LockoutEnabled.ToDecimal(), OracleDbType = OracleDbType.Decimal },
                new OracleParameter{ ParameterName = "LOCKOUTENDDATEUTC", Value = user.LockoutEndDateUtc, OracleDbType = OracleDbType.Date },
                new OracleParameter{ ParameterName = "TWOFACTORENABLED", Value = user.TwoFactorEnabled.ToDecimal(), OracleDbType = OracleDbType.Decimal },
            };

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a user from the Users table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        private int Delete(string userId)
        {
            const string commandText = @"DELETE FROM ANID2USERS WHERE ID = :USERID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter {ParameterName = "USERID", Value = userId, OracleDbType = OracleDbType.Varchar2},
            };

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a user from the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Delete(TUser user)
        {
            return Delete(user.Id);
        }

        /// <summary>
        /// Updates a user in the Users table
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int Update(TUser user)
        {
            const string commandText = @"UPDATE ANID2USERS SET
                USERNAME =:NAME,
                PASSWORDHASH =:PWHASH,
                SECURITYSTAMP =:SECSTAMP, 
                EMAIL=:EMAIL,
                EMAILCONFIRMED=:EMAILCONFIRMED,
                PHONENUMBER=:PHONENUMBER,
                PHONENUMBERCONFIRMED=:PHONENUMBERCONFIRMED,
                ACCESSFAILEDCOUNT=:ACCESSFAILEDCOUNT,
                LOCKOUTENABLED=:LOCKOUTENABLED,
                LOCKOUTENDDATEUTC=:LOCKOUTENDDATEUTC,
                TWOFACTORENABLED=:TWOFACTORENABLED  
                WHERE ID =:USERID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "NAME", Value = user.UserName, OracleDbType = OracleDbType.Varchar2 },
                new OracleParameter{ ParameterName = "PWHASH", Value = user.PasswordHash, OracleDbType = OracleDbType.Clob, IsNullable = true },
                new OracleParameter{ ParameterName = "SECSTAMP", Value = user.SecurityStamp, OracleDbType = OracleDbType.Clob, IsNullable = true },
                new OracleParameter{ ParameterName = "EMAIL", Value = user.Email, OracleDbType = OracleDbType.Varchar2, IsNullable = true },
                new OracleParameter{ ParameterName = "EMAILCONFIRMED", Value = user.EmailConfirmed.ToDecimal(), OracleDbType = OracleDbType.Decimal },
                new OracleParameter{ ParameterName = "PHONENUMBER", Value = user.PhoneNumber, OracleDbType = OracleDbType.Clob, IsNullable = true },
                new OracleParameter{ ParameterName = "PHONENUMBERCONFIRMED", Value = user.PhoneNumberConfirmed.ToDecimal(), OracleDbType = OracleDbType.Decimal },
                new OracleParameter{ ParameterName = "ACCESSFAILEDCOUNT", Value = user.AccessFailedCount, OracleDbType = OracleDbType.Decimal },
                new OracleParameter{ ParameterName = "LOCKOUTENABLED", Value = user.LockoutEnabled.ToDecimal(), OracleDbType = OracleDbType.Decimal },
                new OracleParameter{ ParameterName = "LOCKOUTENDDATEUTC", Value = user.LockoutEndDateUtc, OracleDbType = OracleDbType.Date, IsNullable = true },
                new OracleParameter{ ParameterName = "TWOFACTORENABLED", Value = user.TwoFactorEnabled.ToDecimal(), OracleDbType = OracleDbType.Decimal },
                new OracleParameter{ ParameterName = "USERID", Value = user.Id, OracleDbType = OracleDbType.Varchar2 },
            };

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Returns all list of TUser instances
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TUser> GetUsers()
        {
            var users = new List<TUser>();
            const string commandText = @"SELECT * FROM ANID2USERS";

            var rows = _database.Query(commandText, null);
            foreach (var row in rows)
            {
                var user = (TUser)Activator.CreateInstance(typeof(TUser));
                user.Id = row["ID"];
                user.UserName = row["USERNAME"];
                user.PasswordHash = string.IsNullOrEmpty(row["PASSWORDHASH"]) ? null : row["PASSWORDHASH"];
                user.SecurityStamp = string.IsNullOrEmpty(row["SECURITYSTAMP"]) ? null : row["SECURITYSTAMP"];
                user.Email = string.IsNullOrEmpty(row["EMAIL"]) ? null : row["EMAIL"];
                user.EmailConfirmed = (row["EMAILCONFIRMED"] == "1");
                user.PhoneNumber = string.IsNullOrEmpty(row["PHONENUMBER"]) ? null : row["PHONENUMBER"];
                user.PhoneNumberConfirmed = (row["PHONENUMBERCONFIRMED"] == "1");
                user.LockoutEnabled = (row["LOCKOUTENABLED"] == "1");
                user.LockoutEndDateUtc = string.IsNullOrEmpty(row["LOCKOUTENDDATEUTC"]) ? DateTime.Now : DateTime.Parse(row["LOCKOUTENDDATEUTC"]);
                user.AccessFailedCount = string.IsNullOrEmpty(row["ACCESSFAILEDCOUNT"]) ? 0 : int.Parse(row["ACCESSFAILEDCOUNT"]);
                user.TwoFactorEnabled = (row["TWOFACTORENABLED"] == "1");
                users.Add(user);
            }

            return users;
        }
    }
}
