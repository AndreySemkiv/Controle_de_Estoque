using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Windows;

namespace EstoqueRolos
{
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                string appDataPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "EstoqueRolos"
                );

                string flagFile = Path.Combine(appDataPath, "pending_db_path.txt");
                string databaseFolder = Path.Combine(appDataPath, "Database");
                string targetDatabase = Path.Combine(databaseFolder, "data.db");

                if (File.Exists(flagFile))
                {
                    string newDbPath = File.ReadAllText(flagFile);

                    Directory.CreateDirectory(databaseFolder);

                    File.Copy(newDbPath, targetDatabase, overwrite: true);

                    File.Delete(flagFile);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao aplicar novo banco de dados:" + ex.Message,
                                "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static IConfiguration Configuration { get; private set; } = null!;
        
        public static readonly object DbLock = new object();
       
        public static string DatabaseFilePath { get; private set; } = null!;

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();

            
            var local = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folder = Path.Combine(local, "EstoqueRolos", "Database");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            DatabaseFilePath = Path.Combine(folder, "estoque_bobinas.db");

            EnsureDatabase();
        }

        private void EnsureDatabase()
        {
            
            try
            {
                if (!File.Exists(DatabaseFilePath))
                {
                    
                    File.WriteAllBytes(DatabaseFilePath, new byte[0]);
                }

                
                using (var conn = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={DatabaseFilePath}"))
                {
                    conn.Open();
                    
                    using (var pragma = conn.CreateCommand())
                    {
                        pragma.CommandText = "PRAGMA journal_mode=WAL;";
                        pragma.ExecuteNonQuery();
                    }
                    var cmd = conn.CreateCommand();
                    cmd.CommandText = @"CREATE TABLE IF NOT EXISTS rolos (
                        Code TEXT PRIMARY KEY,
                        Descricao TEXT,
                        Milimetragem INTEGER,
                        MOQ INTEGER,
                        Estoque REAL,
                        MetragemWIP REAL
                    );";
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                
            }
        }

       
        public static void CloseDatabaseConnection()
        {
            try
            {
                
                var appType = typeof(App);
                var field = appType.GetField("DbConnection", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (field != null)
                {
                    var conn = field.GetValue(null) as IDisposable;
                    conn?.Dispose();
                    field.SetValue(null, null);
                }

                // Fecha também conexões dentro de repositórios se existirem
                try
                {
                    var repoType = Type.GetType("EstoqueRolos.Data.RoloRepository");
                    if (repoType != null)
                    {
                        var connField = repoType.GetField("_connection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                        if (connField != null)
                        {
                            var conn = connField.GetValue(null) as IDisposable;
                            conn?.Dispose();
                            connField.SetValue(null, null);
                        }
                    }
                }
                catch { }

               
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Falha ao fechar conexões: " + ex.Message);
            }
        }
    }
}
