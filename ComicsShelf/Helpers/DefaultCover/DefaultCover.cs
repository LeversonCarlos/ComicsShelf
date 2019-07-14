using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers
{
   internal class DefaultCover
   {

      public static async Task LoadDefaultCover()
      {
         try
         {
            if (System.IO.File.Exists(Libraries.LibraryConstants.DefaultCover)) { return; }

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            byte[] bytes = null;
            using (var defaultCover = assembly.GetManifestResourceStream("ComicsShelf.Helpers.DefaultCover.DefaultCover.png"))
            {
               bytes = new byte[defaultCover.Length];
               await defaultCover.ReadAsync(bytes, 0, bytes.Length);
            }
            if (bytes == null || bytes.Length == 0) { return; }

            using (var streamWriter = new System.IO.FileStream(Libraries.LibraryConstants.DefaultCover, System.IO.FileMode.Create, System.IO.FileAccess.Write))
            {
               await streamWriter.WriteAsync(bytes, 0, bytes.Length);
            }

            assembly = null;
         }
         catch (Exception) { throw; }
      }

   }
}
