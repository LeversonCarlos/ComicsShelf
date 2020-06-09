namespace ComicsShelf.Store
{
   partial class StoreService
   {

      public const string add_library_message = "store.add.library.message";
      public void NotifyLibraryAdd(LibraryStoreVM library) =>
         Base.Helpers.Messaging.Send(add_library_message, library.ID);

      public const string remove_library_message = "store.remove.library.message";
      public void NotifyLibraryRemove(LibraryStoreVM library) =>
         Base.Helpers.Messaging.Send(remove_library_message, library.ID);

   }
}
