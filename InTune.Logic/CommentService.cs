using InTune.Data;
using InTune.Domain;
using System;
using System.Collections.Generic;

namespace InTune.Logic
{
    public class CommentService
    {
        public void AddUserComment(Comment comment)
        {
            if (!comment.IsValid())
                throw new Exception("Comment is not valid. Check Comment Text is entered");

            using (DbContext dbc = new DbContext())
            {
                var dao = new CommentDao(dbc, comment);
                dao.InsertContactComment();
            }
        }

        public void AddAccountComment(Comment comment)
        {
            if (!comment.IsValid())
                throw new Exception("Comment is not valid. Check Comment Text is entered");

            using (DbContext dbc = new DbContext())
            {
                dbc.BeginTransaction();
                var ada = new AccountDao(dbc);
                var userIds = ada.ReadAccountUsers(comment.AccountId);
                var dao = new CommentDao(dbc, comment);
                foreach (var uid in userIds)
                {
                    var auComment = new Comment
                    {
                        AccountId = comment.AccountId,
                        ByUserId = comment.ByUserId,
                        ToUserId = uid,
                        EntryId = comment.EntryId,
                        CommentText = comment.CommentText,
                        DateTimeStamp = comment.DateTimeStamp,
                    };

                    if (auComment.EntryId > 0)
                        dao.InsertEntryComment(auComment);
                    else
                        dao.InsertAccountComment(auComment);
                }
                dbc.Commit();
            }
        }

        public IEnumerable<Comment> ReadContactComments(int byUserId, int toUserId)
        {
            using (DbContext dbc = new DbContext())
            {
                //dbc.BeginTransaction();
                var dao = new CommentDao(dbc);
                var result = dao.ReadContactComments(byUserId, toUserId);
                //dao.MarkAsContactCommentsRead(byUserId, toUserId);
                //dbc.Commit();
                return result;
            }
        }

        public void MarkAccountAsUnreadComments(int accountId, int userId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new CommentDao(dbc);
                dao.MarkAccountAsUnreadComments(accountId, userId);
            }
        }

        public IEnumerable<Comment> ReadAccountComments(int accountId, int userId)
        {
            using (DbContext dbc = new DbContext())
            {
                dbc.BeginTransaction();
                var dao = new CommentDao(dbc);
                var result = dao.ReadAccountComments(accountId, userId);
                dao.MarkAsAccountUserCommentsRead(accountId, userId);
                dbc.Commit();
                return result;
            }
        }

        public IEnumerable<Comment> ReadEntryComments(int entryId, int userId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new CommentDao(dbc);
                return dao.ReadEntryComments(entryId, userId);
            }
        }
    }
}
