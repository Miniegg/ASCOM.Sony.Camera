using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace ASCOM.Sony
{
    public class WindowObject
    {
        public string Name;
        public IntPtr Handle;
        public List<int> ChildIndex;
        public string ExactWindowText;

        public WindowObject(string name, IntPtr handle, List<int> childIndex, string exactWindowText = "")
        {
            Name = name;
            Handle = handle;
            ChildIndex = childIndex;
            ExactWindowText = exactWindowText;
        }

        [JsonConstructor]
        public WindowObject(string name, List<int> childIndex, string exactWindowText = "")
        {
            Name = name ?? "";
            Handle = new IntPtr();
            ChildIndex = childIndex;
            ExactWindowText = exactWindowText ?? "";
        }
    }
}
