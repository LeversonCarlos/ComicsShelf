using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
         DependencyService.Get<FirebaseService>()._DatabaseUrl = databaseUrl;
      }

      string _DatabaseUrl;

      bool _HasConnection
      {
         get
         {
            if (Xamarin.Essentials.Connectivity.NetworkAccess != Xamarin.Essentials.NetworkAccess.Internet)
               return false;
            if (BaseAddress == null)
               BaseAddress = new Uri(_DatabaseUrl);
            return true;
         }
      }

      public async Task<ItemVM[]> GetAsync(LibraryVM library)
      {
         try
         {
            if (!_HasConnection)
               throw new Exception("There are no active internet connection");

            var requestUrl = $"libraries/{library.EscapedID}/items.json";
            var responseMessage = await GetAsync(requestUrl);
            responseMessage.EnsureSuccessStatusCode();

            var jsonContent = await responseMessage.Content.ReadAsStringAsync();
            var firebaseItems = await Helpers.Json.Deserialize<Dictionary<string, FirebaseItemVM>>(jsonContent);
            if (firebaseItems == null)
               throw new Exception("Invalid item list received from firebase cloud");

            var litemList = firebaseItems
               .Select(x => x.Value.ToLibraryItem())
               .ToArray();
            return litemList;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return null; }
      }

      public static Task<HttpResponseMessage> SetAsync(ItemVM item) =>
         DependencyService.Get<FirebaseService>()._SetAsync(item);
      async Task<HttpResponseMessage> _SetAsync(ItemVM item)
      {
         try
         {
            if (!_HasConnection)
               throw new Exception("There are no active internet connection");

            var requestUrl = $"libraries/{item.EscapedLibraryID()}/items/{item.EscapedID}.json";
            var jsonParameter = await Helpers.Json.Serialize(item.ToFirebaseItem());
            var httpParameter = new StringContent(jsonParameter, System.Text.Encoding.UTF8, "application/json");

            var responseMessage = await PutAsync(requestUrl, httpParameter);

            return responseMessage;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return null; }
      }

   }
}
