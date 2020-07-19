using System;
using System.Collections.Generic;

namespace ComicsShelf.Helpers
{
   internal class InsightsLogger : IDisposable
   {

      readonly string _Name;
      readonly DateTime _StartTime;
      readonly List<string> _PropertyList;

      public InsightsLogger(string name)
      {
         _Name = name;
         _StartTime = DateTime.Now;
         _PropertyList = new List<string>();
      }

      int _DurationFactor = 1;
      public void SetDurationFactor(int value)
      {
         if (value < 1) value = 1;
         _DurationFactor = value;
      }

      public void Add(string value) =>
         _PropertyList.Add(value);

      public void Add(Exception ex) =>
         Add("Exception", ex.ToString());

      public void Add(string key, string value)
      {
         _PropertyList.Add($"{key}:{value}");
         Insights.TrackMetric(_Name, key, value);
      }


      void Write()
      {
         try
         {
            var secondsDuration = (int)Math.Round(DateTime.Now.Subtract(_StartTime).TotalSeconds / (double)_DurationFactor, 0);
            Add("Seconds", $"{secondsDuration}");
            Insights.TrackEvent(_Name, _PropertyList.ToArray());
         }
         catch { }
      }

      public void Dispose() =>
         Write();

   }
}
