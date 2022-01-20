using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using System.ComponentModel;
using Terraria.ModLoader.Config;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace FayeQB
{
    public class FayeQB : Mod
    {
        public static Dictionary<string, ModHotKey> Hotkeys;
        public static Configuration Config;

        public override void Load()
        {
            Hotkeys = new Dictionary<string, ModHotKey>();
            Hotkeys.Add("Quick Buy", RegisterHotKey("Quick Buy", Keys.LeftShift.ToString()));
            Hotkeys.Add("Quick Buy Stack", RegisterHotKey("Quick Buy Stack", Keys.LeftControl.ToString()));

            Logger.InfoFormat("[{0}] Loaded; keybinded correctly.", DisplayName);
        }

        public override void Unload()
        {
            Hotkeys.Clear();
            Hotkeys = null;
            Config = null;

            Logger.InfoFormat("[{0}] Unloaded; cleared static references correctly.", DisplayName);
        }
    }

    public class Configuration : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public override void OnLoaded()
        {
            FayeQB.Config = this;
        }

        [DefaultValue(1)]
        [Label("Quick Buy Amount")]
        [Tooltip("The amount of blocks bought per trigger.")]
        public int QuickBuyAmount { get; set; }
    }

    public class Player : ModPlayer
    {

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // Checks if a shop is open or if the item you're hovering on is an item you can buy
            // Latter also prevents buying items from any kind of chest (player inventory included)
            // Also, the <Item>.buy property should really have been documented.
            if (Main.npcShop < 0 || !Main.HoverItem.buy) return;

            if (FayeQB.Hotkeys["Quick Buy"].Current) PlayerBuyItem(multiplier: FayeQB.Config.QuickBuyAmount);
            if (FayeQB.Hotkeys["Quick Buy Stack"].JustPressed) PlayerBuyItem(stack: true);
        }

        // Utility method to tidy up the code
        private void PlayerBuyItem(bool stack = false, int multiplier = 1)
        {
            int value = Main.HoverItem.GetStoreValue();
            int maxStack = Main.HoverItem.maxStack;

            if (!player.CanBuyItem(stack ? value * maxStack : value * multiplier)) return;

            if (stack)
            {
                player.BuyItem(value * maxStack);
                player.QuickSpawnItem(Main.HoverItem, maxStack);
                return;
            }

            player.BuyItem(value * multiplier);
            player.QuickSpawnItem(Main.HoverItem, multiplier);
        }
    }
}
