using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Settings
{
   public class LibrarySetting : ObservableObject
   {

      public LibraryVM Library { get; }

      public LibrarySetting(LibraryVM library)
      {
         Library = library;
         this.RemoveCommand = new Command(async () => await this.Remove());
         Helpers.Notify.Message(library, message =>
         {
            this.Message = message;
            this.HasMessage = !string.IsNullOrEmpty(message);
         });
         Helpers.Notify.Progress(library, progress =>
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
         if (!await Helpers.Message.Confirm(string.Format(Resources.Translations.SETTINGS_DRIVE_REMOVE_CONFIRMATION_MESSAGE, Library.Description))) { return; }
         await DependencyService.Get<IStoreService>().RemoveLibraryAsync(Library);
      }

      public override void Dispose()
      {
         Helpers.Notify.MessageUnsubscribe(Library);
         Helpers.Notify.ProgressUnsubscribe(Library);
      }

   }
}
