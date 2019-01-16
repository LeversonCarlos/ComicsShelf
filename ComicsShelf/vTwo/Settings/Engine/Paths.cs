namespace ComicsShelf.vTwo.Settings
{

   partial class Engine
   {
      public Paths Paths { get; set; }

      private void InitializePaths()
      {
         using (var fileSystem = ComicsShelf.Helpers.FileSystem.Get())
         {
            this.Paths = new Paths
            {
               Separator = fileSystem.PathSeparator,
               CachePath = fileSystem.GetCachePath(),
               DataPath = fileSystem.GetDataPath()
            };
         }
      }

   }

   public class Paths
   {
      internal Paths() { }
      public string Separator { get; set; }
      internal string DataPath { get; set; }
      internal string CachePath { get; set; }
      internal string DatabasePath { get { return $"{this.DataPath}{this.Separator}Database.db3"; } }
      internal string SettingsPath { get { return $"{this.DataPath}{this.Separator}Settings.json"; } }
      public string CoversCachePath { get { return $"{this.CachePath}{this.Separator}CoversCache"; } }
      public string FilesCachePath { get { return $"{this.CachePath}{this.Separator}FilesCache"; } }
   }

}