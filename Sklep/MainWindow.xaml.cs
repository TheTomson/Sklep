using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;

namespace Sklep
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int kasa;

        private Dictionary<string, SokInformacje> tabela_sokow;
        private Dictionary<string, KlientInformacje> tabela_klientow;
        public MainWindow()
        {
            kasa = 1000;
            tabela_sokow = new Dictionary<string, SokInformacje>();
            tabela_klientow = new Dictionary<string, KlientInformacje>();
            InitializeComponent();
            kasa_l.Content = kasa;
            PobierzSklep();
            PobierzKlientow();
            AktualizujTabeleSokow();
        }
        public void PobierzSklep()
        {
            using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT NazwaTowaru,Ilosc,Cena FROM Sklep";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var nazwa = reader.GetString(0);
                        var ilosc = Convert.ToInt32(reader.GetString(1));
                        var cena = Convert.ToInt32(reader.GetString(2));
                        tabela_sokow.Add(nazwa, new SokInformacje(ilosc, cena));
                    }
                }

            }
        }

        public void PobierzKlientow()
        {
            using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT Nazwa, Adres, NumerTelefonu, NIP FROM Klienci";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var nazwa = reader.GetString(0);
                        var adres = reader.GetString(1);
                        var numer_tel = reader.GetString(2);
                        var nip = reader.GetString(3);
                        tabela_klientow.Add(nip, new KlientInformacje(nazwa, adres, numer_tel));
                    }
                }
            }
        }

        public void AktualizujTabeleSokow()
        {
            List<WpisWTabeli> tabela = new List<WpisWTabeli>();
            foreach (var sok in tabela_sokow)
            {
                tabela.Add(new WpisWTabeli { nazwa_soku = sok.Key, ilosc = sok.Value.Ilosc, cena = sok.Value.Cena });
            }

            sklep.ItemsSource = tabela;
        }

        private void KupTowar(object sender, RoutedEventArgs e)
        {
            var kuptowar = new KupTowar(this, ref kasa, ref tabela_sokow);
            kuptowar.Show();
        }
        private void DodajKlienta(object sender, RoutedEventArgs e)
        {
            var dodajKlienta = new Klienci(ref tabela_klientow);
            dodajKlienta.Show();
        }
    }
    public class SokInformacje
    {
        public int Ilosc { get; set; }
        public int Cena { get; set; }

        public SokInformacje(int ilosc, int cena)
        {
            Ilosc = ilosc;
            Cena = cena;
        }
    }

    struct WpisWTabeli
    {
        public string nazwa_soku { get; set; }
        public int ilosc { get; set; }
        public int cena { get; set; }
    }

    public class KlientInformacje
    {
        public string Nazwa { get; set; }
        public string Adres { get; set; }
        public string NumerTelefonu { get; set; }

        public KlientInformacje(string n, string a, string nt)
        {
            Nazwa = n;
            Adres = a;
            NumerTelefonu = nt;
        }
    }
}
