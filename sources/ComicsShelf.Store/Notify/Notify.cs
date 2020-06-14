namespace ComicsShelf.Store
{
   public class Notify
   {

      const string LibraryAddMessage = "store.add.library.message";

      internal static void LibraryAdd(LibraryStoreVM library) =>
         Base.Helpers.Messaging.Send(LibraryAddMessage, library);

      public static void LibraryAdded(System.Action<LibraryStoreVM> callback) =>
         Base.Helpers.Messaging.Subscribe(LibraryAddMessage, callback);


      public const string LibraryRemoveMessage = "store.remove.library.message";

      internal static void LibraryRemove(LibraryStoreVM library) =>
         Base.Helpers.Messaging.Send(LibraryRemoveMessage, library.ID);

      public static void LibraryRemoved(System.Action<LibraryStoreVM> callback) =>
         Base.Helpers.Messaging.Subscribe(LibraryRemoveMessage, callback);

   }
}
