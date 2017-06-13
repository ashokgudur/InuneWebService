using System.Net.Mail;

namespace InTune.Domain
{
    public class EmailAddress
    {
        MailAddress _mailAddress;
        public string Address
        {
            get { return _mailAddress.Address; }
        }

        public EmailAddress(string address)
        {
            _mailAddress = new MailAddress(address);
        }
    }
}
