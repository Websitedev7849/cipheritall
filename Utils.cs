using UserAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml;

namespace MenuOptions
{
    class Operations
    {
        public static void AuthenticateUser()
        {
            UserAuthentication userAuthentication;

            try
            {
                userAuthentication = new UserAuthentication();
            }
            catch (UserNotInitializedException e)
            {

                throw new UserNotInitializedException();
            }
            
            userAuthentication.PromptForCredentials();
            bool isUserVerified = userAuthentication.VerifyCredentials();

            if (isUserVerified)
            {
                Console.WriteLine("User Logged in");
            }
            
        }

        public static void InitializeUser()
        {

            while (true) {
                Console.WriteLine("\nYou are logging in for first time ...");
                Console.WriteLine("Please register to continue...");

                Console.WriteLine();
                Console.Write("Continue to register? y/n: ");
                string choice = Console.ReadLine();
                if (choice.ToLower().Equals("n"))
                {
                    System.Environment.Exit(1);
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.Write("Enter Username: ");
                string username = Console.ReadLine();
                
                Console.Write("Enter Password: ");
                string password = Console.ReadLine();

                Console.Write("Confirm Password: ");
                string confirmPassword = Console.ReadLine();

                if (!password.Equals(confirmPassword))
                {
                    Console.WriteLine("Passwords does not match!!! Try Again.");
                    Console.Read();
                    continue;
                }

                if (string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Please enter valid password!!! Try Again.");
                    Console.Read();
                    continue;
                }

                UserAuthentication.registerUser(username, password);
                break;
            }



        }

        
    }
}

namespace Utils
{
    class Configure
    {
        public static void Credentials() {
            //string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CipherItAll"); 
            string path = "D:\\CipherItAll";
            UserCredentials.CurrentDir = path;

            Console.WriteLine(path);

            if (!Directory.Exists(path))
            {
                // Create the directory
                Directory.CreateDirectory(path);
                //Console.WriteLine($"Directory created at: {path}");
            }
            else
            {
                //Console.WriteLine($"Directory already exists at: {path}");
            }

            string filePath = path + "\\Credentials.xml";
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // Create the file
                using (FileStream fs = File.Create(filePath))
                {
                    // Optional: Write some initial content to the file
                    using (StreamWriter writer = new StreamWriter(fs))
                    {
                        writer.WriteLine(
                            "<secret-info>\r\n" +
                            "  <credentials>\r\n" +
                            "    <username></username>\r\n" +
                            "    <password></password>\r\n" +
                            "  </credentials>\r\n" +
                            "  <vault>\r\n" +
                            "    <file last-known-path=\"D:/My Secrets/\">\r\n" +
                            "      <name>data.cia.xslx</name>\r\n" +
                            "      <key>9797b135a4a0c68ef173</key>\r\n" +
                            "    </file>\r\n" +
                            "  </vault>\r\n" +
                            "</secret-info>"
                            );
                    }
                }
                //Console.WriteLine($"File created at: {filePath}");
            }
            else
            {
                //Console.WriteLine($"File Already Exists at: {filePath}");
            }

        }
    }

    interface Cipher
    {
        protected static string CredentialsPath = "D:\\CipherItAll\\Credentials.xml";
        /* Commits an encryption or decryption operation to .cia file or from .cia file Respectively*/
        void Commit();
    }

    class CipherEncryptor: Cipher
    {

        string fileName;
        private byte[] encryptedBuffer;
        private int decryptionKey;


        public CipherEncryptor(string fileName) {

            if (string.IsNullOrEmpty(fileName))
            {

                throw new ArgumentException();
            }

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Here FilePath: {fileName}");
                throw new FileNotFoundException();
            }

            this.fileName = fileName;
        }

