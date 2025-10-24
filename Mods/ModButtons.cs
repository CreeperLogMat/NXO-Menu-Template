using static NXO.Menu.ButtonHandler;
using static NXO.Mods.Categories.Settings;
using static NXO.Mods.Categories.Placeholder;

namespace NXO.Mods
{
    public enum Category
    {
        // Home 
        Home,

        // Mods
        Settings,
        Placeholder
    }

    public class ModButtons
    {
        public static Button[] buttons =
        {
            #region Starting Page
            new Button("Settings", Category.Home, false, false, ()=>ChangePage(Category.Settings)),
            new Button("Placeholder Category", Category.Home, false, false, ()=>ChangePage(Category.Placeholder)),
            #endregion

            #region Settings
            new Button("Switch Hands", Category.Settings, true, false, ()=>SwitchHands(true), ()=>SwitchHands(false)),
            new Button("Toggle Notifications", Category.Settings, true, true, ()=>ToggleNotifications(true), ()=>ToggleNotifications(false)),
            new Button("Disable All Mods", Category.Settings, false, false, ()=>DisableAllMods()),
            #endregion

            #region Placeholder
            new Button("Test Toggle Button", Category.Placeholder, true, false, ()=>PlaceholderMethod()),
            new Button("Test Non-Toggle Button", Category.Placeholder, false, false, ()=>PlaceholderMethod()),
            #endregion
        };
    }
}
