using ComicsShelf.Screens.Reading;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Splash
{
   partial class SplashVM
   {

      bool _IsOpening;
      public bool IsOpening
      {
         get => _IsOpening;
         set => SetProperty(ref _IsOpening, value);
      }

      public Command OpenCommand { get; }
      async Task OpenAsync()
      {
         try
         {
            IsOpening = true;

            var pagesExtraction = await Engine.PagesExtraction.Service.ExecuteAsync(SelectedItem);
            if (!pagesExtraction) return;

            var pagesList = await Engine.PagesExtraction.Service.GetPagesAsync(SelectedItem);
            await Shell.Current.GoToAsync(new ReadingVM(SelectedItem, pagesList));

         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         finally { IsOpening = false; }
      }

   }
}
