using System;
using Xamarin.Forms;

namespace ComicsShelf.Base.Helpers
{
   public class Messaging
   {

      public static void Send(string key)
      {
         Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            MessagingCenter.Send<Application>(Application.Current, key)
         );
      }

      public static void Subscribe(string key, Action<Application> callback) =>
         MessagingCenter.Subscribe<Application>(Application.Current, key, callback);

   }
}
