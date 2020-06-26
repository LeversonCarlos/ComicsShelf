using ComicsShelf.Observables;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ComicsShelf.ViewModels
{

   public class ItemVM : ObservableObject
   {

      public ItemVM()
      {
         this.CoverPath = Helpers.Cover.DefaultCover;
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

      short? _Rating;
      public short? Rating
      {
         get => _Rating;
         set => SetProperty(ref _Rating, value);
      }

      bool _Readed;
      public bool Readed
      {
         get => _Readed;
         set
         {
            ReadingPage = 0;
            ReadingPercent = (value ? 1 : 0);
            ReadingDate = (value ? DateTime.Now : DateTime.MinValue);
            SetProperty(ref _Readed, value);
         }
      }

      DateTime? _ReadingDate;
      public DateTime? ReadingDate
      {
         get => _ReadingDate;
         set => SetProperty(ref _ReadingDate, value);
      }

      short? _ReadingPage;
      public short? ReadingPage
      {
         get => _ReadingPage;
         set => SetProperty(ref _ReadingPage, value);
      }

      double _ReadingPercent;
      public double ReadingPercent
      {
         get => _ReadingPercent;
         set => SetProperty(ref _ReadingPercent, value);
      }

      [JsonIgnore]
      public string CoverPath { get; set; }

      bool _Available;
      public bool Available
      {
         get => _Available;
         set => SetProperty(ref _Available, value);
      }

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
         itemVM.ClearDirty();
      }
   }

}
