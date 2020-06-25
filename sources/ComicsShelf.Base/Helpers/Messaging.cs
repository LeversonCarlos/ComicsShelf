using System;
using Xamarin.Forms;

namespace ComicsShelf.Helpers
{
   public class Messaging
   {

      public static void Send<T>(string key, T data) =>
         MessagingCenter.Send<Application, T>(Application.Current, key, data);

      public static void Subscribe<T>(string key, Action<T> callback) =>
         MessagingCenter.Subscribe<Application, T>(Application.Current, key, (app, data) => callback(data));

      public static void Unsubscribe(string key) =>
         MessagingCenter.Unsubscribe<Application>(Application.Current, key);

   }
}
