using StressTestConDisJai;
using System.Diagnostics;

DeviceJai device1 = new();
//DeviceJai device2 = new();
//DeviceJai device3 = new();

int increment = 0;

while (increment < 100)
{
    device1.ConnectingCamera("169.254.0.1", message => Debug.WriteLine($"{message} device1"));
    //device2.ConnectingCamera("169.254.0.2", message => Debug.WriteLine($"{message} device2"));
    //device3.ConnectingCamera("169.254.0.3", message => Debug.WriteLine($"{message} device3"));

    device1.DisconnectingCamera(message => Debug.WriteLine($"{message} device1"));
    //device2.DisconnectingCamera(message => Debug.WriteLine($"{message} device2"));
   //device3.DisconnectingCamera(message => Debug.WriteLine($"{message} device3"));
    increment++;
}

Console.ReadKey();
