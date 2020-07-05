using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Engine.LegacySettings
{
   public class Service
   {

      public static Task Execute(LibraryVM library) => Task.Factory.StartNew(() => ExecuteAsync(library), TaskCreationOptions.LongRunning);
      private static async Task ExecuteAsync(LibraryVM library)
      {
         var start = DateTime.Now;
         try
         {
            await Task.CompletedTask;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); Helpers.Notify.Message(library, $"Error while importing legacy settings: {ex.Message}"); }
         finally { Helpers.Insights.TrackMetric($"Legacy Settings", DateTime.Now.Subtract(start).TotalSeconds); }
      }

   }
}
