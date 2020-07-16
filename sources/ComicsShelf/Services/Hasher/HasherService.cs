using System;
using System.Security.Cryptography;
using System.Text;
using Xamarin.Forms;

namespace ComicsShelf.Hasher
{
   public class HasherService
   {

      public static void Init(string secret)
      {
         DependencyService.Register<HasherService>();
         DependencyService.Get<HasherService>()._Secret = secret;
      }

      public static string GetHashedString(string value) =>
         DependencyService.Get<HasherService>()._GetHashedString(value);

      string _Secret;
      string _GetHashedString(string value)
      {
         try
         {

            // VALIDATE
            if (string.IsNullOrEmpty(_Secret)) throw new ArgumentNullException("The secret argument must be set before the use of the hasher service");

            // INCOME BYTES
            byte[] valueBytes = Encoding.Unicode.GetBytes(value);
            byte[] saltBytes = Encoding.Unicode.GetBytes(_Secret);

            // SPICING MERGE
            byte[] completeBytes = new byte[valueBytes.Length + saltBytes.Length];
            for (int i = 0; i < valueBytes.Length; i++) completeBytes[i] = valueBytes[i];
            for (int i = 0; i < saltBytes.Length; i++) completeBytes[valueBytes.Length + i] = saltBytes[i];

            // HASH
            HashAlgorithm hash = new MD5CryptoServiceProvider();
            byte[] computedBytes = hash.ComputeHash(completeBytes);

            byte[] hashedBytes = new byte[computedBytes.Length + saltBytes.Length];
            for (int i = 0; i < computedBytes.Length; i++) hashedBytes[i] = computedBytes[i];
            for (int i = 0; i < saltBytes.Length; i++) hashedBytes[computedBytes.Length + i] = saltBytes[i];

            // CONVERT TO BASE 64
            var hashedString = Convert.ToBase64String(hashedBytes);

            // REMOVE SOME INVALID CHARAECTERS
            hashedString = hashedString
               .Replace("#", "")
               .Replace(".", "")
               .Replace("[", "")
               .Replace("]", "")
               .Replace("(", "")
               .Replace(")", "")
               .Replace(" ", "")
               .Replace("/", "_")
               .Replace("=", "_")
               .Replace("___", "_")
               .Replace("__", "_");

            // RESULT
            return hashedString;

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return null; }
      }

   }
}
