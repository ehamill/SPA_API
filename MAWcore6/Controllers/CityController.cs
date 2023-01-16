using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using MAWcore6.Data;
using MAWcore6.Models;
using System.Security.Claims;
using static MAWcore6.Models.CityModels;
using Microsoft.EntityFrameworkCore;
using static MAWcore6.Models.TroopModels;

namespace MAWcore6.Controllers
{

    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<ApplicationUser> _userManager;

        public CityController(ApplicationDbContext _db, UserManager<ApplicationUser> userManager)
        {
            db = _db;
            _userManager = userManager;
        }
        
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            City UserCity = new City();
            UserItems UserItems = new UserItems();
            UserResearch userResearch = new UserResearch();
            List<TroopQueue> TroopQueues = new List<TroopQueue>();

            try
            {
                UserCity = await db.Cities.Include(c => c.Buildings).Include(c => c.Heros).Where(c => c.UserId == UserId).AsSplitQuery().FirstOrDefaultAsync() ?? await CreateCity(UserId);
                UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
                userResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
                TroopQueues = await db.TroopQueues.Where(c => c.CityId == UserCity.CityId && c.Complete == false).ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error at CityController, [GET]: "+ ex.Message);
                Console.WriteLine(ex.Message);
            }

            List<Hero> Heros = await GetHeros(UserCity);
            //await UpdateResources(UserCity);
            List<BuildingCost> ListOfBuildingsCost = GetNewBuildingsCost(UserCity, userResearch);
            List<Troop> Troops = GetTroops(UserCity, userResearch);
            List<Troop> WallDefenses = GetWallDefenses(UserCity, userResearch);
            await CheckTroopQueues(TroopQueues, UserCity);
            //If done, add troops to city... delete queue?? Status..training-complete-cancelled
            //if not...
            if (UserCity.Builder1Busy) {
                await CheckBuilder1(UserCity);
            }
            //GetUpgradeBuildings..only need one for each, can calculate costs off of it
            //Sleep doesn't work..
            //System.Diagnostics.Debug.WriteLine("Testing ... ");
             //Attack(UserCity.CityId);

