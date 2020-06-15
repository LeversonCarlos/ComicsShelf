using ComicsShelf.ViewModels;

namespace ComicsShelf.Store
{
   public class Notify
   {

      const string LibraryAddMessage = "store.add.library.message";

      internal static void LibraryAdd(LibraryVM library) =>
         Helpers.Messaging.Send(LibraryAddMessage, library);

      public static void LibraryAdd(System.Action<LibraryVM> callback) =>
         Helpers.Messaging.Subscribe(LibraryAddMessage, callback);


      public const string LibraryRemoveMessage = "store.remove.library.message";

      internal static void LibraryRemove(LibraryVM library) =>
         Helpers.Messaging.Send(LibraryRemoveMessage, library.ID);

      public static void LibraryRemove(System.Action<LibraryVM> callback) =>
         Helpers.Messaging.Subscribe(LibraryRemoveMessage, callback);

   }
}
