using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Settings
{
   public class SettingsLibraryVM : ObservableObject
   {

      public LibraryVM Library { get; }

      public SettingsLibraryVM(LibraryVM library)
      {
         Library = library;
         RemoveCommand = new Command(async () => await Remove());
         Helpers.Notify.Message(library, message => Message = message);
         Helpers.Notify.Progress(library, progress => Progress = progress);
      }

      string _Message;
      public string Message
      {
         get => _Message;
         set
         {
            SetProperty(ref _Message, value);
            HasMessage = !string.IsNullOrEmpty(value);
         }
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
         set
         {
            SetProperty(ref _Progress, value);
            HasProgress = value > 0 && value < 1;
         }
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
