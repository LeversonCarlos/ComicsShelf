using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ComicsShelf.vTwo.Helpers
{
   internal class FileStream
   {

      public static async Task<T> ReadFile<T>(string path) where T : class
      {
         try
         {
            if (!File.Exists(path)) { return null; }

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

            var value = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(serializedContent);
            return value;

         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("Helpers.FileStream.ReadFile", ex); return null; }
      }

      public static async Task<bool> SaveFile<T>(string path, T value)
      {
         try
         {
            if (value == null) { return false; }

            var serializedContent = Newtonsoft.Json.JsonConvert.SerializeObject(value);
            if (string.IsNullOrEmpty(serializedContent)) { return false; }

            byte[] encodedContent = System.Text.Encoding.Unicode.GetBytes(serializedContent);
            if (encodedContent == null || encodedContent.Length == 0) { return false; }

            using (var fileStream = new System.IO.FileStream(path,
                FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 4096, useAsync: true))
            {
               await fileStream.WriteAsync(encodedContent, 0, encodedContent.Length);
            };
            return File.Exists(path);
         }
         catch (Exception ex) { Engine.AppCenter.TrackEvent("Helpers.FileStream.SaveFile", ex); return false; }
      }

   }
}