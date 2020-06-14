using ComicsShelf.Store;

namespace ComicsShelf.Search
{
   public class Notify
   {

      const string ProgressMessage = "search.progress.message";

      internal static void Progress(LibraryStoreVM library, params string[] messages) =>
         Helpers.Messaging.Send($"{ProgressMessage}.{library.ID}", messages);

      public static void Progress(LibraryStoreVM library, System.Action<string[]> callback) =>
         Helpers.Messaging.Subscribe($"{ProgressMessage}.{library.ID}", callback);


   }
}
