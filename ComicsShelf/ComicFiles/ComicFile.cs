using System;

namespace ComicsShelf.ComicFiles
{
   public class ComicFile
   {
      public string Key { get; set; }

      public string FullText { get; set; }
      public string SmallText { get; set; }

      public string FilePath { get; set; }
      public string FolderPath { get; set; }
      public string CoverPath { get; set; }
      public string CachePath { get; set; }
      public short TotalPages { get; set; }

      public short Rating { get; set; }
      public bool Readed { get; set; }
      public DateTime ReadingDate { get; set; }
      public short ReadingPage { get; set; }
      public double ReadingPercent { get; set; }

   }
}
