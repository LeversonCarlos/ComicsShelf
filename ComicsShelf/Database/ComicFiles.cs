using SQLite;

namespace ComicsShelf.Database
{
   [Table("ComicFiles")]
   public class ComicFiles : File.iFileData
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