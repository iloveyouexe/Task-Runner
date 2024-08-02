using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TaskRunner.Native;

namespace TaskRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("No arguments provided, use 'TaskRunner --help' for more info on commands.");
                return;
            }

            switch (args[0].ToLower())
            {
                case "--move-taskbar":
                    HandleMoveTaskbarCommand(args);
                    break;
                case "--help":
                    ShowHelp();
                    break;
                default:
                    Console.WriteLine("Unknown command.");
                    break;
            }
        }

        static void HandleMoveTaskbarCommand(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Please specify the position to move your taskbar to (top, bottom, left, right).");
                return;
            }

            switch (args[1].ToLower())
            {
                case "top":
                    MoveTaskbar(NativeMethods.ABEdge.ABE_TOP);
                    break;
                case "bottom":
                    MoveTaskbar(NativeMethods.ABEdge.ABE_BOTTOM);
                    break;
                case "left":
                    MoveTaskbar(NativeMethods.ABEdge.ABE_LEFT);
                    break;
                case "right":
                    MoveTaskbar(NativeMethods.ABEdge.ABE_RIGHT);
                    break;
                default:
                    Console.WriteLine("Invalid position specified.");
                    break;
            }
        }

        static void MoveTaskbar(NativeMethods.ABEdge position)
        {
            IntPtr taskbarHandle = NativeMethods.FindWindow("Shell_TrayWnd", null);
            if (taskbarHandle == IntPtr.Zero)
            {
                Console.WriteLine("Taskbar not found.");
                return;
            }

            // RegisterTaskbar(taskbarHandle);

            NativeMethods.APPBARDATA abd = new NativeMethods.APPBARDATA
            {
                cbSize = Marshal.SizeOf(typeof(NativeMethods.APPBARDATA)),
                hWnd = taskbarHandle
            };
            
            switch (position)
            {
                case NativeMethods.ABEdge.ABE_LEFT:
                case NativeMethods.ABEdge.ABE_RIGHT:
                    abd.rc = new NativeMethods.Rect
                    {
                        Top = 0,
                        Bottom = 1080, // Full height
                        Left = 0,
                        Right = 40
                    };
                    break;
                case NativeMethods.ABEdge.ABE_TOP:
                case NativeMethods.ABEdge.ABE_BOTTOM:
                    abd.rc = new NativeMethods.Rect
                    {
                        Left = 0,
                        Right = 1920, // Full width
                        Top = 0,
                        Bottom = 40 // Arbitrary height of 40 pixels for the taskbar
                    };
                    break;
            }

            abd.uEdge = position;
            var hresult = NativeMethods.SHAppBarMessage((uint)NativeMethods.ABMessage.ABM_SETPOS, ref abd);
            Console.WriteLine($"Attempted to move taskbar to {position}. ");   
            if (hresult == 0)
            {
                Console.WriteLine("Taskbar moved. ");
            }
            else
            {
                Console.WriteLine($"HRESULT is: 0x{hresult:x8}");
            }
        }
        

        static void RegisterTaskbar(IntPtr taskbarHandle)
        {
            NativeMethods.APPBARDATA abd = new NativeMethods.APPBARDATA
            {
                cbSize = Marshal.SizeOf(typeof(NativeMethods.APPBARDATA)),
                hWnd = taskbarHandle
            };
            NativeMethods.SHAppBarMessage((uint)NativeMethods.ABMessage.ABM_NEW, ref abd);
        }

        static void ShowHelp()
        {
            Console.WriteLine("Usage: TaskRunner --move-taskbar [top|bottom|left|right]");
            Console.WriteLine("Use this tool to move the Windows taskbar to a specific side of the screen.");
        }
    }
}
