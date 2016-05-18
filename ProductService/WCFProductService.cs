using ProductInterfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;

namespace ProductService
{
    public class WCFProductService : IWCFProductService
    {
       
        public bool login(string login, string haslo)
        {
            var db = new LokalnabazaEntities();
            var listOfRoleId = db.Pracownicies.Select(r => r.login);
            var roles = db.Pracownicies.Where(r => listOfRoleId.Contains(r.login));
            foreach (Pracownicy prac in roles)
            {
                if (prac.login.Equals(login))
                {
                    if (prac.haslo.Equals(haslo))
                    {
                        Console.WriteLine("\nUdana próba logowania Login " + login);
                        return true;
                    }
                }
            }
            Console.WriteLine("\nNie udana próba logowania Login " + login);
            return false;
        }
        public bool creating_account(string login, string haslo, string imie, string nazwisko, string adres)
        {
            try
            {
                Random rand = new Random();
                var db = new LokalnabazaEntities();
                var pracownik2 = new Pracownicy
                {

                    imie = imie,
                    nazwisko = nazwisko,
                    adres = adres,
                    nr_rach = Convert.ToString(rand.Next(999999, 9999999)),
                    login = login,
                    haslo = haslo,
                    stan_konta = 0
                };
                db.Pracownicies.Add(pracownik2);
                db.SaveChanges();
                Console.WriteLine("\nUdane stworzenie konta login " + login);
            }
            catch
            {
                Console.WriteLine("\nNie Udane stworzenie konta login " + login);
                return false;
            }
            return true;
        }
        public bool tworzenieHistori(Pracownicy pracownik, int kwota, int typ)
        {
            try
            {
                var dba = new LokalnabazaEntities();
                var tra = new Transakcje
                {

                    id_user = pracownik.Id,
                    kwota = kwota,
                    rodzaj_tran = typ
                };
                dba.Transakcjes.Add(tra);
                dba.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }
        string IWCFProductService.depositing(string login, string haslo, int quantity) 
        {
            var tranzakcja = new Transakcje();
            var db = new LokalnabazaEntities();
            var listOfRoleId = db.Pracownicies.Select(r => r.login);
            var roles = db.Pracownicies.Where(r => listOfRoleId.Contains(r.login));
            decimal kwota = 0;
            var practemp = new Pracownicy();
            try
            {
                foreach (Pracownicy prac in roles)
                {
                   

                    if (prac.login.Equals(login))
                    {
                        if (prac.haslo.Equals(haslo))
                        {
                            prac.stan_konta = prac.stan_konta + quantity;
                            practemp = prac;
                            kwota = prac.stan_konta;
                            db.Pracownicies.Attach(prac);
                            db.Entry(prac).State = EntityState.Modified;
                        }
                    }
                  
                }
                db.SaveChanges();
                tworzenieHistori(practemp, quantity, 1);
            }
            catch
            {
                Console.WriteLine("Błąd podczas wpłaty przez użytkownika o loginie: " + login + " w wysokości: " + quantity);
                return "Błąd ";

            }
            Console.WriteLine("Dokonanie wpłaty przez użytkownika o loginie: " + login + " w wysokości: " + quantity);
            return "Wpłata dokonana prawidłowo " + "stan konta: " + kwota;

        }
        public string payment(string login, string haslo, int quantity)
        {
            var db = new LokalnabazaEntities();
            var listOfRoleId = db.Pracownicies.Select(r => r.login);
            var roles = db.Pracownicies.Where(r => listOfRoleId.Contains(r.login));
            var practemp = new Pracownicy();
            decimal kwota = 0;
            try
            {
                foreach (Pracownicy prac in roles)
                {
                    if (prac.login.Equals(login))
                    {
                        if (prac.haslo.Equals(haslo))
                        {
                            kwota = prac.stan_konta-quantity;

                            if (kwota > 0)
                            {
                                db.Pracownicies.Attach(prac);
                                db.Entry(prac).State = EntityState.Modified;
                                practemp = prac;
                            }
                            else
                            {
                                return "Błąd ";
                            }

                        }
                    }
                }
                db.SaveChanges();
                tworzenieHistori(practemp, quantity, 0);
                Console.WriteLine("Dokonanie wypłaty przez uzytkownika o loginie: " + login + " w wysokości: " + quantity);
                return "Wypłaty dokonana prawidłowo " + "stan konta: " + kwota;
            }
            catch
            {
                Console.WriteLine("Błąd podczas wypłaty przez użytkownika o loginie: " + login + " w wysokości: " + quantity);
                return "Błąd ";

            }
        }
        public string transaction_history(string login, string haslo)
        {
            string returnText="";
            var db = new LokalnabazaEntities();
            var listOfRoleId = db.Pracownicies.Select(r => r.login);
            var roles = db.Pracownicies.Where(r => listOfRoleId.Contains(r.login));
            foreach (Pracownicy prac in roles)
            {
                returnText += "Użytkownik: "+prac.login+"\n";
                if (prac.login.Equals(login))
                {
                    if (prac.haslo.Equals(haslo))
                    {
                        var listOfRoleIda = db.Transakcjes.Select(r => r.id_user);
                        var rolesa = db.Transakcjes.Where(r => r.id_user == prac.Id);
                        foreach (Transakcje rola in rolesa)
                        {
                            if(rola.rodzaj_tran==1)
                            {
                                returnText += "Wpłacił ";
                            }else
                            {
                                 returnText += "Wypłacił ";
                            }
                            returnText += rola.kwota+"\n";
                        }

                    }
                }
                Console.WriteLine("Wysłanie historii transakcji dla " + login);
                return returnText;
            }
            Console.WriteLine("Błąd podczas historii transakcji dla " + login);
            return "None";
        }
        public string podglad_danych(string login, string haslo)
        {
            var db = new LokalnabazaEntities();
            var listOfRoleId = db.Pracownicies.Select(r => r.login);
            var roles = db.Pracownicies.Where(r => listOfRoleId.Contains(r.login));
            foreach (Pracownicy prac in roles)
            {
                if (prac.login.Equals(login))
                {
                    if (prac.haslo.Equals(haslo))
                    {
                        Console.WriteLine("Wysłanie podgladu danych dla " + login);
                        return "Login: " + prac.login + "\nImie: " + prac.imie + "\nNazwisko: " + prac.nazwisko + "\nNr. rachunku: " + prac.nr_rach + "\nStan konta: " + prac.stan_konta;
                    }
                }
            }
            Console.WriteLine("Błąd podczas podglądu danych dla " + login);
            return "None";
        }
    }
}
