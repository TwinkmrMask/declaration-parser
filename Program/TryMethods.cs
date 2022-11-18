using System;
using System.Collections.Generic;
using System.Windows;

namespace TryMethods
{
    public class TryMethods : FileSearcher.FileSearcher
    {
        public static List<string> TryGetFiles(string directory, string format)
        {
            try
            {
                return GetFiles(directory, format);
            }
            catch (Exception ex)
            {
                ExceptionMethods.CopyException(ex.HResult.ToString(),
                    "Неизвестная ошибка");
                return default;
            }
        }

        public static string TryGetFilename()
        {
            try { return GetFilename(); }
            catch (Exception ex)
            {
                ExceptionMethods.CopyException(ex.HResult.ToString(),
                    "Ошибка получения пути");
                return default;
            }
        }
    }

    public static class ExceptionMethods
    {
        private const string Text = "Ошибка - {0}\nНажмите OK чтобы скопировать код ошибки, или нажмите Отмена чтобы переписать самостоятельно";

        public static void CopyException(string exception, string error)
        {
            if (MessageBox.Show(string.Join(Text, exception),
                    error, MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK)
                Clipboard.SetText(exception);
        }

        public static bool ActionException(string exception, string error) =>
            MessageBox.Show(string.Join(Text, exception),
                error, MessageBoxButton.OKCancel, MessageBoxImage.Error) == MessageBoxResult.OK;
    }
}
