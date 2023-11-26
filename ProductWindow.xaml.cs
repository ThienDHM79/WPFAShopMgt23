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
using WPFAShopMgt23.Model;
using WPFAShopMgt23.Services;
using WPFAShopMgt23.ViewModel;

namespace WPFAShopMgt23
{
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        ShopDbContext _db;
        ProductService _productService;
        Product _product; // cho editing

        public int CurrPage = 1; // phan trang
        public ProductWindow()
        {
            InitializeComponent();
            //init dbcontext in window
            _db = new ShopDbContext();
            //initiate service
            _productService = new ProductService(_db);

            this.DataContext = CurrPage;
        }
        BindingList<Product> _products;
        BindingList<Category> _categories;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _LoadPage(1);

            _categories = new BindingList<Category>(_db.Categories.ToList());
            categoryComboBox.ItemsSource = _categories;

            //xu li all categories. gia tri 0
            var AllCategory = new Category()
            {
                Name = "All"
            };
            _categories.Insert(0, AllCategory);
            categoryComboBox.SelectedIndex = 0;
            //default disable edit, delete before select
            EditButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            this.DataContext = CurrPage;
        }

        private void _LoadPage(int currPage)
        {
            ProductsListView.ItemsSource = _productService.GetProductsByPage(currPage,12);
        }

        private void _loadAllProducts()
        {
            _products = new BindingList<Product>(_productService.GetAllProducts());
            ProductsListView.ItemsSource = _products;
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            var addProductScreen = new ProductAddWindow();
            addProductScreen.Show();
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
                        _products = _productService.GetAllProducts();
                    }
                    else
                    {
                        _products = _productService.GetProductsByCatId(categoryComboBox.SelectedIndex);
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
                        _products = _productService.GetProductsByNameCategory(ProductNameTextBox.Text, categoryComboBox.SelectedIndex);
                    }

                }
                ProductsListView.ItemsSource = _products;
                //check for price range
                if (string.IsNullOrEmpty(LowestPriceTextBox.Text))
                {
                    if (!string.IsNullOrEmpty(HighestPriceTextBox.Text))
                    {
                        ProductsListView.ItemsSource = _products.Where(p => p.PriceInt <= int.Parse(HighestPriceTextBox.Text));
                    }
                }
                if (string.IsNullOrEmpty(HighestPriceTextBox.Text))
                {
                    if (!string.IsNullOrEmpty(LowestPriceTextBox.Text))
                    {
                        ProductsListView.ItemsSource = _products.Where(p => p.PriceInt >= int.Parse(LowestPriceTextBox.Text));
                    }
                }
                if (!string.IsNullOrEmpty(HighestPriceTextBox.Text) && !string.IsNullOrEmpty(LowestPriceTextBox.Text))
                {
                    ProductsListView.ItemsSource = _products.Where(p =>
                     p.PriceInt >= int.Parse(LowestPriceTextBox.Text) &&
                     p.PriceInt <= int.Parse(HighestPriceTextBox.Text));
                }
                
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
                    }
                }
                _loadAllProducts();
            }

        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            //lay product tu selected
            _product = ProductsListView.SelectedItem as Product;
            var editProductScreen = new ProductEditWindow(_product);
            //editProductScreen.Show();
            if (editProductScreen.ShowDialog() == true)
            {
                _product = (Product) editProductScreen.EditingProduct.Clone();
                
            }
            this.Close();
        }

        private void ProductListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EditButton.IsEnabled = (sender != null);
            DeleteButton.IsEnabled = (sender != null);
        }



        PaginationView paginate = new PaginationView();
        private void FirstPageButton_Click(object sender, RoutedEventArgs e)
        {
            _LoadPage(1);
        }
        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            CurrPage++;
            _LoadPage(CurrPage);
        }
        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            while (CurrPage >=2)
            {
                CurrPage--;
                _LoadPage(CurrPage);
            }
        }
        private void LastPageButton_Click(object sender, RoutedEventArgs e)
        {
            var ProductList = _productService.GetAllProducts();
            paginate = new PaginationView(ProductList);
            _LoadPage(paginate.TotalPage);
        }

        private void PreviewInputNum(object sender, TextCompositionEventArgs e)
        {
            Helper.PreviewTextInputQty(sender, e);
        }

        private void PriceAscButton_Click(object sender, RoutedEventArgs e)
        {
            var curList = ProductsListView.Items.Cast<Product>().OrderBy(p => p.PriceInt).ToList();
            ProductsListView.ItemsSource = curList;
        }

        private void PriceDescButton_Click(object sender, RoutedEventArgs e)
        {
            var curList = ProductsListView.Items.Cast<Product>().OrderByDescending(p => p.PriceInt).ToList();
            ProductsListView.ItemsSource = curList;
        }
    }
}
