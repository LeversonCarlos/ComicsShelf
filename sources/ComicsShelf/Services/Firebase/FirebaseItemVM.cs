using ComicsShelf.ViewModels;
using System;

namespace ComicsShelf.Firebase
{

   public class FirebaseItemVM
   {
      public string ID { get; set; }

      public long? SizeInBytes { get; set; }
      public DateTime ReleaseDate { get; set; }

      public short? Rating { get; set; }
      public bool Readed { get; set; }
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
            SizeInBytes = itemVM.SizeInBytes,
            ReleaseDate = itemVM.ReleaseDate,
            Rating = itemVM.Rating,
            Readed = itemVM.Readed,
            ReadingDate = itemVM.ReadingDate,
            ReadingPage = itemVM.ReadingPage,
            ReadingPercent = itemVM.ReadingPercent
         };

   }
}
