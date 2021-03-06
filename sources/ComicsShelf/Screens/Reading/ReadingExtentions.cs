﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ComicsShelf.Screens.Reading
{
   public static class ReadingExtentions
   {

      internal static ReadingVM Instance { get; set; }

      public static string GetRoute() => "reading";
      public static Type GetPageType() => typeof(Reading.Page);

      public static Task GoToAsync(this Shell shell, ReadingVM viewModel)
      {
         ReadingExtentions.Instance = viewModel;
         var navState = new ShellNavigationState(ReadingExtentions.GetRoute());
         return shell.GoToAsync(navState);
      }

   }
}
