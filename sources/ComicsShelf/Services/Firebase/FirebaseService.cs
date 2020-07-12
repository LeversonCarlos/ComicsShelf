using ComicsShelf.ViewModels;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Firebase
{
   public class FirebaseService : HttpClient
   {

      public static void Init(string databaseUrl)
      {
         DependencyService.Register<FirebaseService>();
         DependencyService.Get<FirebaseService>().BaseAddress = new Uri(databaseUrl);
      }

      public static Task<HttpResponseMessage> UpdateAsync(LibraryVM library, ItemVM item) =>
         DependencyService.Get<FirebaseService>()._UpdateAsync(library, item);

      async Task<HttpResponseMessage> _UpdateAsync(LibraryVM library, ItemVM item)
      {
         try
         {
            var requestUrl = $"libraries/{library.GetHashedID()}/items/{item.GetHashedID()}.json";

            var jsonParameter = await Helpers.Json.Serialize(item);
            var httpParameter = new StringContent(jsonParameter, System.Text.Encoding.UTF8, "application/json");

            var responseMessage = await PutAsync(requestUrl, httpParameter);

            return responseMessage;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return null; }
      }

   }
}
