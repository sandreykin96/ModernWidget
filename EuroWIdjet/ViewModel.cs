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
        private Dictionary<string, double> _currency;
        public int PointsNumber { get; set; }

        public int waitingPeriod;

        public Dictionary<string, double> Currency
        {
            get { return _currency; }
            set
            {
                _currency = value;
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
            PointsNumber = 5;
            SycleAsync();
        }

        public async Task SycleAsync()
        {
            await Task.Run(() => Sycle());
        }

        private void Sycle()
        {
            var dateAndNumsQueue = new Queue<DateAndNum>();

            while (true)
            {
                var tempDict = new Dictionary<string, double>();
                double currensy = GetCurrency();

                if (currensy > 0)
                    dateAndNumsQueue.Enqueue(new DateAndNum(DateTime.Now.ToLongTimeString(), currensy));

                if (dateAndNumsQueue.Count > PointsNumber)
                    dateAndNumsQueue.Dequeue();

                foreach (var item in dateAndNumsQueue)
                {
                    tempDict.Add(item.Date, item.Number);
                }

                Currency = tempDict;
                Thread.Sleep(WaitingPeriod);
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

    class DateAndNum
    {
        public string Date { get; set; }
        public double Number { get; set; }

        public DateAndNum(string date, double number)
        {
            Date = date;
            Number = number;
        }
    }
}
