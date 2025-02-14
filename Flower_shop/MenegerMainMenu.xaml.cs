using System;
using System.Collections.Generic;
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
using System.Data.SqlClient;
using System.Data;

namespace MenuManager
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MenegerMainMenu : Window
    {
        private string connectionString = "data source=stud-mssql.sttec.yar.ru,38325;user id=user311_db;password=user311;MultipleActiveResultSets=True;App=EntityFramework";

        public MenegerMainMenu()
        {
            InitializeComponent();

            Loaded += MenegerMainMenu_Loaded;
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
                            FS_Tovar t ON zt.ID_Tovar = t.ID_Tovar;";

                    SqlCommand command = new SqlCommand(query, connection);

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    zakazDataGrid.ItemsSource = dataTable.DefaultView;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки данных: " + ex.Message);
            }
        }

        private void LoadClientNames()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT First_Name FROM FS_Polzovatel";
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
                MessageBox.Show("Ошибка загрузки имен клиентов: " + ex.Message);
            }
        }

        private void LoadProductNames()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT Product_Name FROM FS_Tovar";
                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataReader reader = command.ExecuteReader();

                    List<string> productNames = new List<string>();
                    while (reader.Read())
                    {
                        productNames.Add(reader["Product_Name"].ToString());
                    }
                    tovarCombo.ItemsSource = productNames;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки названий товаров: " + ex.Message);
            }
        }

        private void OformBox_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string clientName = clientCombo.SelectedItem as string;
                string productName = tovarCombo.SelectedItem as string;
                int kolichestvo = int.Parse(colvoBox.Text);
                DateTime orderDate = DateTime.Now;

                int idPolzovatel = GetIdPolzovatel(clientName);
                int idTovar = GetIdTovar(productName);

                decimal priceTovar = GetPriceTovar(idTovar);
                decimal priceZakaza = priceTovar * kolichestvo;

                int idZakaz = InsertZakaz(idPolzovatel, orderDate);

                InsertZakaznieTovari(idZakaz, idTovar, kolichestvo, priceZakaza);

                LoadData();

                MessageBox.Show("Заказ успешно оформлен!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при оформлении заказа: " + ex.Message);
            }
        }

        private int GetIdPolzovatel(string clientName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ID_Polzovatel FROM FS_Polzovatel WHERE First_Name = '" + clientName + "'";
                SqlCommand command = new SqlCommand(query, connection);
                return (int)command.ExecuteScalar();
            }
        }

        private int GetIdTovar(string productName)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT ID_Tovar FROM FS_Tovar WHERE Product_Name = '" + productName + "'";
                SqlCommand command = new SqlCommand(query, connection);
                return (int)command.ExecuteScalar();
            }
        }

        private decimal GetPriceTovar(int idTovar)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Price FROM FS_Tovar WHERE ID_Tovar = " + idTovar;
                SqlCommand command = new SqlCommand(query, connection);
                return (decimal)command.ExecuteScalar();
            }
        }

        private int InsertZakaz(int idPolzovatel, DateTime orderDate)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO FS_Zakaz (ID_Polzovatel, Order_Date) OUTPUT INSERTED.ID_Zakaz VALUES (" + idPolzovatel + ", '" + orderDate.ToString("yyyy-MM-dd HH:mm:ss") + "')";
                SqlCommand command = new SqlCommand(query, connection);
                return (int)(decimal)command.ExecuteScalar();
            }
        }

        private void InsertZakaznieTovari(int idZakaz, int idTovar, int kolichestvo, decimal priceZakaza)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO FS_Zakaznie_Tovari (ID_Zakaz, ID_Tovar, Kolichestvo, Price_Zakaza) VALUES (" + idZakaz + ", " + idTovar + ", " + kolichestvo + ", " + priceZakaza.ToString().Replace(",", ".") + ")";
                SqlCommand command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
        }

        private void DeleteBox_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ColvoBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
