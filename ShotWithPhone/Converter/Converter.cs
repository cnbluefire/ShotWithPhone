using ShotWithPhone.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace ShotWithPhone
{

    public class VisableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var IsOn = (bool)value;
            return IsOn ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var Visable = (Visibility)value;
            return (Visable == Visibility.Visible) ? true : false;
        }
    }

    public class DoubleStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = (string)value;
            return str != "" ? System.Convert.ToDouble(str) : 0.0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var dou = (double)value;
            return dou.ToString();
        }
    }

    public class ColorStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var str = (string)value;
            Color c = new Color();
            switch (str)
            {
                case "Black":
                    c = Colors.Black;
                    break;
                case "White":
                    c = Colors.White;
                    break;
                case "Gray":
                    c = Colors.Gray;
                    break;
                case "Blue":
                    c = Colors.Cyan;
                    break;
                case "Pink":
                    c = Colors.Pink;
                    break;
                case "Gold":
                    c = Colors.Gold;
                    break;
                case "Green":
                    c = Colors.LightGreen;
                    break;
                case "Yellow":
                    c = Color.FromArgb(255, 240, 230, 140);
                    break;
                case "BlueGreen":
                    c = Color.FromArgb(255, 30, 105, 107);
                    break;
                case "BlackBrown":
                    c = Color.FromArgb(255, 19, 18, 15);
                    break;
                case "Brown":
                    c = Color.FromArgb(255, 128, 76, 57);
                    break;
                case "Purple":
                    c = Colors.Purple;
                    break;
                case "Orange":
                    c = Colors.Orange;
                    break;
            }
            Brush brush = new SolidColorBrush(c);
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var c = (SolidColorBrush)value;
            return c.Color.ToString();
        }
    }

    public class ObjectSelectedIndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = value as Windows.UI.Xaml.Controls.GridView;
            if (a != null)
                if (a.Items.Count > 0 && a.SelectedIndex == -1)
                    return 0;
            return -1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value as object;
        }
    }

    public class ObjectIsEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = value as MultipleColorDeviceModel;
            if (a != null && a.Count > 1)
                return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectionIndexVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = (int)value;
            if (a == -1)
                return Visibility.Visible;
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedIndexEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = (int)value;
            if (a > 0)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var a = (bool)value;
            if (!a) return -1;
            else return (int)parameter;
        }
    }

    public class SelectedIndexEnable2Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = (int)value;
            if (a >= 0)
                return true;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var a = (bool)value;
            if (!a) return -1;
            else return (int)parameter;
        }
    }

    public class SelectedIndexPlusHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = (int)value;
            var b = parameter as Windows.UI.Xaml.Controls.ListView;
            if (a <= 0)
                return 0;
            else
                return (b.Items[a + 1] as BitmapListModel).Height;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return 0;
        }
    }

    public class SelectedIndexPlusClipHeightConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var b = parameter as Windows.UI.Xaml.Controls.ListView;
            if (b.SelectedIndex - 1 <= 0)
                return 0;
            else
                return (b.Items[b.SelectedIndex - 1] as BitmapListModel).ClipHeight;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var b = parameter as Windows.UI.Xaml.Controls.ListView;
            if (b.SelectedIndex - 1 <= 0)
                return (int)value;
            else
            {
                (b.Items[b.SelectedIndex - 1] as BitmapListModel).ClipHeight = (int)value;
                return (b.Items[b.SelectedIndex] as BitmapListModel).ClipHeight;
            }
        }
    }

    public class IntDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = (int)value;
            return System.Convert.ToDouble(a);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var dou = (double)value;
            return System.Convert.ToInt32(dou);
        }
    }

    public class ListBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = (ObservableCollection<BitmapListModel>)value;
            if (a.Count > 0) return true;
            else return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException("呸，你没写，辣鸡");
        }
    }

    public class MultipleColorDeviceModelUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var a = (MultipleColorDeviceModel)value;
            if(a != null)
            {
                var uri = new Uri("ms-appx://" + a.First().PhoneScreen);
                if (uri != null) return uri;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return new NotImplementedException("呸，你没写，辣鸡");
        }
    }

}
