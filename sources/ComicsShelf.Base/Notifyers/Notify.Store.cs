using ComicsShelf.ViewModels;
using Xamarin.Forms;

namespace ComicsShelf.Notifyers
{
   partial class Notify
   {

      const string LibraryAddKey = "store.library.add.key";
      public static void LibraryAdd(LibraryVM library) =>
         Device.BeginInvokeOnMainThread(() => Helpers.Messaging.Send(LibraryAddKey, library));
      public static void LibraryAdd(System.Action<LibraryVM> callback) =>
         Helpers.Messaging.Subscribe(LibraryAddKey, callback);

      const string LibraryRemoveKey = "store.library.remove.key";
      public static void LibraryRemove(LibraryVM library) =>
         Helpers.Messaging.Send(LibraryRemoveKey, library);
      public static void LibraryRemove(System.Action<LibraryVM> callback) =>
         Helpers.Messaging.Subscribe(LibraryRemoveKey, callback);

      const string ItemsUpdateKey = "store.items.update.key";
      public static void ItemsUpdate(LibraryVM library, ItemVM[] itemList) =>
         Helpers.Messaging.Send($"{ItemsUpdateKey}.{library.ID}", itemList);
      public static void ItemsUpdate(LibraryVM library, System.Action<ItemVM[]> callback) =>
         Helpers.Messaging.Subscribe($"{ItemsUpdateKey}.{library.ID}", callback);

      const string MessageKey = "store.message.key";
      public static void Message(LibraryVM library, string message, params string[] parameters) =>
         Helpers.Messaging.Send($"{MessageKey}.{library.ID}", new MessageData { Message = message, Parameters = parameters });
      public static void Message(LibraryVM library, System.Action<MessageData> callback) =>
         Helpers.Messaging.Subscribe($"{MessageKey}.{library.ID}", callback);
      public struct MessageData
      {
         public string Message { get; set; }
         public string[] Parameters { get; set; }
      }

      const string ProgressKey = "store.progress.key";
      public static void Progress(LibraryVM library, double progress) =>
         Helpers.Messaging.Send($"{ProgressKey}.{library.ID}", progress);
      public static void Progress(LibraryVM library, System.Action<double> callback) =>
         Helpers.Messaging.Subscribe($"{ProgressKey}.{library.ID}", callback);

   }
}
