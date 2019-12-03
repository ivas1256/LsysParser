using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LsysParser
{
    class MessageProvider
    {
        public static void UnknownError(Exception ex)
        {
            MessageBox.Show("Неизвестная ошибка: " + ex?.Message, "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void Error(Exception ex, string message)
        {
            string fullMessage = ex.Message;
            if (ex.Message.Contains("See the inner exception"))
            {
                fullMessage = "";
                var currEx = ex;
                while (currEx.InnerException != null)
                {
                    fullMessage += currEx.Message + " --> ";
                    currEx = currEx.InnerException;
                }
                fullMessage += currEx.Message + " --> ";
            }

            MessageBox.Show($"{message}{Environment.NewLine}Ошибка: {fullMessage}.", "Ошибка",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void Successfully(string message)
        {
            MessageBox.Show(message, "Информация",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void Info(string message)
        {
            MessageBox.Show(message, "Информация",
                MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        public static void IncorrectData(string message)
        {
            MessageBox.Show(message, "Неверные данные",
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public static bool Confirm(string question)
        {
            return MessageBox.Show(question, "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes;
        }
    }
}
