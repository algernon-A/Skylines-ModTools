﻿using ColossalFramework;
using ModTools.Utils;
using UnityEngine;

namespace ModTools
{
    internal static class Util
    {
        public static void SetMouseScrolling(bool isEnabled)
        {
            try
            {
                var mouseWheelZoom = ReflectionUtil.GetPrivate<SavedBool>(ToolsModifierControl.cameraController, "m_mouseWheelZoom");
                if (mouseWheelZoom.value != isEnabled)
                {
                    mouseWheelZoom.value = isEnabled;
                }
            }
            catch
            {
            }
        }

        public static bool ComponentIsEnabled(Component component)
        {
            var prop = component.GetType().GetProperty("enabled");
            return prop == null || (bool)prop.GetValue(component, null);
        }
    }
}