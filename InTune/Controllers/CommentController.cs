using InTune.Data;
using InTune.Domain;
using InTune.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace InTune.Controllers
{
    public class CommentController : ApiController
    {
        [Route("api/comment/addcomment")]
        [HttpPost]
        public HttpResponseMessage AddComment(Comment comment)
        {
            try
            {
                var cs = new CommentService();

                if (comment.AccountId > 0)
                    cs.AddAccountComment(comment);
                else
                    cs.AddUserComment(comment);

                return Request.CreateResponse(HttpStatusCode.OK, comment);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot add comment", ex);
            }
        }

        [Route("api/comment/account/markasunread")]
        [HttpGet]
        public HttpResponseMessage MarkAsAccountHasUnreadComments(int accountId, int userId)
        {
            var cs = new CommentService();
            cs.MarkAccountAsUnreadComments(accountId, userId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("api/comment/contact/allcomments")]
        [HttpGet]
        public IEnumerable<Comment> GetContactComments(int byUserId, int toUserId)
        {
            var cs = new CommentService();
            var result = cs.ReadContactComments(byUserId, toUserId);
            return result;
        }

        [Route("api/comment/account/allcomments")]
        [HttpGet]
        public IEnumerable<Comment> GetAccountComments(int accountId, int userId)
        {
            var cs = new CommentService();
            var result = cs.ReadAccountComments(accountId, userId);
            return result;
        }

        [Route("api/comment/entry/allcomments")]
        [HttpGet]
        public IEnumerable<Comment> GetEntryComments(int entryId, int userId)
        {
            var cs = new CommentService();
            var result = cs.ReadEntryComments(entryId, userId);
            return result;
        }
    }
}
