using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Flower_shop
{
    public class QuantityToButtonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int quantity)
            {
                return quantity > 0 ? "В корзину" : "Скоро";
            }
            return "В корзину";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class QuantityToEnabledConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int quantity)
            {
                return quantity > 0;
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Логика взаимодействия для MainMenuCustomer.xaml
    /// </summary>
    public partial class MainMenuCustomer : Window
    {
        private string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user311_db;password=user311;MultipleActiveResultSets=True;App=EntityFramework";
        private ObservableCollection<ProductCard> Products { get; set; }

        public MainMenuCustomer()
        {
            InitializeComponent();
            Products = new ObservableCollection<ProductCard>();
            LoadData();
            SetupProductsControl();
        }

        private void SetupProductsControl()
        {
            ProductsItemsControl.ItemsSource = Products;
            ProductsItemsControl.ItemTemplate = (DataTemplate)FindResource("ProductCardTemplate");
        }

        private void LoadData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"
                        SELECT
                            t.ID_Tovar,
                            t.Product_Name,
                            t.Description,
                            t.Image_Url,
                            t.Price,
                            t.Quantity,
                            t.Nalichie,
                            p.Name_Postavchik AS Postavchik,
                            c.Category_Name AS Category
                        FROM FS_Tovar t
                        INNER JOIN FS_Postavchik p ON t.ID_Postavchik = p.ID_Postavchik
                        INNER JOIN FS_Category c ON t.ID_Category = c.ID_Category
                        WHERE t.Nalichie = 1
                        ORDER BY t.Product_Name";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Products.Clear();
                            while (reader.Read())
                            {
                                var product = new ProductCard
                                {
                                    ID_Tovar = reader.GetInt32(0),
                                    Product_Name = reader.GetString(1),
                                    Description = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                    Image_Url = reader.IsDBNull(3) ? "" : reader.GetString(3),
                                    Price = reader.GetDecimal(4),
                                    Quantity = reader.GetInt32(5),
                                    Nalichie = reader.GetBoolean(6),
                                    Postavchik = reader.GetString(7),
                                    Category = reader.GetString(8)
                                };
                                Products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}",
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaximizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (ProductCard)button.DataContext;

            MessageBox.Show($"Товар \"{product.Product_Name}\" добавлен в корзину",
                          "Успешно",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
            // TODO: Реализовать добавление в корзину
        }
    }
}
