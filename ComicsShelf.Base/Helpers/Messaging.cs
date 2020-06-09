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

      public static void Send<T>(string key, T data)
      {
         Xamarin.Essentials.MainThread.BeginInvokeOnMainThread(() =>
            MessagingCenter.Send<Application, T>(Application.Current, key, data)
         );
      }

      public static void Subscribe(string key, Action<Application> callback) =>
         MessagingCenter.Subscribe<Application>(Application.Current, key, callback);

      public static void Subscribe<T>(string key, Action<T> callback) =>
         MessagingCenter.Subscribe<Application, T>(Application.Current, key, (app, data) => { callback(data); });

   }
}
