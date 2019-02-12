using System.Collections.Generic;

namespace ComicsShelf.Libraries
{
   public enum TypeEnum : short { FileSystem = 0, OneDrive = 1 }

   public class Library
   {
      public string LibraryID { get; set; }
      public string Description { get; set; }
      public TypeEnum Type { get; set; }
      public bool Available { get; set; }

      public Dictionary<string, string> KeyValues { get; set; }
      internal async void SetKeyValue(string key, string value) {
         if (this.KeyValues == null) { this.KeyValues = new Dictionary<string, string>(); }
         if (this.KeyValues.ContainsKey(key)) {
            if (this.KeyValues[key] == value) { return; }
            else { this.KeyValues.Remove(key); }
         }
         this.KeyValues.Add(key, value);
         await App.Settings.SaveLibraries();
      }
      internal string GetKeyValue(string key)
      {
         if (this.KeyValues == null) { return string.Empty; }
         if (!this.KeyValues.ContainsKey(key)) { return string.Empty; }
         return this.KeyValues[key];
      }

   }

}