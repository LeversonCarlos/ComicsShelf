namespace ComicsShelf.Store
{
   public class Notify
   {

      public const string add_library_message = "store.add.library.message";
      internal static void LibraryAdd(LibraryStoreVM library) =>
         Base.Helpers.Messaging.Send(add_library_message, library.ID);

      public const string remove_library_message = "store.remove.library.message";
      internal static void LibraryRemove(LibraryStoreVM library) =>
         Base.Helpers.Messaging.Send(remove_library_message, library.ID);

   }
}
