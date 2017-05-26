using InTune.Data;
using InTune.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTune.Logic
{
    public class CommentDao
    {
        DbContext _dbc = null;
        Comment _comment = null;

        public CommentDao(DbContext dbc, Comment comment)
            : this(dbc)
        {
            _comment = comment;
        }

        public CommentDao(DbContext dbc)
        {
            _dbc = dbc;
        }

        public void InsertContactComment()
        {
            var sql = "insert into ContactComment (byUserId, toUserId, commentText, dateTimeStamp, [status]) values (@byUserId, @toUserId, @commentText, @dateTimeStamp, @status)";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@byUserId", _comment.ByUserId);
            _dbc.AddParameterWithValue(cmd, "@toUserId", _comment.ToUserId);
            _dbc.AddParameterWithValue(cmd, "@commentText", _comment.CommentText);
            _dbc.AddParameterWithValue(cmd, "@dateTimeStamp", _comment.DateTimeStamp);
            _dbc.AddParameterWithValue(cmd, "@status", _comment.Status);
            cmd.ExecuteNonQuery();

            _comment.Id = _dbc.GetGeneratedIdentityValue();
        }

        public void InsertAccountComment(Comment comment)
        {
            var sql = "insert into AccountComment (accountId, byUserId, toUserId, commentText, dateTimeStamp, [status]) values (@accountId, @byUserId, @toUserId, @commentText, @dateTimeStamp, @status)";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@accountId", comment.AccountId);
            _dbc.AddParameterWithValue(cmd, "@byUserId", comment.ByUserId);
            _dbc.AddParameterWithValue(cmd, "@toUserId", comment.ToUserId);
            _dbc.AddParameterWithValue(cmd, "@commentText", comment.CommentText);
            _dbc.AddParameterWithValue(cmd, "@dateTimeStamp", comment.DateTimeStamp);
            _dbc.AddParameterWithValue(cmd, "@status", comment.Status);
            cmd.ExecuteNonQuery();
        }

        public void InsertEntryComment(Comment comment)
        {
            var sql = "insert into EntryComment (entryId, byUserId, toUserId, commentText, dateTimeStamp, [status]) values (@entryId, @byUserId, @toUserId, @commentText, @dateTimeStamp, @status)";

            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@entryId", comment.EntryId);
            _dbc.AddParameterWithValue(cmd, "@byUserId", comment.ByUserId);
            _dbc.AddParameterWithValue(cmd, "@toUserId", comment.ToUserId);
            _dbc.AddParameterWithValue(cmd, "@commentText", comment.CommentText);
            _dbc.AddParameterWithValue(cmd, "@dateTimeStamp", comment.DateTimeStamp);
            _dbc.AddParameterWithValue(cmd, "@status", comment.Status);
            cmd.ExecuteNonQuery();
        }

        public IEnumerable<Comment> ReadContactComments(int byUserId, int toUserId)
        {
            var sql = string.Format("select cc.id, cc.commentText, cc.dateTimeStamp, cc.status, u.Name byUserName from ContactComment cc left join [User] u on u.id=cc.byUserId where cc.byUserId in ({0}) and cc.toUserId in ({1}) order by cc.id", string.Format("{0},{1}", byUserId, toUserId), string.Format("{0},{1}", byUserId, toUserId));

            var rdr = _dbc.ExecuteReader(sql);
            var result = new List<Comment>();
            while (rdr.Read())
            {
                result.Add(new Comment
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    ByUserId = byUserId,
                    ToUserId = toUserId,
                    ByUserName = rdr["ByUserName"].ToString(),
                    CommentText = rdr["CommentText"].ToString(),
                    DateTimeStamp = Convert.ToDateTime(rdr["DateTimeStamp"]),
                    Status = (CommentStatus)Convert.ToInt32(rdr["Status"]),
                });
            }

            rdr.Close();
            return result;
        }

        public void MarkAccountAsUnreadComments(int accountId, int userId)
        {
            var sql = string.Format("update AccountUser set HasUnreadComments=1 where accountId={0} and userId={1}", accountId, userId);
            var cmd = _dbc.CreateCommand(sql);
            cmd.ExecuteNonQuery();
        }

        //TODO: make this similar to MarkAsAccountUserCommentsRead
        //public void MarkAsContactCommentsRead(int byUserId, int toUserId)
        public void MarkAsContactCommentsRead(int byUserId, int contactId, int toUserId)
        {
            var sql = "update Contact set HasUnreadComments=0 where id=@contactId";
            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@contactId", contactId);
            cmd.ExecuteNonQuery();

            //sql = "update ContactComment set [status]=1 where accountId=@accountId and toUserId=@toUserId";
            //cmd = _dbc.CreateCommand(sql);
            //_dbc.AddParameterWithValue(cmd, "@accountId", accountId);
            //_dbc.AddParameterWithValue(cmd, "@toUserId", userId);
            //cmd.ExecuteNonQuery();
        }

        public IEnumerable<Comment> ReadAccountComments(int accountId, int userId)
        {
            var sql = string.Format("select ac.id, ac.commentText, ac.dateTimeStamp, ac.status, u.Name byUserName from AccountComment ac left join [User] u on u.id=ac.byUserId where ac.AccountId={0} and ac.toUserId={1} order by ac.id", accountId, userId);

            var rdr = _dbc.ExecuteReader(sql);
            var result = new List<Comment>();
            while (rdr.Read())
            {
                result.Add(new Comment
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    ByUserId = userId,
                    AccountId = accountId,
                    ByUserName = rdr["ByUserName"].ToString(),
                    CommentText = rdr["CommentText"].ToString(),
                    DateTimeStamp = Convert.ToDateTime(rdr["DateTimeStamp"]),
                    Status = (CommentStatus)Convert.ToInt32(rdr["Status"]),
                });
            }

            rdr.Close();
            return result;
        }

        public void MarkAsAccountUserCommentsRead(int accountId, int userId)
        {
            var sql = "update AccountComment set [status]=1 where accountId=@accountId and toUserId=@toUserId";
            var cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@accountId", accountId);
            _dbc.AddParameterWithValue(cmd, "@toUserId", userId);
            cmd.ExecuteNonQuery();

            sql = "update AccountUser set HasUnreadComments=0 where accountId=@accountId and userId=@userId";
            cmd = _dbc.CreateCommand(sql);
            _dbc.AddParameterWithValue(cmd, "@accountId", accountId);
            _dbc.AddParameterWithValue(cmd, "@userId", userId);
            cmd.ExecuteNonQuery();
        }

        public IEnumerable<Comment> ReadEntryComments(int entryId, int userId)
        {
            var sql = string.Format("select ec.id, ec.commentText, ec.dateTimeStamp, ec.status, u.Name byUserName from EntryComment ec left join [User] u on u.id=ec.byUserId where ec.EntryId={0} and ec.toUserId={1} order by ec.id", entryId, userId);

            var rdr = _dbc.ExecuteReader(sql);
            var result = new List<Comment>();
            while (rdr.Read())
            {
                result.Add(new Comment
                {
                    Id = Convert.ToInt32(rdr["id"]),
                    ByUserId = userId,
                    EntryId = entryId,
                    ByUserName = rdr["ByUserName"].ToString(),
                    CommentText = rdr["CommentText"].ToString(),
                    DateTimeStamp = Convert.ToDateTime(rdr["DateTimeStamp"]),
                    Status = (CommentStatus)Convert.ToInt32(rdr["Status"]),
                });
            }

            rdr.Close();
            return result;
        }
    }
}
