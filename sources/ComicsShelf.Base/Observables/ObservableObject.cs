using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ComicsShelf.Observables
{
   public class ObservableObject : INotifyPropertyChanged
   {

      public ObservableObject()
      {
         IsDirty = false;
      }

      public bool IsDirty { get; private set; }
      public void ClearDirty() => IsDirty = false;

      protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action onChanged = null)
      {
         if (EqualityComparer<T>.Default.Equals(backingStore, value))
         { return false; }

         backingStore = value;
         onChanged?.Invoke();
         OnPropertyChanged(propertyName);
         IsDirty = true;
         return true;
      }

      public event PropertyChangedEventHandler PropertyChanged;
      protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
      {
         var changed = PropertyChanged;
         if (changed == null)
            return;

         changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
      }

   }
}