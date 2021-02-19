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
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Linq;

namespace Sklep
{
    /// <summary>
    /// Logika interakcji dla klasy Klienci.xaml
    /// </summary>                                   
    public partial class Klienci : Window
    {
        public event EventHandler AktualizujTabeleKlientow;

        private Dictionary<string, KlientInformacje> tabela_klientow;
        public Klienci(ref Dictionary<string, KlientInformacje> tk)
        {
            tabela_klientow = tk;
            InitializeComponent();
            Loaded += new RoutedEventHandler(DodajKlienci);
        }
        public void DodajKlienci(object sender, RoutedEventArgs e)
        {
            bool dodaj_klienta = true;
            var nazwa_klienta = nazwa_klient.Text;
            var adres_klienta = adres_klient.Text;
            var telefon_klienta = telefon_klient.Text;
            var nip_klienta = nip_kient.Text;

            if (nazwa_klienta.Length > 255 && nazwa_klienta.Length == 0)
            {
                udane_klient.Content = "Nazwa nie może być dłuższa niż 255 znaków oraz nie może być pusty!";
                dodaj_klienta = false;
                return;
            }
            else if (adres_klienta.Length > 255 && adres_klienta.Length == 0)
            {
                udane_klient.Content = "Adres nie może być dłuższy niż 255 znaków oraz nie może być pusty!";
                dodaj_klienta = false;
                return;
            }
            else if (telefon_klienta.Length != 9)
            {
                udane_klient.Content = "Numer telefonu musi posiadać 9 cyfr!";
                dodaj_klienta = false;
                return;
            }
            else if (nip_klienta.Length != 11)
            {
                udane_klient.Content = "NIP musi posiadać 11 cyfr!";
                dodaj_klienta = false;
                return;
            }

            if (tabela_klientow.ContainsKey(nip_klienta))
            {
                udane_klient.Content = "Podany numer NIP jest już w bazie danych!";
                dodaj_klienta = false;
            }

            if (dodaj_klienta)
            {
                var query = String.Format("INSERT INTO Klienci VALUES (NULL,'{0}','{1}','{2}','{3}')", nazwa_klient.Text, adres_klient.Text, telefon_klient.Text, nip_kient.Text);
                using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = query;
                    command.ExecuteScalar();
                    udane_klient.Content = "Udało się! Dodałeś nowego klienta do bazy danych!";
                }
            }

            if (AktualizujTabeleKlientow != null)
            {
                AktualizujTabeleKlientow(this, EventArgs.Empty);
            }
        }
    }
}
