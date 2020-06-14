using ComicsShelf.Store;

namespace ComicsShelf.Engine
{
   public class Notify
   {

      const string LibraryAddMessage = "store.add.library.message";

      internal static void LibraryAdd(LibraryStoreVM library) =>
         Helpers.Messaging.Send(LibraryAddMessage, library);

      public static void LibraryAdd(System.Action<LibraryStoreVM> callback) =>
         Helpers.Messaging.Subscribe(LibraryAddMessage, callback);


      public const string LibraryRemoveMessage = "store.remove.library.message";

      internal static void LibraryRemove(LibraryStoreVM library) =>
         Helpers.Messaging.Send(LibraryRemoveMessage, library.ID);

      public static void LibraryRemove(System.Action<LibraryStoreVM> callback) =>
         Helpers.Messaging.Subscribe(LibraryRemoveMessage, callback);

   }
}
