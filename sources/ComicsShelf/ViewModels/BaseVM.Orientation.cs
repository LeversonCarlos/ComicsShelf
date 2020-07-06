namespace ComicsShelf.ViewModels
{
   partial class BaseVM
   {

      Xamarin.Essentials.DisplayOrientation _DisplayOrientation = Xamarin.Essentials.DisplayOrientation.Unknown;
      public Xamarin.Essentials.DisplayOrientation DisplayOrientation
      {
         get => _DisplayOrientation;
         set
         {
            IsPortraitOrientation = value == Xamarin.Essentials.DisplayOrientation.Portrait;
            IsLandscapeOrientation = value == Xamarin.Essentials.DisplayOrientation.Landscape;
            SetProperty(ref _DisplayOrientation, value);
         }
      }

      bool _IsLandscapeOrientation;
      public bool IsLandscapeOrientation
      {
         get => _IsLandscapeOrientation;
         set => SetProperty(ref _IsLandscapeOrientation, value);
      }

      bool _IsPortraitOrientation;
      public bool IsPortraitOrientation
      {
         get => _IsPortraitOrientation;
         set => SetProperty(ref _IsPortraitOrientation, value);
      }

      public virtual void OnSizeAllocated(double width, double height)
      {
         if (width > height)
            DisplayOrientation = Xamarin.Essentials.DisplayOrientation.Landscape;
         else
            DisplayOrientation = Xamarin.Essentials.DisplayOrientation.Portrait;
      }

   }
}