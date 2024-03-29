﻿using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPFAShopMgt23.Model;
using WPFAShopMgt23.Services;

namespace WPFAShopMgt23
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ShopDbContext _db;
        ReportService _reportService;
        public LiveCharts.SeriesCollection SalesCollection { get; set; }
        public MainWindow()
        {
            InitializeComponent();

            _db = new ShopDbContext();
            _reportService = new ReportService(_db);
            DataContext = this;

        }
        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            ProductWindow productWindow = new ProductWindow();
            productWindow.Show();
            Close();
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderScreen = new OrderAddWindow();
            orderScreen.Show();
            this.Close();
        }

        private void UpdateDateRngChart_Click(object sender, RoutedEventArgs e)
        {
            SalesReport reportPara = new SalesReport();
            DateRangePieChart.Series = _reportService.GetSaleSeries(OrderDatePicker.From, OrderDatePicker.To);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DateTime DateFrom = DateTime.Today.AddMonths(-1).AddSeconds(1);
            DateTime DateTo = DateTime.Now;
            DateRangePieChart.Series = _reportService.GetSaleSeries(DateFrom, DateTo);
            MonthPieChart.Series = _reportService.GetSaleSeries(DateFrom, DateTo);
            TopMonthSaleDataGrid.ItemsSource = _reportService.GetTopReportList(DateFrom, DateTo);
        }

        private void PreviewTextInputNum(object sender, TextCompositionEventArgs e)
        {
            Helper.PreviewTextInputQty(sender, e);
        }

        private void ViewWeekChartButton_Click(object sender, RoutedEventArgs e)
        {
            SalesReport reportPara = new SalesReport(int.Parse(WeekTextBox.Text),null,null);
            Debug.WriteLine(reportPara.WeekNum);
            var daterange = _reportService.GetDateRangeFromWeek(reportPara.WeekNum, 2023);
            WeekPieChart.Series = _reportService.GetSaleSeries(daterange["DateFrom"], daterange["DateTo"]);
            //Debug.WriteLine(daterange["DateFrom"]);
            //Debug.WriteLine(daterange["DateTo"]);

        }

        private void ViewTopRngButton_Click(object sender, RoutedEventArgs e)
        {
            var TopSaleList = _reportService.GetTopReportList(TopSaleDatePicker.From, TopSaleDatePicker.To);
            TopSaleRngDataGrid.ItemsSource = TopSaleList;
        }

        private void ViewMonthChartButton_Click(object sender, RoutedEventArgs e)
        {
            SalesReport reportPara;
            if (!string.IsNullOrEmpty(MonthTextBox.Text) && !string.IsNullOrEmpty(YearTextBox.Text))
            {
                reportPara = new SalesReport(null, int.Parse(MonthTextBox.Text), int.Parse(YearTextBox.Text));
                var daterange = _reportService.GetDateRangeFromMonth(reportPara.MonthNum, reportPara.YearNum);
                MonthPieChart.Series = _reportService.GetSaleSeries(daterange["DateFrom"], daterange["DateTo"]);

            }
        }

        private void ViewMonthTopButton_Click(object sender, RoutedEventArgs e)
        {
            SalesReport reportPara;
            if (!string.IsNullOrEmpty(MonthTopTextBox.Text) && !string.IsNullOrEmpty(YearTopTextBox.Text))
            {
                reportPara = new SalesReport(null, int.Parse(MonthTopTextBox.Text), int.Parse(YearTopTextBox.Text));
            }
            else
            {
                reportPara = new SalesReport(null, null, null);
            }
            var daterange = _reportService.GetDateRangeFromMonth(reportPara.MonthNum, reportPara.YearNum);
            var TopMonthSaleList = _reportService.GetTopReportList(daterange["DateFrom"], daterange["DateTo"]);
            TopMonthSaleDataGrid.ItemsSource = TopMonthSaleList;

        }

        private void CheckOrderButton_Click(object sender, RoutedEventArgs e)
        {
            var CheckOrderScreen = new OrderWindow();
            CheckOrderScreen.Show();
            this.Close();
        }
    }
}
