using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class PlatformHelper
{
    public static bool IsOSX(this RuntimePlatform platform)
    {
        return platform == RuntimePlatform.OSXDashboardPlayer ||
               platform == RuntimePlatform.OSXEditor ||
               platform == RuntimePlatform.OSXPlayer ||
               platform == RuntimePlatform.OSXWebPlayer;
    }

    public static bool IsWindows(this RuntimePlatform platform)
    {
        return platform == RuntimePlatform.WindowsEditor ||
               platform == RuntimePlatform.WindowsPlayer ||
               platform == RuntimePlatform.WindowsWebPlayer;
    }
}
