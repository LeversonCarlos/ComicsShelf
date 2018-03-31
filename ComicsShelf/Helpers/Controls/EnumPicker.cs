using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;

namespace ComicsShelf.Helpers.Controls
{

   public class EnumPicker<T> : Picker where T : struct
   {

      #region New
      public EnumPicker()
      {
         this.SelectedIndexChanged += this.OnSelectedIndexChanged;
         this.enumData = new List<EnumPickerItem>();
         this.Initialize();
      }
      #endregion

      #region Properties
      private List<EnumPickerItem> enumData { get; set; }
      private Type enumType { get; set; }
      #endregion

      #region Initialize

      private void Initialize()
      {
         try
         {

            // ASSEMBLY
            this.enumType = typeof(T);
            // var enumAssembly = (Assembly)((dynamic)enumType).Assembly;
            // var resourceManager = this.Initialize_GetResourceManager(enumAssembly);

            // DATA
            var enumName = enumType.Name.ToUpper();
            var enumValues = (short[])Enum.GetValues(enumType);
            var enumTexts = (string[])Enum.GetNames(enumType);

            // LOOP
            for (int i = 0; i < enumValues.Length; i++)
            {
               var enumValue = enumValues[i];
               var enumAttribute = string.Format("ENUM_{0}_{1}", enumName, enumTexts[i].ToUpper());
               var enumText = R.Strings.ResourceManager.GetString(enumAttribute);
               if (string.IsNullOrEmpty(enumText)) { enumText = enumAttribute; }

               this.enumData.Add(new EnumPickerItem
               {
                  Value = enumValue,
                  Text = enumText
               });
               this.Items.Add(enumText);

            }

            // resourceManager = null;
         }
         catch { }
      }

      /*
      private System.Resources.ResourceManager Initialize_GetResourceManager(Assembly enumAssembly)
      {
         foreach (var definedType in enumAssembly.DefinedTypes)
         {
            if (definedType.Name == "Strings")
            {
               var stringType = definedType.AsType();
               var resourceManager = new global::System.Resources.ResourceManager(stringType.FullName, enumAssembly);
               return resourceManager;
            }
         }
         return null;
      }
      */

      #endregion

      #region SelectedItem
      public static BindableProperty SelectedItemProperty =
         BindableProperty.Create(nameof(SelectedItem),
            typeof(T), typeof(EnumPicker<T>), default(T),
            propertyChanged: OnSelectedItemChanged, defaultBindingMode: BindingMode.TwoWay);
      public T SelectedItem
      {
         get { return (T)GetValue(SelectedItemProperty); }
         set { SetValue(SelectedItemProperty, value); }
      }
      #endregion

      #region OnSelectedIndexChanged
      private void OnSelectedIndexChanged(object sender, EventArgs eventArgs)
      {
         if (SelectedIndex < 0 || SelectedIndex > Items.Count - 1)
         { SelectedItem = default(T); }
         else
         {
            var selectedText = this.Items[SelectedIndex];
            var selectedValue = this.enumData.Where(x => x.Text == selectedText).Select(x => x.Value).FirstOrDefault();
            this.SelectedItem = (T)Enum.Parse(this.enumType, selectedValue.ToString());
         }
      }
      #endregion

      #region OnSelectedItemChanged
      private static void OnSelectedItemChanged(BindableObject bindable, object oldvalue, object newvalue)
      {
         var picker = bindable as EnumPicker<T>;
         if (newvalue != null)
         {
            var selectedText = picker.enumData.Where(x => x.Value == (short)newvalue).Select(x => x.Text).FirstOrDefault();
            picker.SelectedIndex = picker.Items.IndexOf(selectedText);
         }
      }
      #endregion

   }

   internal class EnumPickerItem
   {
      public short Value { get; set; }
      public string Text { get; set; }
   }

}