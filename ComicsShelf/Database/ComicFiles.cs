using SQLite;

namespace ComicsShelf.Database
{
   [Table("ComicFiles")]
   public class ComicFiles : File.iFileData
   {

      [PrimaryKey]
      public string Key { get; set; }
      [Indexed]
      public string LibraryPath { get; set; }
      [Indexed]
      public string FullPath { get; set; }
      public string ParentPath { get; set; }

      public string Text { get; set; }
      public string SmallText { get; set; }
      public string CoverPath { get; set; }

      public string ReleaseDate { get; set; }
      public int Rating { get; set; }

      public bool Readed { get; set; }
      public double ReadingPercent { get; set; }
      public short ReadingPage { get; set; }
      public string ReadingDate { get; set; }

      public bool Available { get; set; }

   }
}