using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace ShotWithPhone
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class TestPhonePage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        private double _MarginTop;
        private double _MarginBottom;
        private double _MarginLeft;
        private double _MarginRight;
        private int _RecHeight;
        private int _RecWidth;
        private BitmapImage _PhoneBitmap;
        private BitmapImage _GBitmap;
        private string _filepath;
        private ObservableCollection<string> _ColorList;

        public double MarginTop { get { return _MarginTop; } set { _MarginTop = value; NotifyPropertyChanged(); NotifyPropertyChanged("RecMargin"); } }
        public double MarginBottom { get { return _MarginBottom; } set { _MarginBottom = value; NotifyPropertyChanged(); NotifyPropertyChanged("RecMargin"); } }
        public double MarginLeft { get { return _MarginLeft; } set { _MarginLeft = value; NotifyPropertyChanged(); NotifyPropertyChanged("RecMargin"); } }
        public double MarginRight { get { return _MarginRight; } set { _MarginRight = value; NotifyPropertyChanged(); NotifyPropertyChanged("RecMargin"); } }
        public int RecHeight { get { return _RecHeight; } set { _RecHeight = value; NotifyPropertyChanged(); } }
        public int RecWidth { get { return _RecWidth; } set { _RecWidth = value; NotifyPropertyChanged(); } }
        public BitmapImage PhoneBitmap
        {
            get
            {
                if (_PhoneBitmap == null) _PhoneBitmap = new BitmapImage();
                return _PhoneBitmap;
            }
            set { _PhoneBitmap = value; NotifyPropertyChanged(); }
        }
        public BitmapImage GBitmap
        {
            get
            {
                if (_GBitmap == null) _GBitmap = new BitmapImage();
                return _GBitmap;
            }
            set { _GBitmap = value; NotifyPropertyChanged(); }
        }
        public Thickness RecMargin
        {
            get
            {
                return new Thickness(MarginLeft, MarginTop, MarginRight, MarginBottom);
            }
            set
            {
                MarginLeft = value.Left;
                MarginTop = value.Top;
                MarginRight = value.Right;
                MarginBottom = value.Bottom;
                NotifyPropertyChanged("MarginLeft");
                NotifyPropertyChanged("MarginTop");
                NotifyPropertyChanged("MarginRight");
                NotifyPropertyChanged("MarginBottom");
            }
        }
        public ObservableCollection<string> ColorList
        {
            get
            {
                if (_ColorList == null)
                    _ColorList = new ObservableCollection<string>();
                return _ColorList;
            }
            set
            {
                _ColorList = value;
                NotifyPropertyChanged();
            }
        }


        public TestPhonePage()
        {
            this.InitializeComponent();
            RecHeight = 1280;
            RecWidth = 720;
            ColorListAdd();
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += Test_BackRequested;
        }

        private void ColorListAdd()
        {
            ColorList.Add("Black");
            ColorList.Add("White");
            ColorList.Add("Gray");
            ColorList.Add("Blue");
            ColorList.Add("Pink");
            ColorList.Add("Gold");
            ColorList.Add("Green");
            ColorList.Add("Yellow");
            ColorList.Add("BlueGreen");
            ColorList.Add("BlackBrown");
            ColorList.Add("Brown");
        }

        private void Test_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack) rootFrame.GoBack();
        }

        private async void Viewbox_Drop(object sender, DragEventArgs e)
        {
            var defer = e.GetDeferral();
            try
            {
                DataPackageView dataView = e.DataView;
                // 拖放类型为文件存储。
                if (dataView.Contains(StandardDataFormats.StorageItems))
                {
                    var files = await dataView.GetStorageItemsAsync();
                    StorageFile file = files.OfType<StorageFile>().First();
                    if (file.FileType == ".png" || file.FileType == ".jpg")
                    {
                        BitmapImage bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(await file.OpenAsync(FileAccessMode.Read));
                        PhoneBitmap = bitmap;
                        _filepath = file.Path;
                    }
                }

                var PhoneInfo = _filepath.Split('_');
                if (PhoneInfo.Count() >= 5)
                {
                    PhoneOEM.Text = UpperFirst(PhoneInfo[2]);
                    if (PhoneOEM.Text == "Samsung") PhoneName.Text = "Galaxy " + UpperFirst(PhoneInfo[3]);
                    PhoneName.Text = UpperFirst(PhoneInfo[3]);
                    string ColorStr = "";
                    int index;
                    if (PhoneInfo[5].IndexOf('-') > 0)
                    {
                        index = PhoneInfo[5].IndexOf('-');
                        ColorStr = UpperFirst(PhoneInfo[5].Substring(0, index));
                    }
                    else if (PhoneInfo[6].IndexOf('-') > 0)
                    {
                        index = PhoneInfo[6].IndexOf('-');
                        ColorStr = UpperFirst(PhoneInfo[6].Substring(0, index));
                        PhoneName.Text += (" " + UpperFirst(PhoneInfo[4]));
                    }

                    if (ColorToStr(ColorStr) != null) Color.SelectedValue = ColorStr;
                }

            }
            finally
            {
                defer.Complete();
            }
        }

        private void Viewbox_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "拖放";
            e.DragUIOverride.IsContentVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.IsContentVisible = true;
            e.Handled = true;
        }

        private void Up_Click(object sender, RoutedEventArgs e)
        {
            if (MarginTop > 0)
                MarginTop--;
            else MarginBottom++;
        }

        private void Down_Click(object sender, RoutedEventArgs e)
        {
            if (MarginBottom > 0)
                MarginBottom--;
            else MarginTop++;
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            if (MarginLeft > 0)
                MarginLeft--;
            else MarginRight++;
        }

        private void Right_Click(object sender, RoutedEventArgs e)
        {
            if (MarginRight > 0)
                MarginRight--;
            else MarginLeft++;
        }

        private void WidthPlus_Click(object sender, RoutedEventArgs e)
        {
            RecWidth++;
        }

        private void HeightPlus_Click(object sender, RoutedEventArgs e)
        {
            RecHeight++;
        }

        private void WidthAntPlus_Click(object sender, RoutedEventArgs e)
        {
            if (RecWidth > 0) RecWidth--;
            else RecWidth = 0;
        }

        private void HeightAntPlus_Click(object sender, RoutedEventArgs e)
        {
            if (RecHeight > 0) RecHeight--;
            else RecHeight = 0;
        }

        private void TextBlock_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (RecHeight == 1280 && RecWidth == 720)
            {
                RecHeight = 1920;
                RecWidth = 1080;
            }
            else
            {
                RecHeight = 1280;
                RecWidth = 720;
            }
        }

        private void Viewbox_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Input.PointerPoint ptrPt = e.GetCurrentPoint(sender as UIElement);
            var _trans = (sender as Viewbox).RenderTransform as ScaleTransform;
            if (_trans.ScaleX == 1 && _trans.ScaleY == 1)
            {
                _trans.ScaleX = 4;
                _trans.ScaleY = 4;
                _trans.CenterX = ptrPt.Position.X;
                _trans.CenterY = ptrPt.Position.Y;
            }
            else
            {
                _trans.ScaleX = 1;
                _trans.ScaleY = 1;
            }
        }

        private async void StackPanel_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            //string JsonModel = "      {" +
            //              "\"PhoneOEM\": \"{0}\"," +
            //              "\"PhoneName\": \"{1}\"," +
            //              "\"Color\": \"{2}\"," +
            //              "\"ColorStr\": \"{3}\"," +
            //              "\"PhoneGlare\": \"{4}\"," +
            //              "\"PhoneScreen\": \"{5}\"," +
            //              "\"MarginLeft\": \"{6}\"," +
            //              "\"MarginTop\": \"{7}\"," +
            //              "\"MarginRight\": \"{8}\"," +
            //              "\"MarginBottom\": \"{9}\"," +
            //              "\"ScreenWidth\": \"{10}\"," +
            //              "\"ScreenHeight\": \"{11}\"" +
            //              "},";
            string[] args = new string[12];
            args[0] = "\n      {\n        \"PhoneOEM\":\"" + PhoneOEM.Text + "\",\n";
            args[1] = "        \"PhoneName\":\"" + PhoneName.Text + "\",\n";
            args[2] = "        \"Color\": \"" + Color.SelectedItem as string + "\",\n";
            args[3] = "        \"ColorStr\": \"" + ColorToStr(Color.SelectedItem as string) + "\",\n";
            args[4] = "        \"PhoneGlare\": \"" + _filepath.Replace("C:\\Users\\blue-\\Documents\\Visual Studio 2015\\Projects\\ShotWithPhone\\ShotWithPhone\\PhoneImageSources\\", "/PhoneImageSources/").Replace("screen", "glare") + "\",\n";
            args[5] = "        \"PhoneScreen\": \"" + _filepath.Replace("C:\\Users\\blue-\\Documents\\Visual Studio 2015\\Projects\\ShotWithPhone\\ShotWithPhone\\PhoneImageSources\\", "/PhoneImageSources/") + "\",\n";
            args[6] = "        \"MarginLeft\": \"" + MarginLeft.ToString() + "\",\n";
            args[7] = "        \"MarginTop\": \"" + MarginTop.ToString() + "\",\n";
            args[8] = "        \"MarginRight\": \"" + MarginRight.ToString() + "\",\n";
            args[9] = "        \"MarginBottom\": \"" + MarginBottom.ToString() + "\",\n";
            args[10] = "        \"ScreenWidth\": \"" + RecWidth.ToString() + "\",\n";
            args[11] = "        \"ScreenHeight\": \"" + RecHeight.ToString() + "\"\n      },";
            string Json = "";
            foreach (string item in args)
            {
                Json += item;
            }
            DataPackage data = new DataPackage();
            data.SetText(Json);
            Clipboard.SetContent(data);
            MessageDialog MD = new MessageDialog("CopyOK!");
            MD.Options = MessageDialogOptions.AcceptUserInputAfterDelay;
            await MD.ShowAsync();
        }

        private string ColorToStr(string color)
        {
            switch (color)
            {
                case "Black":
                    return "黑色";
                case "White":
                    return "白色";
                case "Gray":
                    return "灰色";
                case "Blue":
                    return "蓝色";
                case "Pink":
                    return "粉色";
                case "Gold":
                    return "金色";
                case "Green":
                    return "绿色";
                case "Yellow":
                    return "黄色";
                case "BlueGreen":
                    return "蓝绿色";
                case "BlackBrown":
                    return "木纹棕色";
                case "Brown":
                    return "棕色";
                case "Purple":
                    return "紫色";
                case "Orange":
                    return "橙色";
            }
            return "";
        }

        public static string UpperFirst(string s)
        {
            return System.Text.RegularExpressions.Regex.Replace(s, @"\b[a-z]\w+", delegate (System.Text.RegularExpressions.Match match)
            {
                string v = match.ToString();
                return char.ToUpper(v[0]) + v.Substring(1);
            });
        }

    }
}
