using SQLite;
using System;
using System.Threading.Tasks;

namespace ComicsShelf.Helpers.Settings
{
   public class Settings: IDisposable
   {

      #region Properties 
      public Paths Paths { get; set; }
      public SQLiteConnection Database { get; set; }
      #endregion

      #region Initialize
      internal async Task Initialize()
      {
         await this.Initialize_Path();
         await this.Initialize_Database();
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

      #region Initialize_Database
      private async Task Initialize_Database()
      {
         try
         {
            this.Database = new SQLiteConnection(this.Paths.DatabasePath);
            this.Database.CreateTable<Configs>();
            this.Database.CreateTable<Comics>();
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