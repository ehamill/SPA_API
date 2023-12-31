﻿namespace MAWcore6.Data
{
    public enum Terrain { 
        None = 0,
        Npc = 1,
        Field = 2,//flat, desert..etc??
    }
    public enum BuildingType
    {
        Empty = 0,
        Academy = 1,
        Barrack = 2,
        Beacon_Tower = 3,
        Cottage = 4,
        Embassy = 5,
        Feasting_Hall = 6,
        Forge = 7,
        Inn = 8,
        Marketplace = 9,
        Rally_Spot = 10,
        Relief_Station = 11,
        Stable = 12,
        Town_Hall = 13,
        Warehouse = 14,
        Workshop = 15,
        Farm = 16,
        //Iron_Mill = 17,
        Sawmill = 18,
        Iron_Mine = 19,
        Quarry = 20,
        Walls = 21,
        Not_Found = 50,
    }


    //public enum CityBuildingType
    //{
    //    Empty = 0,
    //    Academy = 1,
    //    Barrack = 2,
    //    Beacon_Tower=3,
    //    Cottage = 4,
    //    Embassy = 5,
    //    Feasting_Hall = 6,
    //    Forge = 7,
    //    Inn = 8,
    //    Marketplace = 9,
    //    Rally_Spot = 10,
    //    Relief_Station = 11,
    //    Stable = 12,
    //    Town_Hall = 13,
    //    Warehouse=14,
    //    Workshop = 15,
    //    Farm = 16,
    //    Iron_Mill = 17,
    //    Sawmill = 18,
    //    Iron_Mine = 19,
    //    Quarry = 20,
    //    Not_Found = 21,
    //}
    public enum FarmBuildingType
    {
        Empty = 0,
        Farm = 1,
        Iron_Mill = 2,
        Sawmill = 3,
        Iron_Mine = 4,
        Quarry = 5,
    }
    public enum TroopType
    {
        Worker = 1,
        Warrior = 2,
        Pikeman = 3,
        Swordsman = 4,
        Archer = 5,
        Battering_Ram = 6,
        Scout = 7,
        Cavalry = 8,
        Cataphract = 9,
        Transporter = 10,
        Ballista = 11,
        Catapult = 12,
        Trap = 13,
        Abatis = 14,
        Archers_Tower = 15,
        Rolling_Log = 16,
        Defensive_Trebuchet = 17
    }
    public enum CreateTerrain
    {
        None = 0,
        Npc = 1,
        Flat = 2,
        Grassland = 3,
        Swamp = 4,
        Lake = 5,
        Hill = 6,
        Desert = 7,
        //Flat = 8,
        //Flat = 9,
    }

}
