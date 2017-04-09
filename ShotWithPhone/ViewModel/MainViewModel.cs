using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight;
using ShotWithPhone.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Display;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Xaml.Controls;
using Windows.Storage.Pickers;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.IO;
using System.Collections;
using Microsoft.Graphics.Canvas;

namespace ShotWithPhone.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            Messenger.Default.Register<int>(this, "ShowHideBorder", (sender) =>
            {
                BorderThickness = new Thickness(sender);
            });

        }

        private AppSettings AppSetting = new AppSettings();
        private DevicesReader _DR;
        private ObservableCollection<DeviceModel> _DevicesList;
        private ObservableCollection<DeviceOEMModel> _DeviceOEMList;
        //private ToastNotificationModel ToastNotification = new ToastNotificationModel();
        private ClipboardModel Clip = new ClipboardModel();
        private string _PhoneImage;
        private ObservableCollection<StorageFile> _ImageFileList;
        private ObservableCollection<BitmapListModel> _BitmapList;
        private ObservableCollection<BitmapListModel> _HeadBitmap;
        private ObservableCollection<BitmapListModel> _BottomBitmap;
        private BitmapListModel _Head, _Bottom;
        private Thickness _BorderThickness;
        private IReadOnlyList<IStorageItem> fileList;
        private int FileListCount = 0, ImageListCount = 0;
        private ICommand _OnHambListLoaded;
        private ICommand _OnBrowserClick;
        private ICommand _OnSaveClick;
        private ICommand _OnPhoneImageRecLoaded;
        private bool _IsSaveEnable = true;
        private bool _IsBrowserEnable = true;
        private ICommand _OnLastButtonClick;
        private ICommand _OnNextButtonClick;
        private ICommand _BrowserShotClick;
        private ICommand _SaveShotClick;
        private ICommand _AddPhoneShotClick;
        private ICommand _NewPrint;
        private ICommand _DeleteThisImage;
        private bool _IsChangeAllImageChecked;
        private WriteableBitmap _ShotBitmap;
        private BitmapImage _PhoneImageBrush;

        public Thickness BorderThickness
        {
            get
            {
                if (_BorderThickness == null)
                    _BorderThickness = new Thickness();
                return _BorderThickness;
            }
            set
            {
                _BorderThickness = value;
                RaisePropertyChanged();
            }
        }


        public bool IsChangeAllImageChecked
        {
            get { return _IsChangeAllImageChecked; }
            set
            {
                _IsChangeAllImageChecked = value;
                RaisePropertyChanged();
            }
        }
        public BitmapImage PhoneImageBrush
        {
            get
            {
                if (_PhoneImageBrush == null) _PhoneImageBrush = new BitmapImage();
                return _PhoneImageBrush;
            }
            set
            {
                _PhoneImageBrush = value;
                RaisePropertyChanged();
            }
        }
        private async Task InitFileList()
        {
            try
            {
                StorageFolder SFolder;
                if (AppSetting.IsMobile)
                    SFolder = await KnownFolders.PicturesLibrary.TryGetItemAsync("Screenshots") as StorageFolder;
                //else SFolder = KnownFolders.PicturesLibrary;
                else SFolder = KnownFolders.PicturesLibrary;
                if (SFolder == null) return;
                fileList = await SFolder.GetItemsAsync();
            }
            catch { fileList = new List<IStorageItem>(); }
        }
        private void ReadList()
        {
            for (int i = fileList.Count - FileListCount - 1, j = 0; i >= 0 && j < ((bool)ApplicationData.Current.LocalSettings.Values["IsMobile"] ? 2 : 10); i--)
            {
                FileListCount++;
                if (fileList[i] is StorageFile)
                {
                    if (fileList[i].Path.EndsWith(".png") || fileList[i].Path.EndsWith(".jpg"))
                    {
                        ImageFileList.Add((StorageFile)fileList[i]);
                        j++;
                        RaisePropertyChanged("IsNextLastButtonShow");
                    }

                }
            }
        }

        public DevicesReader DR
        {
            get
            {
                if (_DR == null)
                    _DR = new DevicesReader();
                return _DR;
            }
            set
            {
                _DR = value;
            }
        }

        public ObservableCollection<DeviceModel> DevicesList
        {
            get
            {
                if (_DevicesList == null)
                    _DevicesList = new ObservableCollection<DeviceModel>();
                return _DevicesList;
            }
            set { _DevicesList = value; }
        }

        public ObservableCollection<DeviceOEMModel> DeviceOEMList
        {
            get
            {
                if (_DeviceOEMList == null)
                    _DeviceOEMList = new ObservableCollection<DeviceOEMModel>();
                return _DeviceOEMList;
            }
            set { _DeviceOEMList = value; }
        }

        public string PhoneImage
        {
            get
            {
                return _PhoneImage;
            }
            set
            {
                _PhoneImage = value;
                RaisePropertyChanged();
            }
        }

        public Visibility IsNextLastButtonShow
        {
            get
            {
                if (ImageFileList.Count > 1)
                    return Visibility.Visible;
                else return Visibility.Collapsed;
            }
        }

        public ObservableCollection<StorageFile> ImageFileList
        {
            get
            {
                if (_ImageFileList == null) _ImageFileList = new ObservableCollection<StorageFile>();
                return _ImageFileList;
            }
            set { _ImageFileList = value; }
        }

        public ObservableCollection<BitmapListModel> BitmapList
        {
            get
            {
                if (_BitmapList == null)
                    _BitmapList = new ObservableCollection<BitmapListModel>();
                return _BitmapList;
            }
            set
            {
                _BitmapList = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<BitmapListModel> HeadBitmap
        {
            get
            {
                if (_HeadBitmap == null) _HeadBitmap = new ObservableCollection<BitmapListModel>();
                return _HeadBitmap;
            }
            set { _HeadBitmap = value; RaisePropertyChanged("HeadBitmap"); }
        }

        public ObservableCollection<BitmapListModel> BottomBitmap
        {
            get
            {
                if (_BottomBitmap == null) _BottomBitmap = new ObservableCollection<BitmapListModel>();
                return _BottomBitmap;
            }
            set { _BottomBitmap = value; RaisePropertyChanged("BottomBitmap"); }
        }

        public ICommand OnHambListLoaded
        {
            get
            {
                if (_OnHambListLoaded == null)
                    _OnHambListLoaded = new RelayCommand<UIElement>((sender) =>
                    {
                        var s = sender as ListView;
                        if (s.Items.Count > 0)
                        {
                            if (s.SelectedIndex == -1)
                                s.SelectedIndex = AppSetting.PhoneListCount;
                            if (s.SelectedIndex != -1)
                                AppSetting.PhoneListCount = s.SelectedIndex;
                        }

                    });
                return _OnHambListLoaded;
            }
            set { }
        }

        public ICommand OnHambListSelectionChanged
        {
            get
            {
                if (_OnHambListLoaded == null)
                    _OnHambListLoaded = new RelayCommand<UIElement>((sender) =>
                    {
                        var s = sender as ListView;
                        if (s.Items.Count > 0)
                        {
                            if (s.SelectedIndex == -1)
                                s.SelectedIndex = AppSetting.PhoneListCount;
                            if (s.SelectedIndex != -1)
                                AppSetting.PhoneListCount = s.SelectedIndex;
                        }

                    });
                return _OnHambListLoaded;
            }
            set { }
        }

        public ICommand OnBrowserClick
        {
            get
            {
                if (_OnBrowserClick == null)
                    _OnBrowserClick = new RelayCommand<ImageBrush>(async (sender) =>
                    {
                        IsBrowserEnable = false;
                        FileOpenPicker FOP = new FileOpenPicker();
                        FOP.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                        FOP.FileTypeFilter.Add(".jpg");
                        FOP.FileTypeFilter.Add(".png");
                        FOP.FileTypeFilter.Add(".jpeg");

                        FOP.ViewMode = PickerViewMode.Thumbnail;
                        StorageFile PhotoFile = await FOP.PickSingleFileAsync();
                        ImageFileList.Insert(0, PhotoFile);
                        RaisePropertyChanged("IsNextLastButtonShow");
                        if (PhotoFile != null)
                            try
                            {
                                await LoadImage(PhotoFile);
                                ImageListCount = 0;
                                IsBrowserEnable = true;
                            }
                            catch
                            {

                            }
                        IsBrowserEnable = true;
                    });
                return _OnBrowserClick;
            }
            set { }
        }

        public ICommand OnSaveClick
        {
            get
            {
                if (_OnSaveClick == null)
                    _OnSaveClick = new RelayCommand<UIElement>(async (sender) =>
                    {
                        IsSaveEnable = false;
                        try
                        {
                            StorageFolder WorkFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("带壳截图", CreationCollisionOption.OpenIfExists);
                            StorageFile file = await SaveImage(WorkFolder, sender,
                                (AppSetting.IsWhiteBackgroundToggleSwitchIsOn) ?
                                BitmapEncoder.JpegEncoderId : BitmapEncoder.PngEncoderId);
                            IsSaveEnable = true;
                            Messenger.Default.Send(file.Path, "ImagePath");
                        }
                        catch
                        {
                            Messenger.Default.Send("保存失败，请重试", "ImagePath");
                        }
                        IsSaveEnable = true;
                    });
                return _OnSaveClick;
            }
        }

        public ICommand OnPhoneImageRecLoaded
        {
            get
            {
                if (_OnPhoneImageRecLoaded == null)
                    _OnPhoneImageRecLoaded = new RelayCommand<ImageBrush>(async (sender) =>
                    {
                        IsBrowserEnable = false;
                        try
                        {
                            await InitFileList();
                            ReadList();
                            await LoadImage(ImageFileList.First());
                            IsBrowserEnable = true;
                        }
                        catch
                        {

                        }
                        IsBrowserEnable = true;
                    });
                return _OnPhoneImageRecLoaded;
            }
            set { }
        }

        public ICommand OnLastButtonClick
        {
            get
            {
                if (_OnLastButtonClick == null)
                    _OnLastButtonClick = new RelayCommand<ImageBrush>(async sender =>
                    {
                        if (ImageListCount > 0)
                        {
                            ImageListCount--;
                            await LoadImage(ImageFileList[ImageListCount]);
                        }
                        else
                        {
                            ImageListCount = ImageFileList.Count - 1;
                            await LoadImage(ImageFileList[ImageListCount]);
                        }
                    });
                return _OnLastButtonClick;
            }
            set { }
        }
        public ICommand OnNextButtonClick
        {
            get
            {
                if (_OnNextButtonClick == null)
                    _OnNextButtonClick = new RelayCommand<ImageBrush>(async sender =>
                    {
                        if (ImageListCount >= ImageFileList.Count - 1)
                        {
                            ReadList();
                        }
                        if (ImageListCount < ImageFileList.Count - 1)
                        {
                            ImageListCount++;
                            await LoadImage(ImageFileList[ImageListCount]);
                        }
                    });
                return _OnNextButtonClick;
            }
            set { }
        }


        public ICommand SaveShotClick
        {
            get
            {
                if (_SaveShotClick == null)
                    _SaveShotClick = new RelayCommand<UIElement>(async (sender) =>
                    {
                        IsSaveEnable = false;
                        var MyInkCanvas = (sender as InkCanvas);
                        int InkCount = MyInkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count;
                        try
                        {
                            StorageFolder WorkFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("长截图", CreationCollisionOption.OpenIfExists);
                            StorageFile file = await WorkFolder.CreateFileAsync(DateTime.Now.ToString("yyMMdd-HHmmss") + ".jpg", CreationCollisionOption.GenerateUniqueName);

                            int ImageHeight = 0, ImageWidth = BitmapList.First().ClipWidth, Point = 0;
                            byte[] ImageByte;
                            if (BitmapList.Count == 0) return;
                            if (BitmapList.Count == 1)
                            {
                                ImageByte = BitmapList.First().PixelBuffer.ToArray();
                                ImageHeight = BitmapList.First().ClipHeight;
                            }
                            else
                            {
                                int HeadHeight = 0, BottomHeight = 0;
                                foreach (BitmapListModel ImageItem in HeadBitmap)
                                {
                                    ImageHeight += ImageItem.ClipHeight;
                                    HeadHeight = ImageItem.ClipHeight;
                                }
                                foreach (BitmapListModel ImageItem in BitmapList)
                                {
                                    ImageHeight += ImageItem.ClipHeight;
                                }
                                foreach (BitmapListModel ImageItem in BottomBitmap)
                                {
                                    ImageHeight += ImageItem.ClipHeight;
                                    BottomHeight = ImageItem.ClipHeight;
                                }

                                ImageByte = new byte[ImageHeight * ImageWidth * 4];

                                foreach (BitmapListModel ImageItem in HeadBitmap)
                                {
                                    byte[] ImageHeadByte = ImageItem.PixelBuffer.ToArray();
                                    for (int i = ImageItem.ClipTop * ImageItem.ClipWidth * 4; i < ImageItem.ClipHeight * ImageItem.ClipWidth * 4 && Point < ImageByte.Length; i++)
                                    {
                                        ImageByte[Point++] = ImageHeadByte[i];
                                    }
                                }

                                foreach (BitmapListModel ImageItem in BitmapList)
                                {
                                    byte[] ImageItemByte = ImageItem.PixelBuffer.ToArray();
                                    for (int i = ImageItem.ClipTop * ImageItem.ClipWidth * 4; i < (ImageItem.ClipHeight + HeadHeight) * ImageItem.ClipWidth * 4 && Point < ImageByte.Length; i++)
                                    {
                                        ImageByte[Point++] = ImageItemByte[i];
                                    }
                                }

                                foreach (BitmapListModel ImageItem in BottomBitmap)
                                {
                                    byte[] ImageBottomByte = ImageItem.PixelBuffer.ToArray();
                                    for (int i = ImageItem.ClipTop * ImageItem.ClipWidth * 4; i < (ImageItem.ClipTop + BottomHeight) * ImageItem.ClipWidth * 4 && Point < ImageByte.Length; i++)
                                    {
                                        ImageByte[Point++] = ImageBottomByte[i];
                                    }
                                }
                            }
                            _ShotBitmap = new WriteableBitmap(ImageWidth, ImageHeight);
                            using (var bitmapStream = _ShotBitmap.PixelBuffer.AsStream())
                            {
                                await bitmapStream.WriteAsync(ImageByte, 0, ImageWidth * ImageHeight * 4 - 1);
                                await bitmapStream.FlushAsync();
                                bitmapStream.Seek(0, SeekOrigin.Begin);

                                CanvasDevice device = CanvasDevice.GetSharedDevice();
                                CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, ImageWidth, ImageHeight, 96);
                                var canvasbitmap = CanvasBitmap.CreateFromBytes(device, _ShotBitmap.PixelBuffer.ToArray(), ImageWidth, ImageHeight, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized);
                                using (var ds = renderTarget.CreateDrawingSession())
                                {
                                    ds.Clear(Colors.White);
                                    ds.DrawImage(canvasbitmap);
                                    if (InkCount > 0)
                                        ds.DrawInk(MyInkCanvas.InkPresenter.StrokeContainer.GetStrokes());
                                }

                                using (IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite))
                                {
                                    await renderTarget.SaveAsync(fileStream, CanvasBitmapFileFormat.Jpeg, 1f);
                                }
                                Messenger.Default.Send(file.Path, "ImagePath");
                            }
                        }
                        catch { }


                        IsSaveEnable = true;
                    });
                return _SaveShotClick;
            }
        }

        public ICommand AddPhoneShotClick
        {
            get
            {
                if (_AddPhoneShotClick == null)
                    _AddPhoneShotClick = new RelayCommand<UIElement>(async (sender) =>
                    {
                        IsSaveEnable = false;
                        var MyInkCanvas = (sender as InkCanvas);
                        int InkCount = MyInkCanvas.InkPresenter.StrokeContainer.GetStrokes().Count;
                        try
                        {
                            int ImageHeight = 0, ImageWidth = BitmapList.First().ClipWidth, Point = 0;
                            byte[] ImageByte;
                            if (BitmapList.Count == 1)
                            {
                                ImageByte = BitmapList.First().PixelBuffer.ToArray();
                                ImageHeight = BitmapList.First().ClipHeight;
                            }
                            else
                            {
                                int HeadHeight = 0, BottomHeight = 0;
                                foreach (BitmapListModel ImageItem in HeadBitmap)
                                {
                                    ImageHeight += ImageItem.ClipHeight;
                                    HeadHeight = ImageItem.ClipHeight;
                                }
                                foreach (BitmapListModel ImageItem in BitmapList)
                                {
                                    ImageHeight += ImageItem.ClipHeight;
                                }
                                foreach (BitmapListModel ImageItem in BottomBitmap)
                                {
                                    ImageHeight += ImageItem.ClipHeight;
                                    BottomHeight = ImageItem.ClipHeight;
                                }

                                ImageByte = new byte[ImageHeight * ImageWidth * 4];

                                foreach (BitmapListModel ImageItem in HeadBitmap)
                                {
                                    byte[] ImageHeadByte = ImageItem.PixelBuffer.ToArray();
                                    for (int i = ImageItem.ClipTop * ImageItem.ClipWidth * 4; i < ImageItem.ClipHeight * ImageItem.ClipWidth * 4 && Point < ImageByte.Length; i++)
                                    {
                                        ImageByte[Point++] = ImageHeadByte[i];
                                    }
                                }

                                foreach (BitmapListModel ImageItem in BitmapList)
                                {
                                    byte[] ImageItemByte = ImageItem.PixelBuffer.ToArray();
                                    for (int i = ImageItem.ClipTop * ImageItem.ClipWidth * 4; i < (ImageItem.ClipHeight + HeadHeight) * ImageItem.ClipWidth * 4 && Point < ImageByte.Length; i++)
                                    {
                                        ImageByte[Point++] = ImageItemByte[i];
                                    }
                                }

                                foreach (BitmapListModel ImageItem in BottomBitmap)
                                {
                                    byte[] ImageBottomByte = ImageItem.PixelBuffer.ToArray();
                                    for (int i = ImageItem.ClipTop * ImageItem.ClipWidth * 4; i < (ImageItem.ClipTop + BottomHeight) * ImageItem.ClipWidth * 4 && Point < ImageByte.Length; i++)
                                    {
                                        ImageByte[Point++] = ImageBottomByte[i];
                                    }
                                }
                            }
                            _ShotBitmap = new WriteableBitmap(ImageWidth, ImageHeight);
                            using (var bitmapStream = _ShotBitmap.PixelBuffer.AsStream())
                            {
                                await bitmapStream.WriteAsync(ImageByte, 0, ImageWidth * ImageHeight * 4 - 1);
                                await bitmapStream.FlushAsync();
                                bitmapStream.Seek(0, SeekOrigin.Begin);

                                CanvasDevice device = CanvasDevice.GetSharedDevice();
                                CanvasRenderTarget renderTarget = new CanvasRenderTarget(device, ImageWidth, ImageHeight, 96);
                                var canvasbitmap = CanvasBitmap.CreateFromBytes(device, _ShotBitmap.PixelBuffer.ToArray(), ImageWidth, ImageHeight, Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized);
                                using (var ds = renderTarget.CreateDrawingSession())
                                {
                                    ds.Clear(Colors.White);
                                    ds.DrawImage(canvasbitmap);
                                    if (InkCount > 0)
                                        ds.DrawInk(MyInkCanvas.InkPresenter.StrokeContainer.GetStrokes());
                                }
                                byte[] Pixel = renderTarget.GetPixelBytes();
                                //IRandomAccessStream stream = new MemoryStream(Pixel).AsRandomAccessStream();
                                StorageFolder SFolder = ApplicationData.Current.TemporaryFolder;
                                StorageFile SF = await SFolder.CreateFileAsync("temp.jpg", CreationCollisionOption.ReplaceExisting);
                                using (var ras = await SF.OpenAsync(FileAccessMode.ReadWrite, StorageOpenOptions.None))
                                {
                                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, ras);
                                    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore, (uint)ImageWidth, (uint)ImageHeight, 96.0, 96.0, Pixel);
                                    await encoder.FlushAsync();
                                }
                                //await LoadImage(SF);
                                ImageFileList.Insert(0, SF);
                                ImageListCount = 0;
                                await LoadImage(ImageFileList.First());
                            }
                            Messenger.Default.Send(true, "GoToPhonePage");
                        }
                        catch
                        {

                        }
                        IsSaveEnable = true;
                    });
                return _AddPhoneShotClick;
            }
        }

        public ICommand NewPrint
        {
            get
            {
                if (_NewPrint == null)
                    _NewPrint = new RelayCommand<InkCanvas>(async (sender) =>
                    {
                        try
                        {
                            BitmapList?.Clear();
                            HeadBitmap?.Clear();
                            BottomBitmap?.Clear();
                            RaisePropertyChanged("BitmapList");
                            RaisePropertyChanged("HeadBitmap");
                            RaisePropertyChanged("BottomBitmap");
                            sender.InkPresenter.StrokeContainer.Clear();

                            int PrintHeight = 1280, PrintWidth = 720;
                            BitmapListModel BLM = new BitmapListModel(PrintWidth, PrintHeight);
                            BLM.ClipHeight = BLM.Height;
                            BLM.ClipWidth = BLM.Width;
                            await BLM.Fill(Colors.White);
                            BitmapList.Add(BLM);

                            Messenger.Default.Send(Visibility.Collapsed, "ReadMe2");
                        }
                        catch
                        {
                            BitmapList?.Clear();
                            HeadBitmap?.Clear();
                            BottomBitmap?.Clear();
                            RaisePropertyChanged("BitmapList");
                            RaisePropertyChanged("HeadBitmap");
                            RaisePropertyChanged("BottomBitmap");
                        }
                    });
                return _NewPrint;
            }
        }

        public ICommand DeleteThisImage
        {
            get
            {
                if (_DeleteThisImage == null)
                    _DeleteThisImage = new RelayCommand<int>(sender =>
                    {
                        BitmapList.RemoveAt(sender);
                        RaisePropertyChanged("BitmapList");
                        if (BitmapList.Count == 0)
                        {
                            HeadBitmap?.Clear();
                            BottomBitmap?.Clear();
                            RaisePropertyChanged("HeadBitmap");
                            RaisePropertyChanged("BottomBitmap");
                        }
                    });
                return _DeleteThisImage;
            }
        }

        public bool ShowHideHeadImage
        {
            get
            {
                if (HeadBitmap.Count > 0) return true;
                else return false;
            }
            set
            {
                if (value) HeadBitmap.Add(_Head);
                else HeadBitmap.Clear();
                RaisePropertyChanged();
            }
        }

        public bool ShowHideBottomImage
        {
            get
            {
                if (BottomBitmap.Count > 0) return true;
                else return false;
            }
            set
            {
                if (value) BottomBitmap.Add(_Bottom);
                else BottomBitmap.Clear();
                RaisePropertyChanged();
            }
        }

        public ICommand BrowserShotClick
        {
            get
            {
                if (_BrowserShotClick == null)
                    _BrowserShotClick = new RelayCommand<UIElement>(async (sender) =>
                    {
                        try
                        {
                            BitmapList?.Clear();
                            HeadBitmap?.Clear();
                            BottomBitmap?.Clear();
                            RaisePropertyChanged("BitmapList");
                            RaisePropertyChanged("HeadBitmap");
                            RaisePropertyChanged("BottomBitmap");

                            sender.Visibility = Visibility.Collapsed;
                            IsBrowserEnable = false;
                            FileOpenPicker FOP = new FileOpenPicker();
                            FOP.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                            FOP.FileTypeFilter.Add(".jpg");
                            FOP.FileTypeFilter.Add(".png");
                            FOP.FileTypeFilter.Add(".jpeg");
                            FOP.ViewMode = PickerViewMode.Thumbnail;
                            await AddShotImage(new List<StorageFile>(await FOP.PickMultipleFilesAsync()));
                        }
                        catch
                        {
                            BitmapList?.Clear();
                            HeadBitmap?.Clear();
                            BottomBitmap?.Clear();
                        }
                        IsBrowserEnable = true;
                        sender.Visibility = Visibility.Visible;
                    });
                return _BrowserShotClick;
            }
        }

        private async Task AddShotImage(List<StorageFile> SelectedFileList)
        {
            List<StorageFile> FileList;
            if (AppSetting.IsOrderFileByDate)
            {
                FileList = (from item in SelectedFileList
                            orderby item.DateCreated.ToFileTime()
                            select item).ToList();
            }
            else FileList = new List<StorageFile>(SelectedFileList);

            int SumHeight = 0, SumWidth = 0, AverageHeight = 0, AverageWidth = 0;
            bool EqualHeightOrWidth = true;
            List<BitmapImage> BL = new List<BitmapImage>();

            foreach (var item in FileList)
            {
                using (var stream = await item.OpenAsync(FileAccessMode.Read))
                {
                    BitmapImage BI = new BitmapImage();
                    await BI.SetSourceAsync(stream);
                    SumHeight += BI.PixelHeight;
                    SumWidth += BI.PixelWidth;
                    BL.Add(BI);
                }
            }
            AverageHeight = SumHeight / BL.Count;
            AverageWidth = SumWidth / BL.Count;

            foreach (var item in BL)
            {
                if (item.PixelHeight != AverageHeight && item.PixelWidth != AverageWidth)
                {
                    EqualHeightOrWidth = false;
                    break;
                }

            }

            if (FileList.Count > 1)
                try
                {
                    if (EqualHeightOrWidth)
                    {
                        _Head = await GetImageHead(FileList.Last(), FileList.First());
                        _Bottom = await GetImageBottom(FileList.Last(), FileList.First());
                        HeadBitmap.Add(_Head);
                        RaisePropertyChanged("ShowHideHeadImage");
                        BottomBitmap.Add(_Bottom);
                        RaisePropertyChanged("ShowHideBottomImage");
                        for (int i = 0; i < FileList.Count - 1; i++)
                        {
                            BitmapList.Add(await CutImage(FileList[i], FileList[i + 1], (int)HeadBitmap.First().ClipHeight, (int)BottomBitmap.First().ClipHeight));
                        }
                        BitmapList.Add(await CutImage(FileList.Last(), (int)HeadBitmap.First().ClipHeight, (int)BottomBitmap.First().ClipHeight));
                        RaisePropertyChanged("BitmapList");
                    }
                    else
                    {
                        for (int i = 0; i < FileList.Count; i++)
                        {
                            BitmapList.Add(await CutImage(FileList[i], 0, 0));
                        }
                        RaisePropertyChanged("BitmapList");
                    }
                    Messenger.Default.Send(Visibility.Collapsed, "ReadMe2");
                }
                catch
                {
                    BitmapList?.Clear();
                    HeadBitmap?.Clear();
                    BottomBitmap?.Clear();
                    RaisePropertyChanged("BitmapList");
                    RaisePropertyChanged("HeadBitmap");
                    RaisePropertyChanged("BottomBitmap");
                }
            if (FileList.Count == 1)
                try
                {
                    BitmapList.Add(await CutImage(FileList.First(), 0, 0));
                    Messenger.Default.Send(Visibility.Collapsed, "ReadMe2");
                }
                catch
                {
                    BitmapList?.Clear();
                    HeadBitmap?.Clear();
                    BottomBitmap?.Clear();
                    RaisePropertyChanged("BitmapList");
                    RaisePropertyChanged("HeadBitmap");
                    RaisePropertyChanged("BottomBitmap");
                }
        }

        public bool IsSaveEnable
        {
            get
            {
                return _IsSaveEnable;
            }
            set
            {
                _IsSaveEnable = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsProgressBarEnable");
            }
        }

        public bool IsProgressBarEnable
        {
            get
            {
                return (!_IsSaveEnable) || (!_IsBrowserEnable);
            }
            set { }
        }

        public bool IsBrowserEnable
        {
            get
            {
                return _IsBrowserEnable;
            }
            set
            {
                _IsBrowserEnable = value;
                RaisePropertyChanged();
                RaisePropertyChanged("IsProgressBarEnable");
            }
        }

        public async void OnMainPageLoaded()
        {
            DevicesList = await DR.GetJson();
            DR.GroupPhoneList(DevicesList, DeviceOEMList);
            RaisePropertyChanged();
            if (AppSetting.IsMobile)
            {
                StatusBar statusBar = StatusBar.GetForCurrentView();
                statusBar.BackgroundColor = (App.Current.Resources["SAC"] as SolidColorBrush).Color;
                statusBar.ForegroundColor = Colors.White;
                statusBar.BackgroundOpacity = 1;
            }
        }



        public async Task LoadImage(StorageFile file)
        {
            using (IRandomAccessStream stream = await file.OpenReadAsync())
            {
                PhoneImageBrush = new BitmapImage();
                await PhoneImageBrush.SetSourceAsync(stream);
                Messenger.Default.Send(new WidthHeight(PhoneImageBrush.PixelWidth, PhoneImageBrush.PixelHeight), "ImageWidthHeight");
            }

        }

        async Task<StorageFile> SaveImage(StorageFolder WorkFolder, UIElement element, Guid BitMapEncGuid)
        {
            var bitmap = new RenderTargetBitmap();
            StorageFile file = await WorkFolder.CreateFileAsync(DateTime.Now.ToString("yyMMdd-HHmmss") + (BitMapEncGuid.Equals(BitmapEncoder.PngEncoderId) ? ".png" : ".jpg"), CreationCollisionOption.GenerateUniqueName);
            if ((int)element.RenderSize.Height > 3000)
                await bitmap.RenderAsync(element, (int)element.RenderSize.Width / 6 * 2, (int)element.RenderSize.Height / 6 * 2);
            else if ((int)element.RenderSize.Height > 2000)
                await bitmap.RenderAsync(element, (int)element.RenderSize.Width / 3 * 2, (int)element.RenderSize.Height / 3 * 2);
            else await bitmap.RenderAsync(element);
            var buffer = await bitmap.GetPixelsAsync();

            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    var encod = await BitmapEncoder.CreateAsync(BitMapEncGuid, stream);
                    encod.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, DisplayInformation.GetForCurrentView().LogicalDpi, DisplayInformation.GetForCurrentView().LogicalDpi, buffer.ToArray());
                    await encod.FlushAsync();
                }
                FileUpdateStatus FUS = await CachedFileManager.CompleteUpdatesAsync(file);
            }
            return file;
        }

        async Task<BitmapListModel> SaveImage(UIElement element)
        {
            var Renderbitmap = new RenderTargetBitmap();
            if ((int)element.RenderSize.Height > 3000)
                await Renderbitmap.RenderAsync(element, (int)element.RenderSize.Width / 6 * 2, (int)element.RenderSize.Height / 6 * 2);
            else if ((int)element.RenderSize.Height > 2000)
                await Renderbitmap.RenderAsync(element, (int)element.RenderSize.Width / 3 * 2, (int)element.RenderSize.Height / 3 * 2);
            else await Renderbitmap.RenderAsync(element);
            var buffer = await Renderbitmap.GetPixelsAsync();
            BitmapListModel bitmap = new BitmapListModel(Renderbitmap.PixelWidth, Renderbitmap.PixelHeight);
            using (var stream = bitmap.PixelBuffer.AsStream())
            {
                byte[] a = buffer.ToArray();
                await stream.WriteAsync(buffer.ToArray(), 0, (int)buffer.Length);
            }
            return bitmap;
        }


        async Task<BitmapListModel> GetImageHead(StorageFile LastImage, StorageFile MiddleImage)
        {
            int tolerance = 4;
            byte[] LastImageByte, MiddleImageByte, HeadImageByte;
            int HeadImageHeight = 0;

            using (IRandomAccessStream stream1 = await LastImage.OpenAsync(FileAccessMode.Read),
                                       stream2 = await MiddleImage.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder Lastdecoder = await BitmapDecoder.CreateAsync(stream1);
                BitmapDecoder Middledecoder = await BitmapDecoder.CreateAsync(stream2);

                PixelDataProvider LastImagePixelData = await Lastdecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                                                BitmapAlphaMode.Straight,
                                                                                new BitmapTransform(),
                                                                                ExifOrientationMode.IgnoreExifOrientation,
                                                                                ColorManagementMode.DoNotColorManage);
                PixelDataProvider MiddleImagePixelData = await Middledecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                                                BitmapAlphaMode.Straight,
                                                                                new BitmapTransform(),
                                                                                ExifOrientationMode.IgnoreExifOrientation,
                                                                                ColorManagementMode.DoNotColorManage);

                LastImageByte = LastImagePixelData.DetachPixelData();
                MiddleImageByte = MiddleImagePixelData.DetachPixelData();

                //获取头部

                for (int i = 0; i < Lastdecoder.PixelHeight; i++)
                {
                    int SamePixel = 0;
                    for (int j = 0; j < Lastdecoder.PixelWidth * 4; j++)
                    {
                        if (Math.Abs(LastImageByte[i * Lastdecoder.PixelWidth * 4 + j] - MiddleImageByte[i * Middledecoder.PixelWidth * 4 + j]) < 15)
                        {
                            SamePixel++;
                        }
                    }
                    if (Lastdecoder.PixelWidth * 4 - SamePixel < Lastdecoder.PixelWidth / tolerance)
                    {
                        HeadImageHeight++;
                    }
                    if (HeadImageHeight != i + 1) break;
                }
                BitmapListModel Bitmap = new BitmapListModel(Convert.ToInt32(Middledecoder.PixelWidth), Convert.ToInt32(Middledecoder.PixelHeight))
                { ClipTop = 0, ClipLeft = 0 };
                Bitmap.ClipWidth = Bitmap.Width;
                Bitmap.ClipHeight = HeadImageHeight;
                await Bitmap.WriteableBitmap.SetSourceAsync(stream2);
                return Bitmap;
            }
        }

        async Task<BitmapListModel> GetImageBottom(StorageFile MiddleImage, StorageFile NextImage)
        {
            int tolerance = 72;
            byte[] MiddleImageByte, NextImageByte, BottomImageByte;
            int BottomImageHeight = 0;

            using (IRandomAccessStream stream1 = await MiddleImage.OpenAsync(FileAccessMode.Read),
                                       stream2 = await NextImage.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder Middledecoder = await BitmapDecoder.CreateAsync(stream1);
                BitmapDecoder Nextdecoder = await BitmapDecoder.CreateAsync(stream2);

                PixelDataProvider MiddleImagePixelData = await Middledecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                                                BitmapAlphaMode.Straight,
                                                                                new BitmapTransform(),
                                                                                ExifOrientationMode.IgnoreExifOrientation,
                                                                                ColorManagementMode.DoNotColorManage);
                PixelDataProvider NextImagePixelData = await Nextdecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                                                BitmapAlphaMode.Straight,
                                                                                new BitmapTransform(),
                                                                                ExifOrientationMode.IgnoreExifOrientation,
                                                                                ColorManagementMode.DoNotColorManage);

                MiddleImageByte = MiddleImagePixelData.DetachPixelData();
                NextImageByte = NextImagePixelData.DetachPixelData();

                //获取尾部

                for (int i = 0; i < Nextdecoder.PixelHeight; i++)
                {
                    int SamePixel = 0;
                    for (int j = 0; j < Nextdecoder.PixelWidth * 4; j++)
                    {
                        if (Math.Abs(NextImageByte[(Nextdecoder.PixelHeight - i - 1) * Nextdecoder.PixelWidth * 4 + j] - MiddleImageByte[(Nextdecoder.PixelHeight - i - 1) * Middledecoder.PixelWidth * 4 + j]) < 5)
                        {
                            SamePixel++;
                        }
                    }
                    if (Nextdecoder.PixelWidth * 4 - SamePixel < Nextdecoder.PixelWidth / tolerance)
                    {
                        BottomImageHeight++;
                    }
                    if (BottomImageHeight != i + 1) break;
                }

                BitmapListModel Bitmap = new BitmapListModel(Convert.ToInt32(Middledecoder.PixelWidth), Convert.ToInt32(Middledecoder.PixelHeight));
                Bitmap.ClipTop = Bitmap.Height - BottomImageHeight;
                Bitmap.ClipLeft = 0;
                Bitmap.ClipHeight = BottomImageHeight;
                Bitmap.ClipWidth = Bitmap.Width;
                await Bitmap.WriteableBitmap.SetSourceAsync(stream1);
                return Bitmap;
            }
        }

        async Task<BitmapListModel> CutImage(StorageFile MiddleImage, int HeadHeight, int BottomHeight)
        {
            using (IRandomAccessStream stream1 = await MiddleImage.OpenAsync(FileAccessMode.Read))
            {

                BitmapDecoder Middledecoder = await BitmapDecoder.CreateAsync(stream1);
                PixelDataProvider MiddleImagePixelData = await Middledecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                                                BitmapAlphaMode.Straight,
                                                                                new BitmapTransform(),
                                                                                ExifOrientationMode.IgnoreExifOrientation,
                                                                                ColorManagementMode.DoNotColorManage);

                byte[] MiddleImageByte = MiddleImagePixelData.DetachPixelData();

                int ImageWidth = (int)Middledecoder.PixelWidth, ImageHeight = (int)Middledecoder.PixelHeight - BottomHeight;
                BitmapListModel Bitmap = new BitmapListModel(Convert.ToInt32(Middledecoder.PixelWidth), ImageHeight)
                {
                    ClipTop = HeadHeight < ImageHeight ? HeadHeight : 0,
                    ClipLeft = 0
                };
                Bitmap.ClipWidth = Bitmap.Width;
                Bitmap.ClipHeight = ImageHeight;
                await Bitmap.WriteableBitmap.SetSourceAsync(stream1);
                return Bitmap;
            }
        }

        async Task<BitmapListModel> CutImage(StorageFile MiddleImage, StorageFile NextImage, int HeadHeight, int BottomHeight)
        {
            byte[] MiddleImageByte, NextImageByte;
            using (IRandomAccessStream stream1 = await MiddleImage.OpenAsync(FileAccessMode.Read),
                                       stream2 = await NextImage.OpenAsync(FileAccessMode.Read))
            {
                BitmapDecoder Middledecoder = await BitmapDecoder.CreateAsync(stream1);
                BitmapDecoder Nextdecoder = await BitmapDecoder.CreateAsync(stream2);

                PixelDataProvider MiddleImagePixelData = await Middledecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                                                BitmapAlphaMode.Straight,
                                                                                new BitmapTransform(),
                                                                                ExifOrientationMode.IgnoreExifOrientation,
                                                                                ColorManagementMode.DoNotColorManage);
                PixelDataProvider NextImagePixelData = await Nextdecoder.GetPixelDataAsync(BitmapPixelFormat.Bgra8,
                                                                                BitmapAlphaMode.Straight,
                                                                                new BitmapTransform(),
                                                                                ExifOrientationMode.IgnoreExifOrientation,
                                                                                ColorManagementMode.DoNotColorManage);

                MiddleImageByte = MiddleImagePixelData.DetachPixelData();
                NextImageByte = NextImagePixelData.DetachPixelData();

                bool IsDifferent = true;
                int SameRow = 0;
                int ImageWidth = (int)Middledecoder.PixelWidth, ImageHeight = (int)Middledecoder.PixelHeight - BottomHeight - HeadHeight;
                List<byte[]> MiddleBottomRowList = new List<byte[]>();
                List<byte[]> NextTopRowList = new List<byte[]>();
                byte[] MiddleRow = new byte[Middledecoder.PixelWidth * 4];
                byte[] NextRow = new byte[Nextdecoder.PixelWidth * 4];

                for (int i1 = Convert.ToInt32(Middledecoder.PixelHeight - BottomHeight - 1), i2 = HeadHeight;
                    i1 > HeadHeight && i2 < Convert.ToInt32(Nextdecoder.PixelHeight - BottomHeight) && IsDifferent;
                    i1--, i2++)
                {
                    for (int j = 0; j < Middledecoder.PixelWidth * 4 && j < Nextdecoder.PixelWidth * 4; j++)
                    {
                        MiddleRow[j] = MiddleImageByte[i1 * Middledecoder.PixelWidth * 4 + j];
                        NextRow[j] = NextImageByte[i2 * Nextdecoder.PixelWidth * 4 + j];
                    }
                    MiddleBottomRowList.Add(MiddleRow);
                    NextTopRowList.Add(NextRow);

                    for (int j = 0; j < MiddleBottomRowList.Count; j++)
                    {
                        int SamePixel = 0;
                        for (int k = 0; k < Middledecoder.PixelWidth; k += 2)
                        {
                            if (MiddleBottomRowList[MiddleBottomRowList.Count - j - 1][k * 4] - NextTopRowList[j][k * 4] < 2 &&
                                MiddleBottomRowList[MiddleBottomRowList.Count - j - 1][k * 4 + 1] - NextTopRowList[j][k * 4 + 1] < 2 &&
                                MiddleBottomRowList[MiddleBottomRowList.Count - j - 1][k * 4 + 2] - NextTopRowList[j][k * 4 + 2] < 2) SamePixel += 2;
                        }
                        if (Middledecoder.PixelWidth - SamePixel > 10)   //匹配不上，跳出
                        {
                            break;
                        }
                        if (SameRow < j)
                        {
                            SameRow = j;
                            if (SameRow > ImageHeight / 8)
                                IsDifferent = false;
                        }
                    }

                }
                BitmapListModel Bitmap = new BitmapListModel(Convert.ToInt32(Middledecoder.PixelWidth), Convert.ToInt32(Middledecoder.PixelHeight))
                {
                    ClipTop = HeadHeight,
                    ClipLeft = 0
                };
                Bitmap.ClipWidth = Bitmap.Width;
                Bitmap.ClipHeight = ImageHeight - (SameRow == ImageHeight ? 0 : SameRow);
                await Bitmap.WriteableBitmap.SetSourceAsync(stream1);
                return Bitmap;
            }

        }

    }

}
