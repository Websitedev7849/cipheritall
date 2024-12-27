# CipherItAll

**CipherItAll** is a command-line-based C# application designed to encrypt and decrypt files of any type or extension. The tool ensures secure file handling through unique encryption keys and an authentication mechanism for added protection.

---

## Features

### 1. File Encryption & Decryption

- **Encrypt any file**: CipherItAll supports encryption of files regardless of their type or extension.
- **Decrypt encrypted files**: Reverse the encryption process using the corresponding key.

### 2. Encryption Key Management

- **Key generation**: Generates a unique encryption key for each file.
- **Vault storage**: Securely stores encryption keys in an XML-based vault for later use during decryption.

### 3. User Authentication

- **First-time user initialization**: Guides new users to register with a username and password.
- **Credential verification**: Requires users to authenticate before committing encryption or decryption operations.
- **Credential storage**: Stores user credentials securely in an XML-based configuration file.

---

## Usage

### Command Structure

Run the application using the following structure:

```
CipherItAll <operation> <file-path>
```

**Parameters**:

- `<operation>`: Specify `encrypt` or `decrypt`.
- `<file-path>`: Path to the file to encrypt or decrypt.

### Example Commands

1. **Encrypt a file**:

   ```
   CipherItAll encrypt path/to/your/file.txt
   ```

   - Outputs the encrypted file and the encryption key.

2. **Decrypt a file**:

   ```
   CipherItAll decrypt path/to/your/encryptedfile.cia.txt
   ```

   - Outputs the decrypted file.

---

## Workflow

1. **Initialize Credentials**

   - On first run, the application will guide you to create a username and password.

2. **Encrypt a File**

   - Provide the file path and run the `encrypt` operation.
   - The encryption key will be stored in the vault.

3. **Decrypt a File**

   - Provide the path to the encrypted file and run the `decrypt` operation.
   - The application retrieves the encryption key from the vault.

4. **Authentication**

   - Each operation requires user authentication to proceed.

---

## Project Structure

### Key Components

- **Program.cs**: Entry point for the application; handles command-line arguments and initiates operations.
- **UserAuthentication.cs**: Manages user registration, credential storage, and authentication.
- **Utils.cs**: Contains utility classes for file encryption, decryption, and key management.

### Key Classes

- **`CipherEncryptor`**: Handles file encryption.
- **`CipherDecryptor`**: Handles file decryption.
- **`UserAuthentication`**: Handles user login and credential verification.
- **`Configure`**: Sets up the environment, including directories and configuration files.

---

## Setup

### Prerequisites

- .NET SDK
- Windows OS (recommended for path configurations)

### Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   ```
2. Navigate to the project directory:
   ```bash
   cd CipherItAll
   ```
3. Build the project:
   ```bash
   dotnet build
   ```

---

## Security Considerations

- Ensure the `Credentials.xml` file is stored in a secure location.
- Do not share your encryption keys or vault file.

---

## Future Enhancements

- Support for additional encryption algorithms.
- Multi-user support with role-based access control.
- Improved UI/UX for command-line prompts.

---

## License

This project is licensed under the MIT License. See the LICENSE file for details.

---

## Contributions

Contributions are welcome! Please fork the repository and submit a pull request for review.

---


