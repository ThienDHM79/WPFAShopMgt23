using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        PagingInfo _paging;
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
            _paging = new PagingInfo()
            {
                CurrentPage = 1,
                RowsPerPage = 10
            };
            _loadCurrentPageProducts();

            var infos = new ObservableCollection<object>();
            for (int i = 1; i <= _paging.TotalPages; i++)
            {
                infos.Add(new
                {
                    Index = i,
                    Total = _paging.TotalPages,
                });
            }
            pagesCombobox.ItemsSource = infos;
            pagesCombobox.SelectedIndex = 0;

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
        }

        private void _LoadPage(int currPage)
        {
            ProductsListView.ItemsSource = _productService.GetProductsByPage(currPage,12);
        }

        private void _loadCurrentPageProducts()
        {
            _paging.TotalItems = _productService.GetProductCount();
            int skip = (_paging.CurrentPage - 1) * (_paging.RowsPerPage);
            List<Product> productList = _productService.GetProductsByPage(skip, _paging.RowsPerPage);
            ProductsListView.ItemsSource = productList;
            _paging.TotalCurrentItems = productList.Count;
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
                _loadCurrentPageProducts();
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

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_paging.CurrentPage < _paging.TotalPages)
            {
                _paging.CurrentPage++;
                _loadCurrentPageProducts();
            }
        }
        private void PrevPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (_paging.CurrentPage > 1)
            {
                _paging.CurrentPage--;
                _loadCurrentPageProducts();
            }
        }
        private void pagesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = pagesCombobox.SelectedIndex;
            _paging.CurrentPage = i + 1;
            _loadCurrentPageProducts();
        }
    }
}
