using System;
using System.Windows;
using System.Windows.Controls;

namespace Flower_shop
{
    public partial class InputDialog : Window
    {
        public string ResponseText
        {
            get { return ResponseTextBox.Text; }
            set { ResponseTextBox.Text = value; }
        }

        public InputDialog(string prompt, string defaultValue = "")
        {
            InitializeComponent();
            PromptText.Text = prompt;
            ResponseText = defaultValue;
            ResponseTextBox.TextChanged += ValidateImageUrl;
            ResponseTextBox.SelectAll();
            ResponseTextBox.Focus();
        }

        private void ValidateImageUrl(object sender, TextChangedEventArgs e)
        {
            string url = ResponseTextBox.Text.Trim();
            bool isValid = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            if (!string.IsNullOrEmpty(url) && !isValid)
            {
                ResponseTextBox.BorderBrush = System.Windows.Media.Brushes.Red;
                ResponseTextBox.ToolTip = "Введите корректный URL изображения (http:// или https://)";
            }
            else
            {
                ResponseTextBox.ClearValue(Border.BorderBrushProperty);
                ResponseTextBox.ToolTip = null;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string url = ResponseTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(url))
            {
                bool isValid = Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                    && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

                if (!isValid)
                {
                    MessageBox.Show("Пожалуйста, введите корректный URL изображения (http:// или https://)",
                                  "Ошибка валидации",
                                  MessageBoxButton.OK,
                                  MessageBoxImage.Warning);
                    return;
                }
            }
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}