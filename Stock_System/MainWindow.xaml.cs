using EstoqueRolos.Data;
using EstoqueRolos.Models;
using EstoqueRolos.Utils;
using EstoqueRolos.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace EstoqueRolos
{
    public partial class MainWindow : Window
    {
        private readonly RoloRepository _repo = new RoloRepository();
        private ObservableCollection<Rolo> _dados = new ObservableCollection<Rolo>();

        public MainWindow()
        {
            InitializeComponent();
            dgRolos.ItemsSource = _dados;
            _ = CarregarAsync();
        }

        private async Task CarregarAsync()
        {
            try
            {
                var todos = await _repo.GetAllAsync();
                _dados.Clear();
                foreach (var r in todos)
                    _dados.Add(r);
                dgRolos.ItemsSource = _dados;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao carregar dados: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void Atualizar_Click(object sender, RoutedEventArgs e) => await CarregarAsync();

        private async void Novo_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new RoloDialog();
            if (dlg.ShowDialog() == true && dlg.RoloEditado != null)
            {
                try
                {
                    var novo = dlg.RoloEditado;
                    await _repo.InsertAsync(novo);
                    _dados.Insert(0, novo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao inserir: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private Rolo? Selecionado => dgRolos.SelectedItem as Rolo;

        private async void Editar_Click(object sender, RoutedEventArgs e)
        {
            if (Selecionado == null)
            {
                MessageBox.Show("Selecione um item para editar.");
                return;
            }

            var dlg = new RoloDialog(Selecionado);
            if (dlg.ShowDialog() == true && dlg.RoloEditado != null)
            {
                try
                {
                    var editado = dlg.RoloEditado;
                    await _repo.UpdateAsync(editado);

                    // Atualiza na lista local
                    var idx = _dados.IndexOf(Selecionado);
                    if (idx >= 0)
                    {
                        _dados[idx] = editado;
                        dgRolos.ItemsSource = null;
                        dgRolos.ItemsSource = _dados;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao atualizar: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void Excluir_Click(object sender, RoutedEventArgs e)
        {
            if (Selecionado == null)
            {
                MessageBox.Show("Selecione um item para excluir.");
                return;
            }
            if (MessageBox.Show($"Excluir rolo '{Selecionado.Code}'?", "Confirmar", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                try
                {
                    await _repo.DeleteAsync(Selecionado.Code);
                    _dados.Remove(Selecionado);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao excluir: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Exportar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Excel (*.xlsx)|*.xlsx",
                    FileName = "Exportacao.xlsx"
                };

                if (dlg.ShowDialog() == true)
                {
                    var caminho = ExcelExporter.Exportar(_dados, Path.GetDirectoryName(dlg.FileName)!);
                    MessageBox.Show("Exportado com sucesso: " + caminho, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (File.Exists(caminho))
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo() { FileName = caminho, UseShellExecute = true });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao exportar: " + ex.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Buscar_Click(object sender, RoutedEventArgs e)
        {
            var termo = txtBusca.Text?.Trim();
            if (string.IsNullOrEmpty(termo))
            {
                dgRolos.ItemsSource = _dados;
                return;
            }

            var filtrados = _dados.Where(r => r.Code != null && r.Code.IndexOf(termo, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
            dgRolos.ItemsSource = filtrados;
        }

        private async void LimparBusca_Click(object sender, RoutedEventArgs e)
        {
            txtBusca.Text = string.Empty;
            dgRolos.ItemsSource = _dados;
            await CarregarAsync();
        }
    }
}
