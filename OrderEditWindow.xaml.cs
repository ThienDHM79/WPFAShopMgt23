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

namespace WPFAShopMgt23
{
    /// <summary>
    /// Interaction logic for OrderEditWindow.xaml
    /// </summary>
    public partial class OrderEditWindow : Window
    {
        ShopDbContext _db;
        OrderService _orderService;
        CustomerService _customerService;
        ProductService _productService;
        public Purchase EditingPurchase { get; set; }
        public OrderEditWindow(Purchase _purchase)
        {
            InitializeComponent();
            _db = new ShopDbContext();
            _orderService = new OrderService(_db);
            _customerService = new CustomerService(_db);
            _productService = new ProductService(_db);
            EditingPurchase = (Purchase)_purchase.Clone();
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            var checkOrderScreen = new OrderWindow();
            checkOrderScreen.Show();
            this.Close();
        }

        private void UpdateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            //data validation
            bool valid = true;
            if (string.IsNullOrEmpty(PurchaseCustomerNameTextBox.Text))
            {
                valid = false;
            }
            if (string.IsNullOrEmpty(PurchasePhoneNumTextBox.Text))
            {
                valid = false;
                MessageBox.Show("field cannot empty");
            }

            if (valid)
            {

                using (var transaction = _db.Database.BeginTransaction())
                {

                        Customer affectedCustomer = _customerService.GetCustomerById((int)EditingPurchase.CustomerId);
                        if (affectedCustomer != null)
                        {
                            affectedCustomer.Name = PurchaseCustomerNameTextBox.Text;
                            affectedCustomer.Address = PurchaseAddressTextBox.Text;
                            affectedCustomer.Tel = PurchasePhoneNumTextBox.Text;
                        }
                        _db.Customers.Update(affectedCustomer);
                         _db.SaveChanges();
                        List<PurchaseDetail> DetailList = new List<PurchaseDetail>(_orderService.GetDetailsFromPurchaseId(EditingPurchase.Id));
                        foreach( var line in PurchaseItemGrid.Items)
                        {
                            PurchaseDetail item = line as PurchaseDetail;
                            if (item != null)
                            {  
                                PurchaseDetail source = DetailList.Find(d => d.Id == item.Id);
                                item.Total = (int)(item.Qty * item.Price);
                                source.Qty = item.Qty;
                                source.Total = item.Total;
                                Debug.WriteLine($"item total {item.Total}. source item total {source.Total}");
                        }
                        }
                        _db.PurchaseDetails.UpdateRange(DetailList);
                        _db.SaveChanges();
                    transaction.Commit();
                    DetailList.ForEach(d => Debug.WriteLine($"qty {d.Qty} total {d.Total}"));

                        int FinalTotal = (int)DetailList.Sum(item => item.Total);
                        Purchase affectedPurchase = _orderService.GetPurchaseById(EditingPurchase.Id);
                        if (affectedPurchase != null)
                        {
                            affectedPurchase.CreatedDate = CreateDatePurchasePicker.SelectedDate;
                            affectedPurchase.UpdatedDate = CreateDatePurchasePicker.SelectedDate;
                            affectedPurchase.FinalTotal = FinalTotal;

                            _db.Purchases.Update(affectedPurchase);
                            _db.SaveChanges();
                    }
                    Debug.WriteLine($"purchase total{affectedPurchase.FinalTotal}");
                        //_db.SaveChanges();
                        MessageBox.Show("transaction success");
                    /*
                    catch (Exception ex)
                    {
                        MessageBox.Show($"transaction failed. error {ex.Message}");
                    }
                    */
                }

            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"clone id {EditingPurchase.Id}");
            Purchase sourcePurchase = _orderService.GetPurchaseById(EditingPurchase.Id);
            CreateDatePurchasePicker.SelectedDate = sourcePurchase.CreatedDate;
            PurchaseTotalTextBox.Text = $"{sourcePurchase.FinalTotal} VND";
            var customer = _customerService.GetCustomerById((int)sourcePurchase.CustomerId);
            if (customer != null)
            {
                PurchaseCustomerNameTextBox.Text = customer.Name;
                PurchaseAddressTextBox.Text = customer.Address;
                PurchasePhoneNumTextBox.Text = (customer.Tel == null) ? "":customer.Tel.ToString();
            }
            //lay lai tu db
            List<PurchaseDetail> DetailList = new List<PurchaseDetail>(_orderService.GetDetailsFromPurchaseId(sourcePurchase.Id));
            foreach( var line in DetailList)
            {
                line.Product = _productService.GetProductById((int)line.ProductId);
            }
            PurchaseItemGrid.ItemsSource = DetailList;
            
            DataContext = EditingPurchase;

        }

    }
}
