using Gabriel.Cat.S.Extension;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JDownloaderClickAndGo
{
    public delegate (string Key, string DataEncrypted) GetKeyAndDataFromHtmlDelegate([NotNull] string htmlWeb);
    public static class JDownloadClickNLoad
    {


        public static string Decrypt([NotNull] string encryptedString, [NotNull] string encryptionKey)
        {
            if (encryptedString.Length == 0)
                throw new ArgumentException(nameof(encryptedString));
            if (encryptionKey.Length == 0)
                throw new ArgumentException(nameof(encryptionKey));

            byte[] ourEnc;
            string ourDec;
            RijndaelManaged myRijndael = new RijndaelManaged();
            try
            {

                myRijndael.Key = HexStringToByte(encryptionKey);
                myRijndael.IV = myRijndael.Key;
                myRijndael.Mode = CipherMode.CBC;
                myRijndael.Padding = PaddingMode.None;

                ourEnc = Convert.FromBase64String(encryptedString);
                ourDec = DecryptStringFromBytes(ourEnc, myRijndael);
            }
            finally
            {
                myRijndael.Dispose();
            }
            return ourDec;

        }


        public static string Encrypt([NotNull] string plainText, [NotNull] string encryptionKey)
        {
            if (plainText.Length == 0)
                throw new ArgumentException(nameof(plainText));
            if (encryptionKey.Length == 0)
                throw new ArgumentException(nameof(encryptionKey));

            byte[] encrypted;
            string encString;
            RijndaelManaged myRijndael = new RijndaelManaged();

            try
            {
                myRijndael.Key = HexStringToByte(encryptionKey);
                myRijndael.IV = myRijndael.Key;
                myRijndael.Mode = CipherMode.CBC;
                myRijndael.Padding = PaddingMode.None;

                encrypted = EncryptStringToBytes(plainText, myRijndael);
                encString = Convert.ToBase64String(encrypted);
            }
            finally
            {
                myRijndael.Dispose();
            }
            return encString;

        }


        private static byte[] EncryptStringToBytes([NotNull] string plainText, [NotNull] RijndaelManaged rijAlg)
        {
            // Check arguments. 
            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentException(nameof(plainText));
            if (rijAlg.Key == null || rijAlg.Key.Length <= 0)
                throw new ArgumentNullException(nameof(rijAlg.Key));
            if (rijAlg.IV == null || rijAlg.IV.Length <= 0)
                throw new ArgumentNullException(nameof(rijAlg.IV));
            byte[] encrypted;
            // Create an RijndaelManaged object 
            // with the specified key and IV. 


            // Create a decrytor to perform the stream transform.
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



            // Return the encrypted bytes from the memory stream. 
            return encrypted;

        }

        private static string DecryptStringFromBytes([NotNull] byte[] cipherText, [NotNull] RijndaelManaged rijAlg)
        {
            // Check arguments. 
            if (cipherText.Length == 0)
                throw new ArgumentException(nameof(cipherText));
            if (rijAlg.Key == null || rijAlg.Key.Length <= 0)
                throw new ArgumentNullException(nameof(rijAlg.Key));
            if (rijAlg.IV == null || rijAlg.IV.Length <= 0)
                throw new ArgumentNullException(nameof(rijAlg.IV));

            // Declare the string used to hold 
            // the decrypted text. 
            string plaintext = default;

            // Create an RijndaelManaged object 
            // with the specified key and IV. 


            // Create a decrytor to perform the stream transform.
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



            return plaintext;

        }

        public static (string Key, string IV) GenerateKeyAndIV()
        {
            // This code is only here for an example
            RijndaelManaged myRijndaelManaged = new RijndaelManaged();
            myRijndaelManaged.Mode = CipherMode.CBC;
            myRijndaelManaged.Padding = PaddingMode.PKCS7;

            myRijndaelManaged.GenerateIV();
            myRijndaelManaged.GenerateKey();
            return (Key: ByteArrayToHexString(myRijndaelManaged.Key), IV: ByteArrayToHexString(myRijndaelManaged.IV));
        }

        public static byte[] HexStringToByte([NotNull] string hexString)
        {
            int bytesCount;
            byte[] bytes;
            try
            {
                bytesCount = (hexString.Length) / 2;
                bytes = new byte[bytesCount];
                for (int x = 0; x < bytesCount; ++x)
                {
                    bytes[x] = Convert.ToByte(hexString.Substring(x * 2, 2), 16);
                }

            }
            catch
            {
                throw;
            }
            return bytes;
        }

        public static string ByteArrayToHexString([NotNull] byte[] byteArrayToHex)
        {
            StringBuilder hex = new StringBuilder(byteArrayToHex.Length * 2);
            for (int i = 0; i < byteArrayToHex.Length; i++)
                hex.AppendFormat("{0:x2}", byteArrayToHex[i]);
            return hex.ToString();
        }

        public static async Task<string[]> DecryptUri([NotNull] this Uri uriWeb, GetKeyAndDataFromHtmlDelegate methodGetKeyAndData=default)
        {
            if (Equals(methodGetKeyAndData, default))
            {
                methodGetKeyAndData= (html) =>
                {
                    string data;
                    Regex regex = new Regex("(<INPUT[^>]*>)");
                    Match match = regex.Match(html).NextMatch();//quito el primero que es el source
                    string key = match.Value.Split('\'')[1];
                    match = match.NextMatch();
                    data = match.Value.Split("VALUE=\"")[1].Replace("\">", "");
                    return (key, data);
                };
            }
            (string key, string data) = methodGetKeyAndData(await uriWeb.DownloadString());
            string dataDecrypted = Decrypt(data, key);
            return dataDecrypted.Contains('\n')? dataDecrypted.Split('\n'):new string[] {dataDecrypted};
        }
    }
}