using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using MAWcore6.Data;
using MAWcore6.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;


namespace MAWcore6.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
        //public CityController()
        //{
        //}
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CityController(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {
            db = _db;
            _userManager = userManager;
        }

        public class Troop { 
            public string TypeString { get; set; } = "";
            public TroopType TypeInt { get; set; } 
            public string PreReq { get; set; } = "";
            public bool ReqMet { get; set; } = false;
            public string Description { get; set; } = "";
            public int Qty { get; set; } = 0;
            public int FoodCost { get; set; } = 0;
            public int StoneCost { get; set; } = 0;
            public int WoodCost { get; set; } = 0;
            public int IronCost { get; set; } = 0;
            public int TimeCost { get; set; } = 0;
            public bool ForWalls { get; set; } = false;
            public int Attack { get; set; } = 0;
            public int Defense { get; set; } = 0;
            public int Speed { get; set; } = 0;
            public int Load { get; set; } = 0;
            public int Life { get; set; } = 0;
            public int Range { get; set; } = 0;
            public string Image { get; set; } = "missing.jpg";
        }

        private List<Troop> GetTroops(City city,UserResearch research) {
            List<Troop> Troops = new List<Troop>();

            var Worker = new Troop()
            {
                TypeString = TroopType.Worker.ToString(),
                TypeInt = TroopType.Worker,
                PreReq = "Requires Barrack level 1.",
                ReqMet = true,
                Description = "Workers are cheap transporters. Not very good for fighting.",
                Qty = city.WorkerQty,
                FoodCost = Constants.WorkerFoodCost,
                StoneCost = 0,
                WoodCost = Constants.WorkerWoodCost,
                IronCost = Constants.WorkerIronCost,
                TimeCost = Constants.WorkerTimeCost,
                Attack = Constants.WorkerAttk,
                Defense = Constants.WorkerDef,
                Life = Constants.WorkerLife,
                Range = Constants.WorkerRange,
                Load = Constants.WorkerLoad,
                Speed = Constants.WorkerSpeed,
            };
            Troops.Add(Worker);
            var Warrior = new Troop()
            {
                TypeString = TroopType.Warrior.ToString(),
                TypeInt = TroopType.Warrior,
                PreReq = "Requires Barrack level 1.",
                ReqMet = true,
                Description = "Workers are cheap transporters. Not very good for fighting.",
                Qty = city.WarriorQty,
                FoodCost = Constants.WarrFoodCost,
                StoneCost = 0,
                WoodCost = Constants.WarrWoodCost,
                IronCost = Constants.WarrIronCost,
                TimeCost = Constants.WarrTimeCost,
                Attack = Constants.WarrAttk,
                Defense = Constants.WarrDef,
                Life = Constants.WarrLife,
                Range = Constants.WarrRange,
                Load = Constants.WarrLoad,
                Speed = Constants.WarrSpeed,
                Image = "Warrior.jpg",
            };
            Troops.Add(Warrior);
            var Scout = new Troop()
            {
                TypeString = TroopType.Scout.ToString(),
                TypeInt = TroopType.Scout,
                PreReq = "Requires Barrack level 2.",
                ReqMet = true,
                Description = "Workers are cheap transporters. Not very good for fighting.",
                Qty = city.WarriorQty,
                FoodCost = Constants.ScoutFoodCost,
                StoneCost = 0,
                WoodCost = Constants.ScoutWoodCost,
                IronCost = Constants.ScoutIronCost,
                TimeCost = Constants.ScoutTimeCost,
                Attack = Constants.ScoutAttk,
                Defense = Constants.ScoutDef,
                Life = Constants.ScoutLife,
                Range = Constants.ScoutRange,
                Load = Constants.ScoutLoad,
                Speed = Constants.ScoutSpeed,
                Image = "Scout.jpg",
            };
            Troops.Add(Scout);
            var Pike = new Troop()
            {
                TypeString = TroopType.Pikeman.ToString(),
                TypeInt = TroopType.Pikeman,
                PreReq = "Requires Barracks level 2 and Military Tradition Level 1.",
                ReqMet = true,
                Description = "Great range.",
                Qty = city.ArcherQty,
                FoodCost = Constants.PikeFoodCost,
                StoneCost = 0,
                WoodCost = Constants.PikeWoodCost,
                IronCost = Constants.PikeIronCost,
                TimeCost = Constants.PikeTimeCost,
                Attack = Constants.PikeAttk,
                Defense = Constants.PikeDef,
                Life = Constants.PikeLife,
                Range = Constants.PikeRange,
                Load = Constants.PikeLoad,
                Speed = Constants.PikeSpeed,
                Image = "Pikeman.jpg",
            };
            Troops.Add(Pike);

            var Archer = new Troop()
            {
                TypeString = TroopType.Archer.ToString(),
                TypeInt = TroopType.Archer,
                PreReq = "Requires Barracks level 4 and Archery level 1",
                ReqMet = true,
                Description = "Great range.",
                Qty = city.ArcherQty,
                FoodCost = Constants.ArchFoodCost,
                StoneCost = 0,
                WoodCost = Constants.ArchWoodCost,
                IronCost = Constants.ArchIronCost,
                TimeCost = Constants.ArchTimeCost,
                Attack = Constants.ArchAttk,
                Defense = Constants.ArchDef,
                Life = Constants.ArchLife,
                Range = Constants.ArchRange,
                Load = Constants.ArchLoad,
                Speed = Constants.ArchSpeed,
                Image = "Archer.jpg",
            };
            Troops.Add(Archer);
            var Ballista = new Troop()
            {
                TypeString = TroopType.Ballista.ToString(),
                TypeInt = TroopType.Ballista,
                PreReq = "Requires Barracks level 4 and Archery level 1",
                ReqMet = true,
                Description = "Great range.",
                Qty = city.BallistaQty,
                FoodCost = Constants.BallFoodCost,
                StoneCost = 0,
                WoodCost = Constants.BallWoodCost,
                IronCost = Constants.BallIronCost,
                TimeCost = Constants.BallTimeCost,
                Attack = Constants.BallAttk,
                Defense = Constants.BallDef,
                Life = Constants.BallLife,
                Range = Constants.BallRange,
                Load = Constants.BallLoad,
                Speed = Constants.BallSpeed,
                Image = "Ballista.jpg",
            };
            Troops.Add(Ballista);
            return Troops;
        
        }
        private List<BuildingCost> GetCostOfTroops(City userCity, UserResearch userResearch)
        {
            var cost = new List<BuildingCost>();

            var worker = new BuildingCost()
            {
                type = TroopType.Worker.ToString(),
                preReq = Constants.WorkerBuildReq,
                reqMet = false,
                food = Constants.WorkerFoodCost,
                stone = 0,
                wood = Constants.WorkerWoodCost,
                iron = Constants.WorkerIronCost,
                time = Constants.WorkerTimeCost,
            };
            cost.Add(worker);

            var warr = new BuildingCost()
            {
                type = TroopType.Warrior.ToString(),
                preReq = Constants.WarrBuildReq,
                reqMet = false,
                food = Constants.WarrFoodCost,
                stone = 0,
                wood = Constants.WarrWoodCost,
                iron = Constants.WarrIronCost,
                time = Constants.WarrTimeCost,
            };
            cost.Add(warr);

            var scout = new BuildingCost()
            {
                type = TroopType.Scout.ToString(),
                preReq = Constants.ScoutBuildReq,
                reqMet = false,
                food = Constants.ScoutFoodCost,
                stone = 0,
                wood = Constants.ScoutWoodCost,
                iron = Constants.ScoutIronCost,
                time = Constants.ScoutTimeCost,
            };
            cost.Add(warr);

            return cost;
        }


        [HttpGet]
        public async Task<JsonResult> Get()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var u = await _userManager.FindByIdAsync(UserId);
            City UserCity = new City();
            try
            {
                UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? await CreateCity(UserId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            UserItems UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
            UserResearch userResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
            List<BuildingCost> ListOfBuildingsCost = GetNewBuildingsCost(UserCity, userResearch);
            List<Troop> Troops = GetTroops(UserCity, userResearch);
            //await CheckBuilder1(UserCity);
            if (UserCity.Builder1Busy) {
                await CheckBuilder1(UserCity);
            }
            //GetUpgradeBuildings..only need one for each, can calculate costs off of it

            return new JsonResult(new { city = UserCity,troops = Troops, userItems = UserItems, userResearch = userResearch, newBuildingsCost = ListOfBuildingsCost });
        }

        public async Task CheckBuilder1(City userCity) {
            DateTime TimeNow = DateTime.UtcNow;
            DateTime ConstructionEnds = userCity.Construction1Ends;

            if (userCity.Builder1Busy == false) {
                return;
            }
            //Can be busy in number of ways:
            // 1. Click build a building, and Construction has not ended yet. recalculate time.
            // 2. Building was done a long time ago. TimeNow > ConstructionEnds
            if (TimeNow >= ConstructionEnds && userCity.Construction1BuildingId != -1)
            {
                userCity.Builder1Busy = false;
                
                var b = userCity.Buildings.Where(c => c.BuildingId == userCity.Construction1BuildingId).FirstOrDefault();
                if (b.Level - userCity.Construction1BuildingLevel > 0) { 
                    //downgrading.. add res
                }

                b.Level = userCity.Construction1BuildingLevel;
                BuildingType BuildingType = GetBuildingType(userCity.BuildingWhat);
                if (b.Level == 0) {
                    BuildingType = BuildingType.Empty;
                }
                b.Image = BuildingType.ToString() + "lvl" + b.Level +".jpg";
                userCity.Construction1BuildingId = -1; //Building Complete.
                userCity.Construction1BuildingLevel = -1;
                userCity.Builder1Time = -1;
            }
            else {
                userCity.Builder1Time = Convert.ToInt32(Math.Floor((ConstructionEnds - TimeNow).TotalSeconds));
            }
            await db.SaveChangesAsync();
        }

        public class UpdateCityModel
        {
            public int cityId { get; set; }
            public int buildingId { get; set; }
            public string buildingType { get; set; }
            public int level { get; set; }
            public int location { get; set; } = -1;
        }

        public class TroopPreReqCheck { 
            public string buildingType { get; set; }
            public bool reqMet { get; set; }
        }

        private List<BuildingCost> GetCostOfTroops555(City userCity, UserResearch userResearch) { 
            var cost = new List<BuildingCost>();

            var worker = new BuildingCost()
            {
                type = TroopType.Worker.ToString(),
                preReq = Constants.WorkerBuildReq,
                reqMet = false,
                food = Constants.WorkerFoodCost,
                stone = 0,
                wood = Constants.WorkerWoodCost,
                iron = Constants.WorkerIronCost,
                time = Constants.WorkerTimeCost,
            };
            cost.Add(worker);
            
            var warr = new BuildingCost()
            {
                type = TroopType.Warrior.ToString(),
                preReq = Constants.WarrBuildReq,
                reqMet = false,
                food = Constants.WarrFoodCost,
                stone = 0,
                wood = Constants.WarrWoodCost,
                iron = Constants.WarrIronCost,
                time  = Constants.WarrTimeCost,
            };
            cost.Add(warr);

            var scout = new BuildingCost()
            {
                type = TroopType.Scout.ToString(),
                preReq = Constants.ScoutBuildReq,
                reqMet = false,
                food = Constants.ScoutFoodCost,
                stone = 0,
                wood = Constants.ScoutWoodCost,
                iron = Constants.ScoutIronCost,
                time = Constants.ScoutTimeCost,
            };
            cost.Add(warr);

            return cost;
        }
        private BuildingCost GetUpgradeCostOfBuilding(UpdateCityModel update,City userCity, UserResearch userResearch)
        {
            var building = userCity.Buildings.Where(c => c.BuildingId == update.buildingId).FirstOrDefault();
            //if downgrading, time is current level.
            var level = update.level;
            if (building.Level > update.level)
            {
                level++;
            }
            BuildingCost bc = new BuildingCost();

            BuildingType BuildingType = GetBuildingType(update.buildingType);
            switch (BuildingType)
            {
                case BuildingType.Academy:
                    bc.type = BuildingType.Academy.ToString(); ;
                    bc.food = Constants.AcademyFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.AcademyStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.AcademyWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.AcademyIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.AcademyTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;

                case BuildingType.Barrack:
                    bc.type = BuildingType.Barrack.ToString(); ;
                    bc.food = Constants.BarrFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.BarrStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.BarrWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.BarrIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.BarrTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Cottage:
                    bc.type = BuildingType.Cottage.ToString(); 
                    bc.food = Constants.CottFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.CottStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.CottWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.CottIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.CottTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Feasting_Hall:
                    bc.type = BuildingType.Feasting_Hall.ToString().Replace("_"," "); 
                    bc.food = Constants.FeastFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.FeastStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.FeastWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.FeastIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.FeastTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Forge:
                    bc.type = BuildingType.Forge.ToString(); 
                    bc.food = Constants.ForgeFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.ForgeStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.ForgeWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.ForgeIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.ForgeTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Inn:
                    bc.type = BuildingType.Inn.ToString(); 
                    bc.food = Constants.InnFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.InnStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.InnWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.InnIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.InnTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Rally_Spot:
                    bc.type = BuildingType.Rally_Spot.ToString().Replace("_", " "); 
                    bc.food = Constants.RallyFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.RallyStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.RallyWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.RallyIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.RallyTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Town_Hall:
                    bc.type = BuildingType.Town_Hall.ToString(); 
                    bc.food = Constants.ThFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.ThStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.ThWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.ThIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.ThTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Walls: 
                    bc.type = BuildingType.Walls.ToString(); 
                    bc.food = Constants.ThFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.ThStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.ThWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.ThIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.ThTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;

            }
            return bc;

        }
        
        private string CheckIfBuildingPreReqMet(City city, UpdateCityModel update) {
            string res =  "ok";
            //var updateBuilding = city.Buildings.Where(c => c.BuildingId == update.buildingId).FirstOrDefault();
            BuildingType buildingType = GetBuildingType(update.buildingType);
            //No building can be more than one level greater than th.
            var th = city.Buildings.Where(c => c.BuildingType == BuildingType.Town_Hall).FirstOrDefault();
            
            if (buildingType == BuildingType.Academy)
            {
                if (th.Level < 2)
                {
                    res = "Requires Town Hall level 2";
                }
            } else if (buildingType == BuildingType.Barrack)
            {
                var RallySpot = city.Buildings.Where(c => c.BuildingType == BuildingType.Rally_Spot).FirstOrDefault();
                if (RallySpot == null)
                {
                    res = "Must build a RallySpot.";
                }
            } else if (buildingType == BuildingType.Cottage)
            {
                if (update.level - 1 > th.Level)
                {
                    res = "Need to upgrade the Town Hall.";
                }
            } else if (buildingType == BuildingType.Inn) { 
                var cottageLvl2 = city.Buildings.Where(c => c.BuildingType == BuildingType.Cottage && c.Level >=2 ).FirstOrDefault();
                if (cottageLvl2 == null) {
                    res = "Must build a Cottage to level 2.";
                }
            }
            else if (buildingType == BuildingType.Town_Hall)
            {
                //Req quary lvl2 and forge lvl1
                int wallsLevel = city.Buildings.Where(c => c.BuildingType == BuildingType.Walls).Select(c => c.Level).FirstOrDefault();
                if (th.Level - wallsLevel >= 2)
                {
                    res = "Must upgrade walls first.";
                }
            } else if (buildingType == BuildingType.Walls)
            {
                //Req quary lvl2 and forge lvl1
                int highestLvlQuarry = city.Buildings.Where(c => c.BuildingType == BuildingType.Quarry).Max(c => c.Level);
                int forgeCount = city.Buildings.Where(c => c.BuildingType == BuildingType.Forge).Count();
                if (highestLvlQuarry < 2)
                {
                    res += "Requires Quarry level 2. ";
                } 
                if (forgeCount == 0)
                {
                    res += "Requires Forge level 1.";
                }
            }
            return res;
        }

        public class MakeTroopsModel
        {
            public int cityId { get; set; }
            public int buildingId { get; set; }
            public string troopType { get; set; } = "";
            public int Qty { get; set; }
        }

        private string CheckIfTroopPreReqMet(City city, MakeTroopsModel update)
        {
            //Troops req barrack lvl and research
            string res = "ok";
            //var updateBuilding = city.Buildings.Where(c => c.BuildingId == update.buildingId).FirstOrDefault();
            BuildingType buildingType = new BuildingType(); // GetTroopType(update.troopType);
            //No building can be more than one level greater than th.
            var th = city.Buildings.Where(c => c.BuildingType == BuildingType.Town_Hall).FirstOrDefault();

            if (buildingType == BuildingType.Academy)
            {
                if (th.Level < 2)
                {
                    res = "Requires Town Hall level 2";
                }
            }
            else if (buildingType == BuildingType.Barrack)
            {
                var RallySpot = city.Buildings.Where(c => c.BuildingType == BuildingType.Rally_Spot).FirstOrDefault();
                if (RallySpot == null)
                {
                    res = "Must build a RallySpot.";
                }
            }
            else if (buildingType == BuildingType.Cottage)
            {
                if (update.Qty - 1 > th.Level)
                {
                    res = "Need to upgrade the Town Hall.";
                }
            }
            else if (buildingType == BuildingType.Inn)
            {
                var cottageLvl2 = city.Buildings.Where(c => c.BuildingType == BuildingType.Cottage && c.Level >= 2).FirstOrDefault();
                if (cottageLvl2 == null)
                {
                    res = "Must build a Cottage to level 2.";
                }
            }
            else if (buildingType == BuildingType.Town_Hall)
            {
                //Req quary lvl2 and forge lvl1
                int wallsLevel = city.Buildings.Where(c => c.BuildingType == BuildingType.Walls).Select(c => c.Level).FirstOrDefault();
                if (th.Level - wallsLevel >= 2)
                {
                    res = "Must upgrade walls first.";
                }
            }
            else if (buildingType == BuildingType.Walls)
            {
                //Req quary lvl2 and forge lvl1
                int highestLvlQuarry = city.Buildings.Where(c => c.BuildingType == BuildingType.Quarry).Max(c => c.Level);
                int forgeCount = city.Buildings.Where(c => c.BuildingType == BuildingType.Forge).Count();
                if (highestLvlQuarry < 2)
                {
                    res += "Requires Quarry level 2. ";
                }
                if (forgeCount == 0)
                {
                    res += "Requires Forge level 1.";
                }
            }
            return res;
        }


        [HttpPost("BuildingDone")]
        public async Task<JsonResult> BuildingDone([FromBody] UpdateCityModel update)
        {
            var message = "ok";

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == update.cityId).FirstOrDefaultAsync() ?? new City();
            UserResearch UserResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserResearch();

            //Check if builders busy ..
            await CheckBuilder1(UserCity);

            List<BuildingCost> ListOfNewBuildingsCost = GetNewBuildingsCost(UserCity, UserResearch);
            
            return new JsonResult(new { message = message, city = UserCity, newBuildingsCost = ListOfNewBuildingsCost });
        }


        [HttpPost("UpdateCity")]
        public async Task<JsonResult> UpdateCity([FromBody] UpdateCityModel update)
        {
            var message = "ok";
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var u = await _userManager.FindByIdAsync(UserId);

            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == update.cityId).FirstOrDefaultAsync() ?? new City();
            UserResearch UserResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserResearch();

            //Check if builders busy ..
            await CheckBuilder1(UserCity);

            string TestingResult = CheckIfBuildingPreReqMet(UserCity, update);
            if (TestingResult != "ok")
            {
                message = TestingResult;
                return new JsonResult(new { message = message });
            }

            BuildingCost BuildingCost= GetUpgradeCostOfBuilding(update,UserCity, UserResearch);
            
            //Check if user has enough resources ..
            await RemoveResourcesAndUpdateConstructionFromCity(UserCity, update, BuildingCost);
            
            List<BuildingCost> ListOfNewBuildingsCost = GetNewBuildingsCost(UserCity, UserResearch);
            //GetUpgradeBuildings..only need one for each, can calculate costs off of it

            return new JsonResult(new { message = message, city = UserCity, newBuildingsCost = ListOfNewBuildingsCost });
        }

        public class SpeedUpModel
        {
            public int cityId { get; set; }
            public bool speedUp5min { get; set; }
            public string usedOn { get; set; }
        }

        [HttpPost("SpeedUpUsed")]
        public async Task<JsonResult> SpeedUpUsed([FromBody] SpeedUpModel model)
        {
            var message = "ok";
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == model.cityId).FirstOrDefaultAsync() ?? new City();
            //UserResearch UserResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserResearch();
            UserItems UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserItems();

            if (model.usedOn == "builder1") {
                if (model.speedUp5min) {
                    UserCity.Construction1Ends = UserCity.Construction1Ends.AddMinutes(-5);
                    UserItems.FiveMinuteSpeedups--;
                }
                
            }
            await db.SaveChangesAsync();

            //Check if builders busy ..
            await CheckBuilder1(UserCity);

            return new JsonResult(new { message = message, city = UserCity });
        }

        public BuildingType GetBuildingType(string name) {
            if (name.ToLower().Contains("academy"))
            {
                return BuildingType.Academy;
            }
            else if (name.ToLower().Contains("beacon"))
            {
                return BuildingType.Beacon_Tower;
            }
            else if (name.ToLower().Contains("barrack"))
            {
                return BuildingType.Barrack;
            }
            else if (name.ToLower().Contains("cottage"))
            {
                return BuildingType.Cottage;
            }
            else if (name.ToLower().Contains("empty"))
            {
                return BuildingType.Empty;
            }
            else if (name.ToLower().Contains("inn"))
            {
                return BuildingType.Inn;
            }
            else if (name.ToLower().Contains("rally"))
            {
                return BuildingType.Rally_Spot;
            }
            else if (name.ToLower().Contains("town"))
            {
                return BuildingType.Town_Hall;
            }
            else {
                return BuildingType.Not_Found;
            }

        }

        private async Task RemoveResourcesAndUpdateConstructionFromCity(City UserCity, UpdateCityModel update, BuildingCost BuildingCost) {
           
            BuildingType buildingType = GetBuildingType(update.buildingType);

            var b = UserCity.Buildings.Where(c => c.BuildingId == update.buildingId).FirstOrDefault();
            string upgrading = "upgrading";
            if (update.level - b.Level < 0)
            {
                upgrading = "downgrading";
                //if (update.level == 0) {
                //    buildingType = BuildingType.Empty;
                //}
            }
            if (upgrading == "upgrading") {
                UserCity.Food = UserCity.Food - BuildingCost.food;
                UserCity.Stone = UserCity.Stone - BuildingCost.stone;
                UserCity.Wood = UserCity.Wood - BuildingCost.wood;
                UserCity.Iron = UserCity.Iron - BuildingCost.iron;
            } else
            {
                UserCity.Food = UserCity.Food + BuildingCost.food;
                UserCity.Stone = UserCity.Stone + BuildingCost.stone;
                UserCity.Wood = UserCity.Wood + BuildingCost.wood;
                UserCity.Iron = UserCity.Iron + BuildingCost.iron;
            }

            UserCity.Construction1Started = DateTime.UtcNow;
            UserCity.Construction1Ends = DateTime.UtcNow.AddSeconds(BuildingCost.time);
            UserCity.Construction1BuildingId = update.buildingId;
            UserCity.Construction1BuildingLevel = update.level;

            UserCity.Builder1Busy = true;
            UserCity.Builder1Time = BuildingCost.time;
            

            b.Image = upgrading + buildingType.ToString() +"lvl"+ update.level;
            b.BuildingType = buildingType;
            b.Description = GetBuildingDescription(buildingType);

            UserCity.BuildingWhat = buildingType.ToString();

            await db.SaveChangesAsync();
        }
        public async Task<City> CreateCity(string UserID) {

            City NewCity = new City()
            {
                UserId = UserID,
                ServerId = 1,
                Image = "cityImage.jpg",
                Food = 5000,
                Stone = 5000,
                Wood = 5000,
                Iron = 5000,
                Gold = 5000,
                ResourcesLastUpdated = DateTime.UtcNow,
                Buildings = new List<Building>()
            };
            await db.Cities.AddAsync(NewCity);

            UserItems NewUserItems = new UserItems() {
                UserId = UserID
            };
            await db.UserItems.AddAsync(NewUserItems);

            UserResearch newUR = new UserResearch() { 
                UserId = UserID,
            };
            await db.UserResearch.AddAsync(newUR);

            await db.SaveChangesAsync();

            for (int i = 0; i <= 25; i++) {
                Building NewBuilding = new Building()
                {
                    CityId = NewCity.CityId,
                    Location = i,
                    BuildingType = BuildingType.Empty,
                    Level = 0,
                    Image = "emptyCitySlot.jpg"
                };
                NewCity.Buildings.Add(NewBuilding);
            }
            var building = NewCity.Buildings.Where(c => c.Location == 3).FirstOrDefault();
            building.BuildingType = BuildingType.Town_Hall;
            building.Level = 1;
            building.Image = "TownHalllvl0.jpg";
            building.Description = "Manage your city.";
            building = NewCity.Buildings.Where(c => c.Location == 0).FirstOrDefault();
            building.BuildingType = BuildingType.Walls;
            building.Level = 0;
            building.Image = "Wallslvl0.jpg";
            building.Description = "Walls Protect the city and offer longer range for AT's.";
            await db.SaveChangesAsync();

            return NewCity;
        }

        private string GetBuildingDescription(BuildingType buildingType) {
            switch (buildingType)
            {
                case BuildingType.Academy:
                    return "Academy allows you to research skills.";
                case BuildingType.Barrack:
                    return "Barracks are where you train your troops.";
                case BuildingType.Cottage:
                   return "Cottages increase your cities population allowing you to make more resoures and train more " +
                        "troops";
                case BuildingType.Feasting_Hall:
                    return "Manage your Heros.";
                case BuildingType.Inn:
                    return "The Inn is where you hire heros. The higher the level, the more hero's to choose from"
                        +" and the higher level of hero.";
                case BuildingType.Rally_Spot:
                    return "Place to heal troops. Test fighting. Limits amount of troops you can send at one time.";
                case BuildingType.Town_Hall:
                    return "Manage your city.";
                case BuildingType.Walls:
                    return "Manage your city's defenses.";
                
                default:
                    return "Empty";
            }

        }

        private List<BuildingCost> GetNewBuildingsCost(City userCity ,UserResearch userResearch)
        {
            List<BuildingCost> lbc = new List<BuildingCost>();
            //[Base Building Time] *(0.9) ^[Construction Level]
            int time = Convert.ToInt32( Math.Ceiling(60 * Math.Pow(0.9, userResearch.Building)));

            UpdateCityModel update = new UpdateCityModel()
            {
                buildingType = "Cottage"
            };
            string TestingResult = "ok";
            bool requirementsMet = true;
            if (TestingResult != "ok")
            {
                requirementsMet = false;
            }
            BuildingCost cott = new BuildingCost()
            {
                type = BuildingType.Cottage.ToString(),
                preReq = TestingResult,
                reqMet = requirementsMet,
                food = Constants.CottFoodReq,
                stone = Constants.CottStoneReq,
                wood = Constants.CottWoodReq,
                iron = Constants.CottIronReq,
                time = Constants.CottTimeReq,
                description = "Increases your population",
            };
            lbc.Add(cott);

            int BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Academy).Count();
            if (BuildingCount == 0)
            {
                update.buildingType = "Academy";
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost Academy = new BuildingCost()
                {
                    type = BuildingType.Academy.ToString().Replace("_", " "),
                    preReq = TestingResult,
                    reqMet = requirementsMet,
                    food = Constants.AcademyFoodReq,
                    stone = Constants.AcademyStoneReq,
                    wood = Constants.AcademyWoodReq,
                    iron = Constants.AcademyIronReq,
                    time = Constants.AcademyTimeReq,
                    description = "Learn new skills and do your research."
                };
                lbc.Add(Academy);
            }

            update.buildingType = "Barrack";
            TestingResult = CheckIfBuildingPreReqMet(userCity, update);
            requirementsMet = true;
            if (TestingResult != "ok")
            {
                requirementsMet = false;
            }
            BuildingCost barr = new BuildingCost()
            {
                type = BuildingType.Barrack.ToString(),
                preReq = TestingResult,
                reqMet = requirementsMet,
                food = Constants.BarrFoodReq,
                stone = Constants.BarrStoneReq,
                wood = Constants.BarrWoodReq,
                iron = Constants.BarrIronReq,
                time = Constants.BarrTimeReq,
                description = "Place where you train your troops",
            };
            lbc.Add(barr);

            BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Feasting_Hall).Count();
            if (BuildingCount == 0) {
                update.buildingType = "Feasting";
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost Feast = new BuildingCost()
                {
                    type = BuildingType.Feasting_Hall.ToString().Replace("_", " "),
                    preReq = TestingResult,
                    reqMet = requirementsMet,
                    food = Constants.FeastFoodReq,
                    stone = Constants.FeastStoneReq,
                    wood = Constants.FeastWoodReq,
                    iron = Constants.FeastIronReq,
                    time = Constants.FeastTimeReq,
                    description = "Where your hero's live and are managed."
                };
                lbc.Add(Feast);
            }
            BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Forge).Count();
            if (BuildingCount == 0)
            {
                update.buildingType = "Forge";
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost Forge = new BuildingCost()
                {
                    type = BuildingType.Forge.ToString().Replace("_", " "),
                    preReq = TestingResult,
                    reqMet = requirementsMet,
                    food = Constants.ForgeFoodReq,
                    stone = Constants.ForgeStoneReq,
                    wood = Constants.ForgeWoodReq,
                    iron = Constants.ForgeIronReq,
                    time = Constants.ForgeTimeReq,
                    description = "Improve your iron working skills.",
                };
                lbc.Add(Forge);
            }

            BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Inn).Count();
            if (BuildingCount == 0)
            {
                update.buildingType = "Inn";
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost inn = new BuildingCost()
                {
                    type = BuildingType.Inn.ToString(),
                    preReq = TestingResult,
                    reqMet = requirementsMet,
                    food = Constants.InnFoodReq,
                    stone = Constants.InnStoneReq,
                    wood = Constants.InnWoodReq,
                    iron = Constants.InnIronReq,
                    time = Constants.InnTimeReq,
                    description = "Recruit new heros.",
                };
                lbc.Add(inn);
            }
            BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Rally_Spot).Count();
            if (BuildingCount == 0)
            {
                update.buildingType = "Rally";
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost rally = new BuildingCost()
                {
                    type = BuildingType.Rally_Spot.ToString().Replace("_", " "),
                    preReq = TestingResult,
                    reqMet = requirementsMet,
                    food = Constants.RallyFoodReq,
                    stone = Constants.RallyStoneReq,
                    wood = Constants.RallyWoodReq,
                    iron = Constants.RallyIronReq,
                    time = Constants.RallyTimeReq,
                    description = "Heal troops, test troops, and increases amount of troops you can send.",
                };
                lbc.Add(rally);

            }
            //BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Feasting_Hall).Count();
            //if (BuildingCount == 0)
            //{

            //}
            

            return lbc;

        }
        
        public async Task<City> UpdateResources(City City)
        {
            DateTime Now = DateTime.UtcNow;
            DateTime Before = City.ResourcesLastUpdated;
            TimeSpan diff = Now.Subtract(Before);
            int mins = (int)Math.Floor(diff.TotalMinutes);
            if (mins > 5)
            {
                //Recalculate ResRates based on farms, tech levels, and bonuses
                //City.Food += City.FoodRate * mins / 60;
                //City.Stone += City.StoneRate * mins / 60;
                //City.Wood += City.WoodRate * mins / 60;
                //City.Iron += City.IronRate * mins / 60;
                City.ResourcesLastUpdated = Now;
            }

            await db.SaveChangesAsync();

            return City;
        }

        //public class test
        //{
        //    public int id2 { get; set; }
        //    public string fuck { get; set; }
        //}

        //[HttpPost("Fucker")]
        ////[Route("Fucker")]
        //public async Task<JsonResult> Fucker55([FromBody] test tt)
        //{
        //    string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var u = await _userManager.FindByIdAsync(UserId);

        //    City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == tt.id2).FirstOrDefaultAsync();
        //    //City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? await CreateCity(UserId);
        //    UserItems UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
        //    UserResearch userResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
        //    List<BuildingCost> ListOfBuildingsCost = GetBuildingsCost(userResearch);
        //    //GetUpgradeBuildings..only need one for each, can calculate costs off of it

        //    return new JsonResult(new { city = UserCity, userItems = UserItems, userResearch = userResearch, newBuildingsCost = ListOfBuildingsCost });
        //}



       

    }

    
}
