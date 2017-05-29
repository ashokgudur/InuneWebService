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
    public class AccountDao
    {
        DbContext _dbc = null;
        Account _account = null;

        public AccountDao(DbContext dbc, Account account)
            : this(dbc)
        {
            _account = account;
        }

        public AccountDao(DbContext dbc)
        {
            _dbc = dbc;
        }

        public bool IsAccountUserExists(AccountContactUser acu)
        {
            var sql = string.Format("select id from AccountUser where userId={0} and accountId={1}", acu.UserId, acu.AccountId, (int)acu.Role);
            var result = _dbc.ExecuteScalar(sql);
            var id = Convert.ToInt32(result);
            return id > 0;
        }

        public bool IsAccountContactExists(AccountContactUser acu)
        {
            var sql = string.Format("select count(id) from AccountContact where contactId={0} and accountId={1} and userId={2}", acu.ContactId, acu.AccountId, acu.UserId);
            var result = _dbc.ExecuteScalar(sql);
            var idCcount = Convert.ToInt32(result);
            return idCcount > 0;
        }

        public bool IsAccountExists(int accountId)
        {
            var sql = string.Format("select count(id) from Account where id={0}", accountId);
            var result = _dbc.ExecuteScalar(sql);
            var idCcount = Convert.ToInt32(result);
            return idCcount > 0;
        }

        public bool IsAccountExists()
        {
            var sql = "select count(a.Name) from Account a inner join accountUser au on a.id=au.accountId " +
                        "where a.Name=@accountName COLLATE Latin1_General_CS_AS and au.userId=@userId";
            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@accountName", _account.Name.Trim());
            _dbc.AddParameterWithValue(cmd, "@userId", _account.UserId);
            var result = cmd.ExecuteScalar();
            var idCcount = Convert.ToInt32(result);
            return idCcount > 0;
        }

        public void InsertAccount()
        {
            var sql = "insert into Account (name, groupId) values (@accountName, @groupId)";
            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@accountName", _account.Name);
            _dbc.AddParameterWithValue(cmd, "@groupId", 0);
            cmd.ExecuteNonQuery();

            _account.Id = _dbc.GetGeneratedIdentityValue();

            var acu = new AccountContactUser
            {
                AccountId = _account.Id,
                UserId = _account.UserId,
            };

            AddAccountUser(acu);
        }

        public void UpdateAccount()
        {
            var sql = "update Account set name=@accountName where id=@accountId";
            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@accountId", _account.Id);
            _dbc.AddParameterWithValue(cmd, "@accountName", _account.Name);
            cmd.ExecuteNonQuery();
        }

        public void AddAccountUser(AccountContactUser acu)
        {
            var sql = "insert into AccountUser (accountId, userId, role, addedOn) values (@accountId, @userId, @role, @addedOn)";
            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@accountId", acu.AccountId);
            _dbc.AddParameterWithValue(cmd, "@userId", acu.UserId);
            _dbc.AddParameterWithValue(cmd, "@role", acu.Role);
            _dbc.AddParameterWithValue(cmd, "@addedOn", DateTime.Now);
            cmd.ExecuteNonQuery();
        }

        public void UpdateAccountUser(AccountContactUser acu)
        {
            var sql = string.Format("update AccountUser set Role={0} where userId={1} and accountId={2}", (int)acu.Role, acu.UserId, acu.AccountId);
            _dbc.ExecuteCommand(sql);
        }

        public void AddAccountSharing(int accountId, UserAccountShareRole[] accountShares)
        {
            var sql = string.Format("delete from AccountUser " +
                                    "where accountId={0} and [Role] > 0", accountId);
            _dbc.ExecuteCommand(sql);

            sql = "insert into AccountUser (accountId, userId, role, addedOn) " +
                   "values (@accountId, @userId, @role, @addedOn)";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@accountId", accountId);
            _dbc.AddParameterWithValue(cmd, "@userId", null);
            _dbc.AddParameterWithValue(cmd, "@role", null);
            _dbc.AddParameterWithValue(cmd, "@addedOn", DateTime.Now);

            foreach (var accountShare in accountShares)
            {
                var userIdParam = cmd.Parameters["@userId"] as IDataParameter;
                userIdParam.Value = accountShare.UserId;

                var roleParam = cmd.Parameters["@role"] as IDataParameter;
                roleParam.Value = accountShare.Role;

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteAccountUser(AccountContactUser acu)
        {
            var sql = string.Format("delete from AccountUser where userId={0} and accountId={1}", acu.ContactUserId, acu.AccountId);
            _dbc.ExecuteCommand(sql);
        }

        public void AddAccountContact(AccountContactUser acu)
        {
            var sql = string.Format("insert into AccountContact (accountId, contactId, userId) values ({0}, {1}, {2})", acu.AccountId, acu.ContactId, acu.UserId);
            _dbc.ExecuteCommand(sql);
        }

        public IList<Contact> ReadAccountSharedContacts(int userId, int accountId)
        {
            var result = new List<Contact>();
            var sql = string.Format("select c.id, c.name, u.id [uid] " +
                                    "from Contact c left join [User] u on c.email=u.email " +
                                    "where c.userId={0} ", userId);

            using (var rdr = _dbc.ExecuteReader(sql))
            {
                while (rdr.Read())
                    result.Add(
                        new Contact
                        {
                            Id = Convert.ToInt32(rdr["id"]),
                            Name = rdr["name"].ToString(),
                            ContactUserId = rdr["uid"] == DBNull.Value ? 0 : Convert.ToInt32(rdr["uid"]),
                        });
            };

            sql = string.Format("select UserId, [Role] from AccountUser " +
                                "where AccountId={0} and [Role] > 0", accountId);

            using (var rdr = _dbc.ExecuteReader(sql))
            {
                while (rdr.Read())
                {
                    var sharedUserId = Convert.ToInt32(rdr["UserId"]);
                    var sharedRole = (UserAccountRole)Convert.ToInt32(rdr["Role"]);
                    var contact = result.Where(c => c.ContactUserId == sharedUserId).SingleOrDefault();
                    if (contact != null)
                        contact.AccountSharedRole = sharedRole;
                };
            };

            return result;
        }

        public int[] ReadAccountUsers(int accountId, UserAccountRole role)
        {
            var result = new List<int>();
            var sql = string.Format("select userId from AccountUser where accountId={0} and role={1}", accountId, (int)role);
            var rdr = _dbc.ExecuteReader(sql);
            while (rdr.Read())
                result.Add(Convert.ToInt32(rdr["userId"]));

            rdr.Close();
            return result.ToArray();
        }

        public int[] ReadAccountUsers(int accountId)
        {
            var result = new List<int>();
            var sql = string.Format("select userId from AccountUser where accountId={0}", accountId);
            var rdr = _dbc.ExecuteReader(sql);
            while (rdr.Read())
                result.Add(Convert.ToInt32(rdr["userId"]));

            rdr.Close();
            return result.ToArray();
        }

        public IList<Account> ReadUserAccounts(int userId, int contactId)
        {
            //Optimized version

            var result = new List<Account>();
            var sql = new StringBuilder();

            sql.Append("select a.id, a.Name, au.Role, au.AddedOn, au.hasUnreadComments, count(c.id) comments from Account a inner join AccountUser au on a.id = au.AccountId left join AccountComment c on au.accountId=c.accountId and au.userId=c.toUserId ");

            if (contactId != 0)
            {
                sql.Append(string.Format(" inner join AccountContact ac on a.id = ac.AccountId and ac.contactId={0} ", contactId));
            }

            sql.Append(string.Format(" where au.userId={0} ", userId));
            sql.Append(" group by a.id, a.Name, au.Role, au.AddedOn, au.hasUnreadComments");

            var rdr = _dbc.ExecuteReader(sql.ToString());
            while (rdr.Read())
            {
                result.Add(new Account
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    Name = rdr["Name"].ToString(),
                    Role = (UserAccountRole)Convert.ToInt32(rdr["Role"]),
                    AddedOn = Convert.ToDateTime(rdr["AddedOn"]),
                    HasComments = Convert.ToInt32(rdr["comments"]) > 0,
                    HasUnreadComments = Convert.ToBoolean(rdr["HasUnreadComments"]),
                });
            }

            rdr.Close();

            if (result.Count > 0)
            {
                var accountIds = string.Join(",", result.Select(a => a.Id).ToArray());
                sql = new StringBuilder();
                sql.Append(string.Format("select accountId, sum(case txnType when 0 then amount when 1 then amount when 2 then -amount else 0 end) Balance from [Entry] where accountId in ({0}) group by accountId", accountIds));
                rdr = _dbc.ExecuteReader(sql.ToString());
                while (rdr.Read())
                {
                    var accountId = Convert.ToInt32(rdr["accountId"]);
                    var balance = rdr["Balance"] is DBNull ? 0 : Convert.ToDecimal(rdr["Balance"]);
                    var account = result.Where(a => a.Id == accountId).FirstOrDefault();
                    account.Balance = balance;
                    account.HasEntries = true;
                }

                rdr.Close();
            }

            return result;
        }

        public IList<Account> ReadContactAccounts(int contactId)
        {
            var result = new List<Account>();
            var sql = string.Format("select a.id, a.Name from Account a inner join AccountContact ac on a.id = ac.AccountId where ac.contactId={0}", contactId);
            var rdr = _dbc.ExecuteReader(sql);
            while (rdr.Read())
            {
                result.Add(new Account
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    Name = rdr["Name"].ToString(),
                });
            }

            rdr.Close();
            return result;
        }

        public IList<Contact> ReadAccountContacts(int userId, int accountId)
        {
            var result = new List<Contact>();
            var sql = string.Format("select c.id, c.Name, u.id uid from Contact c inner join AccountContact ac on c.id = ac.contactId left join [User] u on c.email=u.email where ac.accountId={0} and ac.userId={1}", accountId, userId);
            var rdr = _dbc.ExecuteReader(sql);
            while (rdr.Read())
            {
                result.Add(new Contact
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    Name = rdr["Name"].ToString(),
                    ContactUserId = rdr["uid"] is DBNull ? 0 : Convert.ToInt32(rdr["uid"]),
                });
            }

            rdr.Close();
            return result;
        }
    }
}
