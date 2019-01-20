using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   internal class Messaging
    {

      public enum Keys : short { SearchEngine = 0, PageTapped = 1, ScreenSizeChanged = 2 }

      public static void Send(Keys message)
      {
         Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
         {
            MessagingCenter.Send<Application>(Application.Current, message.ToString());
         });
      }

      public static void Send<T>(Keys message, T data)
      { Send("", message, data); }

      public static void Send<T>(string prefix, Keys message, T data)
      {
         Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
         {
            MessagingCenter.Send<Application, T>(Application.Current, $"{prefix}{message.ToString()}", data);
         });
      }

      public static void Subscribe(Keys message, System.Action<Application> callback)
      { MessagingCenter.Subscribe<Application>(Application.Current, message.ToString(), callback); }

      public static void Subscribe<T>(Keys message, System.Action<T> callback)
      { Subscribe("", message, callback); }

      public static void Subscribe<T>(string prefix, Keys message, System.Action<T> callback)
      { MessagingCenter.Subscribe<Application, T>(Application.Current, $"{prefix}{message.ToString()}", (app, data) => { callback(data); }); }

   }
}
