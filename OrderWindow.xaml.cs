using System;
using System.Collections.Generic;
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
    /// Interaction logic for OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : Window
    {
        ShopDbContext _db;
        OrderService orderService;
        CustomerService customerService;
        ProductService productService;

        DateTime DateFrom;
        DateTime DateTo;
        Purchase _purchase;
        public OrderWindow()
        {
            InitializeComponent();
            _db = new ShopDbContext();
            orderService = new OrderService(_db);
            customerService = new CustomerService(_db);
            productService = new ProductService(_db);
        }
        private void LoadOrdersGrid(List<Purchase> purchaseList)
        {
            foreach (Purchase purchase in purchaseList)
            {
                int customerId;
                if (purchase.CustomerId != null)
                {
                    customerId = (int)purchase.CustomerId;
                    var customer = customerService.GetCustomerById(customerId);
                    if (customer != null)
                    {
                        purchase.Customer = customer;
                    }
                }
            }
            OrderDataGrid.ItemsSource = purchaseList;
        }
        private void _LoadAllOrders()
        {
            List<Purchase> purchaseList = orderService.GetAllPurchases();
            LoadOrdersGrid(purchaseList);
        }

        private void SearchOrderButton_Click(object sender, RoutedEventArgs e)
        {
            DateFrom = OrderDatePicker.From;
            DateTo = OrderDatePicker.To;
            List<Purchase> FilterList;

            if (DateFrom == null && DateTo == null)
            {
                FilterList = orderService.GetAllPurchases();
            }
            else
            {
                FilterList = orderService.GetPurchasebyDateRng(DateFrom, DateTo);
            }

            LoadOrdersGrid(FilterList);
        }

        private void EnableText(bool status)
        {
            CreateDatePurchasePicker.IsEnabled = status;
            PurchaseCustomerNameTextBox.IsEnabled = status;
            PurchaseAddressTextBox.IsEnabled = status;
            PurchasePhoneNumTextBox.IsEnabled = status;
            PurchaseItemGrid.IsReadOnly = !status;

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _LoadAllOrders();
            EnableText(false);
            
        }
        private void ViewOrderDetails(Purchase purchase) {
            if (purchase != null)
            {
                List<PurchaseDetail> PurchaseLineList = orderService.GetDetailsFromPurchaseId(purchase.Id);

                foreach (var line in PurchaseLineList)
                {
                    line.Product = productService.GetProductById((int)line.ProductId);
                }
                PurchaseItemGrid.ItemsSource = PurchaseLineList;
            }
            this.DataContext = purchase;
        }

        private void ViewOrderButton_Click(object sender, RoutedEventArgs e)
        {
            Purchase SelectedPurchase = OrderDataGrid.SelectedItem as Purchase;
            if (SelectedPurchase != null)
            {    
                List<PurchaseDetail> PurchaseLineList = orderService.GetDetailsFromPurchaseId(SelectedPurchase.Id);

                foreach(var line in PurchaseLineList)
                {
                    line.Product = productService.GetProductById((int)line.ProductId);
                    //add vao detail list de view. xoa khi edit
                    SelectedPurchase.PurchaseDetails.Add(line);
                }
                PurchaseItemGrid.ItemsSource = PurchaseLineList;
                _purchase = SelectedPurchase;
            }

            this.DataContext = SelectedPurchase;
        }

        private void EditOrderButton_Click(object sender, RoutedEventArgs e)
        {
            
            Debug.WriteLine($"selected id {_purchase.Id}");
            if (_purchase == null)
            {
                MessageBox.Show("Pls Re-Select Order");
                _purchase = OrderDataGrid.SelectedItem as Purchase;
            }
            if (_purchase != null)
            {
                var EditPurchaseScreen = new OrderEditWindow(_purchase);
                if (EditPurchaseScreen.ShowDialog() == true)
                {
                    _purchase = (Purchase)EditPurchaseScreen.EditingPurchase.Clone();
                };
            }
            _LoadAllOrders();
        }

        private void DeleteOrderButton_Click(object sender, RoutedEventArgs e)
        {
            Purchase SelectedPurchase = OrderDataGrid.SelectedItem as Purchase;
            if (MessageBox.Show("Confirm Delete?",
                    "Confirm", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (SelectedPurchase != null)
                {
                    Purchase deletePurchase = _db.Purchases.Find(SelectedPurchase.Id);
                    if (deletePurchase != null)
                    {
                        _db.Purchases.Remove(deletePurchase);
                        _db.SaveChanges();
                    }
                }
                _LoadAllOrders();
            }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            var mainScreen = new MainWindow();
            mainScreen.Show();
            this.Close();
        }
    }
}
