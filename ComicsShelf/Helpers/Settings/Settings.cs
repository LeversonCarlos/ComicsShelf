using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers.Settings
{
   public class Settings: IDisposable
   {

      public Paths Paths { get; set; }
      internal async Task InitializePath()
      {
         try
         {
            var fileSystem = Helpers.FileSystem.Get();
            this.Paths = new Paths
            {
               Separator = fileSystem.PathSeparator,
               LocalPath = await fileSystem.GetLocalPath()
            };

            if (!System.IO.Directory.Exists(this.Paths.CoversPath))
            { System.IO.Directory.CreateDirectory(this.Paths.CoversPath); }

            if (!System.IO.Directory.Exists(this.Paths.CachePath))
            { System.IO.Directory.CreateDirectory(this.Paths.CachePath); }
         }
         catch (Exception ex) { throw; }
      }

      public void Dispose()
      {
         if (this.Paths != null) { this.Paths = null; }
      }

   }
}