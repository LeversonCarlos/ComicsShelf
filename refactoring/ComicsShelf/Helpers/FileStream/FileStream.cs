using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers
{
   internal class FileStream
   {
      static SemaphoreSlim fileStreamSemaphore = new SemaphoreSlim(1, 1);

      public static T Deserialize<T>(byte[] value) where T : class
      {
         try
         {
            var serializedContent = System.Text.Encoding.Unicode.GetString(value);
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(serializedContent);
            return result;
         }
         catch (Exception) { throw; }
      }

      public static T Deserialize<T>(string value) where T : class
      {
         try
         {
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
            return result;
         }
         catch (Exception) { throw; }
      }

      public static async Task<T> ReadFile<T>(string path) where T : class
      {
         try
         {
            await fileStreamSemaphore.WaitAsync();
            if (!System.IO.File.Exists(path)) { return null; }

            string serializedContent = string.Empty;
            using (var sourceStream = new System.IO.FileStream(path,
               FileMode.Open, FileAccess.Read, FileShare.Read,
               bufferSize: 4096, useAsync: true))
            {
               StringBuilder sb = new StringBuilder();
               byte[] buffer = new byte[0x1000];
               int numRead;
               while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
               {
                  sb.Append(Encoding.Unicode.GetString(buffer, 0, numRead));
               }
               serializedContent = sb.ToString();
            }
            if (string.IsNullOrEmpty(serializedContent)) { return null; }

            var value = Deserialize<T>(serializedContent);
            return value;

         }
         catch (Exception) { throw; }
         finally { fileStreamSemaphore.Release(); }
      }

      public static byte[] Serialize<T>(T value)
      {
         try
         {
            if (value == null) { return null; }

            var serializedContent = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            if (string.IsNullOrEmpty(serializedContent)) { return null; }

            byte[] encodedContent = System.Text.Encoding.Unicode.GetBytes(serializedContent);
            if (encodedContent == null || encodedContent.Length == 0) { return null; }

            return encodedContent;
         }
         catch (Exception) { throw; }
      }

      public static async Task<bool> SaveFile<T>(string path, T value)
      {
         try
         {
            await fileStreamSemaphore.WaitAsync();
            var encodedContent = Serialize(value);
            if (encodedContent == null || encodedContent.Length == 0) { return false; }

            using (var fileStream = new System.IO.FileStream(path,
                FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
               await fileStream.WriteAsync(encodedContent, 0, encodedContent.Length);
            };
            return System.IO.File.Exists(path);
         }
         catch (Exception) { throw; }
         finally { fileStreamSemaphore.Release(); }
      }

   }
}