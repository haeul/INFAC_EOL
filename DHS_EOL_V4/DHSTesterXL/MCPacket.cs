using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DHSTesterXL
{
    [StructLayout(LayoutKind.Explicit)]
    public struct MCWord
    {
        [FieldOffset(0)]
        public short Value;
        [FieldOffset(0)]
        public byte Low;
        [FieldOffset(1)]
        public byte High;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MCDword
    {
        [FieldOffset(0)]
        public int Value;
        [FieldOffset(0)]
        public MCWord Low;
        [FieldOffset(2)]
        public MCWord High;
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct M3Header
    {
        [MarshalAs(UnmanagedType.I2)] public short Subhead;
        [MarshalAs(UnmanagedType.I1)] public byte  NetworkNo;
        [MarshalAs(UnmanagedType.I1)] public byte  PlcNo;
        [MarshalAs(UnmanagedType.I2)] public short ReqModuleIO;
        [MarshalAs(UnmanagedType.I1)] public byte  ReqModuleCh;
        [MarshalAs(UnmanagedType.I2)] public short DataLength;
        [MarshalAs(UnmanagedType.I2)] public short TimerOrCode;

        // Calling this method will return a byte array with the contents
        // of the struct ready to be sent via the tcp socket.
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(M3Header))];

            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }

        // this method will deserialize a byte array into the struct.
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (M3Header)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(M3Header));
            gch.Free();
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct M3Command
    {
        [MarshalAs(UnmanagedType.I2)] public ushort Command;
        [MarshalAs(UnmanagedType.I2)] public ushort SubCommand;
        [MarshalAs(UnmanagedType.I2)] public ushort StartAddr;
        [MarshalAs(UnmanagedType.I2)] public ushort DeviceCode;
        [MarshalAs(UnmanagedType.I2)] public ushort DeviceCount;

        // Calling this method will return a byte array with the contents
        // of the struct ready to be sent via the tcp socket.
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(M3Command))];

            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }

        // this method will deserialize a byte array into the struct.
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (M3Command)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(M3Command));
            gch.Free();
        }
    }

    /// <summary>
    /// 0403 Random Read Request
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct M0403_req
    {
        // Header
        [MarshalAs(UnmanagedType.I2)] public short  Subhead;
        [MarshalAs(UnmanagedType.I1)] public byte   NetworkNo;
        [MarshalAs(UnmanagedType.I1)] public byte   PlcNo;
        [MarshalAs(UnmanagedType.I2)] public short  ReqModuleIO;
        [MarshalAs(UnmanagedType.I1)] public byte   ReqModuleCh;
        [MarshalAs(UnmanagedType.I2)] public short  DataLength;
        [MarshalAs(UnmanagedType.I2)] public short  Response;
        [MarshalAs(UnmanagedType.I2)] public ushort Command;
        [MarshalAs(UnmanagedType.I2)] public ushort SubCommand;
        [MarshalAs(UnmanagedType.I1)] public byte   WordCount;
        [MarshalAs(UnmanagedType.I1)] public byte   DWordCount;

        // 사용자 지정 메모리

        // WORD 데이터
        // CH.1 (LEFT)
        [MarshalAs(UnmanagedType.I4)] public int D5000; // 
        [MarshalAs(UnmanagedType.I4)] public int D5001; // 
        [MarshalAs(UnmanagedType.I4)] public int D5002; // 
        [MarshalAs(UnmanagedType.I4)] public int D5003; // 
        [MarshalAs(UnmanagedType.I4)] public int D5004; // 
        [MarshalAs(UnmanagedType.I4)] public int D5005; // 
        [MarshalAs(UnmanagedType.I4)] public int D5006; // 
        [MarshalAs(UnmanagedType.I4)] public int D5007; // 
        [MarshalAs(UnmanagedType.I4)] public int D5008; // 
        [MarshalAs(UnmanagedType.I4)] public int D5009; // 
        // CH.2 (RIGHT)
        [MarshalAs(UnmanagedType.I4)] public int D5010; // 
        [MarshalAs(UnmanagedType.I4)] public int D5011; // 
        [MarshalAs(UnmanagedType.I4)] public int D5012; // 
        [MarshalAs(UnmanagedType.I4)] public int D5013; // 
        [MarshalAs(UnmanagedType.I4)] public int D5014; // 
        [MarshalAs(UnmanagedType.I4)] public int D5015; // 
        [MarshalAs(UnmanagedType.I4)] public int D5016; // 
        [MarshalAs(UnmanagedType.I4)] public int D5017; // 
        [MarshalAs(UnmanagedType.I4)] public int D5018; // 
        [MarshalAs(UnmanagedType.I4)] public int D5019; // 


        // Calling this method will return a byte array with the contents
        // of the struct ready to be sent via the tcp socket.
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(M0403_req))];

            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }

        // this method will deserialize a byte array into the struct.
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (M0403_req)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(M0403_req));
            gch.Free();
        }
    }

    /// <summary>
    /// 0403 Random Read Response
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct M0403_res
    {
        // Header
        [MarshalAs(UnmanagedType.I2)] public short Subhead;
        [MarshalAs(UnmanagedType.I1)] public byte  NetworkNo;
        [MarshalAs(UnmanagedType.I1)] public byte  PlcNo;
        [MarshalAs(UnmanagedType.I2)] public short ReqModuleIO;
        [MarshalAs(UnmanagedType.I1)] public byte  ReqModuleCh;
        [MarshalAs(UnmanagedType.I2)] public short DataLength;
        [MarshalAs(UnmanagedType.I2)] public short Response;

        // 사용자 지정 메모리

        // WORD 데이터
        // CH.1 (LEFT)
        [MarshalAs(UnmanagedType.I2)] public short D5000; // 
        [MarshalAs(UnmanagedType.I2)] public short D5001; // 
        [MarshalAs(UnmanagedType.I2)] public short D5002; // 
        [MarshalAs(UnmanagedType.I2)] public short D5003; // 
        [MarshalAs(UnmanagedType.I2)] public short D5004; // 
        [MarshalAs(UnmanagedType.I2)] public short D5005; // 
        [MarshalAs(UnmanagedType.I2)] public short D5006; // 
        [MarshalAs(UnmanagedType.I2)] public short D5007; // 
        [MarshalAs(UnmanagedType.I2)] public short D5008; // 
        [MarshalAs(UnmanagedType.I2)] public short D5009; // 
        // CH.2 (RIGHT)
        [MarshalAs(UnmanagedType.I2)] public short D5010; // 
        [MarshalAs(UnmanagedType.I2)] public short D5011; // 
        [MarshalAs(UnmanagedType.I2)] public short D5012; // 
        [MarshalAs(UnmanagedType.I2)] public short D5013; // 
        [MarshalAs(UnmanagedType.I2)] public short D5014; // 
        [MarshalAs(UnmanagedType.I2)] public short D5015; // 
        [MarshalAs(UnmanagedType.I2)] public short D5016; // 
        [MarshalAs(UnmanagedType.I2)] public short D5017; // 
        [MarshalAs(UnmanagedType.I2)] public short D5018; // 
        [MarshalAs(UnmanagedType.I2)] public short D5019; // 


        // Calling this method will return a byte array with the contents
        // of the struct ready to be sent via the tcp socket.
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(M0403_res))];

            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }


        // this method will deserialize a byte array into the struct.
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (M0403_res)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(M0403_res));
            gch.Free();
        }
    }


    //////////////////////////////////////////////////////////////////////////


    /// <summary>
    /// 1402 Random Write Request
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct M1402_req
    {
        // Header
        [MarshalAs(UnmanagedType.I2)] public short  Subhead;
        [MarshalAs(UnmanagedType.I1)] public byte   NetworkNo;
        [MarshalAs(UnmanagedType.I1)] public byte   PlcNo;
        [MarshalAs(UnmanagedType.I2)] public short  ReqModuleIO;
        [MarshalAs(UnmanagedType.I1)] public byte   ReqModuleCh;
        [MarshalAs(UnmanagedType.I2)] public short  DataLength;
        [MarshalAs(UnmanagedType.I2)] public short  Response;
        [MarshalAs(UnmanagedType.I2)] public ushort Command;
        [MarshalAs(UnmanagedType.I2)] public ushort SubCommand;
        [MarshalAs(UnmanagedType.I1)] public byte   WordCount;
        [MarshalAs(UnmanagedType.I1)] public byte   DWordCount;

        // WORD 데이터
        // CH.1 (LEFT)
        [MarshalAs(UnmanagedType.I4)] public int    D5020dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5020dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5021dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5021dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5022dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5022dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5023dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5023dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5024dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5024dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5025dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5025dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5026dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5026dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5027dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5027dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5028dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5028dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5029dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5029dat;
        // CH.2 (RIGHT)
        [MarshalAs(UnmanagedType.I4)] public int    D5030dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5030dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5031dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5031dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5032dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5032dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5033dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5033dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5034dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5034dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5035dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5035dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5036dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5036dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5037dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5037dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5038dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5038dat;
        [MarshalAs(UnmanagedType.I4)] public int    D5039dev;
        [MarshalAs(UnmanagedType.I2)] public ushort D5039dat;


        // Calling this method will return a byte array with the contents
        // of the struct ready to be sent via the tcp socket.
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(M1402_req))];

            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }


        // this method will deserialize a byte array into the struct.
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (M1402_req)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(M1402_req));
            gch.Free();
        }
    }


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct M1402_res
    {
        [MarshalAs(UnmanagedType.I2)] public short Subhead;
        [MarshalAs(UnmanagedType.I1)] public byte  NetworkNo;
        [MarshalAs(UnmanagedType.I1)] public byte  PlcNo;
        [MarshalAs(UnmanagedType.I2)] public short ReqModuleIO;
        [MarshalAs(UnmanagedType.I1)] public byte  ReqModuleCh;
        [MarshalAs(UnmanagedType.I2)] public short DataLength;
        [MarshalAs(UnmanagedType.I2)] public short Response;


        // Calling this method will return a byte array with the contents
        // of the struct ready to be sent via the tcp socket.
        public byte[] Serialize()
        {
            // allocate a byte array for the struct data
            var buffer = new byte[Marshal.SizeOf(typeof(M1402_res))];

            // Allocate a GCHandle and get the array pointer
            var gch = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var pBuffer = gch.AddrOfPinnedObject();

            // copy data from struct to array and unpin the gc pointer
            Marshal.StructureToPtr(this, pBuffer, false);
            gch.Free();

            return buffer;
        }


        // this method will deserialize a byte array into the struct.
        public void Deserialize(ref byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            this = (M1402_res)Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(M1402_res));
            gch.Free();
        }
    }
}
