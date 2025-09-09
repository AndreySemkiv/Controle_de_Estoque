using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;

namespace EstoqueRolos.Views
{
    public partial class CalculadoraMetragem : Window
    {
        public CalculadoraMetragem()
        {
            InitializeComponent();
        }

        
        private void NumberValidationTextBox(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            Regex regex = new Regex(@"[^0-9\.,]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Calcular_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                double R = double.Parse(txtR.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double r = double.Parse(txtr.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double E0 = double.Parse(txtE0.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double E = double.Parse(txtE.Text.Replace(",", "."), CultureInfo.InvariantCulture);
                double Mmax = double.Parse(txtMmax.Text.Replace(",", "."), CultureInfo.InvariantCulture);

                // Confere se os valores estão adequados
                if (R <= r)
                {
                    MessageBox.Show("R deve ser maior que r.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (E < 0)
                {
                    MessageBox.Show("E deve ser maior ou igual a 0.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (E < E0)
                {
                    MessageBox.Show("E deve ser maior ou igual a E0.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Fórmula
                double numerador = Math.Pow(R - E, 2) - Math.Pow(r, 2);
                double denominador = Math.Pow(R - E0, 2) - Math.Pow(r, 2);

                if (denominador <= 0)
                {
                    MessageBox.Show("Divisão inválida. Verifique os valores.", "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                double resultado = (numerador / denominador) * Mmax;

                
                resultado = Math.Floor(resultado);

                txtResultado.Text = resultado.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                MessageBox.Show("Preencha todos os campos corretamente.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Copiar_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtResultado.Text))
                Clipboard.SetText(txtResultado.Text);
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void myComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (myComboBox.SelectedItem is System.Windows.Controls.ComboBoxItem item)
            {
                switch (item.Content.ToString())
                {
                    case "Bobina de 200mm":
                        txtR.Text = "100";
                        txtr.Text = "62,5";
                        txtE0.Text = "0,5";
                        txtMmax.Text = "4200";
                        break;
                    case "Bobina de 250mm":
                        txtR.Text = "125";
                        txtr.Text = "80";
                        txtE0.Text = "0,7";
                        txtMmax.Text = "4200";
                        break;
                    case "Bobina de 355mm":
                        txtR.Text = "177,5";
                        txtr.Text = "112,5";
                        txtE0.Text = "0,9";
                        txtMmax.Text = "4200";
                        break;
                    default:
                        txtR.Clear();
                        txtr.Clear();
                        txtE0.Clear();
                        txtMmax.Clear();
                        break;
                }
            }
        }
    }
}
