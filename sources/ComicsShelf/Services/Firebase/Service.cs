using ComicsShelf.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Firebase
{
   public class FirebaseService : HttpClient
   {

      public static void Init(string databaseUrl, string environment)
      {
         DependencyService.Register<FirebaseService>();
         DependencyService.Get<FirebaseService>()._DatabaseUrl = databaseUrl;
         DependencyService.Get<FirebaseService>()._Environment = environment;
      }

      string _Environment;
      string _DatabaseUrl;
      string _BaseUrl(LibraryVM library) =>
         $"{_Environment}/libraries/{library.EscapedID}";

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

      [DebuggerStepThrough]
      public Task<HttpResponseMessage> PatchAsync(string requestUri, HttpContent requestContent)
      {
         var request = new HttpRequestMessage(new HttpMethod("PATCH"), requestUri);
         request.Content = requestContent;
         return SendAsync(request);
      }

      public async Task<FirebaseItemVM[]> GetItemsAsync(LibraryVM library)
      {
         try
         {
            if (!_HasConnection)
               throw new Exception("There are no active internet connection");

            var requestUrl = $"{_BaseUrl(library)}/items.json";
            var responseMessage = await GetAsync(requestUrl);
            responseMessage.EnsureSuccessStatusCode();

            var jsonContent = await responseMessage.Content.ReadAsStringAsync();
            var firebaseItems = await Helpers.Json.Deserialize<Dictionary<string, FirebaseItemVM>>(jsonContent);
            if (firebaseItems == null)
               throw new Exception("Invalid item list received from firebase cloud");

            var litemList = firebaseItems
               .Select(x => x.Value)
               .ToArray();
            return litemList;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return null; }
      }

      public void AddItemOnThread(LibraryVM library, ItemVM item) => Task.Run(() => AddItemAsync(library, item));
      public async Task<bool> AddItemAsync(LibraryVM library, ItemVM item)
      {
         try
         {
            if (!_HasConnection)
               throw new Exception("There are no active internet connection");

            var requestUrl = $"{_BaseUrl(library)}/items/{item.EscapedID}.json";
            var dataParameter = new { ID = item.ID };
            var jsonParameter = await Helpers.Json.Serialize(dataParameter);
            var httpParameter = new StringContent(jsonParameter, System.Text.Encoding.UTF8, "application/json");

            var responseMessage = await PatchAsync(requestUrl, httpParameter);
            responseMessage.EnsureSuccessStatusCode();

            return true;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return false; }
      }

      public void SetItemOnThread(LibraryVM library, ItemVM item) => Task.Run(() => SetItemAsync(library, item));
      public async Task<bool> SetItemAsync(LibraryVM library, ItemVM item)
      {
         try
         {
            if (!_HasConnection)
               throw new Exception("There are no active internet connection");

            var requestUrl = $"{_BaseUrl(library)}/items/{item.EscapedID}.json";
            var jsonParameter = await Helpers.Json.Serialize(item.ToFirebaseItem());
            var httpParameter = new StringContent(jsonParameter, System.Text.Encoding.UTF8, "application/json");

            var responseMessage = await PatchAsync(requestUrl, httpParameter);
            responseMessage.EnsureSuccessStatusCode();

            return true;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); return false; }
      }

   }
}
