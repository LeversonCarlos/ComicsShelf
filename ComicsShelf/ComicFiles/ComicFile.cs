using System;
using System.Collections.Generic;

namespace ComicsShelf.ComicFiles
{
   public class ComicFile
   {
      public string Key { get; set; }
      public string LibraryKey { get; set; }
      public bool Available { get; set; }

      public string FullText { get; set; }
      public string SmallText { get; set; }

      public DateTime ReleaseDate { get; set; }
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

      // KeyValues
      public Dictionary<string, string> KeyValues { get; set; }
      internal void SetKeyValue(string key, string value)
      {
         if (this.KeyValues.ContainsKey(key))
         {
            if (this.KeyValues[key] == value) { return; }
            else { this.KeyValues.Remove(key); }
         }
         this.KeyValues.Add(key, value);
      }
      internal string GetKeyValue(string key)
      {
         if (!this.KeyValues.ContainsKey(key)) { return string.Empty; }
         return this.KeyValues[key];
      }

   }
}
