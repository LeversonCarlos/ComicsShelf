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
         Notifyers.Notify.Message(library, message =>
         {
            this.Message = message;
            this.HasMessage = !string.IsNullOrEmpty(message);
         });
         Notifyers.Notify.Progress(library, progress =>
         {
            this.Progress = progress;
            this.HasProgress = progress > 0 && progress < 1;
         });
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
         if (!await Helpers.App.ConfirmMessage(string.Format(Strings.REMOVE_LIBRARY_CONFIRMATION_MESSAGE, Library.Description))) { return; }
         await DependencyService.Get<IStoreService>().RemoveLibraryAsync(Library);
      }

      public void Dispose()
      {
         Notifyers.Notify.MessageUnsubscribe(Library);
         Notifyers.Notify.ProgressUnsubscribe(Library);
      }

   }
}
