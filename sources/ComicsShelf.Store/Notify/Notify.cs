namespace ComicsShelf.Store
{
   public class Notify
   {

      public const string AddLibraryMessage = "store.add.library.message";
      internal static void AddLibrary(LibraryStoreVM library) =>
         Base.Helpers.Messaging.Send(AddLibraryMessage, library.ID);

      public const string RemoveLibraryMessage = "store.remove.library.message";
      internal static void RemoveLibrary(LibraryStoreVM library) =>
         Base.Helpers.Messaging.Send(RemoveLibraryMessage, library.ID);

   }
}
