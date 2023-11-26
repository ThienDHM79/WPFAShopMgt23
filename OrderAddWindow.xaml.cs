using Microsoft.EntityFrameworkCore;
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

            //_db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _db = new ShopDbContext();
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
                Debug.WriteLine(ItemQty.Text);
                //line view update
                OrderLineView orderLine = new OrderLineView(_selectedProduct, int.Parse(ItemQty.Text));
                //line list update
                _OrderLineList.Add(orderLine);
            }
            //update finaltotal view
            PurchaseTotalTextBox.Text = $"{_OrderLineList.Sum(line => line.SubTotal)} VND";
        }

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {

            OrderLineView deletingLine  = PurchaseItemGrid.SelectedItem as OrderLineView;
            Debug.WriteLine(_OrderLineList.ToList().ToString());

            //var orderLine = (OrderLineView) row.DataContext;
            _OrderLineList.Remove(deletingLine);

            PurchaseItemGrid.ItemsSource = _OrderLineList;
            PurchaseTotalTextBox.Text = $"{_OrderLineList.Sum(line => line.SubTotal)} VND";
        }

        private async void ConfirmPurchase_Click(object sender, RoutedEventArgs e)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    //get purchase header
                    //db thieu ngay tao
                    //create customer
                    var customer = new Customer()
                    {
                        Name = PurchaseCustomerNameTextBox.Text,
                        Address = PurchaseAddressTextBox.Text,
                        Tel = PurchasePhoneNumTextBox.Text
                    };
                    _db.Customers.Add(customer);
                    _db.SaveChanges();
                    //convert grid data to purchase detail

                    var createdDate = (PurchaseCreateDatePicker.SelectedDate == null)? DateTime.Today : PurchaseCreateDatePicker.SelectedDate;

                    //cal final total
                    int calFinalTotal = _OrderLineList.Sum(line => line.SubTotal);
                    var purchase = new Purchase()
                    {
                        CreatedDate = createdDate,
                        CustomerId = customer.Id,
                        FinalTotal = calFinalTotal
                    };

                    //create purchase id
                    _db.Purchases.Add(purchase);
                    //save order
                    _db.SaveChanges();
                    //create purchase details
                    ICollection<PurchaseDetail> insertList = new List<PurchaseDetail>();
                    foreach (var line in _OrderLineList)
                    {
                        PurchaseDetail LineDetail = new PurchaseDetail()
                        {
                            PurchaseId = purchase.Id,
                            ProductId = line.ProductId,
                            Price = line.Price,
                            Qty = line.Qty,
                            Total = line.SubTotal,
                            //Product = _productService.GetProductById(line.ProductId)
                        };
                        insertList.Add(LineDetail);

                    }
                    _db.ChangeTracker.Clear();
                    _db.PurchaseDetails.AddRange(insertList);
                    //save
                    await _db.SaveChangesAsync();

                    transaction.Commit();

                    MessageBox.Show($"Purchase Created: total {purchase.FinalTotal}, " +
                            $"{purchase.PurchaseDetails.Count()} items");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Transaction fail. Error: {ex.Message}");

                }

            }

        }

        private void CheckOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderWindow = new OrderWindow();
            orderWindow.Show();
            this.Close();
        }
    }
}
