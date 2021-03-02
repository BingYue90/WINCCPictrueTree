using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace WINCCPictrueTree.Resources
{
    /// <summary>
    /// 布尔转显示
    /// </summary>
    public class Bool_VisibilityConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// 布尔取反
    /// </summary>
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("必须为bool值");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    //FF818F93
    public class Bool_ClickConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
             System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("必须为bool值");
            if((bool)value)
            {
            //    Color color = Color.FromRgb(0xFF, 0x81. 0x8F, 93);
            //    return 
            }
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
