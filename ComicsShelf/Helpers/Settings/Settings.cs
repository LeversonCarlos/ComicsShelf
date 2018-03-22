using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers.Settings
{
   public class Settings: IDisposable
   {

      #region Properties 
      public Paths Paths { get; set; }
      // public SQLiteConnection Database { get; set; }
      #endregion

      #region Initialize
      internal async Task Initialize()
      {
         await this.Initialize_Path();
      }
      #endregion

      #region Initialize_Path
      private async Task Initialize_Path()
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
      #endregion

      #region Dispose
      public void Dispose()
      {
         if (this.Paths != null) { this.Paths = null; }
      }
      #endregion

   }
}