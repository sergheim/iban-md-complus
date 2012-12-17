using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using bzIBANcomplus;

namespace wcfIBANservice
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    public class Service1 : IService1
    {
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public string getIBAN(string AccNum)
        {
            bzIBAN bz = new bzIBAN();
            return bz.getIBAN(AccNum);            
        }

        public bool validateIBAN(string iban)
        {
            bzIBAN bz = new bzIBAN();
            return bz.verifyIBAN(iban);
        }
    }
}
