using Microsoft.VisualBasic.CompilerServices;
using System;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace GenieClient
{
    public class Crypto
    {
        private const int KEYSIZE_AES = 128;

        public enum Algorithm : int
        {
            SHA256 = 0,
            SHA512 = 1,
            AES = 2,
        }

        public enum EncodingType : int
        {
            HEX = 0,
            BASE_64 = 1
        }

        // Initialization Vectors that we will use for symmetric encryption/decryption. These
        // byte arrays are completely arbitrary, and you can change them to whatever you like
        private static readonly byte[] IV_8 = [2, 63, 9, 36, 235, 174, 78, 12];
        private static readonly byte[] IV_16 = [15, 199, 56, 77, 244, 126, 107, 239, 9, 10, 88, 72, 24, 202, 31, 108];
        private static readonly byte[] IV_24 = [37, 28, 19, 44, 25, 170, 122, 25, 25, 57, 127, 5, 22, 1, 66, 65, 14, 155, 224, 64, 9, 77, 18, 251];
        private static readonly byte[] IV_32 = [133, 206, 56, 64, 110, 158, 132, 22, 99, 190, 35, 129, 101, 49, 204, 248, 251, 243, 13, 194, 160, 195, 89, 152, 149, 227, 245, 5, 218, 86, 161, 124];

        // Salt value used to encrypt a plain text key. Again, this can be whatever you like
        private static readonly byte[] SALT_BYTES = [162, 27, 98, 1, 28, 239, 64, 30, 156, 102, 223];

        // Error messages
        private const string ERR_NO_KEY = "No encryption key was provided";
        private const string ERR_NO_ALGORITHM = "No algorithm was specified";
        private const string ERR_NO_CONTENT = "No content was provided";
        private const string ERR_INVALID_PROVIDER = "An invalid cryptographic provider was specified for this method";
        private const string ERR_NO_FILE = "The specified file does not exist";
        private const string ERR_FILE_WRITE = "Could not create file";
        private const string ERR_FILE_READ = "Could not read file";

        // Initialization variables
        private static string _key;
        private static Algorithm _algorithm = (Algorithm)(-1);
        private static string _content;
        private static CryptographicException _exception;
        private static EncodingType _encodingType = EncodingType.HEX;

        [Description("The key that is used to encrypt and decrypt data")]
        public static string Key
        {
            get
            {
                return _key;
            }

            set
            {
                _key = value;
            }
        }

        [Description("The algorithm that will be used for encryption and decryption")]
        public static Algorithm EncryptionAlgorithm
        {
            get
            {
                return _algorithm;
            }

            set
            {
                _algorithm = value;
            }
        }

        [Description("The format in which content is returned after encryption, or provided for decryption")]
        public static EncodingType Encoding
        {
            get
            {
                return _encodingType;
            }

            set
            {
                _encodingType = value;
            }
        }

        [Description("Encrypted content to be retrieved after an encryption call, or provided for a decryption call")]
        public static string Content
        {
            get
            {
                return _content;
            }

            set
            {
                _content = value;
            }
        }

        [Description("If an encryption or decryption call returns false, then this will contain the exception")]
        public static CryptographicException CryptoException
        {
            get
            {
                return _exception;
            }
        }

        [Description("Determines whether the currently specified algorithm is a hash")]
        public static bool IsHashAlgorithm
        {
            get
            {
                var switchExpr = _algorithm;
                switch (switchExpr)
                {
                    case Algorithm.SHA256:
                    case Algorithm.SHA512:
                        {
                            return true;
                        }

                    default:
                        {
                            return false;
                        }
                }
            }
        }

        [Description("Encryption of a string using the 'Key' and 'EncryptionAlgorithm' properties")]
        public static bool EncryptString(string Content)
        {
            byte[] cipherBytes;
            try
            {
                cipherBytes = Encrypt(Content);
            }
            catch (CryptographicException ex)
            {
                _exception = new CryptographicException(ex.Message, ex.InnerException);
                return false;
            }

            if (_encodingType == EncodingType.HEX)
            {
                _content = BytesToHex(cipherBytes);
            }
            else
            {
                _content = Convert.ToBase64String(cipherBytes);
            }

            return true;
        }

        public static bool DecryptString()
        {
            byte[] clearText;
            try
            {
                clearText = Decrypt(_content);
            }
            catch (Exception ex)
            {
                _exception = new CryptographicException(ex.Message, ex.InnerException);
                return false;
            }

            _content = System.Text.Encoding.UTF8.GetString(clearText);
            return true;
        }

        public static bool EncryptFile(string Filename, string Target)
        {
            if (!File.Exists(Filename))
            {
                _exception = new CryptographicException(ERR_NO_FILE);
                return false;
            }

            // Make sure the target file can be written
            try
            {
                var fs = File.Create(Target);
                fs.Close();
                fs.Dispose();
                File.Delete(Target);
            }
            catch (Exception)
            {
                _exception = new CryptographicException(ERR_FILE_WRITE);
                return false;
            }

            byte[] inStream;
            byte[] cipherBytes;
            try
            {
                inStream = File.ReadAllBytes(Filename);
            }
            catch (Exception)
            {
                _exception = new CryptographicException(ERR_FILE_READ);
                return false;
            }

            try
            {
                cipherBytes = Encrypt(inStream);
            }
            catch (CryptographicException ex)
            {
                _exception = ex;
                return false;
            }

            string encodedString;
            if (_encodingType == EncodingType.BASE_64)
            {
                encodedString = Convert.ToBase64String(cipherBytes);
            }
            else
            {
                encodedString = BytesToHex(cipherBytes);
            }

            var encodedBytes = System.Text.Encoding.UTF8.GetBytes(encodedString);

            // Create the encrypted file
            var outStream = File.Create(Target);
            outStream.Write(encodedBytes, 0, encodedBytes.Length);
            outStream.Close();
            outStream.Dispose();
            return true;
        }

        public static bool DecryptFile(string Filename, string Target)
        {
            if (!File.Exists(Filename))
            {
                _exception = new CryptographicException(ERR_NO_FILE);
                return false;
            }

            // Make sure the target file can be written
            try
            {
                var fs = File.Create(Target);
                fs.Close();
                fs.Dispose();
                File.Delete(Target);
            }
            catch (Exception)
            {
                _exception = new CryptographicException(ERR_FILE_WRITE);
                return false;
            }

            byte[] inStream;
            byte[] clearBytes;
            try
            {
                inStream = File.ReadAllBytes(Filename);
            }
            catch (Exception)
            {
                _exception = new CryptographicException(ERR_FILE_READ);
                return false;
            }

            try
            {
                clearBytes = Decrypt(inStream);
            }
            catch (Exception ex)
            {
                _exception = new CryptographicException(ex.Message, ex.InnerException);
                return false;
            }

            // Create the decrypted file
            var outStream = File.Create(Target);
            outStream.Write(clearBytes, 0, clearBytes.Length);
            outStream.Close();
            outStream.Dispose();
            return true;
        }

        public static bool GenerateHash(string Content)
        {
            if (Content is null || Content.Equals(string.Empty))
            {
                _exception = new CryptographicException(ERR_NO_CONTENT);
                return false;
            }

            if (_algorithm.Equals(-1))
            {
                _exception = new CryptographicException(ERR_NO_ALGORITHM);
                return false;
            }

            HashAlgorithm hashAlgorithm = null;
            var switchExpr = _algorithm;
            switch (switchExpr)
            {
                case Algorithm.SHA256:
                    {
                        hashAlgorithm = SHA256.Create();
                        break;
                    }

                case Algorithm.SHA512:
                    {
                        hashAlgorithm = SHA512.Create();
                        break;
                    }

                default:
                    {
                        _exception = new CryptographicException(ERR_INVALID_PROVIDER);
                        break;
                    }
            }

            try
            {
                var hash = ComputeHash(hashAlgorithm, Content);
                if (_encodingType == EncodingType.HEX)
                {
                    _content = BytesToHex(hash);
                }
                else
                {
                    _content = Convert.ToBase64String(hash);
                }

                hashAlgorithm.Clear();
                return true;
            }
            catch (CryptographicException ex)
            {
                _exception = ex;
                return false;
            }
            finally
            {
                hashAlgorithm.Clear();
            }
        }

        public static void Clear()
        {
            _algorithm = (Algorithm)(-1);
            _content = string.Empty;
            _key = string.Empty;
            _encodingType = EncodingType.HEX;
            _exception = null;
        }

        private static byte[] Encrypt(byte[] Content)
        {
            if (!IsHashAlgorithm && _key is null)
            {
                throw new CryptographicException(ERR_NO_KEY);
            }

            if (_algorithm.Equals(-1))
            {
                throw new CryptographicException(ERR_NO_ALGORITHM);
            }

            if (Content is null || Content.Equals(string.Empty))
            {
                throw new CryptographicException(ERR_NO_CONTENT);
            }

            byte[] cipherBytes = null;
            int NumBytes;
            SymmetricAlgorithm provider;
            var switchExpr = _algorithm;
            switch (switchExpr)
            {
                case Algorithm.AES:
                    {
                        provider = Aes.Create();
                        NumBytes = KEYSIZE_AES;
                        break;
                    }

                default:
                    {
                        throw new CryptographicException(ERR_INVALID_PROVIDER);
                    }
            }

            try
            {
                // Encrypt the string
                cipherBytes = SymmetricEncrypt(provider, Content, _key, NumBytes);
            }
            catch (CryptographicException ex)
            {
                throw new CryptographicException(ex.Message, ex.InnerException);
            }
            finally
            {
                // Free any resources held by the SymmetricAlgorithm provider
                provider.Clear();
            }

            return cipherBytes;
        }

        private static byte[] Encrypt(string Content)
        {
            return Encrypt(System.Text.Encoding.UTF8.GetBytes(Content));
        }

        private static byte[] Decrypt(byte[] Content)
        {
            if (!IsHashAlgorithm && _key is null)
            {
                throw new CryptographicException(ERR_NO_KEY);
            }

            if (_algorithm.Equals(-1))
            {
                throw new CryptographicException(ERR_NO_ALGORITHM);
            }

            if (Content is null || Content.Length.Equals(0))
            {
                throw new CryptographicException(ERR_NO_CONTENT);
            }

            string encText = System.Text.Encoding.UTF8.GetString(Content);
            if (_encodingType == EncodingType.BASE_64)
            {
                // We need to convert the content to Hex before decryption
                encText = BytesToHex(Convert.FromBase64String(encText));
            }

            byte[] clearBytes = null;
            int NumBytes;
            SymmetricAlgorithm provider;
            var switchExpr = _algorithm;
            switch (switchExpr)
            {
                case Algorithm.AES:
                    {
                        provider = Aes.Create();
                        NumBytes = KEYSIZE_AES;
                        break;
                    }

                default:
                    {
                        throw new CryptographicException(ERR_INVALID_PROVIDER);
                    }
            }

            try
            {
                clearBytes = SymmetricDecrypt(provider, encText, _key, NumBytes);
            }
            catch (CryptographicException)
            {
                throw;
            }
            finally
            {
                // Free any resources held by the SymmetricAlgorithm provider
                provider.Clear();
            }

            // Now return the plain text content
            return clearBytes;
        }

        private static byte[] Decrypt(string Content)
        {
            return Decrypt(System.Text.Encoding.UTF8.GetBytes(Content));
        }

        private static byte[] ComputeHash(HashAlgorithm Provider, string plainText)
        {
            // All hashing mechanisms inherit from the HashAlgorithm base class so we can use that to cast the crypto service provider
            var hash = Provider.ComputeHash(System.Text.Encoding.UTF8.GetBytes(plainText));
            Provider.Clear();
            return hash;
        }

        private static byte[] SymmetricEncrypt(SymmetricAlgorithm Provider, byte[] plainText, string key, int keySize)
        {
            // All symmetric algorithms inherit from the SymmetricAlgorithm base class, to which we can cast from the original crypto service provider
            byte[] ivBytes = null;
            var switchExpr = keySize / (double)8; // Determine which initialization vector to use
            switch (switchExpr)
            {
                case 8:
                    {
                        ivBytes = IV_8;
                        break;
                    }

                case 16:
                    {
                        ivBytes = IV_16;
                        break;
                    }

                case 24:
                    {
                        ivBytes = IV_24;
                        break;
                    }

                case 32:
                    {
                        ivBytes = IV_32;
                        break;
                    }

                default:
                    {
                        break;
                    }
                    // TODO: Throw an error because an invalid key length has been passed
            }

            Provider.KeySize = keySize;

            // Generate a secure key based on the original password by using SALT
            var keyStream = DerivePassword(key, (int)(keySize / (double)8));

            // Initialize our encryptor object
            var trans = Provider.CreateEncryptor(keyStream, ivBytes);

            // Perform the encryption on the textStream byte array
            var result = trans.TransformFinalBlock(plainText, 0, plainText.GetLength(0));

            // Release cryptographic resources
            Provider.Clear();
            trans.Dispose();
            return result;
        }

        private static byte[] SymmetricDecrypt(SymmetricAlgorithm Provider, string encText, string key, int keySize)
        {
            // All symmetric algorithms inherit from the SymmetricAlgorithm base class, to which we can cast from the original crypto service provider
            byte[] ivBytes = null;
            var switchExpr = keySize / (double)8; // Determine which initialization vector to use
            switch (switchExpr)
            {
                case 8:
                    {
                        ivBytes = IV_8;
                        break;
                    }

                case 16:
                    {
                        ivBytes = IV_16;
                        break;
                    }

                case 24:
                    {
                        ivBytes = IV_24;
                        break;
                    }

                case 32:
                    {
                        ivBytes = IV_32;
                        break;
                    }

                default:
                    {
                        break;
                    }
                    // TODO: Throw an error because an invalid key length has been passed
            }

            // Generate a secure key based on the original password by using SALT
            var keyStream = DerivePassword(key, (int)(keySize / (double)8));

            // Convert our hex-encoded cipher text to a byte array
            var textStream = HexToBytes(encText);
            Provider.KeySize = keySize;

            // Initialize our decryptor object
            var trans = Provider.CreateDecryptor(keyStream, ivBytes);

            // Initialize the result stream
            byte[] result = null;
            try
            {
                // Perform the decryption on the textStream byte array
                result = trans.TransformFinalBlock(textStream, 0, textStream.GetLength(0));
            }
            catch (Exception ex)
            {
                throw new CryptographicException("The following exception occurred during decryption: " + ex.Message);
            }
            finally
            {
                // Release cryptographic resources
                Provider.Clear();
                trans.Dispose();
            }

            return result;
        }

        // Converts a byte array to a hex-encoded string
        private static string BytesToHex(byte[] bytes)
        {
            var hex = new StringBuilder();
            for (int n = 0, loopTo = bytes.Length - 1; n <= loopTo; n++)
                hex.AppendFormat("{0:X2}", bytes[n]);
            return hex.ToString();
        }

        // Converts a hex-encoded string to a byte array
        private static byte[] HexToBytes(string Hex)
        {
            int numBytes = (int)(Hex.Length / (double)2);
            var bytes = new byte[numBytes];
            for (int n = 0, loopTo = numBytes - 1; n <= loopTo; n++)
            {
                string hexByte = Hex.Substring(n * 2, 2);
                bytes[n] = Conversions.ToByte(int.Parse(hexByte, System.Globalization.NumberStyles.HexNumber));
            }

            return bytes;
        }

        // This takes the original plain text key and creates a secure key using SALT
        private static byte[] DerivePassword(string originalPassword, int passwordLength)
        {
            return Rfc2898DeriveBytes.Pbkdf2(originalPassword, SALT_BYTES, 5, HashAlgorithmName.SHA1, passwordLength);
        }
    }
}
