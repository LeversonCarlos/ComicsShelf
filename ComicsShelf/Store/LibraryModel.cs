using System.Collections.Generic;

namespace ComicsShelf.Store
{
   public enum LibraryType : short { LocalDrive = 0, OneDrive = 1 }

   public class LibraryModel
   {

      public LibraryModel()
      { this.KeyValues = new Dictionary<string, string>(); }

      public string ID { get; set; }
      public string Key { get; set; }
      public string Path { get; set; }
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

      public bool Removed { get; set; }

   }
}
