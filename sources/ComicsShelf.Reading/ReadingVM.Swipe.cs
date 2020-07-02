using ComicsShelf.ViewModels;
using System.Linq;

namespace ComicsShelf.Reading
{
   partial class ReadingVM
   {

      bool _IsSwipeEnabled;
      public bool IsSwipeEnabled
      {
         get => _IsSwipeEnabled;
         set => SetProperty(ref _IsSwipeEnabled, value);
      }

      bool _ScrollComplete;
      public bool ScrollComplete
      {
         get => _ScrollComplete;
         set
         {
            _ScrollComplete = value;
            IsSwipeEnabled = GetSwipeEnabled();
         }
      }

      bool GetSwipeEnabled()
      {
         if (ScreenSize == null) return true;
         if (ScreenSize.Orientation == PageSizeVM.OrientationEnum.Landscape) return true;
         var imageSize = PagesList?.Where(x => x.Index == ReadingPage)?.Select(x => x.PageSize)?.FirstOrDefault();
         if (imageSize == null) return true;
         if (imageSize.Orientation == PageSizeVM.OrientationEnum.Portrait) return true;
         if (ScrollComplete) return true;
         return false;
      }

   }
}
