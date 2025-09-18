using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
public enum WindowsDisplayMode
{
    Internal,
    External,
    Extend,
    Duplicate
}
public class DisplaySwitcher : MonoBehaviour
{
    public DisplayMode m_displayMode = DisplayMode.Extend;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (Input.GetKeyDown(KeyCode.W))
            {
                //ChangeDisplayMode(m_displayMode);
                //"DELL P2722H"
                DisplayManager dw= new DisplayManager();
                //dw.SetPrimaryDisplay("DELL P2722H");
                //dw.SetPrimaryDisplay("DELL P2722H");
                dw.SetPrimaryDisplay("LG HDR 4K");

                //SwapDisplays();
            }
        }  
    }

    private void ChangeDisplayMode(DisplayMode mode)
    {
        var proc = new Process();
        proc.StartInfo.FileName = "DisplaySwitch.exe";
        switch (mode)
        {
            case DisplayMode.External:
                proc.StartInfo.Arguments = "/external";
                break;
            case DisplayMode.Internal:
                proc.StartInfo.Arguments = "/internal";
                break;
            case DisplayMode.Extend:
                proc.StartInfo.Arguments = "/extend";
                break;
            case DisplayMode.Duplicate:
                proc.StartInfo.Arguments = "/clone";
                break;
        }
        proc.Start();
    }
    private void SwapDisplays()
    {
        var proc = new Process();
        proc.StartInfo.FileName = "DisplaySwitch.exe";
        proc.StartInfo.Arguments = "/swap 1 2";

        //switch (mode)
        //{
        //    case DisplayMode.External:
        //        proc.StartInfo.Arguments = "/external";
        //        break;
        //    case DisplayMode.Internal:
        //        proc.StartInfo.Arguments = "/internal";
        //        break;
        //    case DisplayMode.Extend:
        //        proc.StartInfo.Arguments = "/extend";
        //        break;
        //    case DisplayMode.Duplicate:
        //        proc.StartInfo.Arguments = "/clone";
        //        break;
        //}
        proc.Start();
    }

}

public class DisplayManager : MonoBehaviour
{
    // Constants
    private const int CDS_UPDATEREGISTRY = 0x01;
    private const int CDS_SET_PRIMARY = 0x00000010;
    private const int DISP_CHANGE_SUCCESSFUL = 0;

    // Importing the ChangeDisplaySettingsEx function from user32.dll
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int ChangeDisplaySettingsEx(string lpszDeviceName, ref DEVMODE lpDevMode, IntPtr hwnd, uint dwFlags, IntPtr lParam);

    // Importing the EnumDisplayDevices function to get display device info
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool EnumDisplayDevices(string lpDevice, uint iDevNum, ref DISPLAY_DEVICE lpDisplayDevice, uint dwFlags);

    // Importing the EnumDisplaySettings function to get display settings
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

    // Struct for DISPLAY_DEVICE
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct DISPLAY_DEVICE
    {
        [MarshalAs(UnmanagedType.U4)]
        public int cb;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string DeviceName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceString;
        [MarshalAs(UnmanagedType.U4)]
        public DisplayDeviceStateFlags StateFlags;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceID;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string DeviceKey;
    }

    // Enum for display device flags
    [Flags()]
    public enum DisplayDeviceStateFlags : int
    {
        AttachedToDesktop = 0x1,
        MultiDriver = 0x2,
        PrimaryDevice = 0x4,
        MirroringDriver = 0x8,
        VGACompatible = 0x10,
        Removable = 0x20,
        ModesPruned = 0x8000000,
        Remote = 0x4000000,
        Disconnect = 0x2000000
    }

    // Struct for DEVMODE (Device Mode)
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct DEVMODE
    {
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;
        [MarshalAs(UnmanagedType.U2)]
        public ushort dmSpecVersion;
        [MarshalAs(UnmanagedType.U2)]
        public ushort dmDriverVersion;
        [MarshalAs(UnmanagedType.U2)]
        public ushort dmSize;
        [MarshalAs(UnmanagedType.U2)]
        public ushort dmDriverExtra;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmFields;
        public int dmPositionX;
        public int dmPositionY;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmDisplayOrientation;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmDisplayFixedOutput;
        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        [MarshalAs(UnmanagedType.U2)]
        public ushort dmLogPixels;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmBitsPerPel;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmPelsWidth;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmPelsHeight;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmDisplayFlags;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmDisplayFrequency;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmICMMethod;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmICMIntent;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmMediaType;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmDitherType;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmReserved1;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmReserved2;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmPanningWidth;
        [MarshalAs(UnmanagedType.U4)]
        public uint dmPanningHeight;
    }

    public void SetPrimaryDisplay(string displayName)
    {
        DISPLAY_DEVICE d = new DISPLAY_DEVICE();
        d.cb = Marshal.SizeOf(d);

        if (EnumDisplayDevices(null, 0, ref d, 0))
        {
            DEVMODE dm = new DEVMODE();
            dm.dmSize = (ushort)Marshal.SizeOf(dm);
            if (EnumDisplaySettings(d.DeviceName, -1, ref dm))
            {
                dm.dmFields = 0x180000; // Positioning fields
                dm.dmPositionX = 0;
                dm.dmPositionY = 0;
                int result = ChangeDisplaySettingsEx(d.DeviceName, ref dm, IntPtr.Zero, CDS_SET_PRIMARY | CDS_UPDATEREGISTRY, IntPtr.Zero);

                if (result == DISP_CHANGE_SUCCESSFUL)
                {
                    UnityEngine.Debug.Log("Primary display set successfully!");
                }
                else
                {
                    UnityEngine.Debug.Log("Failed to set primary display.");
                }
            }
        }
    }
}
