using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Oracle.DataAccess.Client;

namespace AspNet.Identity.Oracle
{
    /// <summary>
    /// Class that represents the UserClaims table in the Oracle Database
    /// </summary>
    public class UserClaimsTable
    {
        private OracleDatabase _database;

        /// <summary>
        /// Constructor that takes a Oracle Database instance 
        /// </summary>
        /// <param name="database"></param>
        public UserClaimsTable(OracleDatabase database)
        {
            _database = database;
        }

        /// <summary>
        /// Returns a ClaimsIdentity instance given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public ClaimsIdentity FindByUserId(string userId)
        {
            var claims = new ClaimsIdentity();
            const string commandText = @"SELECT * FROM ANID2USERCLAIMS WHERE USERID = :USERID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value = userId, OracleDbType = OracleDbType.Varchar2 },
            };

            var rows = _database.Query(commandText, parameters);
            foreach (var claim in rows.Select(row => new Claim(row["CLAIMTYPE"], row["CLAIMVALUE"])))
            {
                claims.AddClaim(claim);
            }

            return claims;
        }

        /// <summary>
        /// Deletes all claims from a user given a userId
        /// </summary>
        /// <param name="userId">The user's id</param>
        /// <returns></returns>
        public int Delete(string userId)
        {
            const string commandText = @"DELETE FROM ANID2USERCLAIMS WHERE USERID = :USERID";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value = userId, OracleDbType = OracleDbType.Varchar2 },
            };

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Inserts a new claim in UserClaims table
        /// </summary>
        /// <param name="userClaim">User's claim to be added</param>
        /// <param name="userId">User's id</param>
        /// <returns></returns>
        public int Insert(Claim claim, string userId)
        {
            const string commandText = @"INSERT INTO ANID2USERCLAIMS (CLAIMVALUE, CLAIMTYPE, USERID) VALUES (:VALUE, :TYPE, :USERID)";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value = userId, OracleDbType = OracleDbType.Varchar2 },
                new OracleParameter{ ParameterName = "VALUE", Value = claim.Value, OracleDbType = OracleDbType.Clob },
                new OracleParameter{ ParameterName = "TYPE", Value = claim.Type, OracleDbType = OracleDbType.Clob },
            };

            return _database.Execute(commandText, parameters);
        }

        /// <summary>
        /// Deletes a claim from a user 
        /// </summary>
        /// <param name="user">The user to have a claim deleted</param>
        /// <param name="claim">A claim to be deleted from user</param>
        /// <returns></returns>
        public int Delete(IdentityUser user, Claim claim)
        {
            const string commandText = @"DELETE FROM ANID2USERCLAIMS WHERE USERID = :USERID AND @CLAIMVALUE = :VALUE AND CLAIMTYPE = :TYPE";
            var parameters = new List<OracleParameter>
            {
                new OracleParameter{ ParameterName = "USERID", Value = user.Id, OracleDbType = OracleDbType.Varchar2 },
                new OracleParameter{ ParameterName = "VALUE", Value = claim.Value, OracleDbType = OracleDbType.Clob },
                new OracleParameter{ ParameterName = "TYPE", Value = claim.Type, OracleDbType = OracleDbType.Clob },
            };

            return _database.Execute(commandText, parameters);
        }
    }
}
