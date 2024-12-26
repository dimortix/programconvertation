using System;
using System.Collections.Generic;
using System.Windows;
using System.Globalization;
using System.IO;
using Newtonsoft.Json;


namespace ConverterApp
{
    public partial class MainWindow : Window
    {
        private List<string> operationHistory = new List<string>();
        private UserProfile currentUser = new UserProfile();

        public MainWindow()
        {
            InitializeComponent();
            SetupEventHandlers();
            LoadLastProfile();
        }

        private void SetupEventHandlers()
        {
            btnProfile.Click += (s, e) => ShowProfileWindow();
            btnMeasures.Click += (s, e) => ShowMeasuresConversion();
            btnCurrency.Click += (s, e) => ShowCurrencyConversion();
            btnCrypto.Click += (s, e) => ShowCryptoConversion();
            btnTime.Click += (s, e) => ShowTimeConversion();
            btnHistory.Click += (s, e) => ShowHistory();
            btnExit.Click += (s, e) => Close();
            btnConvert.Click += (s, e) => ConvertValue();
            btnAddToFavorites.Click += (s, e) => AddToFavorites();
            btnRemoveFromFavorites.Click += (s, e) => RemoveFromFavorites();
            btnSaveRate.Click += (s, e) => SaveCustomRate();
            btnDeleteRate.Click += (s, e) => DeleteCustomRate();
        }

        private void ShowProfileWindow()
        {
            var profileWindow = new ProfileWindow(currentUser);
            profileWindow.ShowDialog();
        }

        private void LoadLastProfile()
        {
            try
            {
                if (File.Exists("last_profile.txt"))
                {
                    string lastName = File.ReadAllText("last_profile.txt");
                    if (File.Exists($"profile_{lastName}.json"))
                    {
                        var json = File.ReadAllText($"profile_{lastName}.json");
                        currentUser = JsonConvert.DeserializeObject<UserProfile>(json);
                        UpdateUIFromProfile();
                    }
                }
            }
            catch
            {
                
            }
        }

        private void UpdateUIFromProfile()
        {
            lstFavorites.Items.Clear();
            foreach (var favorite in currentUser.FavoriteConversions)
            {
                lstFavorites.Items.Add(favorite);
            }
            UpdateCustomRatesComboBox();
        }

        private void ShowMeasuresConversion()
        {
            cmbConversionType.Items.Clear();
            cmbConversionType.Items.Add("Километры в метры");
            cmbConversionType.Items.Add("Метры в километры");
            cmbConversionType.Items.Add("Метры в сантиметры");
            cmbConversionType.Items.Add("Сантиметры в метры");
            cmbConversionType.Items.Add("Километры в мили");
            cmbConversionType.Items.Add("Мили в километры");
            cmbConversionType.Items.Add("Килограммы в граммы");
            cmbConversionType.Items.Add("Граммы в килограммы");
            cmbConversionType.SelectedIndex = 0;
        }

        private void ShowCurrencyConversion()
        {
            cmbConversionType.Items.Clear();
            cmbConversionType.Items.Add("RUB в USD");
            cmbConversionType.Items.Add("USD в RUB");
            cmbConversionType.Items.Add("RUB в EUR");
            cmbConversionType.Items.Add("EUR в RUB");
            cmbConversionType.Items.Add("USD в EUR");
            cmbConversionType.Items.Add("EUR в USD");
            cmbConversionType.SelectedIndex = 0;
        }

        private void ShowCryptoConversion()
        {
            cmbConversionType.Items.Clear();
            cmbConversionType.Items.Add("BTC в USD");
            cmbConversionType.Items.Add("USD в BTC");
            cmbConversionType.Items.Add("ETH в USD");
            cmbConversionType.Items.Add("USD в ETH");
            cmbConversionType.Items.Add("BTC в ETH");
            cmbConversionType.Items.Add("ETH в BTC");
            cmbConversionType.SelectedIndex = 0;
        }

        private void ShowTimeConversion()
        {
            cmbConversionType.Items.Clear();
            cmbConversionType.Items.Add("Часы в минуты");
            cmbConversionType.Items.Add("Минуты в часы");
            cmbConversionType.Items.Add("Дни в часы");
            cmbConversionType.Items.Add("Часы в дни");
            cmbConversionType.Items.Add("Минуты в секунды");
            cmbConversionType.Items.Add("Секунды в минуты");
            cmbConversionType.SelectedIndex = 0;
        }

        private void ShowHistory()
        {
            lstHistory.Items.Clear();
            foreach (string operation in operationHistory)
            {
                lstHistory.Items.Add(operation);
            }
        }

        private void AddToFavorites()
        {
            var currentConversion = cmbConversionType.SelectedItem?.ToString();
            if (currentConversion != null && !currentUser.FavoriteConversions.Contains(currentConversion))
            {
                currentUser.FavoriteConversions.Add(currentConversion);
                lstFavorites.Items.Add(currentConversion);
                SaveUserProfile();
            }
        }

        private void RemoveFromFavorites()
        {
            var selectedFavorite = lstFavorites.SelectedItem?.ToString();
            if (selectedFavorite != null)
            {
                currentUser.FavoriteConversions.Remove(selectedFavorite);
                lstFavorites.Items.Remove(selectedFavorite);
                SaveUserProfile();
            }
        }

        private void SaveCustomRate()
        {
            var conversionType = cmbConversionType.SelectedItem?.ToString();
            if (conversionType != null && double.TryParse(txtCustomRate.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double rate))
            {
                currentUser.CustomRates[conversionType] = rate;
                UpdateCustomRatesComboBox();
                SaveUserProfile();
                MessageBox.Show($"Пользовательский курс для '{conversionType}' сохранен!");
            }
            else
            {
                MessageBox.Show("Пожалуйста, введите корректное значение курса!");
            }
        }

