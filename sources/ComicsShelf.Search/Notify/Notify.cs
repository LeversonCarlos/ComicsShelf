using ComicsShelf.ViewModels;

namespace ComicsShelf.Search
{
   public class Notify
   {

      const string ProgressMessage = "search.progress.message";

      internal static void Progress(LibraryVM library, string message, params string[] parameters) =>
         Helpers.Messaging.Send($"{ProgressMessage}.{library.ID}", new { message, parameters });

      public static void Progress(LibraryVM library, System.Action<(string message, string[] parameters)> callback) =>
         Helpers.Messaging.Subscribe($"{ProgressMessage}.{library.ID}", callback);


   }
}
