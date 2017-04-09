using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace ShotWithPhone
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        bool FirstOpen = true;
        int PressCount;
        public AboutPage()
        {
            this.InitializeComponent();
            if(FirstOpen)
            {
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += AboutPage_BackRequested;
                CloseAboutPage.Completed += CloseAboutPage_Completed;
            }
            PressCount = 0;
            var VersionText = Windows.ApplicationModel.Package.Current.Id.Version;
            Version.Text = "Version:" + string.Format("{0}.{1}.{2}.{3}", VersionText.Major, VersionText.Minor, VersionText.Build, VersionText.Revision);
        }

        private void CloseAboutPage_Completed(object sender, object e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack) rootFrame.GoBack();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }

        private void AboutPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
                return;
            if (rootFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                FirstOpen = false;
                rootFrame.GoBack();
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;


            if (rootFrame.CanGoBack)
            {
                // Show UI in title bar if opted-in and in-app backstack is not empty. 
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                // Remove the UI from the title bar if in-app back stack is empty. 
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }

        private void Grid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if ((Math.Abs((MainGrid.RenderTransform as CompositeTransform).TranslateX) >= Window.Current.Bounds.Width/3))
            {
                FirstOpen = false;
                ((DoubleAnimation)CloseAboutPage.Children[0]).To = Window.Current.Bounds.Width;
                CloseAboutPage.Begin();
            }
                
            else OpenAboutPage.Begin();
        }

        private void Grid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (((MainGrid.RenderTransform as CompositeTransform).TranslateX += e.Delta.Translation.X) > MainGrid.Width)
                (MainGrid.RenderTransform as CompositeTransform).TranslateX = MainGrid.Width;
            if ((MainGrid.RenderTransform as CompositeTransform).TranslateX < 0)
                (MainGrid.RenderTransform as CompositeTransform).TranslateX = 0;
        }
        
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            ((PaneThemeTransition)Transitions[0]).Edge = e.NavigationMode != NavigationMode.Back ? EdgeTransitionLocation.Left : EdgeTransitionLocation.Right;
            base.OnNavigatingFrom(e);
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            try { TagTextBlock.Visibility = Visibility.Visible; }
            catch { }
        }

        private void MePointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (PressCount == 5)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                rootFrame.Navigate(typeof(TestPhonePage));
            }
            else PressCount++;
        }
    }
}
