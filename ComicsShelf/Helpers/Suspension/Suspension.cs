using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ComicsShelf
{
   partial class App
   {

      public static bool IsSleeping { get; set; } = false;

      bool Suspending { get; set; } = false;
      System.Timers.Timer SuspendingTimer;
      void SuspendingTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
      {
         this.SuspendingTimer.Stop();
         if (this.Suspending) { App.IsSleeping = true; }
         this.SuspendingTimer.Elapsed -= this.SuspendingTimer_Elapsed;
         this.SuspendingTimer.Dispose();
         this.SuspendingTimer = null;
      }

      void DoSleep()
      {
         try
         {
            if (this.Suspending) { return; }
            this.Suspending = true;

            if (this.SuspendingTimer != null) { return; }
            this.SuspendingTimer = new System.Timers.Timer(10000);
            this.SuspendingTimer.AutoReset = false;
            this.SuspendingTimer.Elapsed += this.SuspendingTimer_Elapsed;
            this.SuspendingTimer.Start();
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

      void DoResume()
      {
         try
         {
            this.Suspending = false;

            if (this.SuspendingTimer != null)
            {
               this.SuspendingTimer.Stop();
               this.SuspendingTimer.Elapsed -= this.SuspendingTimer_Elapsed;
               this.SuspendingTimer.Dispose();
               this.SuspendingTimer = null;
            }

            if (App.IsSleeping)
            {
               App.IsSleeping = false;
               var store = DependencyService.Get<Store.ILibraryStore>();
               foreach (var library in store.Libraries)
               { Services.LibraryService.RefreshLibrary(library); }
            }
         }
         catch (Exception ex) { Helpers.AppCenter.TrackEvent(ex); }
      }

   }
}
