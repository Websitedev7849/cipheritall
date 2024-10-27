using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Authentication;
using System.Xml;
using System.Net;


namespace UserAuth
{
    
    public class UserCredentials
    {

        private static UserCredentials userCredentials = null;

        // The following properties will be read from local files (e.g., env file)
        private string registeredUserName;
        private string registeredPassword;

        private string userName;
        private string password;
        public static string currentDir;
        //private string currentDir = "";

        private UserCredentials()
        {
            XmlDocument creds = new XmlDocument();
            creds.Load(currentDir + "\\Credentials.xml");

            registeredUserName = creds.SelectSingleNode("/secret-info/credentials/username").InnerText.Trim();
            registeredPassword = creds.SelectSingleNode("/secret-info/credentials/password").InnerText.Trim();
            IsUserInitialized();
        }

        public String UserName
        {
            get { return userName; }
            set { this.userName = value; }
        }

        public String Password
        {
            get { return password; }
            set { this.password = value; }
        }

        public static String CurrentDir
        {
            set { currentDir = value; }
            get { return currentDir; }
        }

        public Dictionary<string, string> getDictionary()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("username", this.UserName);
            dict.Add("password", this.Password);
            return dict;
        }

        public static UserCredentials getInstance()
        {
            if (userCredentials == null) {
                userCredentials = new UserCredentials();
            }
            //userCredentials.IsUserInitialized();
            return userCredentials;
        }

        public bool IsUserInitialized()
        {
            if (string.IsNullOrEmpty(registeredUserName) || string.IsNullOrEmpty(registeredPassword))
            {
                throw new UserNotInitializedException();
            }
            else
            {
                return true;
            }
        }

        public bool VerifyCredentials()
        {
            if (!IsUserInitialized())
            {
                throw new UserNotInitializedException();
            }

            if (string.IsNullOrEmpty(registeredUserName) || string.IsNullOrEmpty(registeredPassword))
            {
                throw new UserNotInitializedException();
            }
            return this.password.Equals(this.registeredPassword);
        }
    }

    
    public class UserNotInitializedException : Exception
    {
        public UserNotInitializedException(): base("User Does not exists") { }

    }

    public class UserAuthentication
    {
        private UserCredentials userCredentials;

        public UserAuthentication() { 
            this.userCredentials = UserCredentials.getInstance();

        }

        public void PromptForCredentials()
        { 
            Console.Write("Please Enter Password To Proceed: ");
            userCredentials.Password = Console.ReadLine();
            Console.WriteLine();
        }

        public bool VerifyCredentials()
        {
            if (userCredentials.VerifyCredentials())
            {
                return true;
            }
            throw new InvalidCredentialException();
        }

        public static void registerUser(string username, string password)
        {
            string currentDir = UserCredentials.CurrentDir;
            //string currentDir = "";

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                throw new InvalidDataException();
            }

            XmlDocument creds = new XmlDocument();
            creds.Load(currentDir + "\\Credentials.xml");

            XmlNode registeredUserName = creds.SelectSingleNode("/secret-info/credentials/username");
            XmlNode registeredPassword = creds.SelectSingleNode("/secret-info/credentials/password");

            registeredUserName.InnerText = username;
            registeredPassword.InnerText = password;

            creds.Save(currentDir + "\\Credentials.xml");
        }
    }

    
}
