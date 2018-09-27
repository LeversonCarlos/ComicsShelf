using SQLite;

namespace ComicsShelf.Helpers.Database
{
   [Table("ComicFiles")]
   public class ComicFile
   {

      [PrimaryKey]
      public string Key { get; set; }
      public string ReleaseDate { get; set; }

      [Indexed]
      public string LibraryPath { get; set; }
      [Indexed]
      public string FullPath { get; set; }
      public string ParentPath { get; set; }
      public string CoverPath { get; set; }

      public string FullText { get; set; }
      public string SmallText { get; set; }

      public int Rating { get; set; }
      public bool Readed { get; set; }
      public double ReadingPercent { get; set; }
      public short ReadingPage { get; set; }
      public double ReadingOpacity { get; set; }
      public string ReadingDate { get; set; }

   }
}
