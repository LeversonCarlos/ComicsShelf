﻿using ComicsShelf.Screens.Splash;
using ComicsShelf.Store;
using ComicsShelf.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Reading
{
   public partial class ReadingVM : BaseVM
   {
      IStoreService Store { get => DependencyService.Get<IStoreService>(); }
      Helpers.InsightsLogger _Log { get; set; }

      public ReadingVM(ItemVM item, PageVM[] pagesList)
      {
         IsBusy = true;
         Item = item;
         PagesList = new ObservableList<PageVM>();
         PagesArray = pagesList.Union(new PageVM[] { new PageVM { } }).ToArray();
         BackCommand = new Command(param => Helpers.Modal.Pop());
      }

      public Command BackCommand { get; }
      readonly PageVM[] PagesArray;
      public ItemVM Item { get; }
      public ObservableList<PageVM> PagesList { get; }

      PageSizeVM _ScreenSize;
      public PageSizeVM ScreenSize
      {
         get => _ScreenSize;
         set
         {
            SetProperty(ref _ScreenSize, value);
            ScrollComplete = false;
            IsSwipeEnabled = GetSwipeEnabled();
         }
      }

      public override Task OnAppearing()
      {
         try
         {
            _Log = new Helpers.InsightsLogger($"{Store?.GetLibrary(Item?.LibraryID)?.Type} Reading Screen");
            Helpers.Notify.ReadingStart();
            Helpers.Notify.AppSleep(this, async now => await Helpers.Modal.Pop());
            Task.Run(async () =>
            {
               await PagesList.ReplaceRangeAsync(PagesArray);
               ReadingPage = Item.ReadingPage.HasValue ? Item.ReadingPage.Value : (short)0;
               IsBusy = false;
            });
         }
         catch (Exception ex) { _Log?.Add(ex); }
         return base.OnAppearing();
      }

      public override async Task OnDisappearing()
      {
         try
         {
            Helpers.Notify.AppSleepUnsubscribe(this);
            if (Item.IsDirty)
               await Store.UpdateItemAsync(Item);
         }
         catch (Exception ex) { _Log?.Add(ex); }
         finally { _Log?.Dispose(); }
      }

   }
}
