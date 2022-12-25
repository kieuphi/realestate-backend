using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Shared.Services
{
    public interface IEnCodeTwoWaysService
    {
        Task<string> EncryptTwoWays(string toEncrypt);
        Task<string> DeEncryptTwoWays(string cipherString);
    }

    public class EnCodeTwoWaysService : IEnCodeTwoWaysService
    {
        //public async Task<string> EncryptTwoWays(string toEncrypt)
        //{
        //    byte[] keyArray;
        //    byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt.ToString());

        //    // Get the key from config file
        //    string key = "grexencode";

        //    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        //    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
        //    hashmd5.Clear();

        //    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        //    tdes.Key = keyArray;
        //    tdes.Mode = CipherMode.ECB;
        //    tdes.Padding = PaddingMode.PKCS7;

        //    ICryptoTransform cTransform = tdes.CreateEncryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
        //    tdes.Clear();

        //    return await Task.FromResult(Convert.ToBase64String(resultArray, 0, resultArray.Length));
        //}

        //public async Task<string> DeEncryptTwoWays(string cipherString)
        //{
        //    byte[] keyArray;
        //    byte[] toEncryptArray = Convert.FromBase64String(cipherString);

        //    //Get your key from config file to open the lock!
        //    string key = "grexencode";
        //    MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
        //    keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
        //    hashmd5.Clear();


        //    TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
        //    tdes.Key = keyArray;
        //    tdes.Mode = CipherMode.ECB;
        //    tdes.Padding = PaddingMode.PKCS7;

        //    ICryptoTransform cTransform = tdes.CreateDecryptor();
        //    byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

        //    tdes.Clear();
        //    return await Task.FromResult(UTF8Encoding.UTF8.GetString(resultArray));
        //}

        static byte[] ownKey = { 118, 123, 23, 17, 161, 152, 35, 68, 126, 213, 16, 115, 68, 217, 58, 108, 56, 218, 5, 78, 28, 128, 113, 208, 61, 56, 10, 87, 187, 162, 233, 38 };
        static byte[] ownIV = { 33, 241, 14, 16, 103, 18, 14, 248, 4, 54, 18, 5, 60, 76, 16, 191 };

        private static string RemoveDashes(string str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                if ((str[i] >= '0' && str[i] <= '9')
                    || (str[i] >= 'A' && str[i] <= 'z'
                        || (str[i] == '.' || str[i] == '_')))
                {
                    sb.Append(str[i]);
                }
            }
            return sb.ToString();
        }

        private static string AddDashes(string str)
        {
            int length = str.Length;
            int j = 0;
            for (int i = 2; i < length; i = i + 2)
            {
                str = str.Insert(j + i, "-");
                j++;
            }
            return str;
        }

        public async Task<string> EncryptTwoWays(string toEncrypt)
        {
            string encryptResult;
            using (Rijndael myRijndael = Rijndael.Create())
            {
                byte[] encrypted = Rijndael_Crypto.EncryptStringToBytes(toEncrypt, ownKey, ownIV);
                string stringByte = BitConverter.ToString(encrypted);

                encryptResult = RemoveDashes(stringByte).ToLower();

                return await Task.FromResult(encryptResult);
            }
        }

        public async Task<string> DeEncryptTwoWays(string cipherString)
        {
            string decryptKeyToByte;
            using (Rijndael myRijndael = Rijndael.Create())
            {
                decryptKeyToByte = AddDashes(RemoveDashes(cipherString).ToLower()).ToUpper();

                byte[] data = Array.ConvertAll<string, byte>(decryptKeyToByte.Split('-'), s => Convert.ToByte(s, 16));

                string decryptResult = Rijndael_Crypto.DecryptStringFromBytes(data, ownKey, ownIV);

                return await Task.FromResult(decryptResult);
            }
        }
    }

    public abstract class Rijndael_Abstract : SymmetricAlgorithm
    {
    }

    // Rijndael
    public class Rijndael_Crypto
    {
        public static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {

                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
