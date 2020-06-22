using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ComicsShelf.ViewModels
{

   public class ItemVM
   {

      public ItemVM()
      {
         this.CoverPath = DefaultCover.DefaultCover.Path;
      }

      public string ID { get; set; }
      public string LibraryID { get; set; }
      public string EscapedID { get; set; }

      public string FullText { get; set; }
      public string ShortText { get; set; }
      public string FolderText { get; set; }

      public string FileName { get; set; }
      public string FolderPath { get; set; }
      public string SectionPath { get; set; }

      public long? SizeInBytes { get; set; }
      public DateTime ReleaseDate { get; set; }
      public Dictionary<string, string> KeyValues { get; set; }

      public short? Rating { get; set; }
      public bool Readed { get; set; }
      public DateTime? ReadingDate { get; set; }
      public short? ReadingPage { get; set; }
      public double ReadingPercent { get; set; }

      [JsonIgnore]
      public string CoverPath { get; set; }

      public bool Available { get; set; }
   }

   public static class ItemVMExtention
   {
      public static void SetData(this ItemVM itemVM, ItemVM data)
      {
         itemVM.Rating = data.Rating;
         itemVM.Readed = data.Readed;
         itemVM.ReadingDate = data.ReadingDate;
         itemVM.ReadingPage = data.ReadingPage;
         itemVM.ReadingPercent = data.ReadingPercent;
         itemVM.Available = data.Available;
      }
   }

}
