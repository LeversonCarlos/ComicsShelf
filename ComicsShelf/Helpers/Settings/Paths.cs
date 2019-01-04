using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers.Settings
{
   public class Paths
   {

      internal async Task Initialize()
      {
         try
         {

            using (var fileSystem = Helpers.FileSystem.Get())
            {
               this.Separator = fileSystem.PathSeparator;
               this.CachePath = fileSystem.GetCachePath();
               this.DataPath = fileSystem.GetDataPath();
            }

            if (!System.IO.Directory.Exists(this.CoversCachePath))
            { System.IO.Directory.CreateDirectory(this.CoversCachePath); }

            if (!System.IO.Directory.Exists(this.FilesCachePath))
            { System.IO.Directory.CreateDirectory(this.FilesCachePath); }

         }
         catch (Exception ex) { throw; }
      }

      public string Separator { get; set; }

      internal string DataPath { get; set; }
      internal string CachePath { get; set; }
      public Database.Library[] Libraries { get; set; }

      internal string DatabasePath { get { return $"{this.DataPath}{this.Separator}Database.db3"; } }
      public string CoversCachePath { get { return $"{this.CachePath}{this.Separator}CoversCache"; } }
      public string FilesCachePath { get { return $"{this.CachePath}{this.Separator}FilesCache"; } }

   }
}