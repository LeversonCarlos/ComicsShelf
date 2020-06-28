using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ComicsShelf.Helpers
{
   public class Insights
   {

      public static void Init(string androidSecret)
      {
         Microsoft.AppCenter.AppCenter.Start(
            $"android={androidSecret};" +
            /*
            "uwp={Your UWP App secret here};" +
            "ios={Your iOS App secret here}" +
            */
            "",
            typeof(Analytics), typeof(Crashes));
      }

      public static void TrackEvent(string text) =>
         TrackEvent(text, new string[] { });

      public static void TrackEvent(string text, params string[] propertyList) =>
         TrackEvent(text, GetDictionaryProperties(propertyList));

      public static void TrackEvent(string text, Dictionary<string, string> properties)
      {
         Analytics.TrackEvent($"New App: {text}", properties);

         Console.ForegroundColor = ConsoleColor.Blue;
         Console.WriteLine(text);
         if (properties?.Count > 0)
         {
            foreach (var property in properties)
            { Console.WriteLine($"{property.Key} : {property.Value}"); }
         }
         Console.ResetColor();
      }


      public static void TrackException(Exception ex, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
      {

         var properties = new Dictionary<string, string> {
            { "CallerMemberName", callerMemberName }, { "CallerFilePath", callerFilePath }, { "CallerLineNumber", callerLineNumber.ToString() }
         };

         var exceptionList = new List<string>();
         Action<Exception> getExceptions = null;
         getExceptions = new Action<Exception>(e =>
         {
            if (e == null) { return; }
            exceptionList.Add(ex.Message);
            getExceptions(ex.InnerException);
         });
         getExceptions(ex);

         var eIndex = 0;
         exceptionList.ForEach(e => properties.Add($"Exception {eIndex++}", e));

         AddMetrics(properties);
         Crashes.TrackError(ex, properties);
         Analytics.TrackEvent("Tracked Exception", properties);
         TrackEvent("Tracked Exception", properties);
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

      private static Dictionary<string, string> GetDictionaryProperties(params string[] propertyList)
      {
         try
         {
            if (propertyList == null || propertyList.Length == 0) { return new Dictionary<string, string>(); }
            var properties = propertyList
               .Select(prop => prop.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries))
               .Select(prop => new
               {
                  Key = (prop.Length == 2 ? prop[0] : "Property"),
                  Value = (prop.Length == 2 ? prop[1] : string.Join(":", prop))
               })
               .ToDictionary(k => k.Key, v => v.Value);
            return properties;
         }
         catch { return null; }
      }

   }
}
