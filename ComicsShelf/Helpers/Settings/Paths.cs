namespace ComicsShelf.Helpers.Settings
{
   public class Paths
   {
      private const string DataPath = "ComicsShelfData";
      public string Separator { get; set; }

      public string ComicsPath { get; set; }
      internal string LocalPath { get; set; }

      internal string DatabasePath
      { get { return $"{this.LocalPath}{this.Separator}{DataPath}{this.Separator}Database.db3"; } }

      public string CoversPath
      { get { return $"{this.LocalPath}{this.Separator}{DataPath}{this.Separator}Covers"; } }

      public string CachePath
      { get { return $"{this.LocalPath}{this.Separator}{DataPath}{this.Separator}Cache"; } }

   }
}