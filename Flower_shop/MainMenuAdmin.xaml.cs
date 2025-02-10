using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Flower_shop
{
    public class ProductCard
    {
        public int ID_Tovar { get; set; }
        public string Product_Name { get; set; }
        public string Description { get; set; }
        public string Image_Url { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool Nalichie { get; set; }
        public string Postavchik { get; set; }
        public string Category { get; set; }
        public ObservableCollection<string> Categories { get; set; }
        public ObservableCollection<string> Suppliers { get; set; }
    }

    public partial class MainMenuAdmin : Window
    {
        private string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user311_db;password=user311;MultipleActiveResultSets=True;App=EntityFramework";
        private ObservableCollection<ProductCard> Products { get; set; }
        private ObservableCollection<string> AllCategories { get; set; }
        private ObservableCollection<string> AllSuppliers { get; set; }

        public MainMenuAdmin()
        {
            InitializeComponent();
            Products = new ObservableCollection<ProductCard>();
            AllCategories = new ObservableCollection<string>();
            AllSuppliers = new ObservableCollection<string>();
            LoadData();
            LoadComboBoxData();
            SetupProductsControl();
        }

        private void SetupProductsControl()
        {
            ProductsItemsControl.ItemTemplate = (DataTemplate)FindResource("ProductCardTemplate");
            ProductsItemsControl.ItemsSource = Products;
            
            // Добавляем карточку для добавления нового товара
            var addCard = new ContentControl
            {
                Template = (ControlTemplate)FindResource("AddCardTemplate")
            };
            Products.Add(null); // Placeholder для карточки добавления
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
                                    Category = reader.GetString(8),
                                    Categories = AllCategories,
                                    Suppliers = AllSuppliers
                                };
                                Products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
        }

        private void LoadComboBoxData()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // Загрузка категорий
                    string queryCategory = "SELECT Category_Name FROM FS_Category ORDER BY Category_Name";
                    using (SqlCommand command = new SqlCommand(queryCategory, connection))
                    {
                        AllCategories.Clear();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AllCategories.Add(reader.GetString(0));
                            }
                        }
                    }

                    // Загрузка поставщиков
                    string queryPostavchik = "SELECT Name_Postavchik FROM FS_Postavchik ORDER BY Name_Postavchik";
                    using (SqlCommand command = new SqlCommand(queryPostavchik, connection))
                    {
                        AllSuppliers.Clear();
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AllSuppliers.Add(reader.GetString(0));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке справочных данных: " + ex.Message);
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

        private void ChangeImage_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (ProductCard)button.DataContext;

            // Здесь можно добавить диалог для ввода URL изображения
            var dialog = new InputDialog("Введите URL изображения:", product.Image_Url);
            if (dialog.ShowDialog() == true)
            {
                product.Image_Url = dialog.ResponseText;
                UpdateProduct(product);
            }
        }

        private void DecrementPrice_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (ProductCard)button.DataContext;
            if (product.Price > 0)
            {
                product.Price--;
                UpdateProduct(product);
            }
        }

        private void IncrementPrice_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (ProductCard)button.DataContext;
            product.Price++;
            UpdateProduct(product);
        }

        private void DecrementQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (ProductCard)button.DataContext;
            if (product.Quantity > 0)
            {
                product.Quantity--;
                product.Nalichie = product.Quantity > 0;
                UpdateProduct(product);
            }
        }

        private void IncrementQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (ProductCard)button.DataContext;
            product.Quantity++;
            product.Nalichie = true;
            UpdateProduct(product);
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = (Button)sender;
            var product = (ProductCard)button.DataContext;

            var result = MessageBox.Show(
                "Вы уверены, что хотите удалить этот товар?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = $"DELETE FROM FS_Tovar WHERE ID_Tovar = {product.ID_Tovar}";
                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.ExecuteNonQuery();
                            Products.Remove(product);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при удалении товара: " + ex.Message);
                }
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddProductDialog(AllCategories, AllSuppliers);
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        
                        // Получаем ID категории и поставщика
                        int categoryId = GetCategoryId(connection, dialog.Category);
                        int supplierId = GetPostavchikId(connection, dialog.Supplier);

                        string query = @"
                            INSERT INTO FS_Tovar (
                                Product_Name, Description, Image_Url, 
                                Price, Quantity, Nalichie, 
                                ID_Postavchik, ID_Category
                            ) VALUES (
                                @name, @description, @imageUrl,
                                @price, @quantity, @nalichie,
                                @supplierId, @categoryId
                            )";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", dialog.ProductName);
                            command.Parameters.AddWithValue("@description", dialog.Description);
                            command.Parameters.AddWithValue("@imageUrl", dialog.ImageUrl);
                            command.Parameters.AddWithValue("@price", dialog.Price);
                            command.Parameters.AddWithValue("@quantity", dialog.Quantity);
                            command.Parameters.AddWithValue("@nalichie", dialog.Quantity > 0);
                            command.Parameters.AddWithValue("@supplierId", supplierId);
                            command.Parameters.AddWithValue("@categoryId", categoryId);

                            command.ExecuteNonQuery();
                        }

                        // Перезагружаем данные
                        LoadData();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при добавлении товара: " + ex.Message);
                }
            }
        }

        private void UpdateProduct(ProductCard product)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    
                    int categoryId = GetCategoryId(connection, product.Category);
                    int supplierId = GetPostavchikId(connection, product.Postavchik);

                    string query = @"
                        UPDATE FS_Tovar SET
                            Product_Name = @name,
                            Description = @description,
                            Image_Url = @imageUrl,
                            Price = @price,
                            Quantity = @quantity,
                            Nalichie = @nalichie,
                            ID_Postavchik = @supplierId,
                            ID_Category = @categoryId
                        WHERE ID_Tovar = @id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", product.ID_Tovar);
                        command.Parameters.AddWithValue("@name", product.Product_Name);
                        command.Parameters.AddWithValue("@description", product.Description);
                        command.Parameters.AddWithValue("@imageUrl", product.Image_Url);
                        command.Parameters.AddWithValue("@price", product.Price);
                        command.Parameters.AddWithValue("@quantity", product.Quantity);
                        command.Parameters.AddWithValue("@nalichie", product.Nalichie);
                        command.Parameters.AddWithValue("@supplierId", supplierId);
                        command.Parameters.AddWithValue("@categoryId", categoryId);

                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении товара: " + ex.Message);
            }
        }

        private int GetCategoryId(SqlConnection connection, string categoryName)
        {
            string query = "SELECT ID_Category FROM FS_Category WHERE Category_Name = @name";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", categoryName);
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }

        private int GetPostavchikId(SqlConnection connection, string postavchikName)
        {
            string query = "SELECT ID_Postavchik FROM FS_Postavchik WHERE Name_Postavchik = @name";
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@name", postavchikName);
                object result = command.ExecuteScalar();
                return result != null ? Convert.ToInt32(result) : -1;
            }
        }
    }
}