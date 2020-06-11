using ComicsShelf.Store;

namespace ComicsShelf.Drive
{
   public class Notify
   {

      public const string drive_search_progress = "drive.search.progress.message";
      internal static void SearchProgress(LibraryStoreVM library, params string[] message) =>
         Base.Helpers.Messaging.Send($"{drive_search_progress}.{library.ID}", message);

   }
}
