using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace FB2MSSQL.ViewModel
{
    public class Table : ViewModelBase
    {
        public Table()
        {
            Colunas = new ObservableCollection<Coluna>();
        }
        private bool @checked;

        public Table(string name, bool @checked)
            : this()
        {
            Name = name;
            Checked = @checked;
        }

        public string Name { get; set; }

        public bool Checked
        {
            get { return @checked; }
            set
            {
                @checked = value;
                RaisePropertyChanged(() => Checked);
            }
        }

        public ObservableCollection<Coluna> Colunas { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - Colunas: {1}", Name, Colunas.Count);
        }
    }
}