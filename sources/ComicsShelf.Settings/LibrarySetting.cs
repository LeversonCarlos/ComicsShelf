using ComicsShelf.Observables;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Settings
{
   public class LibrarySetting : ObservableObject, IDisposable
   {

      public LibraryVM Library { get; }

      public LibrarySetting(LibraryVM library)
      {
         Library = library;
         this.RemoveCommand = new Command(async () => await this.Remove());
      }

      string _Message;
      public string Message
      {
         get => _Message;
         set => SetProperty(ref _Message, value);
      }

      bool _HasMessage;
      public bool HasMessage
      {
         get => _HasMessage;
         set => SetProperty(ref _HasMessage, value);
      }

      double _Progress;
      public double Progress
      {
         get => _Progress;
         set => SetProperty(ref _Progress, value);
      }

      bool _HasProgress;
      public bool HasProgress
      {
         get => _HasProgress;
         set => SetProperty(ref _HasProgress, value);
      }

      public Command RemoveCommand { get; set; }
      async Task Remove()
      {
         if (!await Helpers.App.ConfirmMessage(string.Format(R.Settings.REMOVE_LIBRARY_CONFIRMATION_MESSAGE, this.Library.Description))) { return; }
         await DependencyService.Get<IStoreService>().RemoveLibraryAsync(this.Library);
      }

      public void Dispose()
      {
         throw new NotImplementedException();
      }

   }
}
