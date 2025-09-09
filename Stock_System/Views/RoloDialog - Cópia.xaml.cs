using EstoqueRolos.Models;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using static System.Runtime.CompilerServices.RuntimeHelpers;

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
                    Code = existente.Code,
                    Descricao = existente.Descricao,
                    Milimetragem = existente.Milimetragem,
                    MOQ = existente.MOQ,
                    Estoque = existente.Estoque,
                    MetragemWIP = existente.MetragemWIP
                };

                txtCode.Text = RoloEditado.Code;
                txtDescricao.Text = RoloEditado.Descricao;
                txtMM.Text = RoloEditado.Milimetragem.ToString(CultureInfo.InvariantCulture);
                txtMOQ.Text = RoloEditado.MOQ.ToString(CultureInfo.InvariantCulture);
                txtEstoque.Text = RoloEditado.Estoque.ToString(CultureInfo.InvariantCulture);
                txtWIP.Text = RoloEditado.MetragemWIP.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void Salvar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCode.Text))
                {
                    MessageBox.Show("Informe o código do rolo.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var milimetragem = int.Parse(txtMM.Text.Trim(), CultureInfo.InvariantCulture);
                var estoque = decimal.Parse(txtEstoque.Text.Trim(), CultureInfo.InvariantCulture);
                var wip = decimal.Parse(txtWIP.Text.Trim(), CultureInfo.InvariantCulture);
                var moq = decimal.Parse(txtMOQ.Text.Trim(), CultureInfo.InvariantCulture);

                RoloEditado ??= new Rolo();
                RoloEditado.Code = txtCode.Text.Trim();
                RoloEditado.Descricao = txtDescricao.Text.Trim();
                RoloEditado.Milimetragem = milimetragem;
                RoloEditado.Estoque = estoque;
                RoloEditado.MetragemWIP = wip;
                RoloEditado.MOQ = moq;

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
