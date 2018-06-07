﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using roleplay.Main.Vehicles;
using CitizenFX.Core;
using System.Reflection;
using CitizenFX.Core.Native;
using System.Dynamic;
using roleplay.Main.Users;

namespace roleplay.Main
{

    public class VehicleKeysItem : Item
    {

    }
    public class VehicleManager:BaseScript
    {
        public static VehicleManager Instance;

        public VehicleManager()
        {
            Instance = this;
            LoadVehicles();
            EventHandlers["BuyVehicle"] += new Action<Player, string, string>(BuyVehicle);
            EventHandlers["SetCarStatus"] += new Action<Player, string, int>(SetCarStatus);
            EventHandlers["PullCarRequest"] += new Action<Player, string>(PullCarRequest);
        }

        #region Vehicle Prices
        private readonly Dictionary<string,int> _vehiclePrices = new Dictionary<string,int>()
        {
            {"Huntley",0}
        };
        #endregion

        public Dictionary<string,Vehicle> LoadedVehicles = new Dictionary<string, Vehicle>();

        private async void LoadVehicles()
        {
            while (Utility.Instance == null)
            {
                await Delay(1);
            }
            var data = DatabaseManager.Instance.StartQuery("SELECT * FROM VEHICLES");
            while (data.Read())
            {
                var veh = JsonConvert.DeserializeObject<Vehicle>(Convert.ToString(data["vehicle"]));
                if(veh.Status==VehicleStatuses.Missing)
                {
                    veh.Status = VehicleStatuses.Stored;
                }
                LoadedVehicles.Add(Convert.ToString(veh.Plate),veh);
                LoadedVehicles[veh.Plate].id = Convert.ToInt32(data["id"]);
                ItemManager.Instance.DynamicCreateItem(veh.Name+"-"+veh.Plate,"Vehicle Keys",0,0,0,false );
            }
            Utility.Instance.Log("Loaded Vehicles");
            DatabaseManager.Instance.EndQuery(data);
        }

        private void BuyVehicle([FromSource] Player ply, string name, string model)
        {

            var user = UserManager.Instance.GetUserFromPlayer(ply);
            var chara = user.CurrentCharacter;
            var price = _vehiclePrices[name];
            if (MoneyManager.Instance.GetMoney(ply, MoneyTypes.Cash) >= price)
            {
                MoneyManager.Instance.RemoveMoney(ply,MoneyTypes.Cash,price);
                var plate = GetRandomPlateThatsNotTaken();
                var vehicle = new Vehicle(name, model, VehicleStatuses.Stored, price, plate);
                vehicle.RegisteredOwner = chara.FirstName+" "+chara.LastName;
                var item = ItemManager.Instance.DynamicCreateItem(vehicle.Name + "-" + vehicle.Plate, "Vehicle Keys", 0, 0, 0, false);
                LoadedVehicles.Add(vehicle.Plate, vehicle);
                InventoryManager.Instance.AddItem(item.Id, 1, ply);
                DatabaseManager.Instance.Execute("INSERT INTO VEHICLES (vehicle) VALUES('" + JsonConvert.SerializeObject(vehicle) + "');");
                Debug.WriteLine(ItemManager.Instance.LoadedItems[item.Id].Name);
                Utility.Instance.Log(ply.Name + " bought a vehicle! [" + name + "]");
            }
            else if (MoneyManager.Instance.GetMoney(ply, MoneyTypes.Bank) >= price)
            {
                MoneyManager.Instance.RemoveMoney(ply, MoneyTypes.Bank, price);
                var plate = GetRandomPlateThatsNotTaken();
                var vehicle = new Vehicle(name, model, VehicleStatuses.Stored, price, plate);
                vehicle.RegisteredOwner = chara.FirstName + " " + chara.LastName;
                var item = ItemManager.Instance.DynamicCreateItem(vehicle.Name + "-" + vehicle.Plate, "Vehicle Keys", 0, 0, 0, false);
                LoadedVehicles.Add(vehicle.Plate, vehicle);
                InventoryManager.Instance.AddItem(item.Id, 1, ply);
                DatabaseManager.Instance.Execute("INSERT INTO VEHICLES (vehicle) VALUES('" + JsonConvert.SerializeObject(vehicle) + "');");
                Utility.Instance.Log(ply.Name + " bought a vehicle! [" + name + "]");
            }
            else
            {
                return;
            }
        }


