using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LsysParser.Data.Model
{
    public enum CounterType
    {
        TotalCheckedProducts,
        SavedProducts
    }

    public class Project : INotifyPropertyChanged
    {
        public ProjectSettings Settings { get; set; }
        public string Name { get; set; }

        public Project()
        {
            Settings = new ProjectSettings();
        }

        #region counter interface
        int totalCheckedProducts_Counter, savedProducts_Counter, errors_Counter;
        public int TotalCheckedProducts_Counter
        {
            get
            {
                return totalCheckedProducts_Counter;
            }

            private set
            {
                totalCheckedProducts_Counter = value;
            }
        }
        public int SavedProducts_Counter
        {
            get
            {
                return savedProducts_Counter;
            }

            private set
            {
                savedProducts_Counter = value;
            }
        }
        public int Errors_Counter
        {
            get
            {
                return errors_Counter;
            }

            private set
            {
                errors_Counter = value;
            }
        }

        public void Counter(CounterType counterType)
        {
            switch (counterType)
            {
                case CounterType.TotalCheckedProducts:
                    Interlocked.Increment(ref totalCheckedProducts_Counter);
                    PropChanged("TotalCheckedProducts_Counter");
                    break;
                case CounterType.SavedProducts:
                    Interlocked.Increment(ref savedProducts_Counter);
                    PropChanged("SavedProducts_Counter");
                    break;
            }
        }

        void CountError()
        {
            Interlocked.Increment(ref errors_Counter);
            PropChanged("Errors_counter");
        }
        #endregion

        #region status interface
        string status;
        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                status = value;
                PropChanged("Status");
            }
        }

        public delegate void NewProductHandler(Product product);
        public event NewProductHandler NewProductEvent;

        public void NewProduct(Product product)
        {
            NewProductEvent(product);
        }

        public delegate void LogEventHandler(string message);
        public event LogEventHandler LogEvent;

        public void Info(string message)
        {
            LogEvent($"{DateTime.Now}|INFO|{message}");
        }

        public void Error(string message, Exception ex = null)
        {
            CountError();

            var innerMessages = new List<string>();
            var currEx = ex;
            while (currEx.InnerException != null)
            {
                innerMessages.Add(currEx.Message);
                currEx = currEx.InnerException;
            }
            if (currEx.InnerException == null)
                innerMessages.Add(currEx.Message);

            innerMessages.Add(message);
            innerMessages.Reverse();

            LogEvent($"{DateTime.Now}|ERROR|{string.Join(" --> ", innerMessages)}");
        }

        public void UnknownError(Exception ex)
        {
            Error("Неизвестная ошибка", ex);
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        void PropChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
