﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using client.Main.Activities;
using CitizenFX.Core;
using client.Main.EmergencyServices;
using client.Main.EmergencyServices.Police;

namespace client.Main.Criminal
{
    public class Mugging : BaseScript
    {
        public static Mugging Instance;

        private Ped _targetedPed;
        private bool _isMugging;
        private int _mugTimeLeft = 0;

        private const int _mugTimeMin = 35;
        private const int _mugTimeMax = 45;

        private List<string> _callMessages = new List<string>()
        {
            "HELP! I see someone being robbed!",
            "Send police someone is being robbed",
            "This guy is holding a gun to this guys head trying to take his shit",
            "There is someone trying to rob this person",
            "Holy shit this guy is tryingto rob this person",
            "Theres a maniac mugging someone over here , send police"
        };

        private bool _canMug = true;

        private Random _random = new Random();

        public Mugging()
        {
            Instance = this;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            MuggingLogic();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
        }

        private async Task MuggingLogic()
        {
            while (true)
            {
                if (Game.PlayerPed.IsAiming && Game.IsControlJustPressed(0, Control.Context) && !Game.PlayerPed.IsInVehicle() && !Police.Instance.IsOnDuty)
                {
                    if (_canMug)
                    {
#pragma warning disable CS0219 // The variable 'targetId' is assigned but its value is never used
                        var targetId = 0;
#pragma warning restore CS0219 // The variable 'targetId' is assigned but its value is never used
                        Ped target = Utility.Instance.ClosestPed;
                        if (target.Exists() && !Hunting.Instance.Huntable(target) && Utility.Instance.GetDistanceBetweenVector3s(Game.PlayerPed.Position, target.Position) < 6)
                        {
                            if (Police.Instance.CopCount >= 1)
                            {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                                StartRobbery(target);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
                            }
                            else
                            {
                                Utility.Instance.SendChatMessage("[Mugging]", "Not enough police on.", 255, 0, 0);
                            }
                        }
                        else
                        {
                            Utility.Instance.SendChatMessage("[Mugging]", "Can only do this once every 5 minutes!", 255, 0, 0);
                        }
                    }
                }
                await Delay(0);
            }
        }

        private async Task StartRobbery(Ped target)
        {
            _canMug = false;
            _isMugging = true;
            _targetedPed = target;
            _mugTimeLeft = _random.Next(_mugTimeMin, _mugTimeMax);
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            mugTimeUI();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed. Consider applying the 'await' operator to the result of the call.
            async Task mugTimeUI()
            {
                while (_isMugging)
                {
                    Utility.Instance.DrawTxt(0.5f, 0.1f, 0, 0, 1, "MUGGIN THE LOCAL! " + _mugTimeLeft + " Seconds Left!", 255, 0, 0, 255, true);
                    await Delay(0);
                }
            }

            var chance = _random.Next(0, 2);
            TriggerEvent("911CallClientAnonymous", _callMessages[_random.Next(0, _callMessages.Count)]);
            if (chance == 1)
            {
            }

            var amount = _mugTimeLeft;
            for (int i = 0; i < amount; i++)
            {
                if (Game.PlayerPed.IsAiming && Utility.Instance.GetDistanceBetweenVector3s(Game.PlayerPed.Position, target.Position) < 8)
                {
                    _mugTimeLeft = _mugTimeLeft - 1;
                    target.Task.PlayAnimation("random@mugging3", "handsup_standing_base");
                    target.IsPositionFrozen = true;
                    await Delay(1000);
                }
                else
                {
                    CancelRobbery();
                    return;
                }
            }
            Debug.WriteLine(Convert.ToString(_mugTimeLeft));
            _isMugging = false;
            target.IsPositionFrozen = false;
            target.Task.ClearAll();
            TriggerServerEvent("MuggingReward");
            _canMug = false;
            await Delay(150000);
            _canMug = true;
        }

        private void CancelRobbery()
        {
            _isMugging = false;
            _targetedPed.Task.ClearAll();
            _targetedPed.IsPositionFrozen = false;
            _canMug = true;
        }
    }
}
