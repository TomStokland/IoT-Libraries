using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using Windows.Storage.Streams;

public class I2C
{
    public byte Address { get; set; } = 0x40;
    public I2cBusSpeed Speed { get; set; } = I2cBusSpeed.StandardMode;
    public I2cDevice device;

    public I2C(byte address, I2cBusSpeed speed)
	{
        Initialize(address, speed);
	}

    public void Initialize(byte address = 0x40, I2cBusSpeed speed = I2cBusSpeed.StandardMode)
    {
        //MainPage rootPage = MainPage.Current;

        DeviceInformationCollection devices = null;
        Address = address;
        Speed = speed;

        //await rootPage.uart.SendText("I2C Device at Address " + address.ToString() + " Initializing\n\r");

        Task.Run(async () =>
        {

            var settings = new I2cConnectionSettings(Address) { BusSpeed = Speed };
            settings.SharingMode = I2cSharingMode.Shared;

            // Get a selector string that will return all I2C controllers on the system 
            string aqs = I2cDevice.GetDeviceSelector();

            // Find the I2C bus controller device with our selector string
            devices = await DeviceInformation.FindAllAsync(aqs);

            //search for the controller
            if (!devices.Any())
                throw new IOException("No I2C controllers were found on the system");

            //see if we can find the hat
            device = await I2cDevice.FromIdAsync(devices[0].Id, settings);

        }).Wait();

        //await rootPage.uart.SendText("I2C Device at Address " + address.ToString() + " Initialized\n\r");
    }

    public int Write(byte[] dataOut)
    {
        int success = 0;
        try
        {
            device.Write(dataOut);
        }
        catch(Exception e)
        {
            success = e.HResult;
        }

        return success;
    }

    public byte[] WriteRead(byte[] dataOut, int numberOfBytesToRead)
    {
        byte[] dataIn = new byte[numberOfBytesToRead];
        int success = 0;

        try
        {
            device.WriteRead(dataOut, dataIn);
        }
        catch (Exception e)
        {
            success = e.HResult;
            return null;
        }

        return dataIn;
    }

    public int WriteReadPartial(byte[] dataOut, int numberOfBytesToRead)
    {
        byte[] dataIn = new byte[numberOfBytesToRead];
        int success = 0;
        I2cTransferResult result = new I2cTransferResult();

        try
        {
            result = device.WriteReadPartial(dataOut, dataIn);
        }
        catch (Exception e)
        {
            success = e.HResult;
        }

        return success;
    }
}
