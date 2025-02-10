using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Flower_shop
{
    public partial class AddProductDialog : Window
    {
        public string ProductName { get { return ProductNameTextBox.Text; } }
        public string Description { get { return DescriptionTextBox.Text; } }
        public string ImageUrl { get { return ImageUrlTextBox.Text; } }
        public decimal Price { get; private set; }
        public int Quantity { get; private set; }
        public string Category { get { return CategoryComboBox.SelectedItem?.ToString(); } }
        public string Supplier { get { return SupplierComboBox.SelectedItem?.ToString(); } }

        public AddProductDialog(ObservableCollection<string> categories, ObservableCollection<string> suppliers)
        {
            InitializeComponent();
            CategoryComboBox.ItemsSource = categories;
            SupplierComboBox.ItemsSource = suppliers;

            if (categories.Count > 0)
                CategoryComboBox.SelectedIndex = 0;
            if (suppliers.Count > 0)
                SupplierComboBox.SelectedIndex = 0;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ProductName))
            {
                MessageBox.Show("Пожалуйста, введите название товара.");
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Пожалуйста, введите корректную цену.");
                return;
            }

            if (!int.TryParse(QuantityTextBox.Text, out int quantity))
            {
                MessageBox.Show("Пожалуйста, введите корректное количество.");
                return;
            }

            if (Category == null)
            {
                MessageBox.Show("Пожалуйста, выберите категорию.");
                return;
            }

            if (Supplier == null)
            {
                MessageBox.Show("Пожалуйста, выберите поставщика.");
                return;
            }

            Price = price;
            Quantity = quantity;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
} 