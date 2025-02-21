using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Linq;
using Flower_shop;

namespace MenuManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MenegerMainMenu : Window
    {
        private string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user311_db;password=user311;MultipleActiveResultSets=True;App=EntityFramework";
        private Storyboard showFormAnimation;
        private Storyboard hideFormAnimation;
        private DataView ordersView;

        public MenegerMainMenu()
        {
            InitializeComponent();
            Loaded += MenegerMainMenu_Loaded;
            InitializeAnimations();
        }

        private void InitializeAnimations()
        {
            // Создаем анимацию появления формы
            showFormAnimation = new Storyboard();

            var opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            showFormAnimation.Children.Add(opacityAnimation);

            var scaleXAnimation = new DoubleAnimation
            {
                From = 0.8,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.5 }
            };
            Storyboard.SetTargetProperty(scaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            showFormAnimation.Children.Add(scaleXAnimation);

            var scaleYAnimation = new DoubleAnimation
            {
                From = 0.8,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.3),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.5 }
            };
            Storyboard.SetTargetProperty(scaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            showFormAnimation.Children.Add(scaleYAnimation);

            // Создаем анимацию скрытия формы
            hideFormAnimation = new Storyboard();

            var hideOpacityAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTargetProperty(hideOpacityAnimation, new PropertyPath("Opacity"));
            hideFormAnimation.Children.Add(hideOpacityAnimation);

            var hideScaleXAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0.8,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTargetProperty(hideScaleXAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleX)"));
            hideFormAnimation.Children.Add(hideScaleXAnimation);

            var hideScaleYAnimation = new DoubleAnimation
            {
                From = 1,
                To = 0.8,
                Duration = TimeSpan.FromSeconds(0.2),
                EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTargetProperty(hideScaleYAnimation, new PropertyPath("(UIElement.RenderTransform).(ScaleTransform.ScaleY)"));
            hideFormAnimation.Children.Add(hideScaleYAnimation);

            hideFormAnimation.Completed += (s, e) =>
            {
                orderFormPopup.Visibility = Visibility.Collapsed;
                if (mainBorder.Effect is BlurEffect blurEffect)
                {
                    blurEffect.Radius = 0;
                }
            };
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

        private void MenegerMainMenu_Loaded(object sender, RoutedEventArgs e)
        {
            LoadData();
            LoadClientNames();
            LoadProductNames();
        }

        private void LoadData()
        {
            try
            {
                loadingIndicator.Visibility = Visibility.Visible;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT
                            z.ID_Zakaz,
                            z.Order_Date,
                            zt.Kolichestvo,
                            zt.Price_Zakaza,
                            p.First_Name,
                            t.Product_Name
                        FROM
                            FS_Zakaz z
                        INNER JOIN
                            FS_Zakaznie_Tovari zt ON z.ID_Zakaz = zt.ID_Zakaz
                        INNER JOIN
                            FS_Polzovatel p ON z.ID_Polzovatel = p.ID_Polzovatel
                        INNER JOIN
                            FS_Tovar t ON zt.ID_Tovar = t.ID_Tovar
                        ORDER BY z.Order_Date DESC;";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    ordersView = dataTable.DefaultView;
                    ordersItemsControl.ItemsSource = ordersView;
                }
            }
            catch (Exception ex)
            {
                ShowError("Ошибка загрузки данных: " + ex.Message);
            }
            finally
            {
                loadingIndicator.Visibility = Visibility.Collapsed;
            }
        }

        private void LoadClientNames()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT First_Name FROM FS_Polzovatel WHERE Rol = 'Покупатель' ORDER BY First_Name";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    List<string> clientNames = new List<string>();
                    while (reader.Read())
                    {
                        clientNames.Add(reader["First_Name"].ToString());
                    }
                    clientCombo.ItemsSource = clientNames;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки имен клиентов: " + ex.Message,
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void LoadProductNames()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Product_Name FROM FS_Tovar WHERE Nalichie = 1 ORDER BY Product_Name";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    List<string> productNames = new List<string>();
                    while (reader.Read())
                    {
                        productNames.Add(reader["Product_Name"].ToString());
                    }
                    tovarCombo.ItemsSource = productNames;

                    // Добавляем обработчик события выбора товара
                    tovarCombo.SelectionChanged += TovarCombo_SelectionChanged;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки названий товаров: " + ex.Message,
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void TovarCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tovarCombo.SelectedItem != null)
            {
                UpdatePrice();
            }
        }

        private void UpdatePrice()
        {
            try
            {
                string productName = tovarCombo.SelectedItem as string;
                if (string.IsNullOrEmpty(productName)) return;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Price FROM FS_Tovar WHERE Product_Name = @productName";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@productName", productName);

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        decimal price = (decimal)result;
                        int quantity;
                        if (int.TryParse(colvoBox.Text, out quantity))
                        {
                            priceBox.Text = (price * quantity).ToString("N2");
                        }
                        else
                        {
                            priceBox.Text = price.ToString("N2");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при обновлении цены: " + ex.Message,
                              "Ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
        }

        private void ColvoBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tovarCombo.SelectedItem != null)
            {
                UpdatePrice();
            }
        }

        private void ShowOrderForm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ClearInputs();
                orderFormPopup.Visibility = Visibility.Visible;

                // Анимация размытия основного контента
                if (mainBorder.Effect is BlurEffect blurEffect)
                {
                    var blurAnimation = new DoubleAnimation
                    {
                        To = 5,
                        Duration = TimeSpan.FromSeconds(0.3),
                        EasingFunction = new CircleEase { EasingMode = EasingMode.EaseOut }
                    };
                    blurEffect.BeginAnimation(BlurEffect.RadiusProperty, blurAnimation);
                }

                // Устанавливаем целевой объект для анимаций
                foreach (var animation in showFormAnimation.Children)
                {
                    Storyboard.SetTarget(animation, orderFormPopup);
                }

                showFormAnimation.Begin();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при открытии формы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void HideOrderForm_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Анимация отмены размытия
                if (mainBorder.Effect is BlurEffect blurEffect)
                {
                    var blurAnimation = new DoubleAnimation
                    {
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.3),
                        EasingFunction = new CircleEase { EasingMode = EasingMode.EaseIn }
                    };
                    blurEffect.BeginAnimation(BlurEffect.RadiusProperty, blurAnimation);
                }

                // Устанавливаем целевой объект для анимаций
                foreach (var animation in hideFormAnimation.Children)
                {
                    Storyboard.SetTarget(animation, orderFormPopup);
                }

                hideFormAnimation.Begin();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при закрытии формы: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                orderFormPopup.Visibility = Visibility.Collapsed;
            }
        }

        private void OformBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (clientCombo.SelectedItem == null || tovarCombo.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(colvoBox.Text))
                {
                    ShowError("Пожалуйста, заполните все поля");
                    return;
                }

                string clientName = clientCombo.SelectedItem as string;
                string productName = tovarCombo.SelectedItem as string;
                int quantity;
                if (!int.TryParse(colvoBox.Text, out quantity) || quantity <= 0)
                {
                    ShowError("Пожалуйста, введите корректное количество");
                    return;
                }

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Получаем ID клиента
                            int clientId = GetClientId(connection, transaction, clientName);
                            // Получаем ID и проверяем наличие товара
                            int productId = GetProductId(connection, transaction, productName);
                            // Проверяем достаточное количество товара
                            if (!CheckProductQuantity(connection, transaction, productId, quantity))
                            {
                                ShowError("Недостаточное количество товара на складе");
                                return;
                            }

                            // Создаем заказ
                            int orderId = CreateOrder(connection, transaction, clientId);
                            // Добавляем товары в заказ
                            AddOrderItems(connection, transaction, orderId, productId, quantity);
                            // Обновляем количество товара
                            UpdateProductQuantity(connection, transaction, productId, quantity);

                            transaction.Commit();
                            ShowSuccess("Заказ успешно оформлен!");

                            LoadData();
                            HideOrderForm_Click(null, null);
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Ошибка при оформлении заказа: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private int GetClientId(SqlConnection connection, SqlTransaction transaction, string clientName)
        {
            string query = "SELECT ID_Polzovatel FROM FS_Polzovatel WHERE First_Name = @clientName";
            SqlCommand command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@clientName", clientName);
            object result = command.ExecuteScalar();
            if (result == null || result == DBNull.Value)
                throw new Exception("Клиент не найден");
            return (int)result;
        }

        private int GetProductId(SqlConnection connection, SqlTransaction transaction, string productName)
        {
            string query = "SELECT ID_Tovar FROM FS_Tovar WHERE Product_Name = @productName";
            SqlCommand command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@productName", productName);
            object result = command.ExecuteScalar();
            if (result == null || result == DBNull.Value)
                throw new Exception("Товар не найден");
            return (int)result;
        }

        private bool CheckProductQuantity(SqlConnection connection, SqlTransaction transaction, int productId, int requestedQuantity)
        {
            string query = "SELECT Quantity FROM FS_Tovar WHERE ID_Tovar = @productId";
            SqlCommand command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@productId", productId);
            int availableQuantity = (int)command.ExecuteScalar();
            return availableQuantity >= requestedQuantity;
        }

        private int CreateOrder(SqlConnection connection, SqlTransaction transaction, int clientId)
        {
            string query = @"INSERT INTO FS_Zakaz (ID_Polzovatel, Order_Date) 
                           OUTPUT INSERTED.ID_Zakaz 
                           VALUES (@clientId, @orderDate)";
            SqlCommand command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@clientId", clientId);
            command.Parameters.AddWithValue("@orderDate", DateTime.Now);
            return (int)command.ExecuteScalar();
        }

        private void AddOrderItems(SqlConnection connection, SqlTransaction transaction, int orderId, int productId, int quantity)
        {
            // Получаем цену товара
            string priceQuery = "SELECT Price FROM FS_Tovar WHERE ID_Tovar = @productId";
            SqlCommand priceCommand = new SqlCommand(priceQuery, connection, transaction);
            priceCommand.Parameters.AddWithValue("@productId", productId);
            decimal price = (decimal)priceCommand.ExecuteScalar();

            // Вычисляем общую стоимость
            decimal totalPrice = price * quantity;

            // Добавляем товары в заказ
            string query = @"INSERT INTO FS_Zakaznie_Tovari (ID_Zakaz, ID_Tovar, Kolichestvo, Price_Zakaza) 
                           VALUES (@orderId, @productId, @quantity, @totalPrice)";
            SqlCommand command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@orderId", orderId);
            command.Parameters.AddWithValue("@productId", productId);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.Parameters.AddWithValue("@totalPrice", totalPrice);
            command.ExecuteNonQuery();
        }

        private void UpdateProductQuantity(SqlConnection connection, SqlTransaction transaction, int productId, int quantity)
        {
            string query = @"UPDATE FS_Tovar 
                           SET Quantity = Quantity - @quantity,
                               Nalichie = CASE WHEN (Quantity - @quantity) > 0 THEN 1 ELSE 0 END
                           WHERE ID_Tovar = @productId";
            SqlCommand command = new SqlCommand(query, connection, transaction);
            command.Parameters.AddWithValue("@productId", productId);
            command.Parameters.AddWithValue("@quantity", quantity);
            command.ExecuteNonQuery();
        }

        private void ClearInputs()
        {
            clientCombo.SelectedItem = null;
            tovarCombo.SelectedItem = null;
            colvoBox.Text = string.Empty;
            priceBox.Text = string.Empty;
        }

        private void DeleteBox_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button?.DataContext == null) return;

            var row = button.DataContext as DataRowView;
            if (row == null) return;

            int orderId = (int)row["ID_Zakaz"];

            if (!ShowConfirmation("Вы уверены, что хотите отменить выбранный заказ?")) return;

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Возвращаем количество товара на склад
                            string returnQuantityQuery = @"
                                UPDATE t
                                SET t.Quantity = t.Quantity + zt.Kolichestvo,
                                    t.Nalichie = 1
                                FROM FS_Tovar t
                                INNER JOIN FS_Zakaznie_Tovari zt ON t.ID_Tovar = zt.ID_Tovar
                                WHERE zt.ID_Zakaz = @orderId";

                            SqlCommand returnQuantityCommand = new SqlCommand(returnQuantityQuery, connection, transaction);
                            returnQuantityCommand.Parameters.AddWithValue("@orderId", orderId);
                            returnQuantityCommand.ExecuteNonQuery();

                            // Удаляем записи из таблицы заказанных товаров
                            string deleteItemsQuery = "DELETE FROM FS_Zakaznie_Tovari WHERE ID_Zakaz = @orderId";
                            SqlCommand deleteItemsCommand = new SqlCommand(deleteItemsQuery, connection, transaction);
                            deleteItemsCommand.Parameters.AddWithValue("@orderId", orderId);
                            deleteItemsCommand.ExecuteNonQuery();

                            // Удаляем сам заказ
                            string deleteOrderQuery = "DELETE FROM FS_Zakaz WHERE ID_Zakaz = @orderId";
                            SqlCommand deleteOrderCommand = new SqlCommand(deleteOrderQuery, connection, transaction);
                            deleteOrderCommand.Parameters.AddWithValue("@orderId", orderId);
                            deleteOrderCommand.ExecuteNonQuery();

                            transaction.Commit();
                            ShowSuccess("Заказ успешно отменен!");
                            LoadData();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Ошибка при отмене заказа: " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ordersView == null) return;

            string searchText = searchBox.Text.ToLower();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ordersView.RowFilter = "";
            }
            else
            {
                ordersView.RowFilter = $"First_Name LIKE '%{searchText}%' OR " +
                                     $"Product_Name LIKE '%{searchText}%' OR " +
                                     $"CONVERT(ID_Zakaz, 'System.String') LIKE '%{searchText}%'";
            }
        }

        private void ShowError(string message)
        {
            MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowSuccess(string message)
        {
            MessageBox.Show(message, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        private bool ShowConfirmation(string message)
        {
            return MessageBox.Show(message, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question)
                == MessageBoxResult.Yes;
        }
    }
}

