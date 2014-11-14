using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GalileoSkyServer
{
    static class ByteArrayToStringConv
    {
        public static string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }

    public class GalileoSkyData
    {
        public byte Tag { get; set; }

        public object Data{ get; set; }

        public Type TypeOfData{ get; set; }

        public byte[] ToByteArray()
        {
            return TypeOfData != null ? (byte[])TypeOfData.GetMethod("ToByteArray").Invoke(Data, null) : null;
        }
    }

    public class CommandID
    { 
        public CommandID()
        { 
            
        }

        public CommandID(UInt32 inCommandID)
        {
            CommandIDDData = inCommandID;
        }

        public UInt32 CommandIDDData { get; set; }

        public byte[] ToByteArray()
        {
            return BitConverter.GetBytes(CommandIDDData);
        }
    }

    public class Command
    {
        public Command()
        {

        }

        public byte CommandLength { get; set; }
        public String CommandData { get; set; }

        public byte[] ToByteArray()
        {
            byte[] asByteArray = new byte[CommandLength + 1];
            asByteArray[0] = CommandLength;

            Array.Copy(Encoding.ASCII.GetBytes(CommandData), 0, asByteArray, 1, CommandLength);
            return asByteArray;
        }

    }

    public class HardwareVersion
    {
        public HardwareVersion()
        { 
        
        }

        public HardwareVersion(byte inHW)
        {
            HW = inHW;
        }

        public byte HW { get; set; }

        public byte[] ToByteArray()
        {
            return new byte[1] { HW };
        }

    }

    public class SoftwareVersion
    {
        public SoftwareVersion()
        {

        }

        public SoftwareVersion(byte inSW)
        {
            SW = inSW;
        }

        public byte SW { get; set; }

        public byte[] ToByteArray()
        {
            return new byte[1] { SW };
        }
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

        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(IMEI);
        }
    }

    public class TerminalID
    {
        public TerminalID()
        { 
        
        }

        public TerminalID(UInt16 inTerminalID)
        {
            TerminalIDData = inTerminalID;
        }

        public UInt16 TerminalIDData { get; set; }

        public byte[] ToByteArray()
        {
            return BitConverter.GetBytes(TerminalIDData);
        }
    }

    public class PackageNumer
    {
        public PackageNumer()
        {

        }

        public PackageNumer(UInt16 inPackageNumer)
        {
            PackageNumerData = inPackageNumer;
        }

        public UInt16 PackageNumerData { get; set; }

        public byte[] ToByteArray()
        {
            return BitConverter.GetBytes(PackageNumerData);
        }
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

        public byte[] ToByteArray()
        {
            return BitConverter.GetBytes(DateAndTimeData);
        }
    }

    public class CoordinatesAndSatellites
    {
        public CoordinatesAndSatellites()
        {

        }

        public byte SatellitesAmount { get; set; }
        public byte Correctness { get; set; }
        public Int32 Latitude { get; set;}
        public Int32 Longitude { get; set; }

        public override string ToString()
        {
            return String.Format("Satellites: {0}, Correctness: {1}, Latitude: {2}, Longitude: {3}", SatellitesAmount, Correctness, Latitude, Longitude);
        }

        public byte[] ToByteArray()
        {
            byte[] asByteArray = new byte[9];
            asByteArray[0] = (byte)(SatellitesAmount | Correctness);
            Array.Copy(BitConverter.GetBytes(Latitude), 0, asByteArray, 1, 4);
            Array.Copy(BitConverter.GetBytes(Longitude), 0, asByteArray, 5, 4);
            return asByteArray;
        }

    }

    public class SpeedAndDirection
    {
        public SpeedAndDirection()
        {

        }

        public UInt16 Speed { get; set; }
        public UInt16 Direction { get; set; }
    

        public override string ToString()
        {
            return String.Format("Speed: {0}, Direction: {1}", Speed, Direction);
        }

        public byte[] ToByteArray()
        {
            byte[] asByteArray = new byte[4];
            Array.Copy(BitConverter.GetBytes(Speed), 0, asByteArray, 0, 2);
            Array.Copy(BitConverter.GetBytes(Direction), 0, asByteArray, 2, 2);
            return asByteArray;
        }

    }

    public class Height
    {
        public Height()
        {

        }

        public Height(Int16 inHeightData)
        {
            HeightData = inHeightData;
        }

        public Int16 HeightData { get; set; }

        public override string ToString()
        {
            return String.Format("Height: {0}", HeightData);
        }

        public byte[] ToByteArray()
        {
            
            return BitConverter.GetBytes(HeightData);
        }

    }

    public class HDOP
    {
        public HDOP()
        {

        }

        public HDOP(byte inHDOPData)
        {
            HDOPData = inHDOPData;
        }

        public byte HDOPData { get; set; }

        public override string ToString()
        {
            return String.Format("HDOP: {0}", HDOPData);
        }

        public byte[] ToByteArray()
        {

            return new byte[]{HDOPData};
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
                " OuterVoltage: " + (OuterVoltage ? "1" : "0") +
                " CarStatus: " + (CarStatus ? "1" : "0") +
                " Hit: " + (Hit ? "1" : "0") +
                " GpsOrGlonass: " + (GpsOrGlonass ? "1" : "0") +
                " SignalQuality: " + SignalQuality +
                " Signaling: " + (Signaling ? "1" : "0") +
                " Alarm: " + (Alarm ? "1" : "0");
        }

        public byte[] ToByteArray()
        {
            byte[] asByteArray = new byte[2];
            bool SignalQuality0 = (SignalQuality & 1) != 0;
            bool SignalQuality1 = (SignalQuality & (1 << 1) ) != 0;

            BitArray ba = new BitArray(new bool[] { Movement, Slope, iButton, SIM, GeoZone, InnerPowerSupplyVoltage, GPSAntenna, InnerPowerSupplyBusVoltage, OuterVoltage, CarStatus, Hit, GpsOrGlonass, SignalQuality0, SignalQuality1, Signaling, Alarm });
            ba.CopyTo(asByteArray, 0);
            return asByteArray;
        }

    }

}
