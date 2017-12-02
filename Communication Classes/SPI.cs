using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Spi;
using Windows.Devices.Enumeration;

namespace Communications
{
    public class SPI
    {
        SpiDevice spiDevice;

        public SPI(Int32 chipSelect = 0, Int32 speed = 10000000, SpiMode mode = SpiMode.Mode3, SpiSharingMode sharingMode = SpiSharingMode.Shared)
        {
            Initialize(chipSelect, speed, mode, sharingMode);
        }

        private async void Initialize(Int32 chipSelect, Int32 speed, SpiMode mode, SpiSharingMode sharingMode = SpiSharingMode.Shared)
        {
            DeviceInformation devInfo = null;
            try
            {
                var settings = new SpiConnectionSettings(chipSelect);
                settings.ClockFrequency = speed;
                settings.Mode = mode;
                settings.SharingMode = sharingMode;

                try
                {
                    string aqs = SpiDevice.GetDeviceSelector();                     // Get a selector string that will return all SPI controllers on the system
                    var dis = await DeviceInformation.FindAllAsync(aqs);            // Find the SPI bus controller devices with our selector string
                    spiDevice = await SpiDevice.FromIdAsync(dis[0].Id, settings);   // Create an SpiDevice with our bus controller and SPI settings
                    devInfo = dis[0];
                }
                catch (Exception e)
                {
                    throw new Exception("SPI Initialization failed. Exception: " + e.Message + "\n\r");
                }

                if (spiDevice == null)
                {
                    throw new Exception("SPI Controller is currently in use by " +
                        "another application. Please ensure that no other applications are using SPI.");
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("SPI Initialization failed. Exception: " + ex.Message + "\n\r");
            }

        }

        public void SetChipSelect(Int32 chipSelect) => spiDevice.ConnectionSettings.ChipSelectLine = chipSelect;
        public void SetSpeed(Int32 speed) => spiDevice.ConnectionSettings.ClockFrequency = speed;
        public void SetMode(SpiMode mode) => spiDevice.ConnectionSettings.Mode = mode;
        public void SetSharingMode(SpiSharingMode sharingMode) => spiDevice.ConnectionSettings.SharingMode = sharingMode;

        public byte[] Read(int numberOfBytesToRead)
        {
            byte[] readData = new byte[numberOfBytesToRead];

            spiDevice.Read(readData);

            return readData;
        }

        public byte[] TransferFullDuplex(byte[] writeData, int numberOfBytesToRead)
        {
            byte[] readData = new byte[numberOfBytesToRead];

            spiDevice.TransferFullDuplex(writeData, readData);

            return readData;
        }

        public byte[] Transfersequential(byte[] writeData, int numberOfBytesToRead)
        {
            byte[] readData = new byte[numberOfBytesToRead];

            spiDevice.TransferSequential(writeData, readData);

            return readData;
        }

        public void Write(byte[] writeData)
        {
            spiDevice.Write(writeData);
        }

    }
}