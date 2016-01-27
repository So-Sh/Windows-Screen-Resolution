
using System;
//using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;


[StructLayout(LayoutKind.Sequential)]
public struct DEVMODE 
{
	[MarshalAs(UnmanagedType.ByValTStr,SizeConst=32)] public string dmDeviceName;
	public short  dmSpecVersion;
	public short  dmDriverVersion;
	public short  dmSize;
	public short  dmDriverExtra;
	public int    dmFields;

	public short dmOrientation;
	public short dmPaperSize;
	public short dmPaperLength;
	public short dmPaperWidth;

	public short dmScale;
	public short dmCopies;
	public short dmDefaultSource;
	public short dmPrintQuality;
	public short dmColor;
	public short dmDuplex;
	public short dmYResolution;
	public short dmTTOption;
	public short dmCollate;
	[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)] public string dmFormName;
	public short dmLogPixels;
	public short dmBitsPerPel;
	public int   dmPelsWidth;
	public int   dmPelsHeight;

	public int   dmDisplayFlags;
	public int   dmDisplayFrequency;

	public int   dmICMMethod;
	public int   dmICMIntent;
	public int   dmMediaType;
	public int   dmDitherType;
	public int   dmReserved1;
	public int   dmReserved2;

	public int   dmPanningWidth;
	public int   dmPanningHeight;
};



class User_32
{
	[DllImport("user32.dll")]
	public static extern int EnumDisplaySettings (string deviceName, int modeNum, ref DEVMODE devMode );         
	[DllImport("user32.dll")]
	public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

	public const int ENUM_CURRENT_SETTINGS = -1;
	public const int CDS_UPDATEREGISTRY = 0x01;
	public const int CDS_TEST = 0x02;
	public const int DISP_CHANGE_SUCCESSFUL = 0;
	public const int DISP_CHANGE_RESTART = 1;
	public const int DISP_CHANGE_FAILED = -1;
}


namespace ScreenResolution
{

    

	class Resolution:IComparable
	{

        public int Width { get; set; }
        public int Height { get; set; }


        public Resolution(int width, int height)
		{


            Width = width;
            Height = height;
                    
		}

        public static List<Resolution> GetAvailableScreenResolutions()
        {
            List<Resolution> availableResolutions = new List<Resolution>();

            DEVMODE vDevMode = new DEVMODE();
            int i = 0;
            while (0 != User_32.EnumDisplaySettings(null, i, ref vDevMode))
            {
                if (vDevMode.dmDisplayFrequency == 60) 
                {              

                Resolution res = new Resolution(vDevMode.dmPelsWidth, vDevMode.dmPelsHeight);

                if (!(availableResolutions.Any(r => r.Width == res.Width) || availableResolutions.Any(r => r.Height == res.Height))) 
                availableResolutions.Add(res);

                }
                
                i++;
            }

            

            return availableResolutions;

        }

        public static void SetScreenResolution(int width, int height)
        {

            int iWidth = width;
            int iHeight = height;


            DEVMODE dm = new DEVMODE();
            dm.dmDeviceName = new String(new char[32]);
            dm.dmFormName = new String(new char[32]);
            dm.dmSize = (short)Marshal.SizeOf(dm);

            if (0 != User_32.EnumDisplaySettings(null, User_32.ENUM_CURRENT_SETTINGS, ref dm))
            {

                dm.dmPelsWidth = iWidth;
                dm.dmPelsHeight = iHeight;

                int iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_TEST);

                if (iRet == User_32.DISP_CHANGE_FAILED)
                {
                    throw new System.Exception("Unable to process your request");                   
                }
                else
                {
                    iRet = User_32.ChangeDisplaySettings(ref dm, User_32.CDS_UPDATEREGISTRY);

                    switch (iRet)
                    {
                        case User_32.DISP_CHANGE_SUCCESSFUL:
                            {
                                break;

                                //successfull change
                            }
                        case User_32.DISP_CHANGE_RESTART:
                            {
                                throw new System.Exception("Description: You Need To Reboot For The Change To Happen.\n If You Feel Any Problem After Rebooting Your Machine\nThen Try To Change Resolution In Safe Mode.");
                                //windows 9x series you have to restart
                            }
                        default:
                            {
                                throw new System.Exception("Failed To Change The Resolution");
                               
                                //failed to change
                            }
                    }
                }

            }
        }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;

            Resolution otherResolution = obj as Resolution;
            if (otherResolution != null)
                return this.Width.CompareTo(otherResolution.Width);
            else
                throw new ArgumentException("Object is not a Resolution");


        }

    }
}