            return new JsonResult(new { city = UserCity,heros = Heros, troops = Troops, troopQueues = TroopQueues, wallDefenses = WallDefenses, userItems = UserItems, userResearch = userResearch, newBuildingsCost = ListOfBuildingsCost });
        }

        [HttpPost("BuildingDone")]
        public async Task<JsonResult> BuildingDone([FromBody] UpdateCityModel update)
        {
            var message = "ok";

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == update.CityId).FirstOrDefaultAsync() ?? new City();
            UserResearch UserResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserResearch();

            //Check if builders busy ..
            await CheckBuilder1(UserCity);

            List<BuildingCost> ListOfNewBuildingsCost = GetNewBuildingsCost(UserCity, UserResearch);
            
            return new JsonResult(new { message = message, city = UserCity, newBuildingsCost = ListOfNewBuildingsCost });
        }

        [HttpPost("TrainTroops")]
        public async Task<JsonResult> TrainTroops([FromBody] TrainTroopsModel update)
        {
            var message = "ok";

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == update.CityId).FirstOrDefaultAsync() ?? new City();
            //Update Resources...
            UserResearch UserResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserResearch();
            List<TroopQueue> TroopQueues = await db.TroopQueues.Where(c => c.CityId == UserCity.CityId).ToListAsync();
            await CheckTroopQueues(TroopQueues, UserCity);
            var Building = UserCity.Buildings.Where(c => c.BuildingId == update.BuildingId).FirstOrDefault();
            TroopType TroopType = (TroopType)update.TroopTypeInt;
            Result check = CheckTroopRequirements(Building, TroopType, UserResearch, TroopQueues.Count()); //, barrack lvl and research, check if queue full
            if (check.Failed) {
                return new JsonResult(new { message = check.Message});
            }
            
            check = CheckResources(UserCity, UserResearch, TroopType, update.Qty);
            if (check.Failed)
            {
                return new JsonResult(new { message = check.Message });
            }
            //Get cost of troop, b/c used in Remove resources and troop queue
            await RemoveResourcesForTroops(UserCity, UserResearch, TroopType, update.Qty);
            await TroopQueueAdd(update, TroopQueues, UserCity, UserResearch);


            return new JsonResult(new { message = message, city = UserCity, troopQueues = TroopQueues });
        }

        [HttpPost("UpdateCity")]
        public async Task<JsonResult> UpdateCity([FromBody] UpdateCityModel update)
        {
            string message = CheckForUpdateErrors(update);

            if (message != "ok") {
                return new JsonResult(new { message = message });  
            }

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var u = await _userManager.FindByIdAsync(UserId);

            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == update.CityId).FirstOrDefaultAsync() ?? new City();
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

        [HttpPost("SpeedUpUsed")]
        public async Task<JsonResult> SpeedUpUsed([FromBody] SpeedUpModel model)
        {
            var message = "ok";
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == model.CityId).FirstOrDefaultAsync() ?? new City();
            //UserResearch UserResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserResearch();
            UserItems UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserItems();

            if (model.UsedOn == "builder1") {
                if (model.SpeedUp5min) {
                    UserCity.Construction1Ends = UserCity.Construction1Ends.AddMinutes(-5);
                    UserItems.FiveMinuteSpeedups--;
                }
                
            }
            await db.SaveChangesAsync();

            //Check if builders busy ..
            await CheckBuilder1(UserCity);

            return new JsonResult(new { message = message, city = UserCity });
        }

        [HttpPost("HireHero")]
        public async Task<JsonResult> HireHero([FromBody] HireHeroModel model)
        {
            ///GetHeros, check if have enough gold, remove gold,
            //change IsHired to true.
            //Generate a new Hero, and add it to hero list. Return herollst.
            //return city with new gold amt.

            var message = "ok";
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == model.CityId).FirstOrDefaultAsync();

            if (UserCity.UserId != UserId) {
                message = "You dont own this city.";
                return new JsonResult(new { message = message });
            }
            var heros = await db.Heros.Where(c => c.CityId == model.CityId).ToListAsync();

            var HiredHero = heros.Where(c => c.HeroId == model.HeroId).FirstOrDefault();

            var HeroCostInGold = HiredHero.Level * 1000;

            //await UpdateResources(UserCity);

            var GotEnoughGold = UserCity.Gold - HeroCostInGold;

            if (GotEnoughGold < 0 )
            {
                return new JsonResult(new { message = "Need more gold for this hero." });
            }
            var removeGold = new Resources() { 
                Gold = HeroCostInGold,
            };

            await RemoveResourcesFromCity(removeGold, UserCity);

            HiredHero.IsHired = true;
            await db.SaveChangesAsync();

            var newHero = await CreateHeros(1, UserCity.CityId);

            heros.Add(newHero.FirstOrDefault());
            
            return new JsonResult(new { message = message, city = UserCity });
        }

        private async Task CheckBuilder1(City userCity) {
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
                    b.BuildingType = BuildingType.Empty;
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

        private async Task UpdateResources(City userCity)
        {
            DateTime TimeNow = DateTime.UtcNow;
            
            int duration = Convert.ToInt32(Math.Floor((DateTime.UtcNow - userCity.ResourcesLastUpdated).TotalMinutes));
            duration = 6;
            if (duration < 5) {
                return;
            }
            int FoodRate = 100;
            int StoneRate = 100;
            int WoodRate = 100;
            int IronRate = 100;
            int GoldRate = 0;
            
            var farms = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Farm).ToList();
            if (farms.Count() > 0) {
                foreach (var farm in farms)
                {
                    int rate = Constants.FarmFoodRate * farm.Level * (farm.Level + 1) / 2;
                    FoodRate += rate;
                }
            }
            var quarries = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Quarry).ToList();
            if (quarries.Count() > 0)
            {
                foreach (var q in quarries)
                {
                    int rate = Constants.QuarryStoneRate * q.Level * (q.Level + 1) / 2;
                    StoneRate += rate;
                }
            }
            var mills = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Sawmill).ToList();
            if (mills.Count() > 0)
            {
                foreach (var mill in mills)
                {
                    int rate = Constants.SawWoodRate * mill.Level * (mill.Level + 1) / 2;
                    WoodRate += rate;
                }
            }
            var mines = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Iron_Mine).ToList();
            if (mills.Count() > 0)
            {
                foreach (var mine in mines)
                {
                    int rate = Constants.IronMineRate * mine.Level * (mine.Level + 1) / 2;
                    IronRate += rate;
                }
            }
           
            int foodMade = FoodRate * duration / 60;
            userCity.Food = userCity.Food + FoodRate * duration / 60;
            userCity.Stone = userCity.Stone + StoneRate * duration / 60;
            userCity.Wood = userCity.Wood + WoodRate * duration / 60;
            userCity.Iron = userCity.Iron + IronRate * duration / 60;
            userCity.Gold = userCity.Gold + GoldRate * duration / 60;
            userCity.FoodRate = FoodRate;
            userCity.StoneRate = StoneRate;
            userCity.WoodRate = WoodRate;
            userCity.IronRate = IronRate;
            userCity.GoldRate = GoldRate;
            userCity.ResourcesLastUpdated = DateTime.UtcNow;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error at CityController DeleteTroopQueues, []: " + ex.Message + ex.InnerException.Message);
                Console.WriteLine(ex.Message);
            }
        }

        private BuildingCost GetUpgradeCostOfBuilding(UpdateCityModel update,City userCity, UserResearch userResearch)
        {
            var building = userCity.Buildings.Where(c => c.BuildingId == update.BuildingId).FirstOrDefault();
            //if downgrading, time is current level.
            var level = update.Level;
            if (building.Level > update.Level)
            {
                level++;
            }
            BuildingCost bc = new BuildingCost();

            //BuildingType BuildingType = GetBuildingType(update.buildingType);
            //int buildingTypeInt = update.buildingTypeInt;
            switch ((BuildingType)update.BuildingTypeInt)
            {
                case BuildingType.Academy:
                    bc.TypeString = BuildingType.Academy.ToString();
                    bc.BuildingTypeInt = BuildingType.Academy;
                    bc.Food = Constants.AcademyFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.AcademyStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.AcademyWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.AcademyIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.AcademyTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;

                case BuildingType.Barrack:
                    bc.TypeString = BuildingType.Barrack.ToString();
                    bc.BuildingTypeInt = BuildingType.Barrack;
                    bc.Food = Constants.BarrFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.BarrStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.BarrWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.BarrIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.BarrTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Cottage:
                    bc.TypeString = BuildingType.Cottage.ToString();
                    bc.BuildingTypeInt = BuildingType.Cottage;
                    bc.Food = Constants.CottFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.CottStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.CottWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.CottIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.CottTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Feasting_Hall:
                    bc.TypeString = BuildingType.Feasting_Hall.ToString().Replace("_"," ");
                    bc.BuildingTypeInt = BuildingType.Feasting_Hall;
                    bc.Food = Constants.FeastFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.FeastStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.FeastWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.FeastIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.FeastTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Forge:
                    bc.TypeString = BuildingType.Forge.ToString();
                    bc.BuildingTypeInt = BuildingType.Forge;
                    bc.Food = Constants.ForgeFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.ForgeStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.ForgeWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.ForgeIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.ForgeTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Inn:
                    bc.TypeString = BuildingType.Inn.ToString();
                    bc.BuildingTypeInt = BuildingType.Inn;
                    bc.Food = Constants.InnFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.InnStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.InnWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.InnIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.InnTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Rally_Spot:
                    bc.TypeString = BuildingType.Rally_Spot.ToString().Replace("_", " ");
                    bc.BuildingTypeInt = BuildingType.Rally_Spot;
                    bc.Food = Constants.RallyFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.RallyStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.RallyWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.RallyIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.RallyTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Town_Hall:
                    bc.TypeString = BuildingType.Town_Hall.ToString();
                    bc.BuildingTypeInt = BuildingType.Town_Hall;
                    bc.Food = Constants.ThFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.ThStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.ThWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.ThIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.ThTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Walls: 
                    bc.TypeString = BuildingType.Walls.ToString();
                    bc.BuildingTypeInt = BuildingType.Walls;
                    bc.Food = Constants.ThFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.ThStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.ThWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.ThIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.ThTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Farm:
                    bc.TypeString = BuildingType.Farm.ToString();
                    bc.BuildingTypeInt = BuildingType.Farm;
                    bc.Food = Constants.FarmFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.FarmStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.FarmWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.FarmIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.FarmTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Quarry:
                    bc.TypeString = BuildingType.Quarry.ToString();
                    bc.BuildingTypeInt = BuildingType.Quarry;
                    bc.Food = Constants.QuarryFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Stone = Constants.QuarryStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Wood = Constants.QuarryWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Iron = Constants.QuarryIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.Time = Constants.QuarryTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
            }
            return bc;

        }
        
        private string CheckIfBuildingPreReqMet(City city, UpdateCityModel update) {
            string res =  "ok";
            //var updateBuilding = city.Buildings.Where(c => c.BuildingId == update.buildingId).FirstOrDefault();
            BuildingType buildingType = (BuildingType)update.BuildingTypeInt;
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
                if (update.Level - 1 > th.Level)
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
                //int highestLvlQuarry = 0; 
                //var quarries = city.Buildings.Where(c => c.BuildingType == BuildingType.Quarry).ToList();
                //if (quarries.Count() > 0) {
                //    highestLvlQuarry = quarries.Max(c => c.Level);
                //}
                //int forgeCount = city.Buildings.Where(c => c.BuildingType == BuildingType.Forge).Count();
                //if (highestLvlQuarry < 2 || forgeCount == 0)
                //{
                //    res = "";
                //}
                //if (highestLvlQuarry < 2)
                //{
                //    res += "Requires Quarry level 2. ";
                //} 
                //if (forgeCount == 0)
                //{
                //    res += "Requires Forge level 1.";
                //}
            }
            return res;
        }

        private async Task AddTroopsToCity(TroopQueue queue, City city) {
            
            switch (queue.TroopTypeInt)
            {
                case TroopType.Worker:
                    city.WorkerQty = city.WorkerQty + queue.Qty;
                    break;
                case TroopType.Warrior:
                    city.WarriorQty = city.WarriorQty + queue.Qty;
                    break;
                case TroopType.Scout:
                    city.ScoutQty = city.ScoutQty + queue.Qty;
                    break;
                case TroopType.Pikeman:
                    city.PikemanQty = city.PikemanQty + queue.Qty;
                    break;
                case (TroopType.Swordsman):
                    city.SwordsmanQty += queue.Qty;
                    break;
                case TroopType.Transporter:
                    city.TransporterQty = city.TransporterQty + queue.Qty;
                    break;
                case TroopType.Archer:
                    city.ArcherQty = city.ArcherQty + queue.Qty;
                    break;
                case TroopType.Cavalry:
                    city.CavalierQty = city.CavalierQty + queue.Qty;
                    break;
                case TroopType.Cataphract:
                    city.CataphractQty = city.CataphractQty + queue.Qty;
                    break;
                case TroopType.Ballista:
                    city.BallistaQty = city.BallistaQty + queue.Qty;
                    break;
                case TroopType.Catapult:
                    city.CatapultQty = city.CatapultQty + queue.Qty;
                    break;

                case TroopType.Trap:
                    city.TrapQty = city.TrapQty + queue.Qty;
                    break;
                case TroopType.Abatis:
                    city.AbatisQty = city.AbatisQty + queue.Qty;
                    break;
                case TroopType.Archers_Tower:
                    city.ArcherTowerQty = city.ArcherTowerQty + queue.Qty;
                    break;
                case TroopType.Rolling_Log:
                    city.RollingLogQty = city.RollingLogQty + queue.Qty;
                    break;
                case TroopType.Defensive_Trebuchet:
                    city.TrebuchetQty = city.TrebuchetQty + queue.Qty;
                    break;
                default:
                    //log error..
                    break;
            }

            queue.Complete = true;
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error at CityController AddTroopsToCity, []: " + ex.Message + ex.InnerException.Message);
                Console.WriteLine(ex.Message);
            }
        }
        private async Task DeleteTroopQueues(List<int> queueIds, List<TroopQueue> troopQueues)
        {
            try
            {
                foreach (var id in queueIds)
                {
                    //var q = await db.TroopQueues.Where(c => c.TroopQueueId == id).FirstOrDefaultAsync();
                    var q = troopQueues.Where(c => c.TroopQueueId == id).FirstOrDefault();
                    db.TroopQueues.Remove(q);
                }
                await db.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error at CityController DeleteTroopQueues, []: " + ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
        private async Task CheckTroopQueues(List<TroopQueue> troopQueues, City city)
        {
            List<int> QueuesToDelete = new List<int>();
            foreach (var queue in troopQueues)
            {
                if (queue.Complete == false && queue.Ends < DateTime.UtcNow)
                {
                    await AddTroopsToCity(queue, city);
                    QueuesToDelete.Add(queue.TroopQueueId);
                }
                else if (queue.Complete == false)
                { 
                    queue.TimeLeft = Convert.ToInt32(Math.Floor((queue.Ends - DateTime.UtcNow).TotalSeconds));
                    await db.SaveChangesAsync();
                }
            }
            await DeleteTroopQueues(QueuesToDelete, troopQueues);

        }
        private async Task TroopQueueAdd(TrainTroopsModel update, List<TroopQueue> troopQueues, City city, UserResearch userResearch)
        {
            var costOfTroops = GetCostOfTroops(city, userResearch);
            BuildingCost singleTroopCost = costOfTroops.Where(c => c.TroopType == (TroopType)update.TroopTypeInt).FirstOrDefault();

            bool walls = (update.TroopTypeInt >= 13) ? true : false;
            
            var troopQueue = new TroopQueue()
            {
                Starts = DateTime.UtcNow,
                Ends = DateTime.UtcNow.AddSeconds(singleTroopCost.Time * update.Qty), 
                Qty = update.Qty,
                BuildingId = update.BuildingId,
                CityId = city.CityId,
                TroopTypeInt = (TroopType)update.TroopTypeInt,
                TroopTypeString = ((TroopType)update.TroopTypeInt).ToString(),
                TimeLeft = singleTroopCost.Time * update.Qty,
                Complete = false,
                Walls = walls,
            };
            await db.TroopQueues.AddAsync(troopQueue);
            await db.SaveChangesAsync();

            troopQueues.Add(troopQueue);

        }

        private async Task RemoveResourcesForTroops(City city, UserResearch userResearch, TroopType type, int qty)
        {
            var costOfTroops = GetCostOfTroops(city, userResearch);
            BuildingCost singleTroopCost = costOfTroops.Where(c => c.TroopType == type).FirstOrDefault();

            city.Food = city.Food - singleTroopCost.Food * qty;
            city.Stone = city.Stone - singleTroopCost.Stone * qty;
            city.Wood = city.Wood - singleTroopCost.Wood * qty;
            city.Iron = city.Iron - singleTroopCost.Iron * qty;

            await db.SaveChangesAsync();
        }
        
        private Result CheckResources(City city, UserResearch userResearch, TroopType type, int qty) {
            Result result = new Result() { Failed = false, Message = "" };

            var costOfTroops = GetCostOfTroops(city, userResearch);
            var singleTroopCost = costOfTroops.Where(c => c.TroopType == type).FirstOrDefault();

            if (city.Food - singleTroopCost.Food * qty < 0) {
                result.Failed = true;
                result.Message += "Requires " + singleTroopCost.Food * qty + " food. ";
            }
            if (city.Stone - singleTroopCost.Stone * qty < 0)
            {
                result.Failed = true;
                result.Message += "Requires " + singleTroopCost.Stone * qty + " stone. ";
            }
            if (city.Wood - singleTroopCost.Wood * qty < 0)
            {
                result.Failed = true;
                result.Message += "Requires " + singleTroopCost.Wood * qty + " wood. ";
            }
            if (city.Iron - singleTroopCost.Iron * qty < 0)
            {
                result.Failed = true;
                result.Message += "Requires " + singleTroopCost.Iron * qty + " iron. ";
            }

            return result;
        }

        private Result CheckTroopRequirements(Building building, TroopType type,  UserResearch research, int queueCount) {
            Result result = new Result() { Failed = false, Message= ""};
            
            if (building.BuildingType == BuildingType.Walls) {
                return result;
            }

            if (queueCount >= building.Level) {
                result.Failed = true;
                result.Message = "Queue is full. Upgrade level or cancel troops to add to queue.";
            }

            if (type == TroopType.Worker || type == TroopType.Warrior) {
                if (building.Level < 1) {
                    result.Failed = true;
                    result.Message = "Requires Barrack level 1.";
                } 
            }
            if (type == TroopType.Scout)
            {
                if (building.Level < 2)
                {
                    result.Failed = true;
                    result.Message = "Requires Barrack level 2.";
                }
            }
            if (type == TroopType.Pikeman)
            {
                if (building.Level < 2)
                {
                    result.Failed = true;
                    result.Message += "Requires Barrack level 2. ";
                }
                if (research.IronWorking < 1)
                {
                    result.Failed = true;
                    result.Message += "Requires Military Tradition level 1.";
                }
            }
            if (type == TroopType.Swordsman)
            {
                if (building.Level < 3)
                {
                    result.Failed = true;
                    result.Message += "Requires Barrack level 3. ";
                }
                if (research.IronWorking < 1)
                {
                    result.Failed = true;
                    result.Message += "Requires Iron Working level 1.";
                }
            }
            if (type == TroopType.Archer)
            {
                if (building.Level < 4)
                {
                    result.Failed = true;
                    result.Message += "Requires Barrack level 4. ";
                }
                if (research.Archery < 1)
                {
                    result.Failed = true;
                    result.Message += "Requires Archery level 1.";
                }
            }
            if (type == TroopType.Cavalry)
            {
                if (building.Level < 5)
                {
                    result.Failed = true;
                    result.Message += "Requires Barrack level 5. ";
                }
                if (research.HorsebackRiding < 1)
                {
                    result.Failed = true;
                    result.Message += "Requires Horse Back Riding level 1.";
                }
            }
            if (type == TroopType.Ballista)
            {
                if (building.Level < 9)
                {
                    result.Failed = true;
                    result.Message += "Requires Barrack level 9. ";
                }
                if (research.Archery < 6)
                {
                    result.Failed = true;
                    result.Message += "Requires Archery level 6.";
                }
                if (research.MetalCasting < 5)
                {
                    result.Failed = true;
                    result.Message += "Requires Metal Casting level 5.";
                }
            }
            if (building.BuildingType != BuildingType.Barrack && building.BuildingType != BuildingType.Walls)
            {
                result.Failed = true;
                result.Message = "Barrack required to build troops.";
            }
            return result;
        }

        private string CheckForUpdateErrors(UpdateCityModel update) {
            string result = "ok";
            if (update.CityId == 0)
            {
                result += "City not found. update.cityId == " + update.CityId.ToString();
            }
            if (update.BuildingId == 0)
            {
                result += "No buildingId found. update.buildingId == " + update.BuildingId.ToString();
            }
            if (update.BuildingTypeInt == 0)
            {
                result += "No building found. update.buildingTypeInt == " + update.BuildingTypeInt.ToString();
            }

            return result;
        }

        private async Task RemoveResourcesFromCity(Resources res, City UserCity)
        {
            UserCity.Food = UserCity.Food - res.Food;
            UserCity.Stone = UserCity.Stone - res.Stone;
            UserCity.Wood = UserCity.Wood - res.Wood;
            UserCity.Iron = UserCity.Iron - res.Iron;
            UserCity.Gold = UserCity.Gold - res.Gold;

            await db.SaveChangesAsync();
        }

        private BuildingType GetBuildingType(string name) {
            if (name.ToLower().Contains("academy"))
            {
                return BuildingType.Academy;
            }
            else if (name.ToLower().Contains("forge"))
            {
                return BuildingType.Forge;
            }
            else if (name.ToLower().Contains("feast"))
            {
                return BuildingType.Feasting_Hall;
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
            else if (name.ToLower().Contains("farm"))
            {
                return BuildingType.Farm;
            }
            else if (name.ToLower().Contains("sawmill"))
            {
                return BuildingType.Sawmill;
            }
            else if (name.ToLower().Contains("quar"))
            {
                return BuildingType.Quarry;
            }
            else if (name.ToLower().Contains("iron"))
            {
                return BuildingType.Iron_Mine;
            }
            else {
                return BuildingType.Not_Found;
            }

        }

        private async Task RemoveResourcesAndUpdateConstructionFromCity(City UserCity, UpdateCityModel update, BuildingCost BuildingCost) {

            BuildingType buildingType = (BuildingType)update.BuildingTypeInt; //GetBuildingType(update.buildingType);

            var b = UserCity.Buildings.Where(c => c.BuildingId == update.BuildingId).FirstOrDefault();
            string upgrading = "upgrading";
            if (update.Level - b.Level < 0)
            {
                upgrading = "downgrading";
                //if (update.level == 0) {
                //    buildingType = BuildingType.Empty;
                //}
            }
            if (upgrading == "upgrading") {
                UserCity.Food = UserCity.Food - BuildingCost.Food;
                UserCity.Stone = UserCity.Stone - BuildingCost.Stone;
                UserCity.Wood = UserCity.Wood - BuildingCost.Wood;
                UserCity.Iron = UserCity.Iron - BuildingCost.Iron;
            } else
            {
                UserCity.Food = UserCity.Food + BuildingCost.Food;
                UserCity.Stone = UserCity.Stone + BuildingCost.Stone;
                UserCity.Wood = UserCity.Wood + BuildingCost.Wood;
                UserCity.Iron = UserCity.Iron + BuildingCost.Iron;
            }

            UserCity.Construction1Started = DateTime.UtcNow;
            UserCity.Construction1Ends = DateTime.UtcNow.AddSeconds(BuildingCost.Time);
            UserCity.Construction1BuildingId = update.BuildingId;
            UserCity.Construction1BuildingLevel = update.Level;

            UserCity.Builder1Busy = true;
            UserCity.Builder1Time = BuildingCost.Time;
            

            b.Image = upgrading + buildingType.ToString() +"lvl"+ update.Level;
            b.BuildingType = buildingType;
            b.Description = GetBuildingDescription(buildingType);

            UserCity.BuildingWhat = buildingType.ToString();

            await db.SaveChangesAsync();
        }
        private async Task<City> CreateCity(string UserID) {

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
            int Time = Convert.ToInt32( Math.Ceiling(60 * Math.Pow(0.9, userResearch.Building)));

            UpdateCityModel update = new UpdateCityModel()
            {
                BuildingTypeString = "Cottage",
                BuildingTypeInt = (int)BuildingType.Cottage,
            };
            string TestingResult = "ok";
            bool requirementsMet = true;
            if (TestingResult != "ok")
            {
                requirementsMet = false;
            }
            BuildingCost cott = new BuildingCost()
            {
                TypeString = BuildingType.Cottage.ToString(),
                BuildingTypeInt = BuildingType.Cottage,
                PreReq = TestingResult,
                ReqMet = requirementsMet,
                Food = Constants.CottFoodReq,
                Stone = Constants.CottStoneReq,
                Wood = Constants.CottWoodReq,
                Iron = Constants.CottIronReq,
                Time = Constants.CottTimeReq,
                Description = "Increases your population",
            };
            lbc.Add(cott);

            int BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Academy).Count();
            if (BuildingCount == 0)
            {
                update.BuildingTypeString = "Academy";
                update.BuildingTypeInt = (int)BuildingType.Academy;
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost Academy = new BuildingCost()
                {
                    TypeString = BuildingType.Academy.ToString().Replace("_", " "),
                    BuildingTypeInt = BuildingType.Academy,
                    PreReq = TestingResult,
                    ReqMet = requirementsMet,
                    Food = Constants.AcademyFoodReq,
                    Stone = Constants.AcademyStoneReq,
                    Wood = Constants.AcademyWoodReq,
                    Iron = Constants.AcademyIronReq,
                    Time = Constants.AcademyTimeReq,
                    Description = "Learn new skills and do your research."
                };
                lbc.Add(Academy);
            }

            update.BuildingTypeString = "Barrack";
            update.BuildingTypeInt = (int)BuildingType.Barrack;
            TestingResult = CheckIfBuildingPreReqMet(userCity, update);
            requirementsMet = true;
            if (TestingResult != "ok")
            {
                requirementsMet = false;
            }
            BuildingCost barr = new BuildingCost()
            {
                TypeString = BuildingType.Barrack.ToString(),
                BuildingTypeInt = BuildingType.Barrack,
                PreReq = TestingResult,
                ReqMet = requirementsMet,
                Food = Constants.BarrFoodReq,
                Stone = Constants.BarrStoneReq,
                Wood = Constants.BarrWoodReq,
                Iron = Constants.BarrIronReq,
                Time = Constants.BarrTimeReq,
                Description = "Place where you train your troops",
            };
            lbc.Add(barr);

            BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Feasting_Hall).Count();
            if (BuildingCount == 0) {
                update.BuildingTypeString = "Feasting";
                update.BuildingTypeInt = (int)BuildingType.Feasting_Hall;
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost Feast = new BuildingCost()
                {
                    TypeString = BuildingType.Feasting_Hall.ToString().Replace("_", " "),
                    BuildingTypeInt = BuildingType.Feasting_Hall,
                    PreReq = TestingResult,
                    ReqMet = requirementsMet,
                    Food = Constants.FeastFoodReq,
                    Stone = Constants.FeastStoneReq,
                    Wood = Constants.FeastWoodReq,
                    Iron = Constants.FeastIronReq,
                    Time = Constants.FeastTimeReq,
                    Description = "Where your hero's live and are managed."
                };
                lbc.Add(Feast);
            }
            BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Forge).Count();
            if (BuildingCount == 0)
            {
                update.BuildingTypeString = "Forge";
                update.BuildingTypeInt = (int)BuildingType.Forge;
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost Forge = new BuildingCost()
                {
                    TypeString = BuildingType.Forge.ToString().Replace("_", " "),
                    BuildingTypeInt = BuildingType.Forge,
                    PreReq = TestingResult,
                    ReqMet = requirementsMet,
                    Food = Constants.ForgeFoodReq,
                    Stone = Constants.ForgeStoneReq,
                    Wood = Constants.ForgeWoodReq,
                    Iron = Constants.ForgeIronReq,
                    Time = Constants.ForgeTimeReq,
                    Description = "Improve your iron working skills.",
                };
                lbc.Add(Forge);
            }

            BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Inn).Count();
            if (BuildingCount == 0)
            {
                update.BuildingTypeString = "Inn";
                update.BuildingTypeInt = (int)BuildingType.Inn;
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost inn = new BuildingCost()
                {
                    TypeString = BuildingType.Inn.ToString(),
                    BuildingTypeInt = BuildingType.Inn,
                    PreReq = TestingResult,
                    ReqMet = requirementsMet,
                    Food = Constants.InnFoodReq,
                    Stone = Constants.InnStoneReq,
                    Wood = Constants.InnWoodReq,
                    Iron = Constants.InnIronReq,
                    Time = Constants.InnTimeReq,
                    Description = "Recruit new heros.",
                };
                lbc.Add(inn);
            }
            BuildingCount = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Rally_Spot).Count();
            if (BuildingCount == 0)
            {
                update.BuildingTypeString = "Rally";
                update.BuildingTypeInt = (int)BuildingType.Rally_Spot;
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost rally = new BuildingCost()
                {
                    TypeString = BuildingType.Rally_Spot.ToString().Replace("_", " "),
                    BuildingTypeInt = BuildingType.Rally_Spot,
                    PreReq = TestingResult,
                    ReqMet = requirementsMet,
                    Food = Constants.RallyFoodReq,
                    Stone = Constants.RallyStoneReq,
                    Wood = Constants.RallyWoodReq,
                    Iron = Constants.RallyIronReq,
                    Time = Constants.RallyTimeReq,
                    Description = "Heal troops, test troops, and increases amount of troops you can send.",
                };
                lbc.Add(rally);

            }

            BuildingCost farm = new BuildingCost()
            {
                TypeString = BuildingType.Farm.ToString(),
                BuildingTypeInt = BuildingType.Farm,
                PreReq = "",
                ReqMet = true,
                Food = Constants.FarmFoodReq,
                Stone = Constants.FarmStoneReq,
                Wood = Constants.FarmWoodReq,
                Iron = Constants.FarmIronReq,
                Time = Constants.FarmTimeReq,
                Description = "Increases food production.",
                Farm = true,
            };
            lbc.Add(farm);

            BuildingCost quarry = new BuildingCost()
            {
                TypeString = BuildingType.Quarry.ToString(),
                BuildingTypeInt = BuildingType.Sawmill,
                PreReq = "",
                ReqMet = true,
                Food = Constants.QuarryFoodReq,
                Stone = Constants.QuarryStoneReq,
                Wood = Constants.QuarryWoodReq,
                Iron = Constants.QuarryIronReq,
                Time = Constants.QuarryTimeReq,
                Description = "Increases stone production.",
                Farm = true,
            };
            lbc.Add(quarry);

            BuildingCost sawmill = new BuildingCost()
            {
                TypeString = BuildingType.Sawmill.ToString(),
                BuildingTypeInt = BuildingType.Sawmill,
                PreReq = "",
                ReqMet = true,
                Food = Constants.SawFoodReq,
                Stone = Constants.SawStoneReq,
                Wood = Constants.SawWoodReq,
                Iron = Constants.SawIronReq,
                Time = Constants.SawTimeReq,
                Description = "Increases wood production.",
                Farm = true,
            };
            lbc.Add(sawmill);

            BuildingCost ironMine = new BuildingCost()
            {
                TypeString = BuildingType.Iron_Mine.ToString(),
                BuildingTypeInt = BuildingType.Iron_Mine,
                PreReq = "",
                ReqMet = true,
                Food = Constants.IronMineFoodReq,
                Stone = Constants.IronMineStoneReq,
                Wood = Constants.IronMineWoodReq,
                Iron = Constants.IronMineIronReq,
                Time = Constants.IronMineTimeReq,
                Description = "Increases iron production.",
                Farm = true,
            };
            lbc.Add(ironMine);
            return lbc;

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

        private List<Troop> GetWallDefenses(City city, UserResearch research)
        {
            List<Troop> Troops = new List<Troop>();

            var Trap = new Troop()
            {
                TypeString = TroopType.Trap.ToString(),
                TypeInt = TroopType.Trap,
                PreReq = "Requires Walls level 1.",
                ReqMet = true,
                Description = "Trap your enemy.",
                Qty = city.TrapQty,
                ForWalls = true,
                FoodCost = Constants.TrapFoodReq,
                StoneCost = Constants.TrapStoneReq,
                WoodCost = Constants.TrapWoodReq,
                IronCost = Constants.TrapIronReq,
                TimeCost = Constants.TrapTimeReq,
                //Attack = Constants.TrapAttk,
                //Defense = Constants.TrapDef,
                //Life = Constants.TrapLife,
                //Range = Constants.TrapRange,
                //Load = Constants.TrapLoad,
                //Speed = Constants.TrapSpeed,
            };
            Troops.Add(Trap);

            var Abatis = new Troop()
            {
                TypeString = TroopType.Abatis.ToString(),
                TypeInt = TroopType.Abatis,
                PreReq = "Requires Walls level 2.",
                ReqMet = true,
                Description = "Killer Defense against Cavalry.",
                Qty = city.AbatisQty,
                ForWalls = true,
                FoodCost = Constants.AbatisFoodReq,
                StoneCost = Constants.AbatisStoneReq,
                WoodCost = Constants.AbatisWoodReq,
                IronCost = Constants.AbatisIronReq,
                TimeCost = Constants.AbatisTimeReq,
                //Attack = Constants.TrapAttk,
                //Defense = Constants.TrapDef,
                //Life = Constants.TrapLife,
                //Range = Constants.TrapRange,
                //Load = Constants.TrapLoad,
                //Speed = Constants.TrapSpeed,
            };
            Troops.Add(Abatis);

            var AT = new Troop()
            {
                TypeString = TroopType.Archers_Tower.ToString(),
                TypeInt = TroopType.Archers_Tower,
                PreReq = "Requires Walls level 3.",
                ReqMet = true,
                Description = "Great ranged defense.",
                Qty = city.ArcherTowerQty,
                ForWalls = true,
                FoodCost = Constants.ATFoodReq,
                StoneCost = Constants.ATStoneReq,
                WoodCost = Constants.ATWoodReq,
                IronCost = Constants.ATIronReq,
                TimeCost = Constants.ATTimeReq,
            };
            Troops.Add(AT);

            var rl = new Troop()
            {
                TypeString = TroopType.Rolling_Log.ToString().Replace("_"," "),
                TypeInt = TroopType.Rolling_Log,
                PreReq = "Requires Walls level 5.",
                ReqMet = true,
                Description = "Roll Logs on enemy when they reach the gates.",
                Qty = city.RollingLogQty,
                ForWalls = true,
                FoodCost = Constants.RollLogFoodReq,
                StoneCost = Constants.RollLogStoneReq,
                WoodCost = Constants.RollLogWoodReq,
                IronCost = Constants.RollLogIronReq,
                TimeCost = Constants.RollLogTimeReq,
            };
            Troops.Add(rl); 
            
            var treb = new Troop()
            {
                TypeString = TroopType.Defensive_Trebuchet.ToString(),
                TypeInt = TroopType.Defensive_Trebuchet,
                PreReq = "Requires Walls level 7.",
                ReqMet = true,
                Description = "Hail large stones on your enemy.",
                Qty = city.TrebuchetQty,
                ForWalls = true,
                FoodCost = Constants.TrebFoodReq,
                StoneCost = Constants.TrebStoneReq,
                WoodCost = Constants.TrebWoodReq,
                IronCost = Constants.TrebIronReq,
                TimeCost = Constants.TrebTimeReq,
                //Attack = Constants.TrapAttk,
                //Defense = Constants.TrapDef,
                //Life = Constants.TrapLife,
                //Range = Constants.TrapRange,
                //Load = Constants.TrapLoad,
                //Speed = Constants.TrapSpeed,
            };
            Troops.Add(treb);

            return Troops;

        }

        private List<BuildingCost> GetCostOfTroops(City userCity, UserResearch userResearch)
        {
            var cost = new List<BuildingCost>();

            var worker = new BuildingCost()
            {
                TypeString = TroopType.Worker.ToString(),
                TroopType = TroopType.Worker,
                PreReq = Constants.WorkerBuildReq,
                ReqMet = false,
                Food = Constants.WorkerFoodCost,
                Stone = 0,
                Wood = Constants.WorkerWoodCost,
                Iron = Constants.WorkerIronCost,
                Time = Constants.WorkerTimeCost,
            };
            cost.Add(worker);

            var warr = new BuildingCost()
            {
                TypeString = TroopType.Warrior.ToString(),
                TroopType = TroopType.Warrior,
                PreReq = Constants.WarrBuildReq,
                ReqMet = false,
                Food = Constants.WarrFoodCost,
                Stone = 0,
                Wood = Constants.WarrWoodCost,
                Iron = Constants.WarrIronCost,
                Time = Constants.WarrTimeCost,
            };
            cost.Add(warr);

            var scout = new BuildingCost()
            {
                TypeString = TroopType.Scout.ToString(),
                TroopType = TroopType.Scout,
                PreReq = Constants.ScoutBuildReq,
                ReqMet = false,
                Food = Constants.ScoutFoodCost,
                Stone = 0,
                Wood = Constants.ScoutWoodCost,
                Iron = Constants.ScoutIronCost,
                Time = Constants.ScoutTimeCost,
            };
            cost.Add(scout);

            var pike = new BuildingCost()
            {
                TypeString = TroopType.Pikeman.ToString(),
                TroopType = TroopType.Pikeman,
                PreReq = Constants.PikeBuildReq,
                ReqMet = false,
                Food = Constants.PikeFoodCost,
                Stone = 0,
                Wood = Constants.PikeWoodCost,
                Iron = Constants.PikeIronCost,
                Time = Constants.PikeTimeCost,
            };
            cost.Add(pike);

            var arch = new BuildingCost()
            {
                TypeString = TroopType.Archer.ToString(),
                TroopType = TroopType.Archer,
                PreReq = Constants.ArchBuildReq,
                ReqMet = false,
                Food = Constants.ArchFoodCost,
                Stone = 0,
                Wood = Constants.ArchWoodCost,
                Iron = Constants.ArchIronCost,
                Time = Constants.ArchTimeCost,
            };
            cost.Add(arch);

            var cav = new BuildingCost()
            {
                TypeString = TroopType.Cavalry.ToString(),
                TroopType = TroopType.Cavalry,
                PreReq = Constants.CavBuildReq,
                ReqMet = false,
                Food = Constants.CavFoodCost,
                Stone = 0,
                Wood = Constants.CavWoodCost,
                Iron = Constants.CavIronCost,
                Time = Constants.CavTimeCost,
            };
            cost.Add(cav);

            var ball = new BuildingCost()
            {
                TypeString = TroopType.Ballista.ToString(),
                TroopType = TroopType.Ballista,
                PreReq = Constants.BallBuildReq,
                ReqMet = false,
                Food = Constants.BallFoodCost,
                Stone = 0,
                Wood = Constants.BallWoodCost,
                Iron = Constants.BallIronCost,
                Time = Constants.BallTimeCost,
            };
            cost.Add(ball);

            var cata = new BuildingCost()
            {
                TypeString = TroopType.Catapult.ToString(),
                TroopType = TroopType.Catapult,
                PreReq = Constants.CataBuildReq,
                ReqMet = false,
                Food = Constants.CataFoodCost,
                Stone = Constants.CataStoneCost,
                Wood = Constants.CataWoodCost,
                Iron = Constants.CataIronCost,
                Time = Constants.CataTimeCost,
            };
            cost.Add(cata);

            var Trap = new BuildingCost()
            {
                TypeString = TroopType.Trap.ToString(),
                TroopType = TroopType.Trap,
                PreReq = "Requires Walls level 1.",
                ReqMet = false,
                Food = Constants.TrapFoodReq,
                Stone = Constants.TrapStoneReq,
                Wood = Constants.TrapWoodReq,
                Iron = Constants.TrapIronReq,
                Time = Constants.TrapTimeReq,
            };
            cost.Add(Trap);

            
            var Abatis = new BuildingCost()
            {
                TypeString = TroopType.Abatis.ToString().Replace("_", " "),
                TroopType = TroopType.Abatis,
                PreReq = "Requires Walls level 2.",
                ReqMet = false,
                Food = Constants.AbatisFoodReq,
                Stone = Constants.AbatisStoneReq,
                Wood = Constants.AbatisWoodReq,
                Iron = Constants.AbatisIronReq,
                Time = Constants.AbatisTimeReq,
            };
            cost.Add(Abatis);

            var AT = new BuildingCost()
            {
                TypeString = TroopType.Archers_Tower.ToString().Replace("_", " "),
                TroopType = TroopType.Archers_Tower,
                PreReq = "Requires Walls level 3.",
                ReqMet = false,
                Food = Constants.ATFoodReq,
                Stone = Constants.ATStoneReq,
                Wood = Constants.ATWoodReq,
                Iron = Constants.ATIronReq,
                Time = Constants.ATTimeReq,
            };
            cost.Add(AT);

            var rl = new BuildingCost()
            {
                TypeString = TroopType.Rolling_Log.ToString().Replace("_", " "),
                TroopType = TroopType.Rolling_Log,
                PreReq = "Requires Walls level 5.",
                ReqMet = false,
                Food = Constants.RollLogFoodReq,
                Stone = Constants.RollLogStoneReq,
                Wood = Constants.RollLogWoodReq,
                Iron = Constants.RollLogIronReq,
                Time = Constants.RollLogTimeReq,
            };
            cost.Add(rl);

            var treb = new BuildingCost()
            {
                TypeString = TroopType.Defensive_Trebuchet.ToString(),
                TroopType = TroopType.Defensive_Trebuchet,
                PreReq = "Requires Walls level 7.",
                ReqMet = false,
                Food = Constants.TrebFoodReq,
                Stone = Constants.TrebStoneReq,
                Wood = Constants.TrebWoodReq,
                Iron = Constants.TrebIronReq,
                Time = Constants.TrebTimeReq,
            };
            cost.Add(treb);



            return cost;
        }
        //private async Task CheckTroopQueue(List<TroopQueue> troopQueues, City userCity) {
        //    DateTime now = DateTime.UtcNow;
        //    foreach (var queue in troopQueues)
        //    {
        //        if (queue.Ends < now)
        //        {
        //            switch (queue.TroopTypeInt)
        //            {
        //                case (TroopType.Worker):
        //                    userCity.WorkerQty += queue.Qty;
        //                    queue.Complete = true;
        //                    break;
        //                case (TroopType.Warrior):
        //                    userCity.WarriorQty += queue.Qty;
        //                    queue.Complete = true;
        //                    break;
        //                case (TroopType.Pikeman):
        //                    userCity.PikemanQty += queue.Qty;
        //                    queue.Complete = true;
        //                    break;
        //                case (TroopType.Scout):
        //                    userCity.ScoutQty += queue.Qty;
        //                    queue.Complete = true;
        //                    break;
        //                case (TroopType.Swordsman):
        //                    userCity.SwordsmanQty += queue.Qty;
        //                    queue.Complete = true;
        //                    break;
        //                case (TroopType.Archer):
        //                    userCity.ArcherQty   += queue.Qty;
        //                    queue.Complete = true;
        //                    break;
        //                case (TroopType.Cavalry):
        //                    userCity.CavalierQty += queue.Qty;
        //                    queue.Complete = true;
        //                    break;
        //                case (TroopType.Ballista):
        //                    userCity.BallistaQty += queue.Qty;
        //                    queue.Complete = true;
        //                    break;
        //                case (TroopType.Transporter):
        //                    userCity.TransporterQty += queue.Qty;
        //                    queue.Complete = true;
        //                    break;
        //                default:
        //                    break;
        //            }
        //            await db.SaveChangesAsync();
        //        }
        //        else {
        //            queue.TimeLeft = Convert.ToInt32(Math.Floor((queue.Ends - now).TotalSeconds)); 
        //        }

        //    }

        //}

        private void GetHeroPercentages(List<Hero> NewHeros) {
            List<int> highPols = new List<int>();
            List<int> highAttk = new List<int>();
            List<int> highIntel = new List<int>();

            foreach (var h in NewHeros)
            {
                if (h.Politics > 60)
                {
                    highPols.Add(h.Politics);
                }
                if (h.Attack > 60)
                {
                    highAttk.Add(h.Attack);
                }
                if (h.Intelligence > 60)
                {
                    highIntel.Add(h.Intelligence);
                }
            }

            System.Diagnostics.Debug.WriteLine("HighPols: ");
            foreach (var h in highPols)
            {
                System.Diagnostics.Debug.WriteLine(h + ", ");
            }
            System.Diagnostics.Debug.WriteLine("HighAttk: ");
            foreach (var h in highAttk)
            {
                System.Diagnostics.Debug.WriteLine(h + ", ");
            }
            System.Diagnostics.Debug.WriteLine("HighIntel: ");
            foreach (var h in highIntel)
            {
                System.Diagnostics.Debug.WriteLine(h + ", ");
            }
            var percentHighPol = highPols.Count() * 100 / NewHeros.Count();
            var percentHighAttk = highAttk.Count() * 100 / NewHeros.Count();
            var percentHighIntel = highIntel.Count() * 100 / NewHeros.Count();

            System.Diagnostics.Debug.WriteLine("percentHighPol: " + percentHighPol + " percentHighAttk:" + percentHighAttk
                + " percentHighIntel: " + percentHighIntel);
        }

        private async Task<List<Hero>> CreateHeros(int Qty, int CityId)
        {
            List<Hero> NewHeros = new List<Hero>();
            //System.Diagnostics.Debug.WriteLine("hero" + i + ": Pol:" + PolPoints + " Attck: " + AttkPoints + " intel: " + IntelPoints);
            var HeroNames = new List<string>() {
                "Zion", "Davis", "April", "Fritz", "Aarav", "Gates", "Valentino", "Shannon", "Kaya", "Cook", "Jadiel", "Humphrey", "Bria", "Brennan", "Maya", "Leblanc", "Corbin", "Hood", "Yaretzi", "Townsend", "Keira", "Warner", "Broderick", "Landry", "Malakai", "Grant", "Ryan", "Small", "Hayden", "Cole", "Katrina", "Conner", "Caitlyn", "Wells", "Edith", "Barker", "Ivy", "Marquez", "Alexander", "Harvey", "Brynn", "Mcdaniel", "Jarrett", "Olson", "Alayna", "Colon", "Regan", "Fox", "Julio", "Walker", "Sierra", "Elliott", "Janet", "Shelton", "Tess", "Willis"
            }; 

            for (int i = 0; i < Qty; i++)
            {
                Random random = new Random();
                double rand = random.NextDouble();
                int PolPoints = (rand < 0.3) ? random.Next(3, 70) : random.Next(3, 50);
                int IntelPoints = (rand < 0.2) ? random.Next(3, 70) : random.Next(3, 50);
                int AttkPoints = (rand > 0.9) ? random.Next(3, 70) : random.Next(3, 50);
                Hero NewHero = new Hero();
                NewHero.CityId = CityId;
                NewHero.Politics = PolPoints;
                NewHero.Intelligence = IntelPoints;
                NewHero.Attack = AttkPoints;
                NewHero.Level = random.Next(1, 10); ////Adjust Hero Level by Inn level
                NewHero.Name = HeroNames[random.Next(0, HeroNames.Count())];

                NewHeros.Add(NewHero);
                //db.Heros.Add(NewHero);
            }
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception e)
            {
                var m = e.InnerException.Message;
            }

            return NewHeros;

        }

        private async Task<List<Hero>> GetHeros(City userCity)
        {
            var cityHeros = userCity.Heros;

            foreach (var hero in cityHeros)
            {
                DateTime disposeHeroDateTime = DateTime.UtcNow.AddMinutes(-1);
                if (!hero.IsHired && hero.Created < disposeHeroDateTime) {
                    db.Heros.Remove(hero);
                }
            }
            await db.SaveChangesAsync();

            //var Inn = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Inn).FirstOrDefault();
            //int MaxHeroCount = (Inn == null) ? 0 : Inn.Level;

            if (cityHeros.Where(c => c.IsHired == false).Count() < 5) {
                int Qty = 5 - cityHeros.Where(c => c.IsHired == false).Count();
                List<Hero> newHeros = await CreateHeros(Qty, userCity.CityId);
                foreach (var hero in newHeros)
                {
                    cityHeros.Add(hero);
                }
            }
            //System.Diagnostics.Debug.WriteLine("hero" + i + ": Pol:" + PolPoints + " Attck: " + AttkPoints + " intel: " + IntelPoints);

            //GetHeroPercentages(NewHeros);

            return cityHeros;

        }
  

    }

    
}
