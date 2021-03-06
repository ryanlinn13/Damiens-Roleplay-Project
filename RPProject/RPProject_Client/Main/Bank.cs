﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using CitizenFX.Core.UI;
using NativeUI;

namespace client.Main
{
    public class Bank : BaseScript
    {

        public static Bank Instance;
        public List<Vector3> Posistions = new List<Vector3>()
        {
            new Vector3(150.266f,-1040.203f,29.374f),
            new Vector3(-1212.980f,-330.841f,37.787f),
            new Vector3(-2962.582f,482.627f,15.703f),
            new Vector3(-112.202f,6469.295f,31.626f),
            new Vector3(314.187f,-278.621f,54.170f),
            new Vector3(-351.534f,-49.529f,49.042f),
            new Vector3(241.727f,220.706f,106.286f),
        };
        public bool MenuRestricted = false;
        private bool _menuOpen = false;
        private bool _menuCreated = false;
        private UIMenu _menu;
        
        public Bank()
        {
            Instance = this;
            SetupBlips(108, 2);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            GarageCheck();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            DrawMarkers();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
        }

        private async Task DrawMarkers()
        {
            while (true)
            {
                foreach (var pos in Posistions)
                {
                    if (Utility.Instance.GetDistanceBetweenVector3s(pos, Game.PlayerPed.Position) < 30)
                    {
                        World.DrawMarker(MarkerType.HorizontalCircleSkinny, pos - new Vector3(0, 0, 0.8f), Vector3.Zero, Vector3.Zero, Vector3.One, Color.FromArgb(255, 0, 255, 0));
                    }
                }
                await Delay(0);
            }
        }

        private void SetupBlips(int sprite, int color)
        {
            foreach (var var in Posistions)
            {
                var blip = API.AddBlipForCoord(var.X, var.Y, var.Z);
                API.SetBlipSprite(blip, sprite);
                API.SetBlipColour(blip, color);
                API.SetBlipScale(blip, 1f);
                API.SetBlipAsShortRange(blip, true);
                API.BeginTextCommandSetBlipName("STRING");
                API.AddTextComponentString("Bank");
                API.EndTextCommandSetBlipName(blip);
            }
        }

        private async Task GarageCheck()
        {
            while (true)
            {
                _menuOpen = false;
                foreach (var pos in Posistions)
                {
                    var dist = API.Vdist(Game.PlayerPed.Position.X, Game.PlayerPed.Position.Y, Game.PlayerPed.Position.Z, pos.X, pos.Y, pos.Z);
                    if (dist < 3f && !MenuRestricted)
                    {
                        _menuOpen = true;
                    }
                }

                if (_menuOpen && !_menuCreated)
                {
                    _menu = InteractionMenu.Instance._interactionMenuPool.AddSubMenuOffset(
                        InteractionMenu.Instance._interactionMenu, "Bank", "Access your bank account.", new PointF(5, Screen.Height / 2));
                    var depositButton = new UIMenuItem("Deposit");
                    var withdrawlButton = new UIMenuItem("Withdrawl");
                    var transferButton = new UIMenuItem("Transfer");
                    _menu.AddItem(depositButton);
                    _menu.AddItem(withdrawlButton);
                    _menu.AddItem(transferButton);
                    _menu.OnItemSelect += (sender, selectedItem, index) =>
                    {
                        if (selectedItem == depositButton)
                        {
                            InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                            Utility.Instance.KeyboardInput(
                                "Amount of money to deposit into your bank account.", "", 10,
                                delegate (string s)
                                {
                                    TriggerServerEvent("DepositMoney", Convert.ToInt32(s));
                                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                        }
                        if (selectedItem == withdrawlButton)
                        {
                            InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                            Utility.Instance.KeyboardInput(
                                "Amount of money to withdrawl from your bank account.", "", 10,
                                delegate (string s)
                                {
                                    TriggerServerEvent("WithdrawMoney", Convert.ToInt32(s));
                                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                        }
                        if (selectedItem == transferButton)
                        {
                            InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                            Utility.Instance.KeyboardInput(
                                "The ID of the player to send the money to", "", 10,
                                delegate (string idS)
                                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                                    Utility.Instance.KeyboardInput(
                                        "Amount of money to transfer from your bank account.", "", 10,
                                        delegate (string amountS)
                                        {
                                            if (Int32.TryParse(idS, out var id) &&
                                                Int32.TryParse(amountS, out var amount))
                                            {
                                                TriggerServerEvent("TransferMoney", amount, id);
                                            }
                                        });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                                });
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.

                        }
                    };
                    _menuCreated = true;
                    InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
                }
                else if (!_menuOpen && _menuCreated)
                {
                    _menuCreated = false;
                    if (_menu.Visible)
                    {
                        _menu.Visible = false;
                        InteractionMenu.Instance._interactionMenuPool.CloseAllMenus();
                        InteractionMenu.Instance._interactionMenu.Visible = true;
                    }

                    var i = 0;
                    foreach (var item in InteractionMenu.Instance._interactionMenu.MenuItems)
                    {
                        if (item == _menu.ParentItem)
                        {
                            InteractionMenu.Instance._interactionMenu.RemoveItemAt(i);
                            break;
                        }
                        i++;
                    }
                    InteractionMenu.Instance._interactionMenuPool.RefreshIndex();
                }
                await Delay(1000);
            }
        }
    }
}
