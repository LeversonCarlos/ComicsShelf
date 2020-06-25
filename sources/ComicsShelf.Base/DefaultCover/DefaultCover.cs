using System;
using System.Threading.Tasks;

namespace ComicsShelf.DefaultCover
{
   public class DefaultCover
   {

      public static string Path
      {
         get { return $"{Xamarin.Essentials.FileSystem.AppDataDirectory}/DefaultCover.png"; }
      }

      public static async Task ExtractAsync()
      {
         try
         {
            if (System.IO.File.Exists(Path)) { return; }

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            byte[] bytes = null;
            using (var defaultCover = assembly.GetManifestResourceStream("ComicsShelf.Base.DefaultCover.DefaultCover.png"))
            {
               bytes = new byte[defaultCover.Length];
               await defaultCover.ReadAsync(bytes, 0, bytes.Length);
            }
            if (bytes == null || bytes.Length == 0) { return; }

            using (var streamWriter = new System.IO.FileStream(Path, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
               await streamWriter.WriteAsync(bytes, 0, bytes.Length);
            }

            assembly = null;
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
      }

   }
}
