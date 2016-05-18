using ProductInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ProductsClient
{
    class Program
    {
       private static bool zalogowano = false;
       private static int exitss = 0;
       private static string login_Global="";
       private static string haslo_Global = "";
        static void Main(string[] args)
        {
            ChannelFactory<IWCFProductService> channelFactory = new ChannelFactory<IWCFProductService>("ProductServiceEndpoint");
            IWCFProductService proxy = channelFactory.CreateChannel();
            do{
            if(!zalogowano)
            {
            Console.WriteLine("========== Klient ==========");
            Console.WriteLine("[1] - Zaloguj się.");
            Console.WriteLine("[2] - Nowe konto.");
            Console.WriteLine("[3] - Wyjście.");
            int caseSwitch = 5;
            try
            {
                caseSwitch = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                caseSwitch = 5;
            }
            switch (caseSwitch)
            {
                case 1:
                    Logowanie(proxy);
                    break;
                case 2:
                    Rejestracja(proxy);
                    break;
                case 3:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Zły wbybor");
                    break;
            }
            }
            else
            {
                Console.WriteLine("========== Zostałeś zalogowany ==========");
                Console.WriteLine("[1] - Podgląd Danych.");
                Console.WriteLine("[2] - Wpłata.");
                Console.WriteLine("[3] - Wypłata.");
                Console.WriteLine("[4] - Lista transakcji.");
                Console.WriteLine("[5] - Wylogowanie.");

                int caseSwitch2 = 6;
                try
                {
                     caseSwitch2 = Convert.ToInt32(Console.ReadLine());
                }
                catch
                {
                     caseSwitch2 = 6;
                }

                switch (caseSwitch2)
                {
                    case 1:
                        Podglad_danych(proxy);
                        break;
                    case 2:
                        Wplata(proxy);
                        break;
                    case 3:
                        Wypłata(proxy);
                        break;
                    case 4:
                        Transaction(proxy);
                        break;
                    case 5:
                        zalogowano = false;
                        Console.WriteLine("Wylogowano");
                        break;
                    default:
                        Console.WriteLine("Zły wybór");
                        break;
                }
            }

            } while (exitss != 10);
        }
        public static void Wypłata(IWCFProductService proxy)
        {
            Console.WriteLine("========== Wpłata ==========");
            Console.WriteLine("Podaj kwotę do wypłaty: ");
            int kwota=0;
            try
            {
                 kwota = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                kwota = 0;
            }
            string a = proxy.payment(login_Global, haslo_Global, kwota);
            if (!a.Equals("None"))
            {
                Console.WriteLine("Dokonano wypłaty, twoje konto zawiera " + a + " zł");
            }

            Console.WriteLine("Podaj kwotę do wypłaty: ");
        }
        public static void Wplata(IWCFProductService proxy)
        {
            Console.WriteLine("========== Wypłata ==========");
            Console.WriteLine("Podaj kwotę do wpłaty: ");
            int kwota = 0;
            try
            {
                kwota = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                kwota = 0;
            }
            string a = proxy.depositing(login_Global, haslo_Global, Convert.ToInt32(kwota));
            if(!a.Equals("None")){
                Console.WriteLine("Dokonano wpłaty, twoje konto zawiera "+a+" zł");
            }
            else
            {
                Console.WriteLine("========== Błąd ==========");
            }
        }
        public static void Podglad_danych(IWCFProductService proxy)
        {
          Console.WriteLine("========== Podgląd danych ==========");
          string a = proxy.podglad_danych(login_Global, haslo_Global);
          if(!a.Equals("None")){
              Console.WriteLine(a);
          }
          else
          {
              Console.WriteLine("========== Błąd ==========");
          }

        }
        public static void Logowanie(IWCFProductService proxy)
        {
            Console.WriteLine("Podaj login: ");
            string login = Console.ReadLine();
            Console.WriteLine("Podaj hasło: ");

            string haslo = Console.ReadLine();
            byte[] encodedPassword2 = new UTF8Encoding().GetBytes(haslo);
            byte[] hash2 = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword2);

            string encoded2 = BitConverter.ToString(hash2);
            if (proxy.login(login, encoded2))
            {
                haslo_Global = encoded2;
                login_Global = login;
                zalogowano = true;
                Console.WriteLine("Zostałeś pomyślnie zalogowany.");
            }
            else
            {
                zalogowano = false;
                Console.WriteLine("Nie zostałeś zalogowany.");
            }
        }
        public static void Rejestracja(IWCFProductService proxy)
        {
                    Console.WriteLine("Podaj login: ");
                    string loginNew = Console.ReadLine();
                    Console.WriteLine("Podaj hasło: ");
                    string hasloNew = Console.ReadLine();
                    byte[] encodedPassword = new UTF8Encoding().GetBytes(hasloNew);
                    byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedPassword);
                    string encoded = BitConverter.ToString(hash);
                    Console.WriteLine("Podaj imię: ");
                    string imieNew = Console.ReadLine();
                    Console.WriteLine("Podaj nazwisko: ");
                    string nazwiskoNew = Console.ReadLine();
                    Console.WriteLine("Podaj adres: ");
                    string adresNew = Console.ReadLine();
                    if (proxy.creating_account(loginNew, encoded, imieNew, nazwiskoNew, adresNew))
                    {
                        Console.WriteLine("Zarejestrowałeś swoje konto.");
                    }
                    else
                    {
                        Console.WriteLine("========== Błąd ==========");
                    }
        }
        public static void Transaction(IWCFProductService proxy)
        {
            string a = proxy.transaction_history(login_Global, haslo_Global);
            if(!a.Equals("None"))
            {
                Console.WriteLine(proxy.transaction_history(login_Global, haslo_Global));
            }else
            {
                Console.WriteLine("========== Błąd ==========");
            }
        }
    }
}
