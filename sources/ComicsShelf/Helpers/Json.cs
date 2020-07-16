using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers
{
   public class Json
   {

      public static async Task<string> Serialize<T>(T value)
      {
         try
         {
            using (var stream = new MemoryStream())
            {

               await JsonSerializer.SerializeAsync(stream, value);
               await stream.FlushAsync();
               stream.Position = 0;

               using (var reader = new StreamReader(stream))
               {
                  var result = await reader.ReadToEndAsync();
                  return result;
               }

            }
         }
         catch (Exception) { throw; }
      }

      public static async Task<T> Deserialize<T>(string value)
      {
         try
         {
            var byteArray = Encoding.UTF8.GetBytes(value);
            using (var stream = new MemoryStream(byteArray))
            {
               var result = await JsonSerializer.DeserializeAsync<T>(stream);
               return result;
            }
         }
         catch (Exception ex) { throw new Exception($"Error deserializing the following content: {Environment.NewLine}{value}", ex); }
      }

   }
}
