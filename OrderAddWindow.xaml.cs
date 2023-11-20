using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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
    /// Interaction logic for OrderAddWindow.xaml
    /// </summary>
    public partial class OrderAddWindow : Window
    {
        ShopDbContext _db;
        ProductService _productService;
        CategoryService _categoryService;
        BindingList<Product> _products;
        List<Category> _categories;

        Product _selectedProduct;
        ObservableCollection<OrderLineView> _OrderLineList;
        public OrderAddWindow()
        {
            InitializeComponent();
            _db = new ShopDbContext();
            _productService = new ProductService(_db);
            _categoryService = new CategoryService(_db);
            //view
            _OrderLineList = new ObservableCollection<OrderLineView>();
            _selectedProduct = new Product();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _loadAllProducts();

            _categories = _categoryService.GetAllCategories();
            CategoryCombobox.ItemsSource = _categories;

            //xu li all categories. gia tri 0
            var AllCategory = new Category()
            {
                Name = "All"
            };
            _categories.Insert(0, AllCategory);
            CategoryCombobox.SelectedIndex = 0;

            PurchaseItemGrid.ItemsSource = _OrderLineList;
        }
        private void _loadAllProducts()
        {
            _products = new BindingList<Product>(_productService.GetAllProducts());
            BrowseProductListView.ItemsSource = _products;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Helper.PreviewTextInputQty(sender, e);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            var mainscreen = new MainWindow();
            mainscreen.Show();
            this.Close();
        }

        private void SearchProductButton_Click(object sender, RoutedEventArgs e)
        {
            var _products = new List<Product>();
            _products = _productService.GetProductsByNameCategory(ProductNameTextBox.Text, CategoryCombobox.SelectedIndex);

            BrowseProductListView.ItemsSource = _products;
        }

        private void ChooseProductButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedProduct = BrowseProductListView.SelectedItem as Product;
            Debug.WriteLine(_selectedProduct.Name);
            if (_selectedProduct != null)
            {
                SelectedProductName.Text = _selectedProduct.Name;
                SelectedProductPrice.Text = _selectedProduct.Price;
                StockQty.Text = _selectedProduct.Qty.ToString();
            }
            else
            {
                SelectedProductName.Text = "Product Name";
                SelectedProductPrice.Text = "Product Price";
                StockQty.Text = "Stock";
            }
        }

        private void AddItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (ItemQty.Text.IsNullOrEmpty())
            {
                MessageBox.Show("need input qty");
            }
            if (!ItemQty.Text.IsNullOrEmpty())
            {
                Debug.WriteLine(_selectedProduct.Name);
                OrderLineView orderLine = new OrderLineView(_selectedProduct, int.Parse(ItemQty.Text));

                _OrderLineList.Add(orderLine);
            }
        }
    }
}
