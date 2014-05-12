using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace FB2MSSQL.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private string connectionString;
        private string connectionStringMSSQL;

        public MainViewModel()
        {
            Tabelas = new ObservableCollection<Table>();
            TabelasMSSQL = new ObservableCollection<Table>();
            Log = new ObservableCollection<string>();

            if (IsInDesignMode)
            {
                ConnectionString = @"User=SYSDBA;Password=masterkey;Database=C:\temp\CARGAS32_62.GDB;DataSource=localhost;Port=3052;Dialect=3;Charset=NONE;Role=;Connection lifetime=15;Pooling=true;MinPoolSize=0;MaxPoolSize=50;Packet Size=8192;ServerType=0;";
                Tabelas.Add(new Table("AAC", false));
            }
            else
            {
                // Code runs "for real"
            }
            OnMostrarTabelas = new RelayCommand(MostrarTabelas);
            OnMostrarTabelasMSSQL = new RelayCommand(MostrarTabelasMSSQL);
            OnIniciarMigracao = new RelayCommand(IniciarMigracao);
        }

        private void IniciarMigracao()
        {
            if (ConnectionString != null)
            {
                Log.Add("Reconhecendo colunas do Firebird");
                using (var fbConnection = new FbConnection(ConnectionString))
                {
                    fbConnection.Open();
                    foreach (var tabela in Tabelas.Where(t => t.Checked))
                    {
                        Log.Add(string.Format("Reconhecendo colunas da tabela {0}", tabela.Name));
                        var fbCommand = new FbCommand(string.Format("SELECT RF.RDB$FIELD_NAME FIELD_NAME, F.RDB$FIELD_TYPE FIELD_TYPE, COALESCE(F.RDB$FIELD_SUB_TYPE,0) FIELD_SUB_TYPE FROM RDB$RELATION_FIELDS RF JOIN RDB$FIELDS F ON (F.RDB$FIELD_NAME = RF.RDB$FIELD_SOURCE) LEFT OUTER JOIN RDB$CHARACTER_SETS CH ON (CH.RDB$CHARACTER_SET_ID = F.RDB$CHARACTER_SET_ID) LEFT OUTER JOIN RDB$COLLATIONS DCO ON ((DCO.RDB$COLLATION_ID = F.RDB$COLLATION_ID) AND (DCO.RDB$CHARACTER_SET_ID = F.RDB$CHARACTER_SET_ID)) WHERE (RF.RDB$RELATION_NAME = '{0}') AND (COALESCE(RF.RDB$SYSTEM_FLAG, 0) = 0) ORDER BY RF.RDB$FIELD_POSITION;", tabela.Name), fbConnection);
                        using (var fbDataReader = fbCommand.ExecuteReader())
                            while (fbDataReader.Read())
                                tabela.Colunas.Add(new Coluna { Name = fbDataReader.GetString(0).Trim(), Type = fbDataReader.GetInt32(1) , SubType = fbDataReader.GetInt32(2) });
                        Log.Add(string.Format("Reconhecida colunas da tabela {0}", tabela.Name));
                    }
                }
            }
            if (ConnectionStringMSSQL != null)
            {
                Log.Add("Reconhecendo colunas do MS SQL");
                using (var fbConnection = new FbConnection(ConnectionString))
                {
                    fbConnection.Open();
                    foreach (var tabela in Tabelas.Where(t => t.Checked))
                    {
                        Log.Add(string.Format("Reconhecendo colunas da tabela {0}", tabela.Name));
                        var fbCommand = new FbCommand(string.Format("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{0}'", tabela.Name), fbConnection);
                        using (var fbDataReader = fbCommand.ExecuteReader())
                            while (fbDataReader.Read())
                                tabela.Colunas.Add(new Coluna { Name = fbDataReader.GetString(0).Trim(), Type = fbDataReader.GetInt32(1) , SubType = fbDataReader.GetInt32(2) });
                        Log.Add(string.Format("Reconhecida colunas da tabela {0}", tabela.Name));
                    }
                }
            }
        }

        private void MostrarTabelas()
        {
            if (ConnectionString != null)
                using (var fbConnection = new FbConnection(ConnectionString))
                {
                    fbConnection.Open();
                    var fbCommand = new FbCommand(@"SELECT RDB$RELATION_NAME FROM RDB$RELATIONS WHERE RDB$SYSTEM_FLAG = 0 AND RDB$VIEW_BLR IS NULL ORDER BY RDB$RELATION_NAME", fbConnection);
                    using (var fbDataReader = fbCommand.ExecuteReader())
                        while (fbDataReader.Read())
                            Tabelas.Add(new Table(fbDataReader.GetString(0), false));
                }
        }

        public string ConnectionString
        {
            get { return connectionString; }
            set
            {
                connectionString = value;
                RaisePropertyChanged(() => ConnectionString);
                RaisePropertyChanged(() => ThereIsConnectionString);
            }
        }

        public bool ThereIsConnectionString
        {
            get { return !string.IsNullOrWhiteSpace(ConnectionString); }
        }

        public bool TodasMarcadas
        {
            get { return Tabelas.Count > 0 && Tabelas.All(t => t.Checked); }
            set
            {
                Parallel.ForEach(Tabelas, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, tabela => tabela.Checked = value);
            }
        }

        public RelayCommand OnMostrarTabelas { get; set; }

        public ObservableCollection<Table> Tabelas { get; set; }

        private void MostrarTabelasMSSQL()
        {
            if (ConnectionStringMSSQL != null)
                using (var sqlConnection = new SqlConnection(ConnectionStringMSSQL))
                {
                    sqlConnection.Open();
                    var sqlCommand = new SqlCommand(@"SELECT TABLE_NAME FROM information_schema.tables", sqlConnection);
                    using (var sqlDataReader = sqlCommand.ExecuteReader())
                        while (sqlDataReader.Read())
                            TabelasMSSQL.Add(new Table(sqlDataReader.GetString(0), false));
                }
        }

        public string ConnectionStringMSSQL
        {
            get { return connectionStringMSSQL; }
            set
            {
                connectionStringMSSQL = value;
                RaisePropertyChanged(() => ConnectionStringMSSQL);
                RaisePropertyChanged(() => ThereIsConnectionStringMSSQL);
            }
        }

        public bool ThereIsConnectionStringMSSQL
        {
            get { return !string.IsNullOrWhiteSpace(ConnectionStringMSSQL); }
        }

        public bool TodasMarcadasMSSQL
        {
            get { return TabelasMSSQL.Count > 0 && TabelasMSSQL.All(t => t.Checked); }
            set
            {
                Parallel.ForEach(TabelasMSSQL, new ParallelOptions() { MaxDegreeOfParallelism = 10 }, tabela => tabela.Checked = value);
            }
        }

        public RelayCommand OnMostrarTabelasMSSQL { get; set; }

        public ObservableCollection<Table> TabelasMSSQL { get; set; }

        public RelayCommand OnIniciarMigracao { get; set; }

        public ObservableCollection<string> Log { get; set; }
    }
}