using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Windows.Storage;
using ShotWithPhone.Model;
using System.Collections.ObjectModel;
using Windows.ApplicationModel;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace ShotWithPhone
{
    public class DevicesReader : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName]string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public bool FirstLoad = true;

        public async Task<ObservableCollection<DeviceModel>> GetJson()
        {
            StorageFile JsonFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Devices.json"));
            string JsonText = await FileIO.ReadTextAsync(JsonFile);
            var JSON = JsonConvert.DeserializeObject(JsonText);
            var a = ((JObject)JSON)["PhoneList"]["Phone"];
            return JsonConvert.DeserializeObject<ObservableCollection<DeviceModel>>(a.ToString());
        }

        public void GroupPhoneList(ObservableCollection<DeviceModel> DevicesList, ObservableCollection<DeviceOEMModel> DevicesOEMList)
        {
            var ColorList = (from item in DevicesList
                             group item by item.PhoneName into NewColorGrop
                             select NewColorGrop).ToList();

            ObservableCollection<MultipleColorDeviceModel> MultipleColorDevicesList = new ObservableCollection<MultipleColorDeviceModel>();

            for (int i = 0; i < ColorList.Count; i++)
            {
                MultipleColorDeviceModel MultipleColorDevice = new MultipleColorDeviceModel();
                foreach (var item in ColorList[i])
                {
                    MultipleColorDevice.Add(item);
                }
                MultipleColorDevice.PhoneName = ColorList[i].First().PhoneName;
                MultipleColorDevice.OEM = ColorList[i].First().PhoneOEM;
                MultipleColorDevicesList.Add(MultipleColorDevice);
            }

            var OEMList = (from item in MultipleColorDevicesList
                           group item by item.OEM into NewOEMGroup
                           select NewOEMGroup).ToList();

            for (int i = 0; i < OEMList.Count; i++)
            {
                DeviceOEMModel DeviceOEM = new DeviceOEMModel();
                foreach (var item in OEMList[i])
                {
                    DeviceOEM.DevicesList.Add(item);
                }
                DeviceOEM.OEM = OEMList[i].First().OEM;
                DevicesOEMList.Add(DeviceOEM);
            }
        }



    }
}



//{
//  "PhoneOEM": "Apple",
//  "PhoneName": "iPhone6白色",
//  "Color": "White",
//  "ColorStr": "白色",
//  "PhoneGlare": "/PhoneImageSources/res_id_apple_iphone6_portrait_white-portrait_2d-glare.webp.png",
//  "PhoneScreen": "/PhoneImageSources/res_id_apple_iphone6_portrait_white-portrait_2d-screen.webp.png",
//  "MarginLeft": "0",
//  "MarginTop": "0",
//  "MarginRight": "0",
//  "MarginBottom": "0",
//  "ScreenWidth": "720",
//  "ScreenHeight": "1280"
//},