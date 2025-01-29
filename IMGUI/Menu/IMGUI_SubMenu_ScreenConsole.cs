using Architecture;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace IMGUI
{
    public class IMGUI_SubMenu_ScreenConsole : IMGUI_SubMenu
    {
        public IMGUI_SubMenu_ScreenConsole() : base("SCREEN CONSOLE")
        {

        }

        public override Type GetParentSubMenu() => typeof(IMGUI_SubMenu_Main);

        protected override IEnumerable<IMGUI_Element> EnumerateElements(IntWrapper indent)
        {
            yield return new IMGUI_ElementToggle("Display console logs on screen", () => IMGUI_Console.ShouldDisplay.Value, IMGUI_Console.ShouldDisplay.Toggle)
                .SetInteractableFunc(()=>IMGUI_Console.Instance != null);
            indent++;
            foreach((var index, var logType, var value) in IMGUI_Console.DisplayFilter.EnumerateIndicesEnumsAndValues())
            {
                yield return new IMGUI_ElementToggle($"Show log type: {logType}", () => IMGUI_Console.DisplayFilter[logType].Value, IMGUI_Console.DisplayFilter[logType].Toggle)
                    .SetInteractableFunc(() => IMGUI_Console.Instance != null && IMGUI_Console.ShouldDisplay.Value);
            }
            indent--;
        }
    }
}
