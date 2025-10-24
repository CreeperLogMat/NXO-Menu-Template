using Fusion;
using NXO.Menu;
using System;
using UnityEngine;
using UnityEngine.XR;
using static NXO.Utilities.Variables;

namespace NXO.Utilities
{
    public static class InputHandler
    {
        public static bool RTrigger() => ControllerInputPoller.instance.rightControllerIndexFloat > 0.1f;
        public static bool LTrigger() => ControllerInputPoller.instance.leftControllerIndexFloat > 0.1f;
        public static bool RGrip() => ControllerInputPoller.instance.rightGrab;
        public static bool LGrip() => ControllerInputPoller.instance.leftGrab;
        public static bool RPrimary() => ControllerInputPoller.instance.rightControllerPrimaryButton;
        public static bool LPrimary() => ControllerInputPoller.instance.leftControllerPrimaryButton;
        public static bool RSecondary() => ControllerInputPoller.instance.rightControllerSecondaryButton;
        public static bool LSecondary() => ControllerInputPoller.instance.leftControllerSecondaryButton;
    }
}
