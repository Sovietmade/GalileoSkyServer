using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GalileoSkyServer
{
    public class GalileoSkyData
    {
        public Int32 Tag { get; set; }

        public object Data{ get; set; }

        public Type TypeOfData{ get; set; }
    }



    public class HardwareVersion
    {
        public HardwareVersion()
        { 
        
        }

        public HardwareVersion(UInt32 inHW)
        {
            HW = inHW;
        }

        public UInt32 HW { get; set; }

    }

    public class SoftwareVersion
    {
        public SoftwareVersion()
        {

        }

        public SoftwareVersion(UInt32 inSW)
        {
            SW = inSW;
        }

        public UInt32 SW { get; set; }
    }

    public class ImeiData
    {
        public ImeiData()
        {

        }

        public ImeiData(String inIMEI)
        {
            IMEI = inIMEI;
        }
        public String IMEI { get; set; }
    }

    public class TerminalID
    {
        public TerminalID()
        { 
        
        }

        public TerminalID(UInt32 inTerminalID)
        {
            TerminalIDData = inTerminalID;
        }

        public UInt32 TerminalIDData { get; set; }
    }

    public class PackageNumer
    {
        public PackageNumer()
        {

        }

        public PackageNumer(UInt32 inPackageNumer)
        {
            PackageNumerData = inPackageNumer;
        }

        public UInt32 PackageNumerData { get; set; }
    }

    public class DateAndTime
    {
        public DateAndTime()
        {

        }

        public DateAndTime(UInt32 inDateAndTime)
        {
            DateAndTimeData = inDateAndTime;
        }

        public UInt32 DateAndTimeData { get; set; }
        public DateTime ActualDateAndTime
        {
            get {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(DateAndTimeData);
            }
        }
    }

    public class CoordinatesAndSatellites
    {
        public CoordinatesAndSatellites()
        {

        }

        public UInt32 SatellitesAmount { get; set; }
        public UInt32 Correctness { get; set; }
        public Int32 Latitude { get; set;}
        public Int32 Longitude { get; set; }

        public override string ToString()
        {
            return String.Format("Satellites: {0}, Correctness: {1}, Latitude: {2}, Longitude: {3}", SatellitesAmount, Correctness, Latitude, Longitude);
        }

    }

    public class SpeedAndDirection
    {
        public SpeedAndDirection()
        {

        }

        public UInt32 Speed { get; set; }
        public UInt32 Direction { get; set; }
    

        public override string ToString()
        {
            return String.Format("Speed: {0}, Direction: {1}", Speed, Direction);
        }

    }

    public class Height
    {
        public Height()
        {

        }

        public Height(Int32 inHeightData)
        {
            HeightData = inHeightData;
        }

        public Int32 HeightData { get; set; }

        public override string ToString()
        {
            return String.Format("Height: {0}", HeightData);
        }

    }

    public class HDOP
    {
        public HDOP()
        {

        }

        public HDOP(UInt32 inHDOPData)
        {
            HDOPData = inHDOPData;
        }

        public UInt32 HDOPData { get; set; }

        public override string ToString()
        {
            return String.Format("HDOP: {0}", HDOPData);
        }

    }

    public class DeviceStatus
    {
        public DeviceStatus()
        {

        }

        public DeviceStatus(UInt16 inDeviceStatus)
        {
            
        }

        public DeviceStatus(byte[] inDeviceStatus)
        {
            if (inDeviceStatus.Length != 2)
            {

            }

            else 
            {
                BitArray ba = new BitArray(inDeviceStatus);
                Movement = ba.Get(0);
                Slope = ba.Get(1);
                iButton = ba.Get(2);
                SIM = ba.Get(3);
                GeoZone = ba.Get(4);
                InnerPowerSupplyVoltage = ba.Get(5);
                GPSAntenna = ba.Get(6);
                InnerPowerSupplyBusVoltage = ba.Get(7);
                OuterVoltage = ba.Get(8);
                CarStatus = ba.Get(9);
                Hit = ba.Get(10);
                GpsOrGlonass = ba.Get(11);
                Signaling = ba.Get(14);
                Alarm = ba.Get(15);

                byte bSignalQuality = 0;
                if (ba.Get(12))
                {
                    bSignalQuality = (byte)(bSignalQuality ^ 1);
                }
                if (ba.Get(13))
                {
                    bSignalQuality = (byte)(bSignalQuality ^ (1 << 1));
                }
                SignalQuality = bSignalQuality;
            }
        }

        public bool Movement { get; set; }
        public bool Slope { get; set; }
        public bool iButton { get; set; }
        public bool SIM { get; set; }
        public bool GeoZone { get; set; }
        public bool InnerPowerSupplyVoltage { get; set; }
        public bool GPSAntenna { get; set; }
        public bool InnerPowerSupplyBusVoltage { get; set; }
        public bool OuterVoltage { get; set; }
        public bool CarStatus { get; set; }
        public bool Hit { get; set; }
        public bool GpsOrGlonass { get; set; }
        public Byte SignalQuality { get; set; }
        public bool Signaling { get; set; }
        public bool Alarm { get; set; }


        public override string ToString()
        {
            return "Device status: " +
                " Movement: " + (Movement ? "1" : "0") +
                " Slope: " + (Slope ? "1" : "0") +
                " iButton: " + (iButton ? "1" : "0") +
                " SIM: " + (SIM ? "1" : "0") +
                " GeoZone: " + (GeoZone ? "1" : "0") +
                " InnerPowerSupplyVoltage: " + (InnerPowerSupplyVoltage ? "1" : "0") +
                " GPSAntenna: " + (GPSAntenna ? "1" : "0") +
                " InnerPowerSupplyBusVoltage: " + (InnerPowerSupplyBusVoltage ? "1" : "0") +
                " OuterVoltage: " + (InnerPowerSupplyBusVoltage ? "1" : "0") +
                " CarStatus: " + (InnerPowerSupplyBusVoltage ? "1" : "0") +
                " Hit: " + (InnerPowerSupplyBusVoltage ? "1" : "0") +
                " GpsOrGlonass: " + (InnerPowerSupplyBusVoltage ? "1" : "0") +
                " SignalQuality: " + SignalQuality +
                " Signaling: " + (Signaling ? "1" : "0") +
                " Alarm: " + (Alarm ? "1" : "0");
        }

    }

}
