using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ComicsShelf.Helpers
{
   internal class AppCenter
   {

      public static void Initialize()
      {
         Microsoft.AppCenter.AppCenter.Start(
            "android=4ebe7891-1962-4e2a-96c4-c37a7c06c104;" +
            /*
            "uwp={Your UWP App secret here};" +
            "ios={Your iOS App secret here}" +
            */
            "",
            typeof(Analytics), typeof(Crashes));
      }

      public static void TrackEvent(string text)
      { TrackEvent(text, new Dictionary<string, string> { }); }

      public static void TrackEvent(string text, params string[] properties)
      {
         var dict = properties
            .Select(prop => prop.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries))
            .Where(prop => prop.Length == 2)
            .Select(prop => new { Key = prop[0], Value = prop[1] })
            .ToDictionary(k => k.Key, v => v.Value);
         dict.Add("Device", $"{Xamarin.Essentials.DeviceInfo.Name}");
         TrackEvent(text, dict);
      }

      public static void TrackEvent(Exception ex, [CallerMemberName]string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber]int callerLineNumber = 0)
      {
         var trackProp = new Dictionary<string, string> { { "Exception", ex.Message }, { "CallerMemberName", callerMemberName }, { "CallerFilePath", callerFilePath }, { "CallerLineNumber", callerLineNumber.ToString() }, { "ExceptionDetails", ex.ToString() } };
         if (ex.InnerException != null) { trackProp.Add("InnerException", ex.InnerException.Message); }
         trackProp.Add("Device", $"{Xamarin.Essentials.DeviceInfo.Name}");
         Crashes.TrackError(ex, trackProp);
      }

      public static void TrackEvent(string text, Dictionary<string, string> properties)
      { Analytics.TrackEvent(text, properties); }

   }
}
