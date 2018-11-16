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
      {
         Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
         {
            MessagingCenter.Send<Application, T>(Application.Current, message.ToString(), data);
         });
      }

      public static void Subscribe(Keys message, System.Action<Application> callback)
      { MessagingCenter.Subscribe<Application>(Application.Current, message.ToString(), callback); }

      public static void Subscribe<T>(Keys message, System.Action<T> callback)
      { MessagingCenter.Subscribe<Application, T>(Application.Current, message.ToString(),  (app, data)=> { callback(data); }); }

   }
}
