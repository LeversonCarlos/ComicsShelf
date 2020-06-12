using ComicsShelf.Store;

namespace ComicsShelf.Drive
{
   public class Notify
   {

      public const string drive_search_progress = "drive.search.progress.message";
      internal static void SearchProgress(LibraryStoreVM library, params string[] message) =>
         Base.Helpers.Messaging.Send($"{drive_search_progress}.{library.ID}", message);

      public const string drive_search_result = "drive.search.result.message";
      internal static void SearchResult(LibraryStoreVM library, ItemStoreVM[] libraryItems) =>
         Base.Helpers.Messaging.Send($"{drive_search_result}.{library.ID}", libraryItems);

   }
}
