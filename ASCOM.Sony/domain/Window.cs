using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;

namespace ASCOM.Sony
{
    public enum WindowType
    {
        cannotCreateFolder,
        cannotAccessFolder,
        noCameraConnected,
        selectCamera,
        main,
        noWindow,
        unknown
    }

    public class Window
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public WindowType WindowType;
        public List<WindowObject> WindowObjects;

        [JsonConstructor]
        public Window(WindowType windowType, List<WindowObject> windowObject)
        {
            WindowType = windowType;
            WindowObjects = windowObject;
        }
        
        public Window(WindowType windowType, WindowObject windowObject)
        {
            WindowType = windowType;
            WindowObjects = new List<WindowObject>() { windowObject };
        }
    }
}
