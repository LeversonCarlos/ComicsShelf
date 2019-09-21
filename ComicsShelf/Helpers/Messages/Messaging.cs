using Xamarin.Forms;

namespace ComicsShelf
{
   internal class Messaging
   {
      public enum enMessages { LibraryAdded, LibraryRemoved }

      public static void Send(string key)
      {
         Device.BeginInvokeOnMainThread(() =>
         {
            MessagingCenter.Send<Application>(Application.Current, key);
         });
      }

      public static void Send<T>(string key, T data)
      { Send("", key, data); }

      public static void Send<T>(string prefix, string key, T data)
      {
         Device.BeginInvokeOnMainThread(() =>
         {
            MessagingCenter.Send<Application, T>(Application.Current, $"{prefix}{key}", data);
         });
      }

      public static void Subscribe(string key, System.Action<Application> callback)
      { MessagingCenter.Subscribe<Application>(Application.Current, key, callback); }

      public static void Subscribe<T>(string key, System.Action<T> callback)
      { Subscribe("", key, callback); }

      public static void Subscribe<T>(string prefix, string key, System.Action<T> callback)
      { MessagingCenter.Subscribe<Application, T>(Application.Current, $"{prefix}{key}", (app, data) => { callback(data); }); }

      public static void Unsubscribe<T>(string key)
      { Unsubscribe<T>("", key); }

      public static void Unsubscribe<T>(string prefix, string key)
      { MessagingCenter.Unsubscribe<Application, T>(Application.Current, $"{prefix}{key}"); }

   }
}
