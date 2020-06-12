using ComicsShelf.Store;

namespace ComicsShelf.Search
{
   public class Notify
   {

      public const string ProgressMessage = "search.search.progress.message";
      internal static void Progress(LibraryStoreVM library, params string[] message) =>
         Base.Helpers.Messaging.Send($"{ProgressMessage}.{library.ID}", message);

      public const string ResultMessage = "search.search.result.message";
      internal static void Result(LibraryStoreVM library, ItemStoreVM[] libraryItems) =>
         Base.Helpers.Messaging.Send($"{ResultMessage}.{library.ID}", libraryItems);

   }
}
