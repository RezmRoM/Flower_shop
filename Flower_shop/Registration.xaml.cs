using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Flower_shop
{
    public partial class Registration : Window
    {
        private string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user311_db;password=user311;MultipleActiveResultSets=True;App=EntityFramework";
        private const int MIN_PASSWORD_LENGTH = 6;
        private static readonly Regex PhoneRegex = new Regex(@"^\+?[1-9]\d{10}$");

        public Registration()
        {
            InitializeComponent();
            InitializeValidation();
        }

        private void InitializeValidation()
        {
            txtName.TextChanged += ValidateInput;
            txtSurname.TextChanged += ValidateInput;
            txtPhone.TextChanged += ValidateInput;
            txtAddress.TextChanged += ValidateInput;
            txtLogin.TextChanged += ValidateInput;
            txtPassword.TextChanged += ValidateInput;

            ValidateInput(null, null);
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
            var result = MessageBox.Show("Вы уверены, что хотите вернуться к окну авторизации?",
                                       "Подтверждение",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private bool ValidatePhoneNumber(string phoneNumber)
        {
            return PhoneRegex.IsMatch(phoneNumber);
        }

        private void ValidateInput(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            bool isValid = !string.IsNullOrWhiteSpace(txtName.Text) &&
                          !string.IsNullOrWhiteSpace(txtSurname.Text) &&
                          !string.IsNullOrWhiteSpace(txtPhone.Text) &&
                          !string.IsNullOrWhiteSpace(txtAddress.Text) &&
                          !string.IsNullOrWhiteSpace(txtLogin.Text) &&
                          !string.IsNullOrWhiteSpace(txtPassword.Text) &&
                          txtPassword.Text.Length >= MIN_PASSWORD_LENGTH &&
                          ValidatePhoneNumber(txtPhone.Text);

            btnRegister.IsEnabled = isValid;
        }

        private async void registrationButton_Click(object sender, RoutedEventArgs e)
        {
            string firstName = txtName.Text;
            string lastName = txtSurname.Text;
            string phoneNumber = txtPhone.Text;
            string address = txtAddress.Text;
            string login = txtLogin.Text;
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(phoneNumber) ||
                string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.",
                              "Ошибка валидации",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            if (password.Length < MIN_PASSWORD_LENGTH)
            {
                MessageBox.Show($"Пароль должен содержать не менее {MIN_PASSWORD_LENGTH} символов.",
                              "Ошибка валидации",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            if (!ValidatePhoneNumber(phoneNumber))
            {
                MessageBox.Show("Пожалуйста, введите корректный номер телефона в формате +79XXXXXXXXX.",
                              "Ошибка валидации",
                              MessageBoxButton.OK,
                              MessageBoxImage.Warning);
                return;
            }

            try
            {
                loadingIndicator.Visibility = Visibility.Visible;
                mainContent.IsEnabled = false;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string checkQuery = "SELECT COUNT(*) FROM FS_Polzovatel WHERE Login = @login";
                    using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@login", login);
                        int userCount = (int)await checkCommand.ExecuteScalarAsync();
                        if (userCount > 0)
                        {
                            MessageBox.Show("Пользователь с таким логином уже существует.",
                                          "Ошибка регистрации",
                                          MessageBoxButton.OK,
                                          MessageBoxImage.Warning);
                            return;
                        }
                    }

                    string query = @"INSERT INTO FS_Polzovatel 
                                   (First_Name, Last_Name, Phone_Number, Address, Login, Password, Rol)
                                   VALUES (@firstName, @lastName, @phoneNumber, @address, @login, @password, 'Покупатель')";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@firstName", firstName);
                        command.Parameters.AddWithValue("@lastName", lastName);
                        command.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                        command.Parameters.AddWithValue("@address", address);
                        command.Parameters.AddWithValue("@login", login);
                        command.Parameters.AddWithValue("@password", password);

                        await command.ExecuteNonQueryAsync();
                        MessageBox.Show("Регистрация прошла успешно!",
                                      "Успех",
                                      MessageBoxButton.OK,
                                      MessageBoxImage.Information);

                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
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
                loadingIndicator.Visibility = Visibility.Collapsed;
                mainContent.IsEnabled = true;
            }
        }
    }
}
