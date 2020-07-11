using ComicsShelf.ViewModels;
using System;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public class Notify
   {

      internal static void Send<T>(string key, T data) =>
         MessagingCenter.Send<Application, T>(Application.Current, key, data);
      internal static void Subscribe<T>(object subscriber, string key, Action<T> callback) =>
         MessagingCenter.Subscribe<Application, T>(subscriber, key, (app, data) => callback(data), Application.Current);
      internal static void Unsubscribe<T>(object subscriber, string key) =>
         MessagingCenter.Unsubscribe<Application, T>(subscriber, key);

      private static void Subscribe<T>(string key, Action<T> callback) =>
         MessagingCenter.Subscribe<Application, T>(Application.Current, key, (app, data) => callback(data));
      private static void Unsubscribe(string key) =>
         MessagingCenter.Unsubscribe<Application>(Application.Current, key);


      const string AppSleepKey = "app.sleep.key";
      public static void AppSleep() =>
         Send(AppSleepKey, DateTime.Now);

      public static void AppSleep(object subscriber, Action<DateTime> callback) =>
         Subscribe(subscriber, AppSleepKey, callback);

      public static void AppSleepUnsubscribe(object subscriber) =>
         Unsubscribe<DateTime>(subscriber, AppSleepKey);


      const string LibraryAddKey = "store.library.add.key";
      public static void LibraryAdd(LibraryVM library) =>
         Send(LibraryAddKey, library);

      public static void LibraryAdd(object subscriber, Action<LibraryVM> callback) =>
         Subscribe(subscriber, LibraryAddKey, callback);

      public static void LibraryAddUnsubscribe(object subscriber) =>
         Unsubscribe<LibraryVM>(subscriber, LibraryAddKey);


      const string LibraryRemoveKey = "store.library.remove.key";
      public static void LibraryRemove(LibraryVM library) =>
         Send(LibraryRemoveKey, library);
      public static void LibraryRemove(System.Action<LibraryVM> callback) =>
         Subscribe(LibraryRemoveKey, callback);
      public static void LibraryRemoveUnsubscribe() =>
         Unsubscribe($"{LibraryRemoveKey}");

      const string ItemsUpdateKey = "store.items.update.key";
      public static void ItemsUpdate(ItemVM[] itemList) =>
         Send($"{ItemsUpdateKey}", itemList);
      public static void ItemsUpdate(System.Action<ItemVM[]> callback) =>
         Subscribe($"{ItemsUpdateKey}", callback);

      const string SectionsUpdateKey = "store.sections.update.key";
      public static void SectionsUpdate(SectionVM[] sectionsList) =>
         Send($"{SectionsUpdateKey}", sectionsList);
      public static void SectionsUpdate(Action<SectionVM[]> callback) =>
         Subscribe($"{SectionsUpdateKey}", callback);
      public static void SectionsUpdateUnsubscribe() =>
         Unsubscribe($"{SectionsUpdateKey}");

      const string MessageKey = "store.message.key";
      public static void Message(LibraryVM library, string message) =>
         Send($"{MessageKey}.{library.ID}", message);
      public static void Message(LibraryVM library, System.Action<string> callback) =>
         Subscribe($"{MessageKey}.{library.ID}", callback);
      public static void MessageUnsubscribe(LibraryVM library) =>
         Unsubscribe($"{MessageKey}.{library.ID}");

      const string ProgressKey = "store.progress.key";
      public static void Progress(LibraryVM library, double progress) =>
         Send($"{ProgressKey}.{library.ID}", progress);
      public static void Progress(LibraryVM library, System.Action<double> callback) =>
         Subscribe($"{ProgressKey}.{library.ID}", callback);
      public static void ProgressUnsubscribe(LibraryVM library) =>
         Unsubscribe($"{ProgressKey}.{library.ID}");

   }
}
