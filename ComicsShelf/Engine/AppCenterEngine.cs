using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System.Collections.Generic;

namespace ComicsShelf.Engine
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
      { Analytics.TrackEvent(text); }

      public static void TrackEvent(string text, string propertyKey, string propertyValue)
      { Analytics.TrackEvent(text, new Dictionary<string, string> { { propertyKey, propertyValue } }); }

      public static void TrackEvent(string text, Dictionary<string, string> properties)
      { Analytics.TrackEvent(text, properties); }

   }
}
