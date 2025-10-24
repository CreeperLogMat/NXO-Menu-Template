using static NXO.Utilities.Variables;
using NXO.Utilities;
using static NXO.Utilities.NotificationLib;

namespace NXO.Mods.Categories
{
    public class Settings
    {
        public static void SwitchHands(bool setActive) => rightHandedMenu = setActive;
        public static void ToggleNotifications(bool setActive) => toggleNotifications = setActive;
    }
}
