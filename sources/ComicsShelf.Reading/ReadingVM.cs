using ComicsShelf.Observables;
using ComicsShelf.Store;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ComicsShelf.Reading
{
   public partial class ReadingVM : BaseVM
   {

      IStoreService Store { get => DependencyService.Get<IStoreService>(); }

   }
}
