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

      public static void TrackEvent(string text, params string[] propertyList)
      {
         var properties = propertyList
            .Select(prop => prop.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries))
            .Where(prop => prop.Length == 2)
            .Select(prop => new { Key = prop[0], Value = prop[1] })
            .ToDictionary(k => k.Key, v => v.Value);
         TrackEvent(text, properties);
      }

      public static void TrackEvent(Exception ex, [CallerMemberName]string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber]int callerLineNumber = 0)
      {
         var properties = new Dictionary<string, string> { { "Exception", ex.Message }, { "CallerMemberName", callerMemberName }, { "CallerFilePath", callerFilePath }, { "CallerLineNumber", callerLineNumber.ToString() }, { "ExceptionDetails", ex.ToString() } };
         if (ex.InnerException != null) { properties.Add("InnerException", ex.InnerException.Message); }
         AddMetrics(properties);
         Crashes.TrackError(ex, properties);
      }

      public static void TrackEvent(string text, Dictionary<string, string> properties)
      {
         AddMetrics(properties);
         Analytics.TrackEvent(text, properties);
      }

      private static void AddMetrics(Dictionary<string, string> properties)
      {
         try
         {
            var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            properties.Add("Memory KB", $"{(int)(currentProcess.WorkingSet64 * 0.001)}");
            properties.Add("CPU %", $"{Math.Round((double)currentProcess.UserProcessorTime.Ticks / (double)currentProcess.TotalProcessorTime.Ticks * (double)100, 1)}");
         }
         catch { }
         finally
         { properties.Add("Device", $"{Xamarin.Essentials.DeviceInfo.Name}"); }
      }

   }
}
