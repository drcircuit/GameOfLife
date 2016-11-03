using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace ConsoleMadness
{
    public class MouseHandler
    {
        public ConsoleHandle Handle { get; }

        public const int StdInputHandle = -10;

        public const int EnableMouseInput = 0x0010;
        public const int EnableQuickEditMode = 0x0040;
        public const int EnableExtendedFlags = 0x0080;

        
        public MouseHandler()
        {
            Handle = InitMouseHandler();
        }
        [DebuggerDisplay("EventType: {EventType}")]
        [StructLayout(LayoutKind.Explicit)]
        public struct Listener
        {
            [FieldOffset(0)]
            public short EventType;
            [FieldOffset(4)]
            public MouseEventRecord Event;
        }
        public static ConsoleHandle InitMouseHandler()
        {
            var handle = GetStdHandle(StdInputHandle);

            var mode = 0;
            if (!GetConsoleMode(handle, ref mode)) throw new Win32Exception();

            mode |= EnableMouseInput;
            mode &= ~EnableQuickEditMode;
            mode |= EnableExtendedFlags;

            if (!SetConsoleMode(handle, mode)) throw new Win32Exception();
            return handle;
        }
        [DebuggerDisplay("{Position.X}, {Position.Y}")]
        public struct MouseEventRecord
        {
            public Coord Position;
            public Mouse.Button Button;
            public int ControlKeyState;
            public int Flags;
        }

        [DebuggerDisplay("{X}, {Y}")]
        public struct Coord
        {
            public ushort X;
            public ushort Y;
        }

       public class ConsoleHandle : SafeHandleMinusOneIsInvalid
        {
            public ConsoleHandle() : base(false) { }

            protected override bool ReleaseHandle()
            {
                return true; //releasing console handle is not our business
            }
        }


        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetConsoleMode(ConsoleHandle hConsoleHandle, ref int lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern ConsoleHandle GetStdHandle(int nStdHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ReadConsoleInput(ConsoleHandle hConsoleInput, ref Listener lpBuffer, UInt32 nLength, ref UInt32 lpNumberOfEventsRead);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetConsoleMode(ConsoleHandle hConsoleHandle, int dwMode);

    }
}