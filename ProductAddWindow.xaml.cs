using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WPFAShopMgt23
{
    /// <summary>
    /// Interaction logic for ProductAddWindow.xaml
    /// </summary>
    public partial class ProductAddWindow : Window
    {
        public ProductAddWindow()
        {
            InitializeComponent();
        }

        ShopDbContext _db;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _db = new ShopDbContext();
            ProductCateComboBox.ItemsSource = _db.Categories.ToList();
            ProductCateComboBox.SelectedIndex = 0;
            ProductImagePathTextBox.Text = "/image/default.png";
        }
        private void PreviewTextInputQty(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private async void ProductAddConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            bool valid = true;
            if (  ProductNameTextBox.Text == null || ProductNameTextBox.Text == "")
            {
                valid = false;
                MessageBox.Show("Product Name is not null");
            }
            if ( ProductCateComboBox.HasItems == false )
            {
                valid = false;
                MessageBox.Show("Product Category is not null");
            }
            if (valid)
            {
                int CatePos = ProductCateComboBox.SelectedIndex;
                Category checkCate = (Category) ProductCateComboBox.Items[CatePos];
                String CateName = checkCate.Name;
                //get id of cate from DB
                var CateComboBoxID = await _db.Categories.Where(cate => cate.Name == CateName).Select(c => c.Id).FirstOrDefaultAsync
                    ();
                //data validation
                int ProductQty = ( String.IsNullOrEmpty(ProductQtyTextBox.Text) ) ? 0 : int.Parse(ProductQtyTextBox.Text);
                //object to add
                var product = new Product()
                {
                    Name = ProductNameTextBox.Text,
                    CatId = CateComboBoxID,
                    Qty = ProductQty,
                    Price = ProductPriceTextBox.Text,
                    ImagePath = ProductImagePathTextBox.Text
                };

                _db.Products.Add(product);
                await _db.SaveChangesAsync();
                MessageBox.Show(
                    $"Save succesfull. {product.Name}: {CateName} : ${product.Price}");
                this.Close();
            }

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            var productHome = new ProductWindow();
            productHome.Show();
            this.Close();
        }


    }
}
