using ComicsShelf.ViewModels;

namespace ComicsShelf.Notifyers
{
   partial class Notify
   {

      const string LibraryAddMessage = "store.library.add.message";
      public static void LibraryAdd(LibraryVM library) =>
         Helpers.Messaging.Send(LibraryAddMessage, library);
      public static void LibraryAdd(System.Action<LibraryVM> callback) =>
         Helpers.Messaging.Subscribe(LibraryAddMessage, callback);

      const string LibraryRemoveMessage = "store.library.remove.message";
      public static void LibraryRemove(LibraryVM library) =>
         Helpers.Messaging.Send(LibraryRemoveMessage, library.ID);
      public static void LibraryRemove(System.Action<LibraryVM> callback) =>
         Helpers.Messaging.Subscribe(LibraryRemoveMessage, callback);

      const string ItemsUpdateMessage = "store.items.update.message";
      public static void ItemsUpdate(LibraryVM library, ItemVM[] itemList) =>
         Helpers.Messaging.Send($"{ItemsUpdateMessage}.{library.ID}", itemList);
      public static void ItemsUpdate(LibraryVM library, System.Action<ItemVM[]> callback) =>
         Helpers.Messaging.Subscribe($"{ItemsUpdateMessage}.{library.ID}", callback);

      const string ProgressMessage = "store.progress.message";
      public static void Progress(LibraryVM library, string message, params string[] parameters) =>
         Helpers.Messaging.Send($"{ProgressMessage}.{library.ID}", new ProgressData { Message = message, Parameters = parameters });
      public static void Progress(LibraryVM library, System.Action<ProgressData> callback) =>
         Helpers.Messaging.Subscribe($"{ProgressMessage}.{library.ID}", callback);
      public struct ProgressData
      {
         public string Message { get; set; }
         public string[] Parameters { get; set; }
      }

   }
}
