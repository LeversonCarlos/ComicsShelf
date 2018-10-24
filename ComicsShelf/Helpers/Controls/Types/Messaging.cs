using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{
   internal class Messaging
    {

      public enum Keys : short { PageTapped = 0 }

      public static void Send(Keys message)
      {
         Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
         {
            MessagingCenter.Send<Application>(Application.Current, message.ToString());
         });
      }

      public static void Subscribe(Keys message, System.Action<Application> callback)
      { MessagingCenter.Subscribe<Application>(Application.Current, message.ToString(), callback); }

   }
}
