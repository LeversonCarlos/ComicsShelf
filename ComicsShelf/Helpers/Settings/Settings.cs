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
               MainCachePath = await fileSystem.GetCachePath(),
               DataPath = await fileSystem.GetDataPath()
            };

            if (!System.IO.Directory.Exists(this.Paths.CoversCachePath))
            { System.IO.Directory.CreateDirectory(this.Paths.CoversCachePath); }

            if (!System.IO.Directory.Exists(this.Paths.FilesCachePath))
            { System.IO.Directory.CreateDirectory(this.Paths.FilesCachePath); }
         }
         catch (Exception ex) { throw; }
      }

      public void Dispose()
      {
         if (this.Paths != null) { this.Paths = null; }
      }

   }
}