        private void PullCarRequest([FromSource] Player ply, string plate)
        {
            var user = UserManager.Instance.GetUserFromPlayer(ply);
            var chara = user.CurrentCharacter;
            var name = chara.FirstName + " " + chara.LastName;
            Debug.WriteLine(plate);
            if (LoadedVehicles.ContainsKey(plate))
            {
                var veh = LoadedVehicles[plate];
                if (veh.RegisteredOwner == name && veh.Status == VehicleStatuses.Stored)
                {
                    veh.Status = VehicleStatuses.Missing;
                    LoadedVehicles[plate] = veh;

                    dynamic obj = new ExpandoObject();
                    obj.Name = veh.Name;
                    obj.Model = veh.Model;
                    obj.RegisteredOwner = veh.RegisteredOwner;
                    obj.Price = veh.Price;
                    obj.SellPrice = veh.SellPrice;
                    obj.InsurancePrice = veh.InsurancePrice;
                    obj.Insured = veh.Insured;
                    obj.Plate = veh.Plate;
                    obj.ColorPrimary = veh.ColorPrimary;
                    obj.ColorSecondary = veh.ColorSecondary;
                    obj.FrontNeons = veh.FrontNeons;
                    obj.BackNeons = veh.BackNeons;
                    obj.LeftNeons = veh.LeftNeons;
                    obj.RightNeons = veh.RightNeons;
                    obj.WindowTint = veh.WindowTint;
                    obj.WheelType = veh.WheelType;
                    obj.BulletProof = veh.BulletProof;
                    obj.VehicleMod1 = veh.VehicleMod1;
                    obj.VehicleMod2 = veh.VehicleMod2;
                    obj.VehicleMod3 = veh.VehicleMod3;
                    obj.VehicleMod4 = veh.VehicleMod4;
                    obj.VehicleMod5 = veh.VehicleMod5;
                    obj.VehicleMod6 = veh.VehicleMod6;
                    obj.VehicleMod7 = veh.VehicleMod7;
                    obj.VehicleMod8 = veh.VehicleMod8;
                    obj.VehicleMod9 = veh.VehicleMod9;
                    obj.VehicleMod10 = veh.VehicleMod10;
                    obj.VehicleMod11 = veh.VehicleMod11;
                    obj.VehicleMod12 = veh.VehicleMod12;
                    obj.VehicleMod13 = veh.VehicleMod13;
                    obj.VehicleMod14 = veh.VehicleMod14;
                    obj.VehicleMod15 = veh.VehicleMod15;
                    obj.VehicleMod16 = veh.VehicleMod16;
                    obj.VehicleMod17 = veh.VehicleMod17;
                    obj.VehicleMod18 = veh.VehicleMod18;
                    obj.VehicleMod19 = veh.VehicleMod19;
                    obj.VehicleMod20 = veh.VehicleMod20;
                    obj.VehicleMod21 = veh.VehicleMod21;
                    obj.VehicleMod22 = veh.VehicleMod22;
                    obj.VehicleMod23 = veh.VehicleMod23;
                    obj.VehicleMod24 = veh.VehicleMod24;
                    obj.VehicleMod25 = veh.VehicleMod25;
                    obj.VehicleMod26 = veh.VehicleMod26;
                    obj.VehicleMod27 = veh.VehicleMod27;
                    obj.VehicleMod28 = veh.VehicleMod28;
                    obj.VehicleMod29 = veh.VehicleMod29;
                    obj.VehicleMod30 = veh.VehicleMod30;
                    obj.VehicleMod31 = veh.VehicleMod31;
                    obj.VehicleMod32 = veh.VehicleMod32;
                    obj.VehicleMod33 = veh.VehicleMod33;
                    obj.VehicleMod34 = veh.VehicleMod34;
                    obj.VehicleMod35 = veh.VehicleMod35;
                    obj.VehicleMod36 = veh.VehicleMod36;
                    obj.VehicleMod37 = veh.VehicleMod37;
                    obj.VehicleMod38 = veh.VehicleMod38;
                    obj.VehicleMod39 = veh.VehicleMod39;
                    obj.VehicleMod40 = veh.VehicleMod40;
                    obj.VehicleMod41 = veh.VehicleMod41;
                    obj.VehicleMod42 = veh.VehicleMod42;
                    obj.VehicleMod43 = veh.VehicleMod43;
                    obj.VehicleMod44 = veh.VehicleMod44;
                    obj.VehicleMod45 = veh.VehicleMod45;
                    obj.VehicleMod46 = veh.VehicleMod46;
                    obj.VehicleMod47 = veh.VehicleMod47;
                    obj.VehicleMod48 = veh.VehicleMod48;
                    obj.VehicleMod49 = veh.VehicleMod49;
                    obj.VehicleMod50 = veh.VehicleMod50;

                    TriggerClientEvent(ply,"PullCar", obj);
                }
                else if (veh.RegisteredOwner == name && veh.Status == VehicleStatuses.Missing)
                {
                    LoadedVehicles[plate] = veh;
                    TriggerClientEvent(ply,"PutAwayCar",veh.Plate);
                }
            }
        }

        private void SetCarStatus([FromSource] Player ply, string plate, int status)
        {
            var user = UserManager.Instance.GetUserFromPlayer(ply);
            if (user.CurrentCharacter.FirstName + " " + user.CurrentCharacter.LastName ==
                LoadedVehicles[plate].RegisteredOwner)
            {
                LoadedVehicles[plate].Status = (VehicleStatuses)status;
            }
        }

        public void ChangeVehicleOwner(string plate, string name)
        {
            var veh = LoadedVehicles[plate];
            veh.RegisteredOwner = name;
            LoadedVehicles[plate] = veh;
            DatabaseManager.Instance.Execute("UPDATE VEHICLES SET vehicle='"+JsonConvert.SerializeObject(veh)+"' WHERE id="+veh.id+";");
        }

        private string GetRandomPlateThatsNotTaken()
        {
            var plate = "";
            var taken = true;

            while (taken)
            {
                plate = Utility.Instance.RandomString(8);
                foreach (var plateTmp in LoadedVehicles.Keys)
                {
                    if (plateTmp == plate)
                    {
                        taken = true;
                        break;
                    }
                    taken = false;
                }
                taken = false;
            }
            return plate;
        }

    }
}
