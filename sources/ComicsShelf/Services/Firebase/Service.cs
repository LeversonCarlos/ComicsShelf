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
      string _DatabaseUrl;

      public static void Init(string databaseUrl)
      {
         DependencyService.Register<FirebaseService>();
         DependencyService.Get<FirebaseService>()._DatabaseUrl = databaseUrl;
         // DependencyService.Get<FirebaseService>().BaseAddress = new Uri(databaseUrl);
      }

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

      public static Task<ItemVM> GetAsync(ItemVM item) =>
         DependencyService.Get<FirebaseService>()._GetAsync(item);
      async Task<ItemVM> _GetAsync(ItemVM item)
      {
         try
         {
            if (!_HasConnection)
               throw new Exception("There are no active internet connection");

            var requestUrl = $"libraries/{item.EscapedLibraryID()}/items/{item.EscapedID}.json";
            var responseMessage = await GetAsync(requestUrl);
            responseMessage.EnsureSuccessStatusCode();

            var jsonContent = await responseMessage.Content.ReadAsStringAsync();
            var firebaseItem = await Helpers.Json.Deserialize<FirebaseItemVM>(jsonContent);
            if (firebaseItem == null || firebaseItem.ID != item.ID)
               throw new Exception("Invalid item received from firebase cloud");

            item.SetData(firebaseItem.ToLibraryItem());
            return item;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return item; }
      }

      public static Task<ItemVM[]> GetAsync(LibraryVM library) =>
         DependencyService.Get<FirebaseService>()._GetAsync(library);
      async Task<ItemVM[]> _GetAsync(LibraryVM library)
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


   }
}
