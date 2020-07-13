using ComicsShelf.ViewModels;
using System;

namespace ComicsShelf.Firebase
{

   public class FirebaseItemVM
   {
      public string ID { get; set; }

      public short? Rating { get; set; }
      public bool? Readed { get; set; }
      public DateTime? ReadingDate { get; set; }
      public short? ReadingPage { get; set; }
      public double ReadingPercent { get; set; }
   }

   public static class FirebaseItemVMExtention
   {

      public static FirebaseItemVM ToFirebaseItem(this ItemVM itemVM) =>
         new FirebaseItemVM
         {
            ID = itemVM.ID,
            Rating = itemVM.Rating,
            Readed = itemVM.Readed,
            ReadingDate = itemVM.ReadingDate,
            ReadingPage = itemVM.ReadingPage,
            ReadingPercent = itemVM.ReadingPercent
         };

      /*
      public static ItemVM ToLibraryItem(this FirebaseItemVM itemVM) =>
         new ItemVM
         {
            ID = itemVM.ID,
            Rating = itemVM.Rating,
            Readed = itemVM.Readed.HasValue ? itemVM.Readed.Value : false,
            ReadingDate = itemVM.ReadingDate,
            ReadingPage = itemVM.ReadingPage,
            ReadingPercent = itemVM.ReadingPercent
         };
      */

   }
}
