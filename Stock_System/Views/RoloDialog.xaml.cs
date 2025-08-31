using EstoqueRolos.Models;
using System;
using System.Globalization;
using System.Windows;

namespace EstoqueRolos.Views
{
    public partial class RoloDialog : Window
    {
        public Rolo? RoloEditado { get; private set; }

        public RoloDialog(Rolo? existente = null)
        {
            InitializeComponent();

            if (existente != null)
            {
                RoloEditado = new Rolo
                {
                    Id = existente.Id,
                    IdRolo = existente.IdRolo,
                    Milimetragem = existente.Milimetragem,
                    MetragemDisponivel = existente.MetragemDisponivel,
                    WIP = existente.WIP
                };

                txtIdRolo.Text = RoloEditado.IdRolo;
                txtMM.Text = RoloEditado.Milimetragem.ToString(CultureInfo.InvariantCulture);
                txtMetragem.Text = RoloEditado.MetragemDisponivel.ToString(CultureInfo.InvariantCulture);
                cbWip.SelectedIndex = RoloEditado.WIP == "Producao" ? 1 : 0;
            }
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtIdRolo.Text))
                {
                    MessageBox.Show("Informe o Id do rolo.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var mm = int.Parse(txtMM.Text.Trim(), CultureInfo.InvariantCulture);
                var met = double.Parse(txtMetragem.Text.Trim(), CultureInfo.InvariantCulture);
                var wip = ((cbWip.SelectedItem as System.Windows.Controls.ComboBoxItem)!.Content as string)!;

                RoloEditado ??= new Rolo();
                RoloEditado.IdRolo = txtIdRolo.Text.Trim();
                RoloEditado.Milimetragem = mm;
                RoloEditado.MetragemDisponivel = met;
                RoloEditado.WIP = wip;

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao validar dados: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
