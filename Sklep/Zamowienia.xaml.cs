using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Data.Sqlite;

namespace Sklep
{
    /// <summary>
    /// Logika interakcji dla klasy Zamowienia.xaml
    /// </summary>
    public partial class Zamowienia : Window
    {
        private Dictionary<string, SokInformacje> tabela_sokow;
        private Dictionary<string, KlientInformacje> tabela_klientow;
        MainWindow parent_window;
        public Zamowienia(MainWindow parent, ref Dictionary<string, SokInformacje> ts, ref Dictionary<string, KlientInformacje> tk)
        {
            InitializeComponent();
            tabela_sokow = ts;
            tabela_klientow = tk;
            parent_window = parent;
            UstawListy();
        }

        private void UstawListy()
        {
            List<string> klienci = new List<string>();
            foreach (var klient in tabela_klientow)
            {
                klienci.Add(String.Format("{0}-{1}", klient.Value.Nazwa, klient.Key));
            }

            klienci_box.ItemsSource = klienci;

            List<string> soki = new List<string>();
            foreach (var sok in tabela_sokow)
            {
                soki.Add(sok.Key);
            }

            soki_box.ItemsSource = soki;
        }

        private void DodajZamowienie(object sender, RoutedEventArgs e)
        {
            var nip_klienta = klienci_box.SelectedItem.ToString().Split('-')[1];
            var id_klienta = tabela_klientow[nip_klienta].ID;
            var id_soku = tabela_sokow[soki_box.SelectedItem.ToString()].ID;
            var cena_za_sok = tabela_sokow[soki_box.SelectedItem.ToString()].Cena;
            var ilosc = Convert.ToInt32(ilosc_sokow.Text);

            naleznosc.Content = Convert.ToString(ilosc * cena_za_sok);

            var query = String.Format("INSERT INTO Zamowienia VALUES (NULL, {0}, {1}, {2}, {3})", id_klienta, id_soku, ilosc, cena_za_sok);
            using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = query;
                command.ExecuteScalar();
                informacje.Content = "Zamowienie zostało dodane do bazy danych!";
            }

            parent_window.PobierzZamowienia();
            parent_window.AktualizujTabeleZamowien();
        }
    }
}
