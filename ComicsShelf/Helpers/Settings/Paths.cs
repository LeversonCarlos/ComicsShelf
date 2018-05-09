namespace ComicsShelf.Helpers.Settings
{
   public class Paths
   {
      public string Separator { get; set; }

      public string LibraryPath { get; set; }

      internal string DataPath { get; set; }

      internal string DatabasePath
      { get { return $"{this.DataPath}{this.Separator}Database.db3"; } }

      internal string MainCachePath { get; set; }
      public string CoversCachePath { get { return $"{this.MainCachePath}{this.Separator}CoversCache"; } }
      public string FilesCachePath { get { return $"{this.MainCachePath}{this.Separator}FilesCache"; } }

   }
}