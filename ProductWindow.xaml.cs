using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFAShopMgt23.Services;

namespace WPFAShopMgt23
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        ShopDbContext _db;
        ProductService _productService;
        public ProductWindow()
        {
            InitializeComponent();
            //init dbcontext in window
            _db = new ShopDbContext();
            //initiate service
            _productService = new ProductService(_db);
        }
        BindingList<Product> _products;
        BindingList<Category> _categories;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _loadAllProducts();

            _categories = new BindingList<Category>(_db.Categories.ToList());
            categoryComboBox.ItemsSource = _categories;

            //xu li all categories. gia tri 0
            var AllCategory = new Category()
            {
                Name = "All"
            };
            _categories.Insert(0, AllCategory);
            categoryComboBox.SelectedIndex = 0;
        }

        private void _loadAllProducts()
        {
            _products = _productService.GetAllProducts();
            ProductsListView.ItemsSource = _products;
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var addProductScreen = new ProductAddWindow();
            addProductScreen.Show();
            this.Close();
        }
        


        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            var mainscreen = new MainWindow();
            mainscreen.Show();
            this.Close();
        }

        private void searchButton_Click(object sender, RoutedEventArgs e)
        {
            //have search input
            if (ProductNameTextBox.Text != string.Empty || categoryComboBox.SelectedIndex != 0)
            {
                var _products = new List<Product>();
                if (ProductNameTextBox.Text == string.Empty)
                {
                    if (categoryComboBox.SelectedIndex == 0)
                    {
                        _products = _db.Products.ToList();
                    }
                    else
                    {
                        _products = _db.Products.Where(p => p.CatId == categoryComboBox.SelectedIndex).ToList();
                    }

                }
                if (ProductNameTextBox.Text != string.Empty)
                {
                    if (categoryComboBox.SelectedIndex == 0)
                    {
                        _products = _productService.GetProductsByName(ProductNameTextBox.Text);
                    }
                    else
                    {
                        _products = _db.Products.Where(p => p.Name.Contains(ProductNameTextBox.Text))
                                .Where(p => p.CatId == categoryComboBox.SelectedIndex).ToList();
                    }

                }
                //
                ProductsListView.ItemsSource = _products;
            }
            else
            {
                Window_Loaded(sender, e);
            }
        }

        //chua delete done
        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            Product SelectedProduct = ProductsListView.SelectedItem as Product;
            Debug.WriteLine($"DB ID: {SelectedProduct.Id}, SelectedIndex: {ProductsListView.SelectedIndex}");

            if (MessageBox.Show("Confirm Delete?",
                    "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (SelectedProduct != null)
                {
                    Product deleteproduct = _db.Products.Find(SelectedProduct.Id);
                    if (deleteproduct != null)
                    {
                        _db.Products.Remove(deleteproduct);
                        _db.SaveChanges();
                        //screen auto update deleted
                        //_products.Remove(deleteproduct);
                    }
                }
                _loadAllProducts();
            }

        }
    }
}
