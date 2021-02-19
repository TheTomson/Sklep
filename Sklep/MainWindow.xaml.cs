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
        private Dictionary<int, ZamowienieInformacje> tabela_zamowien;
        public MainWindow()
        {
            PobierzSrodki();
            tabela_sokow = new Dictionary<string, SokInformacje>();
            tabela_klientow = new Dictionary<string, KlientInformacje>();
            tabela_zamowien = new Dictionary<int, ZamowienieInformacje>();
            InitializeComponent();
            kasa_l.Content = kasa;
            PobierzSklep();
            PobierzKlientow();
            PobierzZamowienia();
            AktualizujTabeleSokow();
            AktualizujTabeleZamowien();
        }

        private void PobierzSrodki()
        {
            using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT Ilosc FROM Srodki";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        kasa = reader.GetInt32(0);
                    }
                }
            }
        }

        public void PobierzSklep()
        {
            using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT IdTowaru, NazwaTowaru, Ilosc, Cena FROM Sklep";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var nazwa = reader.GetString(1);
                        var ilosc = reader.GetInt32(2);
                        var cena = reader.GetInt32(3);
                        tabela_sokow.Add(nazwa, new SokInformacje(id, ilosc, cena));
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
                command.CommandText = @"SELECT Id, Nazwa, Adres, NumerTelefonu, NIP FROM Klienci";
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var nazwa = reader.GetString(1);
                        var adres = reader.GetString(2);
                        var numer_tel = reader.GetString(3);
                        var nip = reader.GetString(4);
                        tabela_klientow.Add(nip, new KlientInformacje(id, nazwa, adres, numer_tel));
                    }
                }
            }
        }

        public void PobierzZamowienia()
        {
            tabela_zamowien.Clear();
            using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"SELECT Zamowienia.ID, Klienci.Nazwa, Klienci.NIP, Sklep.NazwaTowaru, Zamowienia.Ilosc, Zamowienia.Koszt FROM Zamowienia INNER JOIN Klienci ON Zamowienia.IdKlienta=Klienci.ID INNER JOIN Sklep ON Zamowienia.IdTowaru=Sklep.IdTowaru";

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var id = reader.GetInt32(0);
                        var nazwa = reader.GetString(1);
                        var nip = reader.GetString(2);
                        var sok = reader.GetString(3);
                        var ilosc = reader.GetInt32(4);
                        var koszt = reader.GetInt32(5);
                        tabela_zamowien.Add(id, new ZamowienieInformacje(id, nazwa, nip, sok, ilosc, koszt));
                    }
                }
            }
        }

        public void AktualizujTabeleSokow()
        {
            List<WpisWTabeliSokow> tabela = new List<WpisWTabeliSokow>();
            foreach (var sok in tabela_sokow)
            {
                tabela.Add(new WpisWTabeliSokow { nazwa_soku = sok.Key, ilosc = sok.Value.Ilosc, cena = sok.Value.Cena });
            }

            sklep.ItemsSource = tabela;
        }

        public void AktualizujTabeleZamowien()
        {
            List<ZamowienieInformacje> zamowienia = new List<ZamowienieInformacje>();
            foreach(var zamowienie in tabela_zamowien)
            {
                zamowienia.Add(zamowienie.Value);
            }

            lista_zamowien.ItemsSource = zamowienia;
        }

        public void AktualizujSrodki()
        {

            using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = String.Format("UPDATE Srodki SET Ilosc={0}", kasa);
                command.ExecuteScalar();
            }
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

        private void DodajZamowienie(object sender, RoutedEventArgs e)
        {
            var dodaj_zamowienie = new Zamowienia(this, ref tabela_sokow, ref tabela_klientow);
            dodaj_zamowienie.Show();
        }

        private void ZamknijOnko(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var sok in tabela_sokow)
            {
                using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
                {
                    connection.Open();
                    var command = connection.CreateCommand();
                    command.CommandText = String.Format("UPDATE Sklep SET Ilosc={0} WHERE IdTowaru={1}", sok.Value.Ilosc, sok.Value.ID);
                    command.ExecuteScalar();
                }
            }
            AktualizujSrodki();
        }

        private void RealizujZamówienie(object sender, RoutedEventArgs e)
        {
            try
            {
                int numer_zamowienia = Convert.ToInt32(id_zamowienia.Text);

                if (tabela_zamowien.ContainsKey(numer_zamowienia))
                {
                    var zamowienie = tabela_zamowien[numer_zamowienia];
                    var sok = zamowienie.NazwaSoku;
                    var wymagana_ilosc_soku = zamowienie.Ilosc;

                    if (tabela_sokow[sok].Ilosc >= wymagana_ilosc_soku)
                    {
                        tabela_sokow[sok].Ilosc -= wymagana_ilosc_soku;
                        kasa += wymagana_ilosc_soku * tabela_sokow[sok].Cena;
                        using (var connection = new SqliteConnection("Data Source=SklepDB.db"))
                        {
                            connection.Open();
                            var command = connection.CreateCommand();
                            command.CommandText = String.Format("DELETE FROM Zamowienia WHERE ID={0}", numer_zamowienia);
                            command.ExecuteScalar();
                        }

                        kasa_l.Content = kasa;
                        tabela_zamowien.Remove(numer_zamowienia);
                        AktualizujTabeleZamowien();
                    } 
                    else
                    {
                        info.Content = String.Format("Za mało {0}", sok);
                    }
                }
                else
                {
                    info.Content = "Nie ma zamówienia o podanym numerze!";
                }
            } 
            catch (Exception ex) 
            {
                info.Content = "Błąd! Należy wprowadzić liczbę!";
            }
            
        }
    }
    public class SokInformacje
    {
        public int ID { get; set; }
        public int Ilosc { get; set; }
        public int Cena { get; set; }

        public SokInformacje(int id, int ilosc, int cena)
        {
            ID = id;
            Ilosc = ilosc;
            Cena = cena;
        }
    }

    struct WpisWTabeliSokow
    {
        public string nazwa_soku { get; set; }
        public int ilosc { get; set; }
        public int cena { get; set; }
    }

    public class KlientInformacje
    {
        public int ID { get; set; }
        public string Nazwa { get; set; }
        public string Adres { get; set; }
        public string NumerTelefonu { get; set; }
       
        public KlientInformacje(int id, string nazwa, string adres, string telefon)
        {
            ID = id;
            Nazwa = nazwa;
            Adres = adres;
            NumerTelefonu = telefon;
        }
    }

    public class ZamowienieInformacje
    {
        public int NumerZamowienia { get; set; }
        public string NazwaKlienta { get; set; }
        public string NipKlienta { get; set; }
        public string NazwaSoku { get; set; }
        public int Ilosc { get; set; }
        public int Cena { get; set; }

        public ZamowienieInformacje(int id_zamowienia, string nazwa, string nip, string sok, int ilosc, int cena)
        {
            NumerZamowienia = id_zamowienia;
            NazwaKlienta = nazwa;
            NipKlienta = nip;
            NazwaSoku = sok;
            Ilosc = ilosc;
            Cena = cena;
        }
    }
}
