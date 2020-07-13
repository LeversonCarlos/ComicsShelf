using System;
using System.Collections.Generic;
using System.Reflection;

namespace ComicsShelf.ViewModels
{

   public class ItemVM : ObservableObject
   {

      public ItemVM()
      {
         KeyValues = new Dictionary<string, string>();
         CoverPath = Helpers.Cover.DefaultCover;
      }

      public string ID { get; set; }
      public string EscapedID { get; set; }
      public string LibraryID { get; set; }

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
         set => SetProperty(ref _Readed, value);
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

      string _CoverPath;
      public string CoverPath
      {
         get => _CoverPath;
         set => SetProperty(ref _CoverPath, value);
      }

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
         itemVM.SizeInBytes = data.SizeInBytes;
         itemVM.ReleaseDate = data.ReleaseDate;
         itemVM.Rating = data.Rating;
         itemVM.Readed = data.Readed;
         itemVM.ReadingDate = data.ReadingDate;
         itemVM.ReadingPage = data.ReadingPage;
         itemVM.ReadingPercent = data.ReadingPercent;
         itemVM.Available = data.Available;
         itemVM.ClearDirty();
      }

      public static string GetCachePath(this ItemVM itemVM) => $"{Helpers.Paths.FilesCache}/{itemVM.EscapedID}";

   }

}
