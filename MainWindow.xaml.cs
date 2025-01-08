using System;
using System.Windows;

namespace SalesCostCalculator
{
    public partial class MainWindow : Window
    {
        // Global değişkenler
        private const decimal TaxRate = 0.05m; // %5 EA Tax
        private const decimal MaxBuyRatio = 0.10m; // MaxBuy %10 lower

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                decimal salePrice = 0;
                decimal buyPrice = 0;
                decimal profitMargin = 0;

                // Kullanıcı girişlerini kontrol et
                bool isSalePriceProvided = !string.IsNullOrEmpty(SalesTextBox.Text);
                bool isBuyPriceProvided = !string.IsNullOrEmpty(CostTextBox.Text);
                bool isProfitMarginProvided = !string.IsNullOrEmpty(ProfitRatioTextBox.Text);

                if (isSalePriceProvided)
                {
                    salePrice = Convert.ToDecimal(SalesTextBox.Text);
                }

                if (isBuyPriceProvided)
                {
                    buyPrice = Convert.ToDecimal(CostTextBox.Text);
                }

                if (isProfitMarginProvided)
                {
                    profitMargin = Convert.ToDecimal(ProfitRatioTextBox.Text) / 100;
                }

                // Durumları kontrol et ve hesaplama yap
                if (isSalePriceProvided && isBuyPriceProvided)
                {
                    DisplayResults(salePrice, buyPrice);
                }
                else if (isBuyPriceProvided && isProfitMarginProvided)
                {
                    salePrice = CalculateOptimalSalePrice(buyPrice, profitMargin);
                    DisplayResults(salePrice, buyPrice);
                }
                else if (isSalePriceProvided && isProfitMarginProvided)
                {
                    buyPrice = CalculateOptimalBuyPrice(salePrice, profitMargin);
                    DisplayResults(salePrice, buyPrice);
                }
                else if (isSalePriceProvided) 
                {
                    buyPrice = CalculateMaxBuyPrice(salePrice);
                    DisplayResults(salePrice, buyPrice);
                }
                else
                {
                    MessageBox.Show("Please enter at least the Sale Price.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Please enter valid numeric values.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private decimal CalculateMaxBuyPrice(decimal salePrice)
        {
            // Optimal alış fiyatını hesapla
            return salePrice * (1 - MaxBuyRatio);
        }

        private decimal CalculateOptimalSalePrice(decimal buyPrice, decimal profitMargin)
        {
            return buyPrice * (1 + profitMargin) / (1 - TaxRate);
        }

        private decimal CalculateOptimalBuyPrice(decimal salePrice, decimal profitMargin)
        {
            return salePrice / (1 + profitMargin) * (1 - TaxRate);
        }

        private void DisplayResults(decimal salePrice, decimal buyPrice, bool isBuyPriceOptimal = false)
        {
            // Satıştan elde edilen gelir (vergi dahil değil)
            decimal netSaleRevenue = salePrice * (1 - TaxRate);

            // Net kâr hesaplama
            decimal profit = netSaleRevenue - buyPrice;

            // Kâr oranı hesaplama
            decimal profitMargin = profit / buyPrice;

            // Sonuçları uygun şekilde göster
            ResultLabel.Content =
                $"Sale Price: {salePrice}\n" +
                $"Revenue (After Tax): {netSaleRevenue}\n" +
                $"Buy Price: {buyPrice}\n" +
                $"Profit Margin: {profitMargin:P2}\n" +
                $"Net Profit: {profit}";

            // Kâr/Zarar durumuna göre renk belirle
            if (profit > 0)
            {
                ResultLabel.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Green);
            }
            else if (profit < 0)
            {
                ResultLabel.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red);
            }
            else
            {
                ResultLabel.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Gray);
            }
        }
    }
}