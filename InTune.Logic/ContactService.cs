using InTune.Data;
using InTune.Domain;
using System;
using System.Collections.Generic;

namespace InTune.Logic
{
    public class ContactService
    {
        public void AddContact(Contact contact)
        {
            if (!contact.IsValid())
                throw new Exception("Contact is not valid. Check Name, Mobile, and Email is entered");

            using (DbContext dbc = new DbContext())
            {
                var dao = new ContactDao(dbc, contact);

                if (dao.IsContactExists())
                    throw new Exception("Contact already exists.");

                dao.InsertContact();
            }
        }

        public void UpdateContact(Contact contact)
        {
            if (!contact.IsValid())
                throw new Exception("Contact is not valid. Check Name, Mobile, and Email is entered");

            using (DbContext dbc = new DbContext())
            {
                var dao = new ContactDao(dbc, contact);

                if (dao.IsContactExists())
                    throw new Exception("Contact with this email already exists.");

                dao.UpdateContact();
            }
        }

        public IList<Contact> ReadAllContacts(int userId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new ContactDao(dbc);
                return dao.ReadContacts(userId);
            }
        }

        public Contact ReadContact(int contactId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new ContactDao(dbc);
                return dao.ReadContact(contactId);
            }
        }

    }
}
