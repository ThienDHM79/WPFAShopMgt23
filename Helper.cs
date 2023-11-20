using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WPFAShopMgt23
{
    public class Helper
    {
        public static void PreviewTextInputQty(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        public static bool ProductValidation(string? ProductName, bool CategoriesHasItem)
        {
            bool valid = true;
            if (ProductName == null || ProductName == "")
            {
                valid = false;
                MessageBox.Show("Product Name is not null");
            }
            if (CategoriesHasItem == false)
            {
                valid = false;
                MessageBox.Show("Product Category is not null");
            }
            return valid;
        }


    }
}
