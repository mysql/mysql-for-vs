using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace MySql.Design
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct PICTDESCBmp
	{ 
		int		cbSizeOfStruct; 
		int		picType; 
		IntPtr	hbitmap;
		IntPtr	hpalette;
		int		unused;

		public PICTDESCBmp(Bitmap bmp)
		{
			cbSizeOfStruct = Marshal.SizeOf(typeof(PICTDESCBmp));
			picType = 1;
			hpalette = IntPtr.Zero;
			unused = 0;
			hbitmap = bmp.GetHbitmap();
		}
	}

	/// <summary>
	/// Summary description for NativeMethods.
	/// </summary>
	internal class NativeMethods
	{
		[DllImport("olepro32.dll", PreserveSig=false)]
		internal static extern void OleCreatePictureIndirect(ref PICTDESCBmp pPictDesc, ref Guid riid, 
			bool fOwn, out stdole.IPictureDisp ppvObj);
	}
}