        private void DeleteCustomRate()
        {
            var selectedRate = cmbCustomRates.SelectedItem?.ToString();
            if (selectedRate != null && selectedRate != "Нет сохраненных курсов")
            {
                var conversionName = selectedRate.Split(':')[0].Trim();

                if (currentUser.CustomRates.ContainsKey(conversionName))
                {
                    var result = MessageBox.Show(
                        $"Вы уверены, что хотите удалить курс '{conversionName}'?",
                        "Подтверждение удаления",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        currentUser.CustomRates.Remove(conversionName);
                        UpdateCustomRatesComboBox();
                        SaveUserProfile();
                        MessageBox.Show("Курс успешно удален!");
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите курс для удаления!");
            }
        }

        private void UpdateCustomRatesComboBox()
        {
            cmbCustomRates.Items.Clear();
            foreach (var rate in currentUser.CustomRates)
            {
                cmbCustomRates.Items.Add($"{rate.Key}: {rate.Value}");
            }

            if (cmbCustomRates.Items.Count == 0)
            {
                cmbCustomRates.Items.Add("Нет сохраненных курсов");
            }

            cmbCustomRates.SelectedIndex = 0;
        }

        private void ConvertValue()
        {
            if (string.IsNullOrEmpty(txtInput.Text) || !double.TryParse(txtInput.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
            {
                MessageBox.Show("Пожалуйста, введите корректное число");
                return;
            }

            string result = "";
            string conversionType = cmbConversionType.SelectedItem?.ToString();

            if (currentUser.CustomRates.ContainsKey(conversionType))
            {
                double customRate = currentUser.CustomRates[conversionType];
                result = $"{value} {conversionType.Split(' ')[0]} = {(value * customRate):F2} {conversionType.Split(' ')[2]}";
            }
            else
            {
                switch (conversionType)
                {
                    
                    case "Километры в метры":
                        result = $"{value} км = {(value * 1000):F2} м";
                        break;
                    case "Метры в километры":
                        result = $"{value} м = {(value / 1000):F2} км";
                        break;
                    case "Метры в сантиметры":
                        result = $"{value} м = {(value * 100):F2} см";
                        break;
                    case "Сантиметры в метры":
                        result = $"{value} см = {(value / 100):F2} м";
                        break;
                    case "Километры в мили":
                        result = $"{value} км = {(value * 0.621371):F2} миль";
                        break;
                    case "Мили в километры":
                        result = $"{value} миль = {(value / 0.621371):F2} км";
                        break;
                    case "Килограммы в граммы":
                        result = $"{value} кг = {(value * 1000):F2} г";
                        break;
                    case "Граммы в килограммы":
                        result = $"{value} г = {(value / 1000):F2} кг";
                        break;

                    
                    case "RUB в USD":
                        result = $"{value} RUB = {(value * 0.011):F2} USD";
                        break;
                    case "USD в RUB":
                        result = $"{value} USD = {(value / 0.011):F2} RUB";
                        break;
                    case "RUB в EUR":
                        result = $"{value} RUB = {(value * 0.010):F2} EUR";
                        break;
                    case "EUR в RUB":
                        result = $"{value} EUR = {(value / 0.010):F2} RUB";
                        break;
                    case "USD в EUR":
                        result = $"{value} USD = {(value * 0.91):F2} EUR";
                        break;
                    case "EUR в USD":
                        result = $"{value} EUR = {(value / 0.91):F2} USD";
                        break;

                    
                    case "BTC в USD":
                        result = $"{value} BTC = {(value * 101000):F2} USD";
                        break;
                    case "USD в BTC":
                        result = $"{value} USD = {(value / 101000):F8} BTC";
                        break;
                    case "ETH в USD":
                        result = $"{value} ETH = {(value * 3500):F2} USD";
                        break;
                    case "USD в ETH":
                        result = $"{value} USD = {(value / 3500):F8} ETH";
                        break;
                    case "BTC в ETH":
                        result = $"{value} BTC = {(value * 101000 / 3500):F8} ETH";
                        break;
                    case "ETH в BTC":
                        result = $"{value} ETH = {(value * 3500 / 101000):F8} BTC";
                        break;

                    
                    case "Часы в минуты":
                        result = $"{value} часов = {(value * 60):F2} минут";
                        break;
                    case "Минуты в часы":
                        result = $"{value} минут = {(value / 60):F2} часов";
                        break;
                    case "Дни в часы":
                        result = $"{value} дней = {(value * 24):F2} часов";
                        break;
                    case "Часы в дни":
                        result = $"{value} часов = {(value / 24):F2} дней";
                        break;
                    case "Минуты в секунды":
                        result = $"{value} минут = {(value * 60):F2} секунд";
                        break;
                    case "Секунды в минуты":
                        result = $"{value} секунд = {(value / 60):F2} минут";
                        break;
                }
            }

            txtResult.Text = $"Результат: {result}";
            operationHistory.Add(result);
            lstHistory.Items.Add(result);
        }

        private void SaveUserProfile()
        {
            try
            {
                var json = JsonConvert.SerializeObject(currentUser);
                File.WriteAllText($"profile_{currentUser.Name}.json", json);
                File.WriteAllText("last_profile.txt", currentUser.Name);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении профиля: {ex.Message}");
            }
        }

        private void lstFavorites_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }
    }
}