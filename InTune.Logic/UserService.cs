using InTune.Data;
using InTune.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InTune.Logic
{
    public class UserService
    {
        public void Register(User user)
        {
            if (!user.IsValid())
                throw new Exception("User is not valid. Check Name, Mobile, Email, AtUserName, and Password is entered");

            using (DbContext dbc = new DbContext())
            {
                var dao = new UserDao(dbc, user);

                if (dao.IsUserExists())
                    throw new Exception("User already exists.");

                dao.InsertUser();
            }
        }

        public void UpdateUser(User user)
        {
            if (!user.IsValid())
                throw new Exception("User is not valid. Check Name, Mobile, Email, AtUserName, and Password is entered");

            using (DbContext dbc = new DbContext())
            {
                var dao = new UserDao(dbc, user);

                if (dao.IsUserExists())
                    throw new Exception("User already exists.");

                dao.UpdateUser();
            }
        }

        public void ForgotPassword(string email)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new UserDao(dbc);
                var user = dao.ReadUserByEmail(email);
                if (user.IsNew)
                    throw new Exception("User with this email address was not found.");
                try
                {
                    Emailer.SendMail(createMessage(email, user.Password));
                }
                catch (Exception ex)
                {
                    logError(dbc, ex.ToString());
                    throw ex;
                }
            }
        }

        private void logError(DbContext dbc, string message)
        {
            var logger = new ErrorLogDao(dbc);
            logger.LogError(message);
        }

        private EmailMessage createMessage(string email, string password)
        {
            return new EmailMessage
            {
                FromAddress = "intune.userservice@gmail.com",
                FromAddressDisplayName = "Intune user services",
                FromPassword = StringCipher.Decrypt("zNElhP2H2K9AVdqSolXa5g==", "SynergyUserFeedbackMailPassword"),
                ToAddress = email,
                ToAddressDisplayName = "Intune",
                Subject = "Your Intune Password",
                Body = string.Format("Dear Intune User,\n\nYour login password is:\n{0}\n\nSincerely,\nIntune Team.\n", password)
            };
        }

        public User Login(User loginInfo)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new UserDao(dbc);
                var user = dao.ReadUserByEmail(loginInfo.Email);
                const string errorMsg = "Incorrect login credentials.";

                if (user == null)
                    throw new Exception(errorMsg);

                if (user.Password != loginInfo.Password)
                    throw new Exception(errorMsg);

                user.SessionToken = Guid.NewGuid().ToString();
                //TODO: 1.Create a session table
                //2. Store this sessionToken in the session table
                //3. Validate it with sub-sequent requests.
                //4. Subsequent requests should send the token in request header.
                //5. Subsequent requests should read the token from the request header 
                //   and verify it with value stored in the session table

                return user;
            }
        }

        public User ReadUserById(int userId)
        {
            using (DbContext dbc = new DbContext())
            {
                var dao = new UserDao(dbc);
                return dao.ReadUserById(userId);
            }
        }
    }
}
