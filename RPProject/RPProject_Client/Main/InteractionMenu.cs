﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;
using client.Main.Items;

namespace client.Main
{

    public class ModifiedMenuPool : MenuPool
    {
        public UIMenu AddSubMenuOffset(UIMenu menu, string text, string desc, PointF offset)
        {
            var item = new UIMenuItem(text, desc);
            menu.AddItem(item);
            var submenu = new UIMenu(menu.Title.Caption, text, offset);
            this.Add(submenu);
            menu.BindMenuToItem(submenu, item);
            return submenu;
        }
        public UIMenu AddSubMenuOffset(UIMenu menu, string text, PointF offset)
        {
            var item = new UIMenuItem(text);
            menu.AddItem(item);
            var submenu = new UIMenu(menu.Title.Caption, text, offset);
            this.Add(submenu);
            menu.BindMenuToItem(submenu, item);
            return submenu;
        }
    }

    public class InteractionMenu : BaseScript
    {
        public static InteractionMenu Instance;

        public List<UIMenu> _menus = new List<UIMenu>();

        private string lastnumber = "";

        public readonly ModifiedMenuPool _interactionMenuPool;
        public readonly UIMenu _interactionMenu;
        public InteractionMenu()
        {
            Instance = this;



            EventHandlers["PhoneLookAt"] += new Action(async() =>
            {
                API.TaskStartScenarioInPlace(Game.PlayerPed.Handle, "WORLD_HUMAN_STAND_MOBILE",0,true);
                await Delay(3000);
                Game.PlayerPed.Task.ClearAll();
            });


            _interactionMenuPool = new ModifiedMenuPool();
            var banner = new UIResRectangle();
            banner.Color = Color.FromArgb(160,30,30,30);
            _interactionMenu = new UIMenu("Interaction Menu","A menu to intereact with the world!",new PointF(5,Screen.Height/2));
            _interactionMenu.Title.Centered = true;
            _interactionMenu.MouseEdgeEnabled = false;
            _interactionMenu.ControlDisablingEnabled = true;
            var animationsButton = new UIMenuItem("Animations Menu", "Browse all the animations!");
            _interactionMenu.AddItem(animationsButton);

            var phoneMenu = _interactionMenuPool.AddSubMenuOffset(_interactionMenu, "Phone", "Access your phone.", new PointF(5, Screen.Height / 2));
            var emergencyButton = new UIMenuItem("(911) Emergency Call");
            var nonEmergencyButton = new UIMenuItem("(311) Non Emergency Call");
            var textButton = new UIMenuItem("Text Somebody", "Use thier server ID or actual phone number");
            var tweetButton = new UIMenuItem("Post to Twitter");
            var torButton = new UIMenuItem("Post to TOR");
            var advertButton = new UIMenuItem("Post to craigslist");
            phoneMenu.AddItem(emergencyButton);
            phoneMenu.AddItem(nonEmergencyButton);
            phoneMenu.AddItem(textButton);
            phoneMenu.OnItemSelect += delegate(UIMenu sender, UIMenuItem item, int index)
            {
                if (item == emergencyButton)
                {
                    _interactionMenuPool.CloseAllMenus();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                    Utility.Instance.KeyboardInput("This is 911, what is your emergency?","",2000, delegate(string s)
                    {
                        TriggerEvent("911CallClient",s);
                    } );
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                }
                else if (item == nonEmergencyButton)
                {
                    _interactionMenuPool.CloseAllMenus();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                    Utility.Instance.KeyboardInput("This is 311, what is your emergency?", "", 2000, delegate (string s)
                    {
                        TriggerEvent("311CallClient", s);
                    });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                }
                else if (item == textButton)
                {
                    _interactionMenuPool.CloseAllMenus();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                    Utility.Instance.KeyboardInput("The server id or phone number of the person you are trying to text.", lastnumber, 10,
                        delegate(string s)
                        {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                            Utility.Instance.KeyboardInput("The message you are trying to send to the person.","",2000,
                                delegate(string s1)
                                {
                                    lastnumber = s;
                                    TriggerServerEvent("TextingFromClient",s,s1);
                                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                }
            };

            var cardButton = new UIMenuItem("Show Buisness Card", "Show buisness card with name and phone number to everyone nearby.");
            _interactionMenu.AddItem(cardButton);

            var idButton = new UIMenuItem("Show ID", "Show your identification to everyone nearby.");
            _interactionMenu.AddItem(idButton);

            _interactionMenu.OnItemSelect += (sender, item, index) =>
            {
                if (item == animationsButton)
                {
                    _interactionMenuPool.CloseAllMenus();
                    TriggerEvent("OpenAnimationsMenu");
                }
                else if (item == idButton)
                {
                    TriggerServerEvent("RequestID", Game.Player.ServerId);
                }
                else if (item == cardButton)
                {
                    TriggerServerEvent("BuisnessCardRequest", Game.Player.ServerId);
                }
            };

            _interactionMenuPool.Add(_interactionMenu);
            _interactionMenuPool.RefreshIndex();
            _interactionMenuPool.CloseAllMenus(); ;

#pragma warning disable CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
            Tick += new Func<Task>(async delegate
#pragma warning restore CS1998 // This async method lacks 'await' operators and will run synchronously. Consider using the 'await' operator to await non-blocking API calls, or 'await Task.Run(...)' to do CPU-bound work on a background thread.
            {
                _interactionMenuPool.ProcessMenus();
                if ( API.IsInputDisabled(2) && API.IsControlJustReleased(0, 288) && !_interactionMenuPool.IsAnyMenuOpen() && !Restraints.Instance.Restrained)
                {
                    _interactionMenu.Visible = true;
                }
            });
        }
    }
}
