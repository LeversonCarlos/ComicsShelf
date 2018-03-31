using SQLite;

namespace ComicsShelf.Helpers.Settings
{
   [Table("Comics")]
   public class Comics : File.iFileData
   {

      [PrimaryKey]
      public string FullPath { get; set; }

      public string ReleaseDate { get; set; }

      public bool Readed { get; set; }
      public short ReadingPercent { get; set; }
      public short ReadingPage { get; set; }
      public string ReadingDate { get; set; }

      public short? Rate { get; set; }

   }
}