﻿using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using roleplay.Main.Users;

namespace roleplay.Main
{
    public class UserManager : BaseScript
    {
        public static UserManager Instance;
        public UserManager()
        {
            Instance = this;
            SetupEvents();
        }

        private  List<User> _activeUsers = new List<User>();

        private void SetupEvents()
        {
            EventHandlers["roleplay:setFirstSpawn"] += new Action<Player>(Spawned);
        }

        //Handles all the code for first spawn.
        private List<Player> _playersFirstSpawned = new List<Player>();
        private void Spawned([FromSource] Player source)
        {
            TriggerEvent("roleplay:firstspawn");
            LoadUser(source);
        }

        private void LoadUser(Player player)
        {
            if (player != null)
            {
                var steamid = player.Identifiers["steam"];
                var license = player.Identifiers["license"];
                var tmpUser = new User();
                var data = DatabaseManager.Instance.StartQuery("SELECT * FROM USERS WHERE steam = '" + steamid + "'");

                while (data.Read())
                {
                    if (data["perms"] != null)
                    {
                        tmpUser.Source = player;
                        tmpUser.License = license;
                        tmpUser.SteamId = steamid;
                        tmpUser.Permissions = Convert.ToInt32(data["perms"]);
                        DatabaseManager.Instance.EndQuery(data);
                        _activeUsers.Add(tmpUser);
                        if (tmpUser.Permissions > 0)
                        {
                            Admin.Instance.ActiveAdmins.Add(player);
                            TriggerClientEvent(tmpUser.Source,"setAsAdmin");
                        }
                        Utility.Instance.Log("Loaded Player [ "+player.Name+" ]");
                        tmpUser.LoadCharacters();
                        return;
                    }
                }

                DatabaseManager.Instance.EndQuery(data);

                tmpUser.Source = player;
                tmpUser.License = license;
                tmpUser.SteamId = steamid;
                tmpUser.Permissions = 0;
                _activeUsers.Add(tmpUser);
                Utility.Instance.Log("Player Did Not Exist, Created New User [ " + player.Name + " ]");
                DatabaseManager.Instance.Execute(
                    "INSERT INTO USERS (steam,license,perms,whitelisted,banned) VALUES('" + steamid + "','" + license +
                    "',0,0,0);");
                return;

            }
        }

        public User GetUserFromPhoneNumber(string number)
        {
            foreach (var ply in new PlayerList())
            {
                var user = GetUserFromPlayer(ply);
                if (user.CurrentCharacter != null && user.CurrentCharacter.PhoneNumber==number)
                {
                    return user;
                }
            }
            return null;
        }

        public User GetUserFromPlayer(Player player)
        {
            foreach (User user in _activeUsers)
            {
                if (user.Source.Name == player.Name)
                {
                    return user;
                }
            }
            return null;
        }
        public User GetUserFromPlayer(string name)
        {
            foreach (User user in _activeUsers)
            {
                if (user.Source.Name == name)
                {
                    return user;
                }
            }
            return null;
        }

        public void RemoveUserByPlayer(Player player)
        {
            foreach (User user in _activeUsers)
            {
                if (user.Source.Name == player.Name)
                {
                    _activeUsers.Remove(user);
                    return;
                }
            }
        }

    }
}
