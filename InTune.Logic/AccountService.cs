using InTune.Data;
using InTune.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTune.Logic
{
    public class AccountService
    {
        public void AddAccount(Account account)
        {
            if (!account.IsValid())
                throw new Exception("Account name not entered");

            using (DbContext dbc = new DbContext())
            {
                dbc.BeginTransaction();
                var dao = new AccountDao(dbc, account);

                if (dao.IsAccountExists())
                    throw new Exception("Account already exists.");

                dao.InsertAccount();
                dbc.Commit();
            }
        }

        public void UpdateAccount(Account account)
        {
            if (!account.IsValid())
                throw new Exception("Account is not valid. Check Name is entered");

            using (DbContext dbc = new DbContext())
            {
                var dao = new AccountDao(dbc, account);

                if (dao.IsAccountExists())
                    throw new Exception("Account already exists.");

                dao.UpdateAccount();
            }
        }

        public void DeleteAccountUser(AccountContactUser acu)
        {
            using (DbContext dbc = new DbContext())
            {
                var adao = new AccountDao(dbc);
                adao.DeleteAccountUser(acu);
            }
        }

        public void AddAccountUser(AccountContactUser acu)
        {
            using (DbContext dbc = new DbContext())
            {
                dbc.BeginTransaction();

                var udao = new UserDao(dbc);
                if (!udao.IsUserExists(acu.UserId))
                    throw new Exception("User doesn't exists.");

                var adao = new AccountDao(dbc);
                if (!adao.IsAccountExists(acu.AccountId))
                    throw new Exception("Account doesn't exists.");

                AccountContactUser acuParam = null;

                if (acu.ContactUserId == 0)
                    acuParam = new AccountContactUser
                    {
                        AccountId = acu.AccountId,
                        UserId = acu.UserId,
                        Role = acu.Role,
                    };
                else
                    acuParam = new AccountContactUser
                    {
                        AccountId = acu.AccountId,
                        UserId = acu.ContactUserId,
                        Role = acu.Role,
                    };

                if (adao.IsAccountUserExists(acuParam))
                {
                    adao.UpdateAccountUser(acuParam);
                    dbc.Commit();
                    return;
                }

                adao.AddAccountUser(acuParam);

                if (acu.ContactUserId == 0)
                    return;

                //link contact and account in source user
                acuParam = new AccountContactUser
                {
                    AccountId = acu.AccountId,
                    ContactId = acu.ContactId,
                    UserId = acu.UserId,
                    Role = acu.Role,
                };
                addAccountContact(dbc, acuParam);

                //create contact if doesn't exist in target user
                var user = udao.ReadUserById(acu.UserId);
                var cdao = new ContactDao(dbc);
                var contact = cdao.ReadContactByEmail(acu.ContactUserId, user.Email);
                if (contact == null)
                {
                    contact = new Contact
                    {
                        Name = user.Name,
                        Email = user.Email,
                        Mobile = user.Mobile,
                        UserId = acu.ContactUserId,
                        CreatedOn = DateTime.Now,
                    };
                    cdao.InsertContact(contact);
                }
                //link contact and account in target user
                acuParam = new AccountContactUser
                {
                    AccountId = acu.AccountId,
                    ContactId = contact.Id,
                    UserId = acu.ContactUserId,
                    Role = acu.Role,
                };
                addAccountContact(dbc, acuParam);

                dbc.Commit();
            }
        }

        public void AddAccountContact(AccountContactUser acu)
        {
            using (DbContext dbc = new DbContext())
            {
                addAccountContact(dbc, acu);
            }
        }

        private void addAccountContact(DbContext dbc, AccountContactUser acu)
        {
            var cdao = new ContactDao(dbc);
            if (!cdao.IsContactExists(acu.ContactId))
                throw new Exception("Contact doesn't exists.");

            var adao = new AccountDao(dbc);
            if (!adao.IsAccountExists(acu.AccountId))
                throw new Exception("Account doesn't exists.");

            if (adao.IsAccountContactExists(acu))
            {
                return;
                //throw new Exception("This Account contact already exists.");
            }

            adao.AddAccountContact(acu);
        }

        public IList<Account> ReadAllAccounts(int userId, int contactId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new AccountDao(dbc);
                return dao.ReadUserAccounts(userId, contactId);
            }
        }

        public IList<Account> ReadContactAccounts(int contactId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new AccountDao(dbc);
                return dao.ReadContactAccounts(contactId);
            }
        }

        public IList<Contact> ReadAccountSharedContacts(int userId, int accountId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new AccountDao(dbc);
                return dao.ReadAccountSharedContacts(userId, accountId);
            }
        }

        public int[] ReadAccountUsers(int accountId, UserAccountRole role)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new AccountDao(dbc);
                return dao.ReadAccountUsers(accountId, role);
            }
        }

        public IList<Contact> ReadAccountContacts(int userId, int accountId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new AccountDao(dbc);
                return dao.ReadAccountContacts(userId, accountId);
            }
        }

        public void AddEntry(Entry entry)
        {
            if (!entry.IsValid())
                throw new Exception("Details not entered");

            using (DbContext dbc = new DbContext())
            {
                dbc.BeginTransaction();
                var adao = new AccountDao(dbc);
                if (!adao.IsAccountExists(entry.AccountId))
                    throw new Exception("Account not exists.");

                var edao = new EntryDao(dbc, entry);
                edao.InsertEntry();
                dbc.Commit();
            }
        }

        public Entry ReadAccountEntry(int entryId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new EntryDao(dbc);
                return dao.ReadAccountEntry(entryId);
            }
        }

        public IList<Entry> ReadAccountEntries(int accountId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new EntryDao(dbc);
                return dao.ReadAccountEntries(accountId);
            }
        }
    }
}
