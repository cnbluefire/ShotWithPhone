using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ShotWithPhone.Model;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Notifications;
using System.Xml;
using System.Threading.Tasks;
using Windows.System.Profile;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Core;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Composition;
using Microsoft.Graphics.Canvas.Effects;
using GalaSoft.MvvmLight.Messaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI.Input;
using Windows.UI.Input.Inking;
using System.Numerics;
using System.Collections.ObjectModel;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace ShotWithPhone
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        AppSettings AppSetting = new AppSettings();
        private string filePath;
        public BitmapListModel _bitmap
        {
            get
            {
                if (IMAGE.SelectedIndex > 0) return IMAGE.Items[IMAGE.SelectedIndex - 1] as BitmapListModel;
                else return null;
            }
        }

        public MainPage()
        {
            this.InitializeComponent();

            var coreTitleBar = Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar;
            var appTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            appTitleBar.ButtonBackgroundColor = Colors.Transparent;
            Window.Current.SetTitleBar(HeadBackgroungRec);

            MyInkCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;
            MyInkCanvas.InkPresenter.IsInputEnabled = false;
            Messenger.Default.Register<WidthHeight>(this, "ImageWidthHeight", (sender) => { ImageWidthHeight = sender; });
            Messenger.Default.Register<string>(this, "ImagePath", async (FilePath) =>
            {
                try
                {
                    this.filePath = FilePath;
                    StorageFile SF = await StorageFile.GetFileFromPathAsync(FilePath);
                    BitmapImage bitmap = new BitmapImage();
                    await bitmap.SetSourceAsync(await SF.OpenAsync(FileAccessMode.Read));
                    LittlePhoto.Source = bitmap;
                    LittlePhotoShow.Begin();
                }
                catch { }
            });
            Messenger.Default.Register<bool>(this, "GoToPhonePage", (sender) =>
            {
                if (sender == true) MainPivot.SelectedIndex = 0;
            });
            Messenger.Default.Register<Visibility>(this, "ReadMe2", (sender) =>
            {
                ReadMe2Grid.Visibility = sender;
            });
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (AppSetting.IsMobile) PCTitle.Height = new GridLength(0);
            else MobileHead.Height = new GridLength(0);
            if (AppSetting.IsNotMobile || (AppSetting.IsMobile && AppSetting.IsGlassEnable))
            {
                InitializedFrostedGlass(HambBackground, Colors.White, 50f);
                if(AppSetting.IsNotMobile) InitializedFrostedHostGlass(HeadBackgroungRec, (App.Current.Resources["SAC"] as SolidColorBrush).Color);
            }
            else
            {
                HambBackground.Fill = new SolidColorBrush(Colors.White);
            }
            base.OnNavigatedTo(e);
        }

        bool CanMove = true;

        double view
        {
            get
            {
                if (ScreenViewbox.ActualHeight != 0)
                {
                    return ScreenGrid.ActualHeight / ScreenViewbox.ActualHeight;
                }
                return 1;
            }
        }
        double imageview
        {
            get
            {
                if (ImageWidthHeight != null)
                {
                    var viewHeight = PhoneImageRec.ActualHeight / ImageWidthHeight.ImageHeight;
                    var viewWidth = PhoneImageRec.ActualWidth / ImageWidthHeight.ImageWidth;
                    return viewHeight > viewWidth ? viewHeight : viewWidth;
                }
                return 1;
            }
        }

        double imageview2
        {
            get
            {
                if (ImageWidthHeight != null)
                {
                    var viewHeight = PhoneImageRec.ActualHeight / ImageWidthHeight.ImageHeight;
                    var viewWidth = PhoneImageRec.ActualWidth / ImageWidthHeight.ImageWidth;
                    return viewHeight < viewWidth ? viewHeight : viewWidth;
                }
                return 1;
            }
        }
        WidthHeight ImageWidthHeight = null;

        private void Hamb_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            BlackRec.Visibility = Visibility.Visible;
        }

        private void Hamb_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (Window.Current.Bounds.Width < 800)
            {
                if (((HambGrid.RenderTransform as CompositeTransform).TranslateX += e.Delta.Translation.X) > 300)
                    (HambGrid.RenderTransform as CompositeTransform).TranslateX = 300;
                if ((HambGrid.RenderTransform as CompositeTransform).TranslateX < 0)
                    (HambGrid.RenderTransform as CompositeTransform).TranslateX = 0;
                BlackRec.Opacity = ((HambGrid.RenderTransform as CompositeTransform).TranslateX) / 600;
            }
        }

        private void Hamb_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (Math.Abs((HambGrid.RenderTransform as CompositeTransform).TranslateX) >= 100)
                OpenHambPanel.Begin();
        }

        private void BlackRec_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            CloseHambPanel.Begin();
        }

        private void HambButton_Click(object sender, RoutedEventArgs e)
        {
            BlackRec.Visibility = Visibility.Visible;
            OpenHambPanel.Begin();
        }

        private void InitializedFrostedGlass(UIElement glassHost, Color color, float Bluramount)
        {
            Visual hostVisual = ElementCompositionPreview.GetElementVisual(glassHost);
            Compositor compositor = hostVisual.Compositor;

            // Create a glass effect, requires Win2D NuGet package
            var glassEffect = new GaussianBlurEffect
            {
                BlurAmount = Bluramount,
                BorderMode = EffectBorderMode.Hard,
                Source = new ArithmeticCompositeEffect
                {
                    MultiplyAmount = 0,
                    Source1Amount = 0.5f,
                    Source2Amount = 0.5f,
                    Source1 = new CompositionEffectSourceParameter("backdropBrush"),
                    Source2 = new ColorSourceEffect
                    {
                        Color = color
                    }
                }
            };

            //  Create an instance of the effect and set its source to a CompositionBackdropBrush
            var effectFactory = compositor.CreateEffectFactory(glassEffect);
            var backdropBrush = compositor.CreateBackdropBrush();
            var effectBrush = effectFactory.CreateBrush();
            effectBrush.SetSourceParameter("backdropBrush", backdropBrush);

            // Create a Visual to contain the frosted glass effect
            var glassVisual = compositor.CreateSpriteVisual();
            glassVisual.Brush = effectBrush;

            // Add the blur as a child of the host in the visual tree
            ElementCompositionPreview.SetElementChildVisual(glassHost, glassVisual);

            // Make sure size of glass host and glass visual always stay in sync
            var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
            bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);

            glassVisual.StartAnimation("Size", bindSizeAnimation);
        }
        private void InitializedFrostedHostGlass(UIElement glassHost, Color color)
        {
            string sv = AnalyticsInfo.VersionInfo.DeviceFamilyVersion;
            ulong v = ulong.Parse(sv);
            ulong v1 = (v & 0xFFFF000000000000L) >> 48;
            ulong v2 = (v & 0x0000FFFF00000000L) >> 32;
            ulong v3 = (v & 0x00000000FFFF0000L) >> 16;
            ulong v4 = (v & 0x000000000000FFFFL);
            string version = $"{v1}.{v2}.{v3}.{v4}";
            System.Diagnostics.Debug.WriteLine(version);

            if (v3 > 14393 && AppSetting.IsNotMobile)
            {
                Visual hostVisual = ElementCompositionPreview.GetElementVisual(glassHost);
                Compositor compositor = hostVisual.Compositor;
                var colorEffect = new ArithmeticCompositeEffect
                {
                    MultiplyAmount = 0,
                    Source1Amount = 0.3f,
                    Source2Amount = 0.7f,
                    Source1 = new CompositionEffectSourceParameter("backdropBrush"),
                    Source2 = new ColorSourceEffect
                    {
                        Color = color
                    }
                };

                var effectFactory = compositor.CreateEffectFactory(colorEffect);
                var backdropBrush = compositor.CreateHostBackdropBrush();
                var effectBrush = effectFactory.CreateBrush();
                effectBrush.SetSourceParameter("backdropBrush", backdropBrush);

                var glassVisual = compositor.CreateSpriteVisual();
                glassVisual.Brush = effectBrush;

                ElementCompositionPreview.SetElementChildVisual(glassHost, glassVisual);

                var bindSizeAnimation = compositor.CreateExpressionAnimation("hostVisual.Size");
                bindSizeAnimation.SetReferenceParameter("hostVisual", hostVisual);

                glassVisual.StartAnimation("Size", bindSizeAnimation);
            }
        }

        private void HambListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var a = (sender as ListView).SelectedItem as MultipleColorDeviceModel;
            ColorPanel.ItemsSource = a;
            ColorPanel.SelectedIndex = 0;
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            Frame root = Window.Current.Content as Frame;
            if (root == null)
            {
                root = new Frame();
                Window.Current.Content = root;
            }
            root.Navigate(typeof(AboutPage));
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            BeWhite.Begin();
        }

        private void ColorPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectionChangedAnimation.Begin();
        }

        private void Image_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (ImageWidthHeight != null)
            {
                var transgroup = PhoneImageBrush.Transform as TransformGroup;
                var Compositetrans = transgroup.Children[0] as CompositeTransform;
                var Scaletrans = transgroup.Children[1] as ScaleTransform;
                if (Compositetrans != null && CanMove)
                {
                    if (Compositetrans.TranslateX + e.Delta.Translation.X < 0 &&
                        Compositetrans.TranslateX + e.Delta.Translation.X > PhoneImageRec.ActualWidth - ImageWidthHeight.ImageWidth * imageview)
                        Compositetrans.TranslateX += e.Delta.Translation.X * view;
                    if (Compositetrans.TranslateY + e.Delta.Translation.Y < 0 &&
                        Compositetrans.TranslateY + e.Delta.Translation.Y > PhoneImageRec.ActualHeight - ImageWidthHeight.ImageHeight * imageview)
                        Compositetrans.TranslateY += e.Delta.Translation.Y * view;
                    if (Compositetrans.TranslateX > 0) Compositetrans.TranslateX = 0;
                    if (Compositetrans.TranslateY > 0) Compositetrans.TranslateY = 0;

                    if (Compositetrans.TranslateX < PhoneImageRec.ActualWidth - ImageWidthHeight.ImageWidth * imageview)
                        Compositetrans.TranslateX = PhoneImageRec.ActualWidth - ImageWidthHeight.ImageWidth * imageview;
                    if (Compositetrans.TranslateY < PhoneImageRec.ActualHeight - ImageWidthHeight.ImageHeight * imageview)
                        Compositetrans.TranslateY = PhoneImageRec.ActualHeight - ImageWidthHeight.ImageHeight * imageview;
                }
            }
        }

        private void Image_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            e.TranslationBehavior.DesiredDeceleration = 1;
        }

        private void Image_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (ImageWidthHeight != null)
            {
                var transgroup = PhoneImageBrush.Transform as TransformGroup;
                var Compositetrans = transgroup.Children[0] as CompositeTransform;

                if (PhoneImageBrush.Stretch == Stretch.UniformToFill)
                {
                    PhoneImageBrush.Stretch = Stretch.Uniform;
                    PhoneImageBrush.AlignmentX = AlignmentX.Center;
                    PhoneImageBrush.AlignmentY = AlignmentY.Center;
                    Compositetrans.TranslateX = Compositetrans.TranslateY = 0;
                    CanMove = false;
                }
                else
                {
                    PhoneImageBrush.Stretch = Stretch.UniformToFill;
                    PhoneImageBrush.AlignmentX = AlignmentX.Left;
                    PhoneImageBrush.AlignmentY = AlignmentY.Top;
                    CanMove = true;
                }
            }
        }

        private async void MainGrid_Drop(object sender, DragEventArgs e)
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
                        PhoneImageBrush.ImageSource = bitmap;
                        Messenger.Default.Send(new WidthHeight(bitmap.PixelWidth, bitmap.PixelHeight), "ImageWidthHeight");
                    }
                }
            }
            finally
            {
                defer.Complete();
            }

        }

        private void MainGrid_DragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
            e.DragUIOverride.Caption = "拖放";
            e.DragUIOverride.IsContentVisible = false;
            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.IsContentVisible = true;
            e.Handled = true;
        }

        private async void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFile SF = await StorageFile.GetFileFromPathAsync(filePath);
                DataTransferManager.GetForCurrentView().DataRequested +=
                                (DataTransferManager s, DataRequestedEventArgs Dataargs) =>
                                {
                                    Dataargs.Request.Data.Properties.Title = "共享图像";
                                    Dataargs.Request.Data.Properties.Description = "共享以下图片。";
                                    Dataargs.Request.Data.SetBitmap(RandomAccessStreamReference.CreateFromFile(SF));
                                };
                DataTransferManager.ShowShareUI();
            }
            catch { }
            finally
            {
                LittlePhotoHide.Begin();
                LittlePhotoShow.Stop();
            }

        }

        private async void LittlePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                StorageFile SF = await StorageFile.GetFileFromPathAsync(filePath);
                await Windows.System.Launcher.LaunchFileAsync(SF);
            }
            catch { }
            finally
            {
                LittlePhotoHide.Begin();
                LittlePhotoShow.Stop();
            }
        }

        private void AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)(sender as AppBarToggleButton).IsChecked)
            {
                ShowEditToolBar.Begin();
                MyInkCanvas.InkPresenter.IsInputEnabled = true;
            }
            else
            {
                HideEditToolBar.Begin();
                MyInkCanvas.InkPresenter.IsInputEnabled = false;
            }
        }

        private void EditToolBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSetting.IsNotMobile || AppSetting.IsMobile && AppSetting.IsGlassEnable)
            {
                InitializedFrostedGlass(EditToolBarBackground, Colors.White, 5f);
            }
            else
            {
                EditToolBarBackground.Fill = new SolidColorBrush(Colors.White);
            }
        }

        private void UpImage_Click(object sender, RoutedEventArgs e)
        {
            if (IMAGE.SelectedIndex - 1 > 0 && IMAGE.Items[IMAGE.SelectedIndex - 1] as BitmapListModel != null)
                (IMAGE.Items[IMAGE.SelectedIndex - 1] as BitmapListModel).ClipHeight--;
        }

        private void DownImage_Click(object sender, RoutedEventArgs e)
        {
            if (IMAGE.SelectedIndex - 1 > 0 && IMAGE.Items[IMAGE.SelectedIndex - 1] as BitmapListModel != null)
                (IMAGE.Items[IMAGE.SelectedIndex - 1] as BitmapListModel).ClipHeight++;
        }

        private void UpRepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ChangeAllImageToggle.IsChecked)
            {
                try
                {
                    var head = IMAGEHead.Items?.First() as BitmapListModel;
                    if (head != null)
                    {
                        if (head.ClipHeight < 0) head.ClipHeight = 0;
                        else head.ClipBottom--;

                    }
                    foreach (BitmapListModel Item in IMAGE.Items)
                    {
                        if (Item.ClipTop < 0) Item.ClipTop = 0;
                        else Item.ClipTop--;
                    }
                }
                catch { }
            }
            else if (_bitmap != null)
            {
                if (_bitmap.ClipHeight < 0) _bitmap.ClipHeight = 0;
                else _bitmap.ClipHeight--;
            }
        }

        private void DownRepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ChangeAllImageToggle.IsChecked)
            {
                try
                {
                    var head = IMAGEHead.Items?.First() as BitmapListModel;
                    if (head != null)
                    {
                        if (head.ClipHeight > head.Height) head.ClipHeight = head.Height;
                        else head.ClipBottom++;

                    }
                    foreach (BitmapListModel Item in IMAGE.Items)
                    {
                        if (Item.ClipTop > Item.Height) Item.ClipTop = Item.Height;
                        else Item.ClipTop++;
                    }
                }
                catch { }
            }
            else if (_bitmap != null)
            {
                if (_bitmap.ClipHeight > _bitmap.Height) _bitmap.ClipHeight = _bitmap.Height;
                else _bitmap.ClipHeight++;
            }
        }

        private void ClearAllInk_Click(object sender, RoutedEventArgs e)
        {
            MyInkCanvas.InkPresenter.StrokeContainer.Clear();
        }

        private void DeleteButtonBackground_Loaded(object sender, RoutedEventArgs e)
        {
            if (AppSetting.IsNotMobile || AppSetting.IsMobile && AppSetting.IsGlassEnable)
            {
                InitializedFrostedGlass((sender as Rectangle), Colors.White, 5f);
            }
            else
            {
                (sender as Rectangle).Fill = new SolidColorBrush(Colors.White);
            }
        }

        private void HambShotList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            double _scr = ShotViewbox.ActualHeight / IMAGE.Items.Count * IMAGE.SelectedIndex - ShotViewbox.ActualHeight / IMAGE.Items.Count / 3;
            IMAGEScroll.ChangeView(null, _scr > 0 ? _scr : 0, null);
            var _listview = (sender as ListView);
            if (_listview.SelectionMode == ListViewSelectionMode.Single && _listview.SelectedIndex > 0)
            {
                UpImage.IsEnabled = true;
                DownImage.IsEnabled = true;
            }
            else if (_listview.SelectionMode != ListViewSelectionMode.None)
            {
                UpImage.IsEnabled = false;
                DownImage.IsEnabled = false;
            }

        }

        private void ChangeAllImageToggle_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)ChangeAllImageToggle.IsChecked)
            {
                IMAGEHead.SelectionMode = ListViewSelectionMode.Multiple;
                IMAGEBottom.SelectionMode = ListViewSelectionMode.Multiple;
                IMAGE.SelectionMode = ListViewSelectionMode.Multiple;
                HambShotList.SelectionMode = ListViewSelectionMode.None;
                IMAGEHead.SelectAll();
                IMAGEBottom.SelectAll();
                IMAGE.SelectAll();
                UpImage.IsEnabled = true;
                DownImage.IsEnabled = true;
            }
            else
            {
                IMAGEHead.SelectionMode = ListViewSelectionMode.Single;
                IMAGEBottom.SelectionMode = ListViewSelectionMode.Single;
                IMAGE.SelectionMode = ListViewSelectionMode.Single;
                HambShotList.SelectionMode = ListViewSelectionMode.Single;
                IMAGEHead.SelectedIndex = -1;
                IMAGEBottom.SelectedIndex = -1;
                IMAGE.SelectedIndex = -1;
                HambShotList.SelectedIndex = -1;
                UpImage.IsEnabled = false;
                DownImage.IsEnabled = false;
            }
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            UpImage.IsEnabled = false;
            DownImage.IsEnabled = false;
            ChangeAllImageToggle.IsEnabled = false;
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            ChangeAllImageToggle.IsEnabled = true;
        }

        private void UpImage_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (UpImage.IsEnabled)
                Messenger.Default.Send(1, "ShowHideBorder");
            else
                Messenger.Default.Send(0, "ShowHideBorder");
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (AppSetting.IsGlassEnable && AppSetting.IsNotMobile)
            {
                if (Window.Current.Bounds.Width > 800)
                {
                    InitializedFrostedHostGlass(HambBackground, Colors.White);
                }
                else
                {
                    InitializedFrostedGlass(HambBackground, Colors.White, 50f);
                }
            }

        }




        //private async Task SwitchPhonePhoto(int Count)
        //{
        //    SwitchPhonePhotoOne.Begin();
        //    PhoneImage.Source = await LoadImage(DM.PhoneList[Count].PhoneScreen);
        //    PhoneGlare.Source = await LoadImage(DM.PhoneList[Count].PhoneGlare);
        //    PhoneScreen.Height = DM.PhoneList[Count].ScreenHeight == "" ? 0.0 : System.Convert.ToDouble(DM.PhoneList.First().ScreenHeight);
        //    PhoneScreen.Width = DM.PhoneList[Count].ScreenHeight == "" ? 0.0 : System.Convert.ToDouble(DM.PhoneList.First().ScreenWidth);
        //    PhoneScreen.Margin = DM.PhoneList[Count].PhoneMargin;
        //    SwitchPhonePhotoTwo.Begin();
        //}

        //private async void Browser_Click(object sender, RoutedEventArgs e)
        //{
        //    FileOpenPicker FOP = new FileOpenPicker();
        //    FOP.FileTypeFilter.Add(".jpg");
        //    FOP.FileTypeFilter.Add(".png");
        //    FOP.FileTypeFilter.Add(".jpeg");

        //    FOP.ViewMode = PickerViewMode.Thumbnail;
        //    StorageFile PhotoFile = await FOP.PickSingleFileAsync();
        //    try
        //    {
        //        using (IRandomAccessStream stream = await PhotoFile.OpenReadAsync())
        //        {
        //            BitmapImage Image = new BitmapImage();
        //            BitmapEncoder BE = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
        //            Image.SetSource(stream);
        //            PhoneScreen.Source = Image;
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}


        //private async void Save_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {

        //        BeWhite.Begin();
        //        (sender as Button).IsEnabled = false;

        //        StorageFolder WorkFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("带壳截图", CreationCollisionOption.OpenIfExists);
        //        StorageFile file = await SaveImage(WorkFolder, ScreenGrid,
        //            ((bool)ApplicationData.Current.LocalSettings.Values["IsWhiteBackgroundToggleSwitchIsOn"]) ?
        //            BitmapEncoder.JpegEncoderId : BitmapEncoder.PngEncoderId);
        //        if (FirstClick && (AnalyticsInfo.VersionInfo.DeviceFamily != "Windows.Mobile"))
        //        {
        //            FirstClick = false;
        //            await file.DeleteAsync();
        //            file = await SaveImage(WorkFolder, ScreenGrid,
        //                ((bool)ApplicationData.Current.LocalSettings.Values["IsWhiteBackgroundToggleSwitchIsOn"]) ?
        //                BitmapEncoder.JpegEncoderId : BitmapEncoder.PngEncoderId);
        //        }

        //        string xml = "<toast lang=\"zh-CN\">" +
        //                        "<visual>" +
        //                            "<binding template=\"ToastGeneric\">" +
        //                                "<text>带壳截图</text>" +
        //                                "<text>点击打开</text>" +
        //                                "<image placement=\"inline\" src=\"" + file.Path + "\" />" +
        //                            "</binding>" +
        //                        "</visual>" +
        //                        "<actions>" +
        //                            "<action content=\"打开\" arguments=\"Open\" />" +
        //                            "<action content=\"共享\" arguments=\"Share\" />" +
        //                            "" +
        //                        "</actions>" +
        //                     "</toast>";

        //        (sender as Button).IsEnabled = true;
        //        ToastNotificationManager.History.Clear();
        //        Windows.Data.Xml.Dom.XmlDocument doc = new Windows.Data.Xml.Dom.XmlDocument();
        //        // 加载XML
        //        doc.LoadXml(xml);
        //        // 创建通知实例
        //        ToastNotification notification = new ToastNotification(doc);
        //        notification.Activated += Notification_Activated;
        //        // 显示通知
        //        ToastNotifier nt = ToastNotificationManager.CreateToastNotifier();
        //        nt.Show(notification);

        //    }
        //    catch (Exception ex) { (sender as Button).IsEnabled = true; }
        //}

        //private async void Notification_Activated(ToastNotification sender, object args)
        //{
        //    ToastActivatedEventArgs a = args as ToastActivatedEventArgs;
        //    if(a.Arguments == null)
        //    {
        //        string FILEPATH = ApplicationData.Current.LocalSettings.Values["FILEPATH"] as string;
        //        StorageFile SF = await StorageFile.GetFileFromPathAsync(FILEPATH);
        //        await Windows.System.Launcher.LaunchFileAsync(SF);
        //    }

        //}

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    OnNavigatedModel ONM = e.Parameter as OnNavigatedModel;
        //    try
        //    {
        //        if (ONM.Mode == "Share")
        //            PhoneScreen.Source = ONM.E as BitmapImage;
        //    }catch { }

        //    Frame rootFrame = Window.Current.Content as Frame;


        //    if (rootFrame.CanGoBack)
        //    {
        //        // Show UI in title bar if opted-in and in-app backstack is not empty. 
        //        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        //    }
        //    else
        //    {
        //        // Remove the UI from the title bar if in-app back stack is empty. 
        //        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        //    }

        //    base.OnNavigatedTo(e);
        //}

        //private void HambListView_Loaded(object sender, RoutedEventArgs e)
        //{
        //    (sender as ListView).UpdateLayout();
        //    try
        //    {
        //        (sender as ListView).SelectedIndex = (int)ApplicationData.Current.LocalSettings.Values["PhoneListCount"];
        //    }
        //    catch
        //    {

        //    }
        //}


        //private async void HambListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if ((sender as ListView).SelectedIndex != -1)
        //    {
        //        ApplicationData.Current.LocalSettings.Values["PhoneListCount"] = (sender as ListView).SelectedIndex;
        //        DM = DR.MultipleColorDevicesList[(sender as ListView).SelectedIndex];
        //        ColorPanel.ItemsSource = DM.PhoneList;
        //        ColorPanel.UpdateLayout();
        //        ColorPanel.SelectedIndex = 0;
        //        await SwitchPhonePhoto(0);
        //        ColorGridViewButton.IsEnabled = ((ColorPanel.ItemsSource as List<DeviceModel>).Count == 1) ? false : true;
        //    }
        //}

        //private void AboutButton_Click(object sender, RoutedEventArgs e)
        //{
        //    FirstOpen = false;
        //    Frame rootFrame = Window.Current.Content as Frame;
        //    rootFrame.Navigate(typeof(AboutPage));
        //}

        //private async void ColorPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    int a = (sender as GridView).SelectedIndex;
        //    if(a >= 0) await SwitchPhonePhoto(a);
        //}
    }
}
