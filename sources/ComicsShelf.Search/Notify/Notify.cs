using ComicsShelf.ViewModels;

namespace ComicsShelf.Search
{
   public class Notify
   {

      const string ProgressMessage = "search.progress.message";
      public struct ProgressData
      {
         public ProgressData(string message, params string[] parameters)
         {
            this.Message = message;
            this.Parameters = parameters;
         }
         public string Message;
         public string[] Parameters;
      }

      internal static void Progress(LibraryVM library, string message, params string[] parameters) =>
         Helpers.Messaging.Send($"{ProgressMessage}.{library.ID}", new ProgressData(message, parameters));

      public static void Progress(LibraryVM library, System.Action<ProgressData> callback) =>
         Helpers.Messaging.Subscribe($"{ProgressMessage}.{library.ID}", callback);


   }
}
