using ComicsShelf.Observables;
using ComicsShelf.ViewModels;
using System;

namespace ComicsShelf.Settings
{
   public class LibrarySetting : ObservableObject, IDisposable
   {

      public LibraryVM Library { get; }

      public LibrarySetting(LibraryVM library)
      {
         Library = library;
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

      public void Dispose()
      {
         throw new NotImplementedException();
      }

   }
}
