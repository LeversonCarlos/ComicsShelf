using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers
{
   public class Cover
   {

      public static string DefaultCover => $"{Xamarin.Essentials.FileSystem.AppDataDirectory}/DefaultCover.png";

      public static Task Init() => Task.Factory.StartNew(() => InitAsync(), TaskCreationOptions.LongRunning);

      private static async Task InitAsync()
      {
         await ExtractDefaultCoverAsync();
      }

      private static async Task ExtractDefaultCoverAsync()
      {
         try
         {
            if (System.IO.File.Exists(DefaultCover)) { return; }

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            byte[] bytes = null;
            using (var defaultCover = assembly.GetManifestResourceStream("ComicsShelf.Base.Resources.DefaultCover.png"))
            {
               bytes = new byte[defaultCover.Length];
               await defaultCover.ReadAsync(bytes, 0, bytes.Length);
            }
            if (bytes == null || bytes.Length == 0) { return; }

            using (var streamWriter = new System.IO.FileStream(DefaultCover, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
               await streamWriter.WriteAsync(bytes, 0, bytes.Length);
            }

            assembly = null;
         }
         catch (Exception ex) { Insights.TrackException(ex); }
      }

   }
}
