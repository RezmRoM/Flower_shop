using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MenuManager;

namespace Flower_shop
{
    public partial class MainWindow : Window
    {
        private string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user311_db;password=user311;MultipleActiveResultSets=True;App=EntityFramework";

        public MainWindow()
        {
            InitializeComponent();
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
            var result = MessageBox.Show("Вы уверены, что хотите выйти из приложения?",
                                       "Подтверждение выхода",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private async void authorization_Click(object sender, RoutedEventArgs e)
        {
            string login = loginBox.Text;
            string password = passwordBox.Text;

            if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.",
                              "Ошибка валидации",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Показываем индикатор загрузки
                loadingIndicator.Visibility = Visibility.Visible;
                rootContent.IsEnabled = false;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    string query = "SELECT Rol FROM FS_Polzovatel WHERE login = @login AND password = @password";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);

                        object result = await command.ExecuteScalarAsync();

                        if (result != null)
                        {
                            string role = result.ToString();
                            Window nextWindow = null;

                            switch (role)
                            {
                                case "Админ":
                                    nextWindow = new MainMenuAdmin();
                                    break;
                                case "Покупатель":
                                    nextWindow = new MainMenuCustomer();
                                    break;
                                case "Менеджер":
                                    nextWindow = new MenegerMainMenu();
                                    break;
                                default:
                                    MessageBox.Show("Неизвестная роль пользователя.",
                                                  "Ошибка авторизации",
                                                  MessageBoxButton.OK,
                                                  MessageBoxImage.Error);
                                    return;
                            }

                            if (nextWindow != null)
                            {
                                nextWindow.Show();
                                this.Close();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Неверный логин или пароль.",
                                          "Ошибка авторизации",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Warning);
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Ошибка при подключении к базе данных: {ex.Message}",
                              "Ошибка базы данных",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла непредвиденная ошибка: {ex.Message}",
                              "Системная ошибка",
                              MessageBoxButton.OK,
                              MessageBoxImage.Error);
            }
            finally
            {
                // Скрываем индикатор загрузки
                loadingIndicator.Visibility = Visibility.Collapsed;
                rootContent.IsEnabled = true;
            }
        }

        private void registration_Click(object sender, RoutedEventArgs e)
        {
            Registration registrationWindow = new Registration();
            registrationWindow.Show();
            this.Close();
        }

        private void login_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void password_TextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInput();
        }

        private void ValidateInput()
        {
            bool isValid = !string.IsNullOrEmpty(loginBox.Text) && !string.IsNullOrEmpty(passwordBox.Text);
            authorization.IsEnabled = isValid;
        }
    }
}
