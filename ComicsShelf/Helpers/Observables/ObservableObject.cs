using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ComicsShelf.Helpers.Observables
{
   public class ObservableObject : INotifyPropertyChanged
   {

      #region SetProperty
      protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null)
      {
         if (EqualityComparer<T>.Default.Equals(backingStore, value))
            return false;

         backingStore = value;
         onChanged?.Invoke();
         OnPropertyChanged(propertyName);
         return true;
      }
      #endregion

      #region OnPropertyChanged
      public event PropertyChangedEventHandler PropertyChanged;
      protected void OnPropertyChanged([CallerMemberName]string propertyName = "")
      {
         var changed = PropertyChanged;
         if (changed == null)
            return;

         changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }
      #endregion

   }
}