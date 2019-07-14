using System.Collections.Generic;

namespace ComicsShelf.Libraries
{
   public enum LibraryType : short { LocalDrive = 0, OneDrive = 1 }

   public class LibraryModel
   {
      public const string SyncFile_OLD = "ComicsShef.library";
      public const string SyncFile = "ComicsShelf.library";

      public LibraryModel()
      { this.KeyValues = new Dictionary<string, string>(); }

      public string ID { get; set; }
      public string LibraryKey { get; set; }
      public string LibraryPath { get; set; }
      public string Description { get; set; }
      public LibraryType Type { get; set; }

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
