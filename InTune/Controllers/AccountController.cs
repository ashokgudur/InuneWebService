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
    public class AccountController : ApiController
    {
        [Route("api/account/create")]
        [HttpPost]
        public HttpResponseMessage CreateAccount(Account account)
        {
            try
            {
                var a = new AccountService();
                a.AddAccount(account);
                return Request.CreateResponse(HttpStatusCode.OK, account);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot create this account", ex);
            }
        }

        [Route("api/account/update")]
        [HttpPost]
        public HttpResponseMessage UpdateAccount(Account account)
        {
            try
            {
                var a = new AccountService();
                a.UpdateAccount(account);
                return Request.CreateResponse(HttpStatusCode.OK, account);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot update this account", ex);
            }
        }

        [Route("api/account/allaccounts")]
        [HttpGet]
        public IEnumerable<Account> GetAllAccounts(int userId, int contactId)
        {
            var cs = new AccountService();
            var result = cs.ReadAllAccounts(userId, contactId);
            return result;
        }

        [Route("api/account/adduser")]
        [HttpPost]
        public HttpResponseMessage AddAccountUser(AccountContactUser acu)
        {
            try
            {
                var a = new AccountService();
                a.AddAccountUser(acu);
                return Request.CreateResponse(HttpStatusCode.OK, acu);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot add account user", ex);
            }
        }

        [Route("api/account/sharing")]
        [HttpPost]
        public HttpResponseMessage AddAccountSharing(int accountId, UserAccountShareRole[] accountShares)
        {
            try
            {
                var a = new AccountService();
                a.AddAccountSharing(accountId, accountShares);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot remove account user", ex);
            }
        }

        [Route("api/account/deleteuser")]
        [HttpPost]
        public HttpResponseMessage DeleteAccountUser(AccountContactUser acu)
        {
            try
            {
                var a = new AccountService();
                a.DeleteAccountUser(acu);
                return Request.CreateResponse(HttpStatusCode.OK, acu);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot remove account user", ex);
            }
        }

        [Route("api/account/addcontact")]
        [HttpPost]
        public HttpResponseMessage AddAccountContact(AccountContactUser acu)
        {
            try
            {
                var a = new AccountService();
                a.AddAccountContact(acu);
                return Request.CreateResponse(HttpStatusCode.OK, acu);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot add account contact", ex);
            }
        }

        [Route("api/account/contact/accounts")]
        [HttpGet]
        public IEnumerable<Account> GetContactAccounts(int contactId)
        {
            var a = new AccountService();
            var result = a.ReadContactAccounts(contactId);
            return result;
        }

        [Route("api/account/account/sharedcontacts")]
        [HttpGet]
        public IEnumerable<Contact> GetAccountSharedContacts(int userId, int accountId)
        {
            var a = new AccountService();
            var result = a.ReadAccountSharedContacts(userId, accountId);
            return result;
        }

        [Route("api/account/account/users")]
        [HttpGet]
        public int[] GetAccountUsers(int accountId, UserAccountRole role)
        {
            var a = new AccountService();
            var result = a.ReadAccountUsers(accountId, role);
            return result;
        }

        [Route("api/account/account/contacts")]
        [HttpGet]
        public IEnumerable<Contact> GetAccountContacts(int userId, int accountId)
        {
            var a = new AccountService();
            var result = a.ReadAccountContacts(userId, accountId);
            return result;
        }

        [Route("api/account/addentry")]
        [HttpPost]
        public HttpResponseMessage AddAccountEntry(Entry entry)
        {
            try
            {
                var a = new AccountService();
                a.AddEntry(entry);
                return Request.CreateResponse(HttpStatusCode.OK, entry);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException("Cannot add account entry", ex);
            }
        }

        [Route("api/account/entry")]
        [HttpGet]
        public Entry GetAccountEntry(int entryId)
        {
            var a = new AccountService();
            var result = a.ReadAccountEntry(entryId);
            return result;
        }

        [Route("api/account/entries")]
        [HttpGet]
        public IEnumerable<Entry> GetAccountEntries(int accountId)
        {
            var a = new AccountService();
            var result = a.ReadAccountEntries(accountId);
            return result;
        }
    }
}
