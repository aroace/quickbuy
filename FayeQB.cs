using Terraria.ModLoader;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;
using Terraria;
using System.Collections.Generic;

namespace FayeQB
{
    public class FayeQB : Mod
    {
        public static Dictionary<string, ModHotKey> Hotkeys;

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

            Logger.InfoFormat("[0] Unloaded; cleared static references correctly.", DisplayName);
        }
    }

    public class Player : ModPlayer
    {

        private bool PlayerCanBuyCalculatedPrice(bool stack = false)
        {
            int value = Main.HoverItem.GetStoreValue();

            if (stack) return player.CanBuyItem(value * Main.HoverItem.maxStack);
            return player.CanBuyItem(value);
        }
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // Checks if a shop is open or if the item you're hovering on is an item you can buy
            // Latter also prevents buying items from any kind of chest (player inventory included)
            // Also, the <Item>.buy property should really have been documented.
            if (Main.npcShop < 0 || !Main.HoverItem.buy) return;

            if (FayeQB.Hotkeys["Quick Buy"].Current && PlayerCanBuyCalculatedPrice())
            {
                player.QuickSpawnItem(Main.HoverItem);
                player.BuyItem(Main.HoverItem.GetStoreValue());
            }

            if (FayeQB.Hotkeys["Quick Buy Stack"].JustPressed && PlayerCanBuyCalculatedPrice(stack: true))
            {
                player.QuickSpawnItem(Main.HoverItem, Main.HoverItem.maxStack);
                player.BuyItem(Main.HoverItem.GetStoreValue() * Main.HoverItem.maxStack);
            }
        }
    }
}