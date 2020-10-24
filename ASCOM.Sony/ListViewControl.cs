using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;

namespace ASCOM.Sony
{
    public class ListViewControl
    {
        private const int LVM_FIRST = 0x1000;
        private const int LVM_GETITEMCOUNT = LVM_FIRST + 4;
        private const int LVM_SETITEMSTATE = LVM_FIRST + 43;

        private const int LVIS_UNCHECKED = 0x1000;
        private const int LVIS_CHECKED = 0x2000;
        private const int LVIS_STATEIMAGEMASK = 0x3000;
        private const int LVIS_SELECTED = 0x2;
        private const int LVIF_STATE = 0x8;

        private const uint MEM_COMMIT = 0x1000;
        private const uint PAGE_READWRITE = 0x4;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct LVITEM
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszText;
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns;
            public IntPtr puColumns;
        };

        [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageLVItem(IntPtr hWnd, int msg, int wParam, ref LVITEM lvi);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wparam, IntPtr lparam);
        
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);


        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(ProcessAccessFlags processAccess, bool bInheritHandle, int processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool WriteProcessMemory(
          IntPtr hProcess,
          IntPtr lpBaseAddress,
          byte[] lpBuffer,
          Int32 nSize,
          out IntPtr lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
            static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            [Out] byte[] lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress, int dwSize, AllocationType dwFreeType);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

        [DllImport("kernel32.dll", SetLastError = true)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CloseHandle(IntPtr hObject);

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        public enum AllocationType
        {
            Commit = 0x1000,
            Reserve = 0x2000,
            Decommit = 0x4000,
            Release = 0x8000,
            Reset = 0x80000,
            Physical = 0x400000,
            TopDown = 0x100000,
            WriteWatch = 0x200000,
            LargePages = 0x20000000
        }

        [Flags]
        public enum MemoryProtection
        {
            Execute = 0x10,
            ExecuteRead = 0x20,
            ExecuteReadWrite = 0x40,
            ExecuteWriteCopy = 0x80,
            NoAccess = 0x01,
            ReadOnly = 0x02,
            ReadWrite = 0x04,
            WriteCopy = 0x08,
            GuardModifierflag = 0x100,
            NoCacheModifierflag = 0x200,
            WriteCombineModifierflag = 0x400
        }

        public static byte[] getBytes(LVITEM str)
        {
            int size = Marshal.SizeOf(str);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(str, ptr, false);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// Select first row on the given listview
        /// </summary>
        /// <param name="listHandle">The listview whose items are to be selected</param>
        public static void SelectFirstItem(IntPtr listHandle)
        {
            try
            {
                var itemCount = SendMessage(listHandle, LVM_GETITEMCOUNT, 0, (IntPtr)0);
                uint procId = 0;
                GetWindowThreadProcessId(listHandle, out procId);
                var flags = ProcessAccessFlags.VirtualMemoryOperation | ProcessAccessFlags.VirtualMemoryRead | ProcessAccessFlags.VirtualMemoryWrite;
                var hProc = OpenProcess(flags, false, (int)procId);

                // Reserve space in the processes memory and write the new LV_ITEM structure`s data to it

                LVITEM lvItem = new LVITEM();
                lvItem.mask = LVIF_STATE;
                lvItem.stateMask = LVIS_STATEIMAGEMASK;
                lvItem.state = LVIS_CHECKED;
                byte[] lvItemBytes = getBytes(lvItem);

                IntPtr LvItemPtr = VirtualAllocEx(hProc, IntPtr.Zero, Marshal.SizeOf(lvItem), (uint)AllocationType.Commit, (uint)MemoryProtection.ExecuteReadWrite);
                IntPtr numOfBytesWritten;

                byte[] readBytes = new byte[100];
                ReadProcessMemory(hProc, LvItemPtr, readBytes, Marshal.SizeOf(lvItem), out numOfBytesWritten);

                WriteProcessMemory(hProc, LvItemPtr, lvItemBytes, Marshal.SizeOf(lvItem), out numOfBytesWritten);


                // Send the message to set the items state
                var test = SendMessage(listHandle, LVM_SETITEMSTATE, 0, LvItemPtr);

                VirtualFreeEx(hProc, LvItemPtr, 0, AllocationType.Release); // Release the processes memory used for the LV_ITEM structure`s data
                CloseHandle(hProc);

                //SetItemState(listHandle, 1, LVIS_STATEIMAGEMASK, LVIS_SELECTED);
            }
            catch(Exception e)
            {

            }
        }
        /// <summary>
        /// Select all rows on the given listview
        /// </summary>
        /// <param name="listHandle">The listview whose items are to be selected</param>
        public static void CheckAllItems(IntPtr listHandle)
        {
            SetItemState(listHandle, -1, LVIS_STATEIMAGEMASK, LVIS_CHECKED);
        }

        /// <summary>
        /// Deselect all rows on the given listview
        /// </summary>
        /// <param name="listHandle">The listview whose items are to be deselected</param>
        public static void UncheckAllItems(IntPtr listHandle)
        {
            SetItemState(listHandle, -1, LVIS_STATEIMAGEMASK, LVIS_UNCHECKED);
        }

        /// <summary>
        /// Set the item state on the given item
        /// </summary>
        /// <param name="list">The listview whose item's state is to be changed</param>
        /// <param name="itemIndex">The index of the item to be changed</param>
        /// <param name="mask">Which bits of the value are to be set?</param>
        /// <param name="value">The value to be set</param>
        public static void SetItemState(IntPtr listHandle, int itemIndex, int mask, int value)
        {
            LVITEM lvItem = new LVITEM();
            lvItem.stateMask = mask;
            lvItem.state = value;
            SendMessageLVItem(listHandle, LVM_SETITEMSTATE, itemIndex, ref lvItem);
        }
    }
}
