using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ComicsShelf.Helpers.Observables
{
   public class ObservableObject : INotifyPropertyChanged
   {

      protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null, bool AlwaysInvokePropertyChanged = false)
      {
         if (EqualityComparer<T>.Default.Equals(backingStore, value))
         {
            if (AlwaysInvokePropertyChanged) { OnPropertyChanged(propertyName); }
            return false;
         }

         backingStore = value;
         onChanged?.Invoke();
         OnPropertyChanged(propertyName);
         return true;
      }

      public event PropertyChangedEventHandler PropertyChanged;
      protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
      {
         var changed = PropertyChanged;
         if (changed == null)
            return;

         changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

   }
}