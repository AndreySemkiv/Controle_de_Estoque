using EstoqueRolos.Data;
using EstoqueRolos.Models;
using EstoqueRolos.Utils;
using EstoqueRolos.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Data.Sqlite;
using Microsoft.Win32;

namespace EstoqueRolos
{
    public partial class MainWindow : Window
    {
        private string _appDatabasePath = App.DatabaseFilePath;
        private System.Timers.Timer? _autosaveTimer;
        private object _dbLock = new object();
        private readonly RoloRepository _repo = new RoloRepository();
        private ObservableCollection<Rolo> _dados = new ObservableCollection<Rolo>();

        public MainWindow()
        {
            InitializeComponent();
            StartAutoSaveTimer();
            dgRolos.ItemsSource = _dados;
            _ = CarregarAsync();
        }

        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            var result = MessageBox.Show("Deseja fazer um backup do seu banco de dados?", "Backup do Banco de Dados", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                return;
            }

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    var dlg = new SaveFileDialog();
                    dlg.FileName = $"estoque_bobinas_backup_{DateTime.Now:yyyyMMdd_HHmmss}.db";
                    dlg.Filter = "SQLite Database (*.db)|*.db|All files (*.*)|*.*";

                    if (dlg.ShowDialog(this) == true)
                    {
                        File.Copy(App.DatabaseFilePath, dlg.FileName, overwrite: true);
                        MessageBox.Show("Backup realizado com sucesso.", "Backup", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Falha ao realizar backup: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    e.Cancel = true;
                }
            }
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
                var dlg = new SaveFileDialog
                {
                    Filter = "Excel (*.xlsx)|*.xlsx",
                    FileName = "Exportacao.xlsx"
                };

                if (dlg.ShowDialog() == true)
                {
                    var caminho = ExcelExporter.Exportar(_dados, Path.GetDirectoryName(dlg.FileName)!);
                    MessageBox.Show("Exportado com sucesso: " + caminho, "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (File.Exists(caminho))
                        Process.Start(new ProcessStartInfo() { FileName = caminho, UseShellExecute = true });
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

        private void Calcular_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new CalculadoraMetragem { Owner = this };
            dlg.ShowDialog();
        }

        private void StartAutoSaveTimer()
        {
            _autosaveTimer = new System.Timers.Timer(5000);
            _autosaveTimer.Elapsed += (s, e) => DoCheckpoint();
            _autosaveTimer.AutoReset = true;
            _autosaveTimer.Start();
        }

        private void DoCheckpoint()
        {
            lock (_dbLock)
            {
                try
                {
                    using (var connection = new SqliteConnection($"Data Source={_appDatabasePath}"))
                    {
                        connection.Open();
                        using (var cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = "PRAGMA wal_checkpoint(TRUNCATE);";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch
                {

                }
            }
        }


        private void MenuCarregarBanco_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var openFileDialog = new Microsoft.Win32.OpenFileDialog
                {
                    Filter = "Banco de Dados SQLite (*.db;*.sqlite)|*.db;*.sqlite|Todos os arquivos (*.*)|*.*",
                    Title = "Selecionar Banco de Dados"
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string novoBanco = openFileDialog.FileName;
                    string destinoBanco = App.DatabaseFilePath;

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    File.Copy(novoBanco, destinoBanco, overwrite: true);

                    try
                    {
                        var settingsType = Type.GetType("Stock_System.Properties.Settings")
                                           ?? Type.GetType("EstoqueRolos.Properties.Settings")
                                           ?? AppDomain.CurrentDomain.GetAssemblies()
                                                .Select(a => a.GetType("Properties.Settings"))
                                                .FirstOrDefault(t => t != null);

                        if (settingsType != null)
                        {
                            var defaultProp = settingsType.GetProperty("Default", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
                            var settingsInstance = defaultProp?.GetValue(null);
                            if (settingsInstance != null)
                            {
                                var prop = settingsType.GetProperty("DatabasePath")
                                           ?? settingsType.GetProperty("DatabaseFilePath")
                                           ?? settingsType.GetProperty("ConnectionString")
                                           ?? settingsType.GetProperty("DbPath");

                                if (prop != null && prop.CanWrite)
                                    prop.SetValue(settingsInstance, destinoBanco);

                                var saveMethod = settingsType.GetMethod("Save", Type.EmptyTypes);
                                saveMethod?.Invoke(settingsInstance, null);
                            }
                        }
                    }
                    catch { }

                    MessageBox.Show("Banco de dados carregado com sucesso! O sistema será reiniciado.",
                                    "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Reinicializa o aplicativo de forma segura
                    string exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;

                    Task.Run(() =>
                    {
                        // Aguarda o app atual encerrar antes de abrir o novo
                        System.Threading.Thread.Sleep(1000);
                        System.Diagnostics.Process.Start(exePath);
                    });

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Application.Current.Shutdown();
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar banco de dados:\n{ex.Message}",
                                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
