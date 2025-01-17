﻿using irsdkSharp.Serialization;
using irsdkSharp.Serialization.Models.Session;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace irsdkSharp.ConsoleTest
{
    class Program
    {
        private static IRacingSDK sdk;
        private static IRacingSessionModel _session;
        private static int _DriverId = -1;
        private static int _lastUpdate = -1;
        static void Main(string[] args)
        {
            sdk = new IRacingSDK();
            sdk.OnDataChanged += Sdk_OnDataChanged;
            Console.ReadLine();
        }

        private static void Sdk_OnDataChanged(object sender, EventArgs e)
        {
            var currentlyConnected = sdk.IsConnected();
            if (currentlyConnected)
            {
                // Is the session info updated?
                int newUpdate = sdk.Header.SessionInfoUpdate;
                if (newUpdate != _lastUpdate)
                {
                    _lastUpdate = newUpdate;
                    _session = sdk.GetSerializedSessionInfo();
                }

                if (_DriverId == -1)
                {
                    _DriverId = (int)sdk.GetData("PlayerCarIdx");
                    Console.WriteLine(_DriverId);
                }

                var data = sdk.GetSerializedData();

                if (data != null && _session != null)
                {
                    Console.SetCursorPosition(0, 0);


                    foreach (var car in data.Data.Cars.OrderByDescending(x => x.CarIdxLap).ThenByDescending(x => x.CarIdxLapDistPct))
                    {
                        var currentData = _session.DriverInfo.Drivers.Where(y => y.CarIdx == car.CarIdx).FirstOrDefault();
                        if (currentData != null && car.CarIdxEstTime != 0)
                        {
                            Console.WriteLine($"{currentData.CarNumber}\t{string.Format("{0:0.00}", car.CarIdxEstTime)}\t{string.Format("{0:0.00}", car.CarIdxLapDistPct * 100)}");
                        }

                    }
                }
            }
        }

    }
}