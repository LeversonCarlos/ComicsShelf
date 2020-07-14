using ComicsShelf.ViewModels;
using System;
using System.Threading;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public class Notify
   {


      static SemaphoreSlim sendSemaphore = new SemaphoreSlim(1, 1);
      internal static async void Send<T>(string key, T data)
      {
         try
         {
            await sendSemaphore.WaitAsync();
            MessagingCenter.Send<Application, T>(Application.Current, key, data);
         }
         catch (Exception ex) { Helpers.Insights.TrackException(ex); }
         finally { sendSemaphore.Release(); }
      }

      internal static void Subscribe<T>(object subscriber, string key, Action<T> callback) =>
         MessagingCenter.Subscribe<Application, T>(subscriber, key, (app, data) => callback(data), Application.Current);
      internal static void Unsubscribe<T>(object subscriber, string key) =>
         MessagingCenter.Unsubscribe<Application, T>(subscriber, key);


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
      public static void LibraryRemove(object subscriber, Action<LibraryVM> callback) =>
         Subscribe(subscriber, LibraryRemoveKey, callback);
      public static void LibraryRemoveUnsubscribe(object subscriber) =>
         Unsubscribe<LibraryVM>(subscriber, LibraryRemoveKey);


      const string LibraryDataKey = "store.library.data.key";
      public static void LibraryData(LibraryVM library) =>
         Send(LibraryDataKey, library);
      public static void LibraryData(object subscriber, Action<LibraryVM> callback) =>
         Subscribe(subscriber, LibraryDataKey, callback);
      public static void LibraryDataUnsubscribe(object subscriber) =>
         Unsubscribe<LibraryVM>(subscriber, LibraryDataKey);


      const string ItemsUpdateKey = "store.items.update.key";
      public static void ItemsUpdate(ItemVM[] itemList) =>
         Send(ItemsUpdateKey, itemList);
      public static void ItemsUpdate(object subscriber, Action<ItemVM[]> callback) =>
         Subscribe(subscriber, ItemsUpdateKey, callback);
      public static void ItemsUpdateUnsubscribe(object subscriber) =>
         Unsubscribe<ItemVM[]>(subscriber, ItemsUpdateKey);


      const string SectionsUpdateKey = "store.sections.update.key";
      public static void SectionsUpdate(SectionVM[] sectionsList) =>
         Send(SectionsUpdateKey, sectionsList);
      public static void SectionsUpdate(object subscriber, Action<SectionVM[]> callback) =>
         Subscribe(subscriber, SectionsUpdateKey, callback);
      public static void SectionsUpdateUnsubscribe(object subscriber) =>
         Unsubscribe<SectionVM[]>(subscriber, SectionsUpdateKey);


      const string MessageKey = "store.message.key";
      public static void Message(LibraryVM library, string message) =>
         Send($"{MessageKey}.{library.ID}", message);
      public static void Message(object subscriber, LibraryVM library, Action<string> callback) =>
         Subscribe(subscriber, $"{MessageKey}.{library.ID}", callback);
      public static void MessageUnsubscribe(object subscriber, LibraryVM library) =>
         Unsubscribe<LibraryVM>(subscriber, $"{MessageKey}.{library.ID}");


      const string ProgressKey = "store.progress.key";
      public static void Progress(LibraryVM library, double progress) =>
         Send($"{ProgressKey}.{library.ID}", progress);
      public static void Progress(object subscriber, LibraryVM library, Action<double> callback) =>
         Subscribe(subscriber, $"{ProgressKey}.{library.ID}", callback);
      public static void ProgressUnsubscribe(object subscriber, LibraryVM library) =>
         Unsubscribe<LibraryVM>(subscriber, $"{ProgressKey}.{library.ID}");


      const string ReadingStartKey = "reading.start.key";
      public static void ReadingStart() =>
         Send(ReadingStartKey, DateTime.Now);
      public static void ReadingStart(object subscriber, Action<DateTime> callback) =>
         Subscribe(subscriber, ReadingStartKey, callback);
      public static void ReadingStartUnsubscribe(object subscriber) =>
         Unsubscribe<DateTime>(subscriber, ReadingStartKey);


   }
}
