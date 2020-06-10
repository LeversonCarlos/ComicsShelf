namespace ComicsShelf.Store
{
   internal partial class Sync
   {

      static string base_sync() => "ComicsShelf";
      
      string LibrarySyncKey(string libraryID) => $"{SyncKey()}.Library.{libraryID}";

   }
}
