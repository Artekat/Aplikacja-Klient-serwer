using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ProductInterfaces
{
    [ServiceContract]
    public interface IWCFProductService
    {
     
        [OperationContract]
        bool creating_account(string login,string haslo,string imie, string nazwisko, string adres);

        [OperationContract]
        string depositing(string login, string haslo, int quantity); // wpłata

        [OperationContract]
        string payment(string login, string haslo, int quantity); // wypłata

        [OperationContract]
        string transaction_history(string login, string haslo);

        [OperationContract]
        string podglad_danych(string login, string haslo);

        [OperationContract]
        bool login(string login, string haslo);

    }
}
