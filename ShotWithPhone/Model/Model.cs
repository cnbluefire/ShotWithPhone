using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Data.Xml.Dom;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace ShotWithPhone.Model
{
    public class DeviceOEMModel : ObservableCollection<MultipleColorDeviceModel>, INotifyPropertyChanged
    {
        protected override event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private string _OEM;
        private ObservableCollection<MultipleColorDeviceModel> _DevicesList;
        public string OEM
        {
            get
            {
                return _OEM;
            }
            set
            {
                _OEM = value;
                NotifyPropertyChanged();
            }
        }
        public ObservableCollection<MultipleColorDeviceModel> DevicesList
        {
            get
            {
                if (_DevicesList == null)
                    _DevicesList = new ObservableCollection<MultipleColorDeviceModel>();
                return _DevicesList;
            }
            set
            {
                _DevicesList = value;
            }
        }

    }

    public class MultipleColorDeviceModel : ObservableCollection<DeviceModel>, INotifyPropertyChanged
    {
        protected override event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private string _PhoneName;
        private string _OEM;

        public string PhoneName
        {
            get
            {
                return _PhoneName;
            }
            set
            {
                _PhoneName = value;
                NotifyPropertyChanged();
            }
        }

        public string OEM
        {
            get
            {
                return _OEM;
            }
            set
            {
                _OEM = value;
                NotifyPropertyChanged();
            }
        }

    }


    public class DeviceModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private string _PhoneGlare = "";
        private string _PhoneScreen = "";
        private string _ScreenWidth = "";
        private string _ScreenHeight = "";
        public Thickness _PhoneMargin;


        public string PhoneOEM = "";
        public string PhoneName = "";
        public string Color = "";
        public string ColorStr = "";
        public string ScreenWidth
        {
            get
            {
                return _ScreenWidth;
            }
            set
            {
                _ScreenWidth = value;
                NotifyPropertyChanged();
            }
        }
        public string ScreenHeight
        {
            get
            {
                return _ScreenHeight;
            }
            set
            {
                _ScreenHeight = value;
                NotifyPropertyChanged();
            }
        }
        public string PhoneGlare
        {
            get
            {
                return _PhoneGlare;
            }
            set
            {
                _PhoneGlare = value;
                NotifyPropertyChanged();
            }
        }
        public string PhoneScreen
        {
            get
            {
                return _PhoneScreen;
            }
            set
            {
                _PhoneScreen = value;
                NotifyPropertyChanged();
            }
        }
        public string MarginLeft
        {
            get
            {
                return PhoneMargin.Left.ToString();
            }
            set
            {
                _PhoneMargin.Left = Convert.ToDouble(value);
                NotifyPropertyChanged();
            }
        }

        public string MarginRight
        {
            get
            {
                return PhoneMargin.Right.ToString();
            }
            set
            {
                _PhoneMargin.Right = Convert.ToDouble(value);
                NotifyPropertyChanged();
            }
        }

        public string MarginTop
        {
            get
            {
                return PhoneMargin.Top.ToString();
            }
            set
            {
                _PhoneMargin.Top = Convert.ToDouble(value);
                NotifyPropertyChanged();
            }
        }

        public string MarginBottom
        {
            get
            {
                return PhoneMargin.Bottom.ToString();
            }
            set
            {
                _PhoneMargin.Bottom = Convert.ToDouble(value);
                NotifyPropertyChanged();
            }
        }

        public Thickness PhoneMargin
        {
            get
            {
                if (_PhoneMargin == null) _PhoneMargin = new Thickness(0);
                return _PhoneMargin;
            }
            set
            {
                _PhoneMargin = value;
                NotifyPropertyChanged();
            }
        }


    }


    public class BitmapListModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public BitmapListModel(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }

        private int _Width;
        private int _Height;
        private int _ClipTop;
        private int _ClipLeft;
        private int _ClipRight;
        private int _ClipBottom;
        private WriteableBitmap _WriteableBitmap;

        public int Height { get { return _Height; } set { _Height = value; NotifyPropertyChanged(); } }
        public int Width { get { return _Width; } set { _Width = value; NotifyPropertyChanged(); } }

        public Thickness Margin
        {
            get
            {
                return new Thickness(TransLeft, TransTop, 0, 0);
            }
        }

        public int ClipHeight
        {
            get { return ClipBottom - ClipTop; }
            set
            {
                ClipBottom = ClipTop + value;
                NotifyPropertyChanged();
            }
        }
        public int ClipWidth
        {
            get { return ClipRight - ClipLeft; }
            set
            {
                ClipRight = ClipLeft + value;
                NotifyPropertyChanged();
            }
        }
        public int ClipTop
        {
            get { return _ClipTop; }
            set
            {
                _ClipTop = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ClipHeight");
                NotifyPropertyChanged("TransTop");
            }
        }
        public int ClipLeft
        {
            get { return _ClipLeft; }
            set
            {
                _ClipLeft = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ClipWidth");
                NotifyPropertyChanged("TransLeft");
            }
        }
        public int ClipRight
        {
            get { return _ClipRight; }
            set
            {
                if (value < ClipLeft) _ClipRight = ClipLeft;
                else if (value > Width) _ClipRight = Width;
                else _ClipRight = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ClipWidth");
            }
        }
        public int ClipBottom
        {
            get { return _ClipBottom; }
            set
            {
                if (value < ClipTop) _ClipBottom = ClipTop;
                else if (value > Height) _ClipBottom = Height;
                else _ClipBottom = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ClipHeight");
            }
        }

        public int TransTop
        {
            get { return -ClipTop; }
        }

        public int TransLeft
        {
            get { return -ClipLeft; }
        }

        public WriteableBitmap WriteableBitmap
        {
            get
            {
                if (_WriteableBitmap == null) _WriteableBitmap = new WriteableBitmap(Width, Height);
                return _WriteableBitmap;
            }
            set { _WriteableBitmap = value; NotifyPropertyChanged(); }
        }
        public IBuffer PixelBuffer { get { return WriteableBitmap.PixelBuffer; } }

        public async Task SetSourceAsync(IRandomAccessStream streamSource)
        {
            await WriteableBitmap.SetSourceAsync(streamSource);
        }

        public async Task SetSourceAsync(StorageFile SF)
        {
            if (SF != null)
                try
                {
                    using (IRandomAccessStream stream = await SF.OpenAsync(FileAccessMode.Read))
                    {
                        await WriteableBitmap.SetSourceAsync(stream);
                    }
                }
                catch
                {
                    throw new Exception("文件不是图片！");
                }
        }

        public async Task Fill(Color color)
        {
            byte[] Colorbyte = new byte[Width * Height * 4];
            for (int i = 0; i < Colorbyte.Length; i += 4)
            {
                Colorbyte[i] = color.B;
                Colorbyte[i + 1] = color.G;
                Colorbyte[i + 2] = color.R;
                Colorbyte[i + 3] = color.A;
            }
            using (var stream = PixelBuffer.AsStream())
            {
                await stream.WriteAsync(Colorbyte, 0, Colorbyte.Length);
                await stream.FlushAsync();
            }
        }
    }

    public class AppSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public int PhoneListCount
        {
            get
            {
                return ReadSettings(nameof(PhoneListCount), -1);
            }
            set
            {
                SaveSettings(nameof(PhoneListCount), value);
            }
        }

        public bool IsMobile
        {
            get
            {
                return ReadSettings(nameof(IsMobile), Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"));
            }
        }

        public bool IsNotMobile
        {
            get
            {
                return ReadSettings(nameof(IsNotMobile), !Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"));
            }
        }

        public bool IsOpenFOPWithStart
        {
            get
            {
                return ReadSettings(nameof(IsOpenFOPWithStart), false);
            }
            set
            {
                SaveSettings(nameof(IsOpenFOPWithStart), value);
                NotifyPropertyChanged();
            }
        }

        public bool IsGlareShowToggleSwitchIsOn
        {
            get
            {
                return ReadSettings(nameof(IsGlareShowToggleSwitchIsOn), true);
            }
            set
            {
                SaveSettings(nameof(IsGlareShowToggleSwitchIsOn), value);
                NotifyPropertyChanged();
            }
        }

        public bool IsGlassEnable
        {
            get
            {
                if(IsNotMobile) SaveSettings(nameof(IsGlassEnable), true);
                return ReadSettings(nameof(IsGlassEnable), false);
            }
            set
            {
                SaveSettings(nameof(IsGlassEnable), value);
                NotifyPropertyChanged();
            }
        }

        public bool IsWhiteBackgroundToggleSwitchIsOn
        {
            get
            {
                return ReadSettings(nameof(IsWhiteBackgroundToggleSwitchIsOn), true);
            }
            set
            {
                SaveSettings(nameof(IsWhiteBackgroundToggleSwitchIsOn), value);
                NotifyPropertyChanged();
            }
        }

        public bool IsClipboardEnable
        {
            get
            {
                return ReadSettings(nameof(IsClipboardEnable), false);
            }
            set
            {
                SaveSettings(nameof(IsClipboardEnable), value);
                NotifyPropertyChanged();
            }
        }

        public bool IsCutShot
        {
            get
            {
                return ReadSettings(nameof(IsCutShot), false);
            }
            set
            {
                SaveSettings(nameof(IsCutShot), value);
                NotifyPropertyChanged();
            }
        }

        public bool IsOrderFileByDate
        {
            get
            {
                return ReadSettings(nameof(IsOrderFileByDate), IsMobile ? true : false);
            }
            set
            {
                SaveSettings(nameof(IsOrderFileByDate), value);
                NotifyPropertyChanged();
            }
        }

        public ApplicationDataContainer LocalSettings { get; set; }
        public AppSettings()
        {
            LocalSettings = ApplicationData.Current.LocalSettings;
            SaveSettings(nameof(IsMobile), Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"));
            SaveSettings(nameof(IsNotMobile), !Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"));
        }

        private void SaveSettings(string key, object value)
        {
            LocalSettings.Values[key] = value;
        }
        private T ReadSettings<T>(string key, T defaultValue)
        {
            if (LocalSettings.Values.ContainsKey(key))
            {
                return (T)LocalSettings.Values[key];
            }
            else
            {
                LocalSettings.Values[key] = defaultValue;
            }
            if (null != defaultValue)
            {
                return defaultValue;
            }
            return default(T);
        }
        protected void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }

    public class OnNavigatedModel
    {
        public string Mode;
        public object E = new object();
    }

    public class WidthHeight
    {
        public WidthHeight(int ImageWidth, int ImageHeight)
        {
            this.ImageWidth = ImageWidth;
            this.ImageHeight = ImageHeight;
        }

        public int ImageHeight;
        public int ImageWidth;
    }

    public class ToastNotificationModel
    {
        public ToastNotificationModel()
        {
            Messenger.Default.Register<string>(this, "ImagePath", (sender) =>
            {
                if (sender.EndsWith(".jpg") || sender.EndsWith(".png"))
                    CreateNotification(sender, 2);
                else CreateNotification(sender, 3);
            });
        }

        XmlDocument xml = new XmlDocument();
        string str1 = "<toast lang=\"zh-CN\" launch=\"{0}\">" +
                        "<visual>" +
                            "<binding template=\"ToastGeneric\">" +
                                "<text>带壳截图</text>" +
                                "<text>点击打开</text>" +
                                "<image placement=\"inline\" src=\"{0}\" />" +
                            "</binding>" +
                        "</visual>" +
                        "<actions>" +
                            "<action content=\"共享\" activationType =\"foreground\" arguments=\"{0}Share\" />" +
                            "" +
                        "</actions>" +
                     "</toast>";
        string str2 = "<toast lang=\"zh-CN\" launch=\"{0}\">" +
                "<visual>" +
                    "<binding template=\"ToastGeneric\">" +
                        "<text>带壳截图</text>" +
                        "<text>点击打开</text>" +
                        "<image placement=\"inline\" src=\"{0}\" />" +
                    "</binding>" +
                "</visual>" +
             "</toast>";
        string str3 = "<toast lang=\"zh-CN\" launch=\"{0}\">" +
                "<visual>" +
                    "<binding template=\"ToastGeneric\">" +
                        "<text>带壳截图</text>" +
                        "<text>{0}</text>" +
                    "</binding>" +
                "</visual>" +
             "</toast>";
        public void ClearNotification()
        {
            ToastNotificationManager.History.Clear();
        }

        public void CreateNotification(string ImagePath, int Count)
        {
            string str;
            switch (Count)
            {
                case 1:
                    str = string.Format(str1, ImagePath);
                    xml.LoadXml(string.Format(str1, ImagePath));
                    break;
                case 2:
                    str = string.Format(str2, ImagePath);
                    xml.LoadXml(string.Format(str2, ImagePath));
                    break;
                case 3:
                    str = string.Format(str3, ImagePath);
                    xml.LoadXml(string.Format(str3, ImagePath));
                    break;
                default:
                    str = string.Format(str3, ImagePath);
                    xml.LoadXml(string.Format(str3, ImagePath));
                    break;
            }

            ToastNotification toast = new ToastNotification(xml);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }

    public class ClipboardModel
    {
        DataPackage dataPackage = new DataPackage();
        public ClipboardModel()
        {
            Messenger.Default.Register<string>(this, "ImagePath", async (sender) =>
            {
                if (ApplicationData.Current.LocalSettings.Values["IsClipboardEnable"] == null ? false : (bool)ApplicationData.Current.LocalSettings.Values["IsClipboardEnable"])
                    if (sender != null)
                    {
                        StorageFile file = await StorageFile.GetFileFromPathAsync(sender);
                        RandomAccessStreamReference streamReference = RandomAccessStreamReference.CreateFromFile(file);
                        dataPackage.SetBitmap(streamReference);
                        Clipboard.SetContent(dataPackage);
                    }
            });
        }

    }

}
