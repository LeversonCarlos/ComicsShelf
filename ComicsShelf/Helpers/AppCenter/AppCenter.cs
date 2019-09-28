using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ComicsShelf.Helpers
{
   public partial class AppCenter
   {

      public static void Initialize()
      {
         Microsoft.AppCenter.AppCenter.Start(
            $"android={AppCenter_androidSecret};" +
            /*
            "uwp={Your UWP App secret here};" +
            "ios={Your iOS App secret here}" +
            */
            "",
            typeof(Analytics), typeof(Crashes));
      }

      public static void TrackEvent(string text)
      { TrackEvent(text, new string[] { }); }

      public static void TrackEvent(string text, params string[] properties)
      {
         var trackProp = properties
            .Select(prop => prop.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries))
            .Where(prop => prop.Length == 2)
            .Select(prop => new { Key = prop[0], Value = prop[1] })
            .ToDictionary(k => k.Key, v => v.Value);
         AddMetrics(trackProp);
         TrackEvent(text, trackProp);
      }

      public static void TrackEvent(Exception ex, [CallerMemberName]string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber]int callerLineNumber = 0)
      {
         var trackProp = new Dictionary<string, string> { { "Exception", ex.Message }, { "CallerMemberName", callerMemberName }, { "CallerFilePath", callerFilePath }, { "CallerLineNumber", callerLineNumber.ToString() }, { "ExceptionDetails", ex.ToString() } };
         if (ex.InnerException != null) { trackProp.Add("InnerException", ex.InnerException.Message); }
         AddMetrics(trackProp);
         Crashes.TrackError(ex, trackProp);
      }

      public static void TrackEvent(string text, Dictionary<string, string> properties)
      { Analytics.TrackEvent(text, properties); }

      private static void AddMetrics(Dictionary<string, string> properties)
      {
         try
         {
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            properties.Add("WorkingSet (in bytes)", $"{currentProcess.WorkingSet64}");
            properties.Add("PrivateMemorySize (in bytes)", $"{currentProcess.PrivateMemorySize64}");
            properties.Add("PagedMemorySize (in bytes)", $"{currentProcess.PagedMemorySize64}");
            properties.Add("PagedSystemMemorySize (in bytes)", $"{currentProcess.PagedSystemMemorySize64}");
            properties.Add("UserProcessorTime (in seconds)", $"{currentProcess.UserProcessorTime.TotalSeconds}");
            properties.Add("TotalProcessorTime (in seconds)", $"{currentProcess.TotalProcessorTime.TotalSeconds}");
         }
         catch { }
         finally
         { properties.Add("Device", $"{Xamarin.Essentials.DeviceInfo.Name}"); }
      }

   }
}
