using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EuroWIdjet
{
    class ApplicationViewModel : INotifyPropertyChanged
    {
        public Dictionary<string, double> currency;

        public int waitingPeriod;

        public Dictionary<string, double> Currency
        {
            get { return currency; }
            set
            {
                currency = value;
                OnPropertyChanged("Currency");
            }
        }

        public int WaitingPeriod
        {
            get { return waitingPeriod; }
            set
            {
                if (value > 0)
                {
                    waitingPeriod = value;
                    OnPropertyChanged("WaitingPeriod");
                }
            }
        }

        public ApplicationViewModel()
        {
            Currency = new Dictionary<string, double>();
            WaitingPeriod = 1000;

            double currency = GetCurrency();

            if (currency > 0)
                Currency.Add(DateTime.Now.ToLongTimeString(), currency);

            SycleAsync();
        }

        public async void SycleAsync()
        {
            await Task.Run(() => Sycle());
        }

        private void Sycle()
        {
            while (true)
            {
                int i = 0;
                var tempDict = new Dictionary<string, double>();
                while (i < 5)
                {
                    double currensy = GetCurrency();

                    if (currensy > 0)
                        tempDict.Add(DateTime.Now.ToLongTimeString(), currensy);

                    Thread.Sleep(WaitingPeriod);
                    i++;
                }

                Currency.Clear();
                Currency = tempDict;
            }
        }

        public double GetCurrency()
        {
            return EuroWIdjet.Currency.GetCourse();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
