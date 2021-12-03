using PvDotNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StressTestConDisJai
{
    internal class DeviceJai
    {
        private PvDevice? mDevice = null;
        private PvStream? mStream = null;

        bool CheckIsConnected = false;

        // Enum ExposureAuto
        enum ExposureAuto
        {
            Off,
            Continuous
        }

        int increment = 0;

        public void ConnectingCamera(string device, Action<string> message)
        {
            // If you connect and disconnect multiple times, we get a memory leak.
            if (!CheckIsConnected)
            {
                try
                {
                    // When requesting by ip address if more than one network interface there will be errors.
                    // This method of request by ip works with only one network interface.
                    mDevice = PvDevice.CreateAndConnect(device);

                    // Create and Open stream.
                    mStream = PvStream.CreateAndOpen(device);

                    CheckIsConnected = false;
                }
                catch (PvException ex)
                {
                    DisconnectingCamera(message => Debug.WriteLine(message));
                    Debug.WriteLine(ex);
                    message(ex.Message);
                    return;
                }

/*                increment++;
                message($"Connectiong Device {increment}");*/

                Configuring();
            }
            increment++;
            message($"Connectiong Device {increment}");
        }

        private void Configuring()
        {
            try
            {
                // Perform GigE Vision only configuration
                PvDeviceGEV? lDGEV = mDevice as PvDeviceGEV;
                if (lDGEV != null)
                {
                    // Negotiate packet size
                    lDGEV.NegotiatePacketSize();

                    // Set stream destination.
                    PvStreamGEV? lSGEV = mStream as PvStreamGEV;
                    lDGEV.SetStreamDestination(lSGEV.LocalIPAddress, lSGEV.LocalPort);

                    CameraSetParameters();
                }
            }
            catch (PvException ex)
            {
                DisconnectingCamera(message => Debug.WriteLine(message));
                Debug.WriteLine(ex.Message);
                //Close();
            }
        }

        // Set camera parameters
        private void CameraSetParameters()
        {
            Debug.WriteLine("Start configutation");
            // Get camera parameters for debugging 
            PvGenParameter getAcquisitionFrameRate = mDevice.Parameters.Get("AcquisitionFrameRate");
            PvGenParameter getWidth = mDevice.Parameters.Get("Width");
            PvGenParameter getHeight = mDevice.Parameters.Get("Height");

            // Constant parameters for Line camera.
            const float fps = 30.00f;
            const int width = 1280;
            const int height = 1;

            // Set BinningHorizontal
            // Set BinningVertical
            // Before we change resolution!
            mDevice.Parameters.SetIntegerValue("BinningHorizontal", 2);
            mDevice.Parameters.SetIntegerValue("BinningVertical", 2);
            mDevice.Parameters.SetIntegerValue("Width", width);
            mDevice.Parameters.SetIntegerValue("Height", height);
            mDevice.Parameters.SetFloatValue("AcquisitionFrameRate", fps);
            mDevice.Parameters.SetEnumValue("ExposureAuto", (long)ExposureAuto.Off);
            mDevice.Parameters.SetIntegerValue("OffsetY", 512);

            Debug.WriteLine("End configutation");
        }

        public void DisconnectingCamera(Action<string> message)
        {
            if (mStream != null)
            {
                // Close and release stream
                mStream.Close();
                mStream = null;
            }

            if (mDevice != null)
            {
                // Disconnect and release device
                mDevice.Disconnect();
                mDevice.Dispose();
                //mDevice = null;
            }
            message($"Disconnect Device {increment}");
        }
    }
}