        public int EncryptFile()
        {

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fs.Length];


            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            int KEY = (int) new Random().NextInt64(1, 128);

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(buffer[i] + KEY);
            }

            this.encryptedBuffer = buffer;
            this.decryptionKey = KEY;

            return KEY;
        }

        public void Commit()
        {
            // create filename with cia extension
            // e.g Testing.cia.txt or Testing.cia.pdf
            string[] splitFileName = fileName.Split(".");
            string encryptedFileName = "";
            for (int i = 0; i < splitFileName.Length; i++)
            {
                if (i == splitFileName.Length - 1)
                {
                    encryptedFileName += ("cia." + splitFileName[i]);
                    continue;
                }
                encryptedFileName += (splitFileName[i] + ".");
            }


            string encryptedFilePath = Directory.GetCurrentDirectory();
            encryptedFilePath += "\\" + encryptedFileName;


            // save the file name and key in vault
            XmlDocument credsXml = new XmlDocument();
            credsXml.Load(Cipher.CredentialsPath);

            XmlNode vaultNode = credsXml.SelectSingleNode("/secret-info/vault");

            XmlElement fileNode = credsXml.CreateElement("file");

            XmlElement nameNode = credsXml.CreateElement("name");
            nameNode.InnerText = encryptedFileName;
            fileNode.AppendChild(nameNode);

            XmlElement keyNode = credsXml.CreateElement("key");
            keyNode.InnerText = this.decryptionKey + "";
            fileNode.AppendChild(keyNode);

            vaultNode.AppendChild(fileNode);


            // create a .cia extension file
            File.Create(encryptedFilePath).Close();

            FileStream fs = new FileStream(encryptedFilePath, FileMode.Open, FileAccess.Write);
            fs.Write(encryptedBuffer, 0, encryptedBuffer.Length);
            fs.Close();

            // commit the XML
            credsXml.Save(Cipher.CredentialsPath);
        }
    }

    class CipherDecryptor : Cipher
    {

        string fileName;
        byte[] decryptedBuffer;
        private XmlNode fileNode;
        private XmlDocument creds;

        public CipherDecryptor(string fileName)
        {
            string[] fileNameSplit = fileName.Split('.');
            if (!string.Equals(fileNameSplit[fileNameSplit.Length - 2], "cia"))
            {
                throw new InvalidFileException("Error: File is not .cia extension file");
            }

            if (string.IsNullOrEmpty(fileName))
            {

                throw new ArgumentException();
            }

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"Here FilePath: {fileName}");
                throw new FileNotFoundException();
            }

            this.fileName = fileName;
        }


        public void DecryptFile()
        {
            //Find if file is in vault
            int decryptionKey = this.findKey();

            if (decryptionKey == -1)
            {
                throw new KeyNotFoundException("Error: Decryption Key is not present in the vault.");
            }

            FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[fs.Length];

            fs.Read(buffer, 0, buffer.Length);
            fs.Close();

            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = (byte)(buffer[i] - decryptionKey);
            }

            fs.Close();

            
            this.decryptedBuffer = buffer;

        }

        public void Commit()
        {
            // create filename with .decrypt.cia extension
            // e.g Testing.cia.txt or Testing.decrypt.cia.pdf
            string decryptedFileName = fileName.Replace("cia", "decrypt.cia");


            string decryptedFilePath = Directory.GetCurrentDirectory();
            decryptedFilePath += "\\" + decryptedFileName;

            // create a .cia extension file
            File.Create(decryptedFilePath).Close();

            FileStream fs = new FileStream(decryptedFilePath, FileMode.Open, FileAccess.Write);
            fs.Write(decryptedBuffer, 0, decryptedBuffer.Length);
            fs.Close();

            this.fileNode.ChildNodes[0].InnerText = "";
            this.fileNode.ChildNodes[1].InnerText = "";

            this.creds.Save(Cipher.CredentialsPath);

        }

        private int findKey()
        {
            //Find if file is in vault
            XmlDocument creds = new XmlDocument();
            creds.Load(Cipher.CredentialsPath);

            this.creds = creds;

            XmlNodeList fileNodes = creds.SelectNodes("/secret-info/vault/file");

            foreach (XmlNode file in fileNodes)
            {
                XmlNode nameNode = file.ChildNodes[0];
                XmlNode keyNode = file.ChildNodes[1];

                if (string.Equals(nameNode.InnerText, fileName))
                {
                    this.fileNode = file;
                    return int.Parse(keyNode.InnerText);
                }
            }

            return -1;
        }

        
    }

    public class InvalidFileException : Exception
    {
        public InvalidFileException() : base() { }

        public InvalidFileException(string message) : base(message) { }

        public InvalidFileException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class KeyNotFoundException: Exception
    {
        public KeyNotFoundException() : base() {        }
        public KeyNotFoundException(string message) : base(message) { }
    }
}