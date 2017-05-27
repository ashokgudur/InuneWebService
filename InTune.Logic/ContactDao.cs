using InTune.Data;
using InTune.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTune.Logic
{
    public class ContactDao
    {
        DbContext _dbc = null;
        Contact _contact = null;

        public ContactDao(DbContext dbc, Contact contact)
            : this(dbc)
        {
            _contact = contact;
        }

        public ContactDao(DbContext dbc)
        {
            _dbc = dbc;
        }

        public bool IsContactExists(int contactId)
        {
            var sql = string.Format("select count(id) from Contact where id={0}", contactId);
            var result = _dbc.ExecuteScalar(sql);
            var idCcount = Convert.ToInt32(result);
            return idCcount > 0;
        }

        public bool IsContactExists()
        {
            //TODO: Include checks for Mobile exists.

            var sql = new StringBuilder();
            sql.Append("select count(id) from Contact where email=@contactEmail and userId=@userId");
            if (_contact.Id != 0)
                sql.Append(" and id <> " + _contact.Id);

            var cmd = _dbc.CreateCommand(sql.ToString());
            _dbc.AddParameterWithValue(cmd, "@contactEmail", _contact.Email.Trim());
            _dbc.AddParameterWithValue(cmd, "@userId", _contact.UserId);
            var result = cmd.ExecuteScalar();
            var idCcount = Convert.ToInt32(result);
            return idCcount > 0;
        }

        public void InsertContact()
        {
            InsertContact(_contact);
        }

        public void InsertContact(Contact contact)
        {
            //TODO: Check UserId Exists before insert into contact

            var sql = "insert into Contact (userId, name, email, mobile, address, createdOn) values (@userId, @contactName, @contactEmail, @contactMobile, @contactAddress, @contactCreatedOn)";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@userId", contact.UserId);
            _dbc.AddParameterWithValue(cmd, "@contactName", contact.Name);
            _dbc.AddParameterWithValue(cmd, "@contactEmail", contact.Email);
            _dbc.AddParameterWithValue(cmd, "@contactMobile", contact.Mobile);
            _dbc.AddParameterWithValue(cmd, "@contactAddress", contact.Address);
            _dbc.AddParameterWithValue(cmd, "@contactCreatedOn", contact.CreatedOn);
            cmd.ExecuteNonQuery();

            contact.Id = _dbc.GetGeneratedIdentityValue();
        }

        public void UpdateContact()
        {
            var sql = "update Contact set name=@contactName, email=@contactEmail, mobile=@contactMobile, address=@contactAddress where id=@contactId";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@contactId", _contact.Id);
            _dbc.AddParameterWithValue(cmd, "@contactName", _contact.Name);
            _dbc.AddParameterWithValue(cmd, "@contactEmail", _contact.Email);
            _dbc.AddParameterWithValue(cmd, "@contactMobile", _contact.Mobile);
            _dbc.AddParameterWithValue(cmd, "@contactAddress", _contact.Address);
            cmd.ExecuteNonQuery();
        }

        public Contact ReadContact(int contactId)
        {
            var sql = string.Format("select * from Contact c where c.id={0}", contactId);
            using (var rdr = _dbc.ExecuteReader(sql))
            {
                if (rdr.Read())
                {
                    return new Contact
                    {
                        Id = Convert.ToInt32(rdr["id"]),
                        UserId = Convert.ToInt32(rdr["UserId"]),
                        Name = rdr["Name"].ToString(),
                        Email = rdr["Email"].ToString(),
                        Mobile = rdr["Mobile"].ToString(),
                        Address = rdr["Address"].ToString(),
                        CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                        HasUnreadComments = Convert.ToBoolean(rdr["HasUnreadComments"]),
                    };
                }
            }

            return new Contact();
        }

        public IList<Contact> ReadContacts(int userId)
        {
            var result = new List<Contact>();
            var sql = string.Format("select c.id, c.name, c.email, c.mobile, c.address, c.createdOn, c.hasUnreadComments, u.id [uid], count(cc.id) comments from Contact c left join [User] u on c.email=u.email left join ContactComment cc on u.id=cc.byUserId or u.id=cc.toUserId where c.userId={0} group by c.id, c.name, c.email, c.mobile, c.address, c.createdOn, c.hasUnreadComments, u.id", userId);
            var rdr = _dbc.ExecuteReader(sql);
            while (rdr.Read())
            {
                result.Add(new Contact
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    UserId = userId,
                    Name = rdr["Name"].ToString(),
                    Email = rdr["Email"].ToString(),
                    Mobile = rdr["Mobile"].ToString(),
                    Address = rdr["Address"].ToString(),
                    CreatedOn = Convert.ToDateTime(rdr["CreatedOn"]),
                    ContactUserId = rdr["uid"] is DBNull ? 0 : Convert.ToInt32(rdr["uid"]),
                    HasComments = Convert.ToInt32(rdr["comments"]) > 0,
                    HasUnreadComments = Convert.ToBoolean(rdr["HasUnreadComments"]),
                });
            }

            rdr.Close();
            return result;
        }

        public Contact ReadContactByEmail(int userId, string email)
        {
            var sql = "select * from Contact where email=@contactEmail and userId=@userId";
            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@contactEmail", email);
            _dbc.AddParameterWithValue(cmd, "@userId", userId);
            var rdr = cmd.ExecuteReader();
            Contact result = null;
            if (rdr.Read())
            {
                result = new Contact
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    UserId = Convert.ToInt32(rdr["UserId"]),
                    Name = rdr["Name"].ToString(),
                    Mobile = rdr["Mobile"].ToString(),
                    Email = rdr["Email"].ToString(),
                    Address = rdr["Address"].ToString(),
                    CreatedOn = Convert.ToDateTime(rdr["CreatedOn"])
                };
            }

            rdr.Close();
            return result;
        }
    }
}
