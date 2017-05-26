using InTune.Data;
using InTune.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTune.Logic
{
    public class UserDao
    {
        DbContext _dbc = null;
        User _user = null;

        public UserDao(DbContext dbc, User user)
            : this(dbc)
        {
            _user = user;
        }

        public UserDao(DbContext dbc)
        {
            _dbc = dbc;
        }

        public bool IsUserExists(int userId)
        {
            var sql = string.Format("select count(id) from [User] where id={0}", userId);
            var result = _dbc.ExecuteScalar(sql);
            var idCcount = Convert.ToInt32(result);
            return idCcount > 0;
        }

        public bool IsUserExists()
        {
            //TODO: Add additional checks for existence of Mobile, AtUserName also
            var sql = "select count(id) from [User] where email=@userEmail";
            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@userEmail", _user.Email.Trim());
            var result = cmd.ExecuteScalar();
            var idCcount = Convert.ToInt32(result);
            return idCcount > 0;
        }

        public void InsertUser()
        {
            var sql = "insert into [User] (name, mobile, email, atUserName, password, createdOn) values (@userName, @userMobile, @userEmail, @userAtUserName, @userPassword, @userCreatedOn)";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@userName", _user.Name);
            _dbc.AddParameterWithValue(cmd, "@userMobile", _user.Mobile);
            _dbc.AddParameterWithValue(cmd, "@userEmail", _user.Email);
            _dbc.AddParameterWithValue(cmd, "@userAtUserName", _user.AtUserName);
            _dbc.AddParameterWithValue(cmd, "@userPassword", _user.Password);
            _dbc.AddParameterWithValue(cmd, "@userCreatedOn", _user.CreatedOn);
            cmd.ExecuteNonQuery();

            _user.Id = _dbc.GetGeneratedIdentityValue();
        }

        public User ReadUserById(int id)
        {
            var sql = string.Format("select * from [User] where id={0}", id);
            return readUser(_dbc.CreateCommand(sql));
        }

        public User ReadUserByEmail(string email)
        {
            var sql = "select * from [User] where email=@userEmail";
            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@userEmail", email);
            return readUser(cmd);
        }

        private User readUser(IDbCommand cmd)
        {
            var rdr = cmd.ExecuteReader();
            User result = null;
            if (rdr.Read())
            {
                result = new User
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    Name = rdr["Name"].ToString(),
                    Mobile = rdr["Mobile"].ToString(),
                    Email = rdr["Email"].ToString(),
                    AtUserName = rdr["AtUserName"].ToString(),
                    Password = rdr["Password"].ToString(),
                    CreatedOn = Convert.ToDateTime(rdr["CreatedOn"])
                };
            }

            rdr.Close();

            return result;
        }
    }
}
