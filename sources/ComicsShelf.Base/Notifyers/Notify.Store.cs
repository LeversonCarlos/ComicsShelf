using ComicsShelf.ViewModels;
using System;
using Xamarin.Forms;

namespace ComicsShelf.Notifyers
{
   partial class Notify
   {

      const string LibraryAddKey = "store.library.add.key";
      public static void LibraryAdd(LibraryVM library) =>
         Device.BeginInvokeOnMainThread(() => Helpers.Messaging.Send(LibraryAddKey, library));
      public static void LibraryAdd(System.Action<LibraryVM> callback) =>
         Helpers.Messaging.Subscribe(LibraryAddKey, callback);

      const string LibraryRemoveKey = "store.library.remove.key";
      public static void LibraryRemove(LibraryVM library) =>
         Helpers.Messaging.Send(LibraryRemoveKey, library);
      public static void LibraryRemove(System.Action<LibraryVM> callback) =>
         Helpers.Messaging.Subscribe(LibraryRemoveKey, callback);

      const string ItemsUpdateKey = "store.items.update.key";
      public static void ItemsUpdate(ItemVM[] itemList) =>
         Helpers.Messaging.Send($"{ItemsUpdateKey}", itemList);
      public static void ItemsUpdate(System.Action<ItemVM[]> callback) =>
         Helpers.Messaging.Subscribe($"{ItemsUpdateKey}", callback);

      const string SectionsUpdateKey = "store.sections.update.key";
      public static void SectionsUpdate(FolderVM[] sectionsList) =>
         Helpers.Messaging.Send($"{SectionsUpdateKey}", sectionsList);
      public static void SectionsUpdate(System.Action<FolderVM[]> callback) =>
         Helpers.Messaging.Subscribe($"{SectionsUpdateKey}", callback);

      const string MessageKey = "store.message.key";
      public static void Message(LibraryVM library, string message) =>
         Helpers.Messaging.Send($"{MessageKey}.{library.ID}", message);
      public static void Message(LibraryVM library, System.Action<string> callback) =>
         Helpers.Messaging.Subscribe($"{MessageKey}.{library.ID}", callback);
      public static void MessageUnsubscribe(LibraryVM library) =>
         Helpers.Messaging.Unsubscribe($"{MessageKey}.{library.ID}");

      const string ProgressKey = "store.progress.key";
      public static void Progress(LibraryVM library, double progress) =>
         Helpers.Messaging.Send($"{ProgressKey}.{library.ID}", progress);
      public static void Progress(LibraryVM library, System.Action<double> callback) =>
         Helpers.Messaging.Subscribe($"{ProgressKey}.{library.ID}", callback);
      public static void ProgressUnsubscribe(LibraryVM library) =>
         Helpers.Messaging.Unsubscribe($"{ProgressKey}.{library.ID}");

      const string CoverSliderTimerKey = "cover.slider.timer.key";
      public static void CoverSliderTimer() =>
         Helpers.Messaging.Send($"{CoverSliderTimerKey}", DateTime.Now);
      public static void CoverSliderTimer(System.Action<DateTime> callback) =>
         Helpers.Messaging.Subscribe($"{CoverSliderTimerKey}", callback);
      public static void CoverSliderTimerUnsubscribe() =>
         Helpers.Messaging.Unsubscribe($"{CoverSliderTimerKey}");

   }
}
