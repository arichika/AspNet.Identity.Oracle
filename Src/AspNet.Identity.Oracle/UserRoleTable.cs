using System.Collections.Generic;
using System.Linq;
using Oracle.DataAccess.Client;

namespace AspNet.Identity.Oracle
{
    /// <summary>
    /// Class that represents the UserRoles table in the Oracle Database
    /// </summary>
    public class UserRolesTable
    {
        private OracleDatabase _database;

        /// <summary>
        /// Constructor that takes a Oracle Database instance 
        /// </summary>
        /// <param name="database"></param>
        public UserRolesTable(OracleDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns a list of user's roles
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public List<string> FindByUserId(string userId)
        {
            const string commandText = @"SELECT ROLES.NAME FROM USERROLES, ROLES WHERE USERROLES.USERID = :USERID AND USERROLES.ROLEID = ROLES.ID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value = userId, OracleDbType = OracleDbType.Varchar2 }
            };

            var rows = _database.Query(commandText, parameters);

            return rows.Select(row => row["NAME"]).ToList();
        }

        /// <summary>
        /// Deletes all roles from a user in the UserRoles table
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            const string commandText = @"DELETE FROM USERROLES WHERE USERID = :USERID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value = userId, OracleDbType = OracleDbType.Varchar2 }
            };

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new role for a user in the UserRoles table
        /// </summary>
        /// <param name="user">The User</param>
        /// <param name="roleId">The Role's id</param>
        /// <returns></returns>
        public int Insert(IdentityUser user, string roleId)
        {
            const string commandText = @"INSERT INTO USERROLES (USERID, ROLEID) VALUES (:USERID, :ROLEID)";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value = user.Id, OracleDbType = OracleDbType.Varchar2},
                new OracleParameter{ ParameterName = "ROLEID", Value = roleId, OracleDbType = OracleDbType.Varchar2},
            };

            return _database.Execute(commandText, parameters);
        }
    }
}
