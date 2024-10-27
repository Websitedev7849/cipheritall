// See https://aka.ms/new-console-template for more information
using UserAuth;
using System.Security.Authentication;
using Utils;


class Program
{

    
    static void Main(string[] args)
    {



        Utils.Configure.Credentials();

        Console.WriteLine("Welcome to CipherItAll - File Encryption - Decryption Tool");

        if (args.Length < 0)
        {
            Console.WriteLine("No argument was passed");
            return;
        }

        //Find the file whose location is at args[1]
        string fileName = args[1];

        Cipher cipher = null;



        // check the first arg
        if ("encrypt".Equals(args[0]))
        {
            
            try
            {
                //Console.WriteLine(filePath);
                CipherEncryptor encryptor = new Utils.CipherEncryptor(fileName);
                int encryptionKey = encryptor.EncryptFile();
                cipher = (Cipher) encryptor;

                Console.WriteLine();
                Console.WriteLine($"File Encrypted Successfully with Key: {encryptionKey}");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Error: Please specify the file path!!!");
                Console.ReadLine();
                return;
            }
            catch (FileNotFoundException e){
                Console.WriteLine(e.Message);
                Console.WriteLine($"Error: File not found at path: {fileName}");
                return;
            }

        }
        else if ("decrypt".Equals(args[0]))
        {

            try
            {
                CipherDecryptor decryptor = new Utils.CipherDecryptor(fileName);
                decryptor.DecryptFile();

                Console.WriteLine($"File Decrytped Successfully: {fileName} -> {fileName.Replace("cia", "decrypt.cia")}");
                
                cipher = (Cipher) decryptor;
            }
            catch (InvalidFileException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            catch (Utils.KeyNotFoundException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            

        }

        // Authenticate operation in the end
        try
        {
            Console.WriteLine();
            Console.WriteLine("Please enter the password to Commit Operation.");        
            MenuOptions.Operations.AuthenticateUser();

            // Commit the operation after execution
            cipher.Commit();
        }
        catch (UserNotInitializedException e)
        {
            Console.WriteLine("\nUser not Initialized!!!");
            MenuOptions.Operations.InitializeUser();

            // Commit the operation after execution
            cipher.Commit();
        }
        catch (InvalidCredentialException e)
        {
            Console.WriteLine("Invalid Credentials!!! Try Again...");
            Console.ReadLine();
        }



        

    }
}

