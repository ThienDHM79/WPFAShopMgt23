using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
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
using WPFAShopMgt23.Model;
using WPFAShopMgt23.Services;

namespace WPFAShopMgt23
{
    /// <summary>
    /// Interaction logic for ProductEditWindow.xaml
    /// </summary>
    public partial class ProductEditWindow : Window
    {
        ShopDbContext _db;
        CategoryService _categoryService;
        ProductService _productService;
        public Product EditingProduct { get; set; }
        public ProductEditWindow(Product _product)
        {
            InitializeComponent();
            _db = new ShopDbContext();
            EditingProduct = (Product) _product.Clone();
            _categoryService = new CategoryService(_db);
            _productService = new ProductService(_db);
        }
        private void PreviewTextInputQty(object sender, TextCompositionEventArgs e)
        {
            Helper.PreviewTextInputQty(sender, e);
        }
        private void ProductEditOK_Click(object sender, RoutedEventArgs e)
        {
            bool valid = Helper.ProductValidation(ProductNameTextBox.Text, ProductCateComboBox.HasItems);
            if (valid)
            {
                Category SelectedCate = ProductCateComboBox.SelectedItem as Category;
                
                //data validation
                int ProductQty = (String.IsNullOrEmpty(ProductQtyTextBox.Text)) ? 0 : int.Parse(ProductQtyTextBox.Text);
                //update object in main product screen
                Product inputProduct = new Product()
                {
                    Name = ProductNameTextBox.Text,
                    CatId = _categoryService.GetCategoryIdFromView(SelectedCate),
                    Qty = ProductQty,
                    Price = ProductPriceTextBox.Text
                };
                Debug.WriteLine(SelectedCate.Name);
                Debug.WriteLine(_categoryService.GetCategoryIdFromView(SelectedCate));
                //update service
                Product affectedProduct = _productService.GetProductById(EditingProduct.Id);
                affectedProduct.Name = inputProduct.Name;

                //pending - update category of product
                affectedProduct.CatId = inputProduct.CatId;
                affectedProduct.Qty = inputProduct.Qty;
                affectedProduct.Price = inputProduct.Price;

                _db.Products.Update(affectedProduct);
                _db.SaveChanges();
                MessageBox.Show(
                    $"Update successful.{EditingProduct.Name}. " +
                    $"Cate: {SelectedCate.Name} . " +
                    $"qty:{EditingProduct.Qty}. " +
                    $"price:{EditingProduct.Price}");
            }
            this.DialogResult = true;
            var productMainscreen = new ProductWindow();
            productMainscreen.Show();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _db = new ShopDbContext();
            ProductCateComboBox.ItemsSource = _db.Categories.ToList();
            ProductCateComboBox.SelectedIndex = _categoryService.CateIdToComboBoxIndexConverter(EditingProduct);
            ProductCateComboBox.SelectedItem = _categoryService.GetCategorybyId((int)EditingProduct.CatId);
            //lay data tu main product screen
            DataContext = EditingProduct;
            
        }
    }
}
