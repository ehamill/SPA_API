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
        //localhost:33999/City
        [HttpGet]
        public async Task<JsonResult> Get()
        { 
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var u = await _userManager.FindByIdAsync(UserId);

            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? await CreateCity(UserId);
            UserItems UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
            UserResearch userResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
            List<BuildingCost> ListOfBuildingsCost = GetBuildingsCost(userResearch);
            //GetUpgradeBuildings..only need one for each, can calculate costs off of it

            return new JsonResult(new { city = UserCity, userItems = UserItems, userResearch = userResearch, newBuildingsCost = ListOfBuildingsCost });
        }


        //works with fetch "city/Fucker" ... city has to be lower case..hmm
        //[HttpGet("Fucker")]
        //public async Task<JsonResult> Fucker()
        //{
        //    string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var u = await _userManager.FindByIdAsync(UserId);

        //    City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? await CreateCity(UserId);
        //    UserItems UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
        //    UserResearch userResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
        //    List<BuildingCost> ListOfBuildingsCost = GetBuildingsCost(userResearch);
        //    //GetUpgradeBuildings..only need one for each, can calculate costs off of it

        //    return new JsonResult(new { city = UserCity, userItems = UserItems, userResearch = userResearch, newBuildingsCost = ListOfBuildingsCost });
        //}

        //Method POST ... city/Fucker
        //[HttpPost("Fucker")]
        //public async Task<JsonResult> Fucker()
        //{
        //    string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //    var u = await _userManager.FindByIdAsync(UserId);

        //    City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? await CreateCity(UserId);
        //    UserItems UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
        //    UserResearch userResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
        //    List<BuildingCost> ListOfBuildingsCost = GetBuildingsCost(userResearch);
        //    //GetUpgradeBuildings..only need one for each, can calculate costs off of it

        //    return new JsonResult(new { city = UserCity, userItems = UserItems, userResearch = userResearch, newBuildingsCost = ListOfBuildingsCost });
        //}


        public class test
        {
            public int id2 { get; set; }
            public string fuck { get; set; }
        }

        [HttpPost("Fucker")]
        //[Route("Fucker")]
        public async Task<JsonResult> Fucker55([FromBody] test tt)
        {
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var u = await _userManager.FindByIdAsync(UserId);

            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == tt.id2).FirstOrDefaultAsync();
            //City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? await CreateCity(UserId);
            UserItems UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
            UserResearch userResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
            List<BuildingCost> ListOfBuildingsCost = GetBuildingsCost(userResearch);
            //GetUpgradeBuildings..only need one for each, can calculate costs off of it

            return new JsonResult(new { city = UserCity, userItems = UserItems, userResearch = userResearch, newBuildingsCost = ListOfBuildingsCost });
        }

        public class UpdateCityModel
        {
            public int cityId { get; set; }
            public int buildingId { get; set; }
            public string buildingType { get; set; }
            public int level { get; set; }
        }


        private BuildingCost GetCostOfBuilding(UpdateCityModel model, UserResearch userResearch)
        {

            BuildingCost bc = new BuildingCost();
            switch (model.buildingType)
            {
                case "Cottage":
                    bc.type = "Cottage";
                    bc.food = CottFoodReq * Convert.ToInt32(Math.Pow(2, model.level - 1));
                    bc.stone = CottStoneReq * Convert.ToInt32(Math.Pow(2, model.level - 1));
                    bc.wood = CottWoodReq * Convert.ToInt32(Math.Pow(2, model.level - 1));
                    bc.iron = CottIronReq * Convert.ToInt32(Math.Pow(2, model.level - 1));
                    bc.time = CottTimeReq * Convert.ToInt32(Math.Pow(2, model.level - 1));
                    break;
                case "Inn":
                    bc.type = "Inn";
                    bc.food = InnFoodReq * Convert.ToInt32(Math.Pow(2, model.level - 1));
                    bc.stone = InnStoneReq * Convert.ToInt32(Math.Pow(2, model.level - 1));
                    bc.wood = InnWoodReq * Convert.ToInt32(Math.Pow(2, model.level - 1));
                    bc.iron = InnIronReq * Convert.ToInt32(Math.Pow(2, model.level - 1));
                    bc.time = InnTimeReq * Convert.ToInt32(Math.Pow(2, model.level - 1));
                    break;

                case "Acedemy":
                    //Console.WriteLine("Failed measurement.");
                    break;

                case "Barrack":
                    //Console.WriteLine("Failed measurement.");
                    break;
            }
            return bc;

        }

        //public class Result { 
        //    public bool passed { get; set; }
        //    public string message { get; set; }
        
        //}
        private string CheckIfPreReqMet(City city, UpdateCityModel update) {
            string res =  "ok";
            var updateBuilding = city.Buildings.Where(c => c.BuildingId == update.buildingId).FirstOrDefault();
            
            //No building can be more than one level greater than th.
            var th = city.Buildings.Where(c => c.BuildingType == CityBuildingType.Town_Hall).FirstOrDefault();
            if (update.level - 1 > th.Level)
            {
                res = "Need to upgrade the Town Hall.";
            }
            if (update.buildingType == "Inn") { 
                var cottageLvl2 = city.Buildings.Where(c => c.BuildingType == CityBuildingType.Cottage && c.Level >=2 ).FirstOrDefault();
                if (cottageLvl2 == null) {
                    res = "Must build a Cottage to level 2.";
                }
            }
            if (update.buildingType == "Barrack")
            {
                var RallySpot = city.Buildings.Where(c => c.BuildingType == CityBuildingType.Rally_Spot).FirstOrDefault();
                if (RallySpot == null)
                {
                    res = "Must build a RallySpot.";
                }
            }
            return res;
        }

        [HttpPost("UpdateCity")]
        public async Task<JsonResult> UpdateCity([FromBody] UpdateCityModel update)
        {

            var message = "ok";
            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var u = await _userManager.FindByIdAsync(UserId);

            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == update.cityId).FirstOrDefaultAsync();
            UserResearch UserResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync();

            //Check if builders busy ..
            string TestingResult = CheckIfPreReqMet(UserCity, update);
            if (TestingResult != "ok")
            {
                message = TestingResult;
                return new JsonResult(new { data = message });
            }

            BuildingCost BuildingCost= GetCostOfBuilding(update, UserResearch);
            
            //Check if user has enough resources ..
            await RemoveResourcesAndUpdateConstructionFromCity(UserCity, update, BuildingCost);
            
            
            List<BuildingCost> ListOfBuildingsCost = GetBuildingsCost(UserResearch);
            //GetUpgradeBuildings..only need one for each, can calculate costs off of it

            return new JsonResult(new { message = message, city = UserCity, });
        }

        private async Task RemoveResourcesAndUpdateConstructionFromCity(City UserCity, UpdateCityModel update, BuildingCost BuildingCost) {
            //bool ret = true;

            UserCity.Food = UserCity.Food - BuildingCost.food;
            UserCity.Stone = UserCity.Stone - BuildingCost.stone;
            UserCity.Wood = UserCity.Wood - BuildingCost.wood;
            UserCity.Iron = UserCity.Iron - BuildingCost.iron;

            UserCity.Construction1Started = DateTime.UtcNow;
            UserCity.Construction1Ends = DateTime.UtcNow.AddSeconds(BuildingCost.time);
            UserCity.Construction1BuildingId = update.buildingId;

            UserCity.Builder1Busy = true;
            UserCity.Builder1Time = BuildingCost.time;


            await db.SaveChangesAsync();
            
            //return ret;
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
                    BuildingType = CityBuildingType.Empty,
                    Level = 0,
                    Image = "emptyCitySlot.jpg"
                };
                NewCity.Buildings.Add(NewBuilding);
            }
            var th = NewCity.Buildings.Where(c => c.Location == 2).FirstOrDefault();
            th.BuildingType = CityBuildingType.Town_Hall;
            th.Level = 1;
            th.Image = "TownHall.jpg";
            await db.SaveChangesAsync();

            return NewCity;
        }

        public List<BuildingCost> GetBuildingsCost(UserResearch UserResearch)
        {
            List<BuildingCost> lbc = new List<BuildingCost>();
            BuildingCost cott = new BuildingCost()
            {
                type = BuildingType.Cottage.ToString(),
                preReq = "",
               food = CottFoodReq,
               stone = CottStoneReq,
               wood = CottWoodReq,
               iron = CottIronReq,
               time = CottTimeReq,
            };
            lbc.Add(cott);
            BuildingCost inn = new BuildingCost()
            {
                type = BuildingType.Inn.ToString(),
                preReq = InnPrereq,
                food = InnFoodReq,
                stone = InnStoneReq,
                wood = InnWoodReq,
                iron = InnIronReq,
                time = InnTimeReq,
            };
            lbc.Add(inn);
            BuildingCost barr = new BuildingCost()
            {
                type = BuildingType.Barrack.ToString(),
                preReq = BarrPrereq,
                food = BarrFoodReq,
                stone = BarrStoneReq,
                wood = BarrWoodReq,
                iron = BarrIronReq,
                time = BarrTimeReq,
            };
            lbc.Add(barr);

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
            return City;
        }

        #region Constants
//int BaseResRate = 100;

        #region Farms
        
        string FarmPrereq = " ";
        int FarmFoodReq = 50;// FoodReq = FarmFoodReq * 2^(nextLevel -1) 
        int FarmStoneReq = 200;
        int FarmWoodReq = 300;
        int FarmIronReq = 150;
        int FarmTimeReq = 30; // seconds TimeReq * 2^(nextLevel -1)
        int FarmPopulationReq = 10; // int level = 4; int farmPop = farmPopReq * level*(level + 1) / 2;
        int FarmFoodRate = 100;//  farmPopReq* level*(level + 1) / 2; Triangular number Seq //Tri sequence
        int FarmFoodCapacity = 10000; // Tri seq 10000, 30000, 60000, etc
                                      //quest level 1	Beginner Guidelines (1)	food 100 wood 500 stone 500 iron 300
                                      //quest level Level 5 food 1,000 wood 5,000 stone 3,000 iron 2,500

        int QuarryFoodReq = 180;//FarmFoodReq * 2^(nextLevel -1) 
        int QuarryStoneReq = 150;
        int QuarryWoodReq = 500;
        int QuarryIronReq = 400;
        int QuarryTimeReq = 60; //secs 2^
        int QuarryPopulationReq = 20; //tri seq
        int QuarryStoneRate = 100; //tri seq
        int QuarryStoneCap = 10000; //tri seq.
                                    //Quest lvl1 1	Beginner's Guidelines (1)	food 300 lumber 750 stone	300	iron 500

        int SawFoodReq = 100; //2^
        int SawWoodReq = 100;
        int SawStoneReq = 250;
        int SawIronReq = 300;
        int SawTimeReq = 45; //secs  2^
        int SawPopReq = 10; //tri seq
        int SawWoodRate = 100; //tri seq
        int SawMaxCap = 10000; //tri seq
                               //Quest Sawmill Lv.2 Award: Food 1,000 Lumber 1,000 Stone 1,000 Iron 1,000


        int IronMineFoodReq = 210;
        int IronMineWoodReq = 600;
        int IronMineStoneReq = 500;
        int IronMineIronReq = 200;
        int IronMineTimeReq = 90;
        int IronMinePopReq = 25;
        int IronMineRate = 100;
        int IronMineCap = 10000;
        //Quest: Level 1	Beginner Guidelines (1)	500	800	750	500
        #endregion

        
        #region Troops

        string WorkerBuildReq = "Barracks Level 1";
        int WorkerFoodCost = 50;
        int WorkerLumberCost = 150;
        int WorkerIronCost = 10;
        int WorkerTimeCost = 50;//secs
        int WorkerLife= 100;
        int WorkerPopulation = 1;
        int WorkerAttk= 5;
        int WorkerDef= 10;
        int WorkerLoad= 200;
        int WorkerFoodCity = 2; //per hour
        int WorkerSpeed= 180;
        int WorkerRange= 10;
        //int WorkerQuest = { qtyWorkers:10, award:{ food:500, lumber:1500, iron:100 } };


        string WarrBuildReq = "Barracks Level 1";
        int WarrFoodCost = 80;
        int WarrLumberCost = 100;
        int WarrIronCost = 50;
        int WarrTimeCost = 25;//secs
        int WarrLife= 200;
        int WarrPopulation = 1;
        int WarrAttk= 50;
        int WarrDef= 50;
        int WarrLoad= 20;
        int WarrFoodCity = 3; //per hour
        int WarrSpeed= 200;
        int WarrRange= 20;
        //int WarrQuest = { qtywarrs:10, award:{ food:800, lumber:1000, iron:500 } };

        string ScoutBuildReq = "Barracks Level 2";
        int ScoutFoodCost = 120;
        int ScoutLumberCost = 200;
        int ScoutIronCost = 150;
        int ScoutTimeCost = 100;//secs
        int ScoutLife= 100;
        int ScoutPopulation = 1;
        int ScoutAttk= 20;
        int ScoutDef= 20;
        int ScoutLoad= 5;
        int ScoutFoodCity = 5; //per hour
        int ScoutSpeed= 3000;
        int ScoutRange= 20;
        //int ScoutQuest = { qtyScouts:10, award:{ food:1200, lumber:2000, iron:1500 } };

        string PikeBuildReq = "Barracks Level 2";
        string PikeBuildReq2 = "Military Tradition Level 1";
int PikeFoodCost = 150;
        int PikeLumberCost = 500;
        int PikeIronCost = 100;
        int PikeTimeCost = 2 * 60 + 30;//secs
        int PikeLife= 300;
        int PikePopulation = 1;
        int PikeAttk= 150;
        int PikeDef= 150;
        int PikeLoad= 40;
        int PikeFoodCity = 6; //per hour
        int PikeSpeed= 300;
        int PikeRange= 50;
        // int PikeQuest = { qtyPikes:10, award:{ food:1500, lumber:5000, iron:1000 } };

        string SwordBuildReq = "Barracks Level 3";
        string SwordBuildReq2 = "Iron Working Level 1";
        int SwordFoodCost = 200;
        int SwordLumberCost = 150;
        int SwordIronCost = 400;
        int SwordTimeCost = 3 * 60 + 45;//secs
        int SwordLife= 350;
        int SwordPopulation = 1;
        int SwordAttk= 100;
        int SwordDef= 250;
        int SwordLoad= 30;
        int SwordFoodCity = 7; //per hour
        int SwordSpeed= 275;
        int SwordRange= 30;
        //int SwordQuest = { qtySwords:10, award:{ food:2000, lumber:1500, iron:4000 } };

        string ArchBuildReq = "Barracks Level 4";
        string ArchBuildReq2 = "Archery Level 1";
        int ArchFoodCost = 300;
        int ArchLumberCost = 350;
        int ArchIronCost = 300;
        int ArchTimeCost = 5 * 60 + 50;//secs
        int ArchLife= 250;
        int ArchPopulation = 2;
        int ArchAttk= 120;
        int ArchDef= 50;
        int ArchLoad= 25;
        int ArchFoodCity = 9; //per hour
        int ArchSpeed= 250;
        int ArchRange= 1200;
        //int ArchQuest = { qtyArchs:10, award:{ food:3000, lumber:3500, iron:3000 } };

        string CavBuildReq = "Barracks Level 5";
        string CavBuildReq2 = "Horseback Riding Level 1";
        int CavFoodCost = 1000;
        int CavLumberCost = 600;
        int CavIronCost = 500;
        int CavTimeCost = 8 * 60 + 20;//secs
        int CavLife= 500;
        int CavPopulation = 3;
        int CavAttk= 250;
        int CavDef= 180;
        int CavLoad= 100;
        int CavFoodCity = 18; //per hour
        int CavSpeed= 1000;
        int CavRange= 100;
        //int CavQuest = { qtyCavs:10, award:{ food:10000, lumber:6000, iron:5000 } };

        string CatBuildReq = "Barracks Level 7";
        string CatBuildReq2 = "Iron Working Level 5";
        string CatBuildReq3 = "Horseback Riding Level 5";
        int CatFoodCost = 2000;
        int CatLumberCost = 500;
        int CatIronCost = 2500;
        int CatTimeCost = 25 * 60;//secs
        int CatLife= 1000;
        int CatPopulation = 6;
        int CatAttk= 350;
        int CatDef= 350;
        int CatLoad= 80;
        int CatFoodCity = 35; //per hour
        int CatSpeed= 750;
        int CatRange= 80;
        //int CatQuest = { qtyCats:10, award:{ food:8000, lumber:10000, iron:8000 } };

        string TransBuildReq = "Barracks Level 6";
        string TransBuildReq2 = "Logistics Level 1";
        string TransBuildReq3 = "Metal Casting Level 5";
        int TransFoodCost = 600;
        int TransLumberCost = 1500;
        int TransIronCost = 350;
        int TransTimeCost = 16 * 60 + 40;//secs
        int TransLife= 700;
        int TransPopulation = 4;
        int TransAttk= 10;
        int TransDef= 60;
        int TransLoad= 5000;
        int TransFoodCity = 10; //per hour
        int TransSpeed= 150;
        int TransRange= 10;
        //Logistics lvl 1= 10% inc= 5500, lvl10 = 100% = 10000

        string BallBuildReq = "Barracks Level 9";
        string BallBuildReq2 = "Archery Level 6";
        string BallBuildReq3 = "Metal Casting Level 5";
        int BallFoodCost = 2500;
        int BallLumberCost = 3000;
        int BallIronCost = 1800;
        int BallTimeCost = 50 * 60;//secs
        int BallLife= 320;
        int BallPopulation = 5;
        int BallAttk= 450;
        int BallDef= 160;
        int BallLoad= 35;
        int BallFoodCity = 50; //per hour
        int BallSpeed= 100;
        int BallRange= 1400;

        string RamBuildReq = "Barracks Level 9";
        string RamBuildReq2 = "Iron Working Level 8";
        string RamBuildReq3 = "Metal Casting Level 7";
        int RamFoodCost = 4000;
        int RamLumberCost = 6000;
        int RamIronCost = 1500;
        int RamTimeCost = 75 * 60;//secs
        int RamLife= 5000;
        int RamPopulation = 10;
        int RamAttk= 250;
        int RamDef= 160;
        int RamLoad= 45;
        int RamFoodCity = 50; //per hour
        int RamSpeed= 120;
        int RamRange= 600;

        string CataBuildReq = "Barracks Level 10";
        string CataBuildReq2 = "Archery Level 10";
        string CataBuildReq3 = "Metal Casting Level 10";
        int CataFoodCost = 5000;
        int CataStoneCost = 8000;
        int CataLumberCost = 5000;
        int CataIronCost = 1200;
        int CataTimeCost = 100 * 60;//secs
        int CataLife= 480;
        int CataPopulation = 8;
        int CataAttk= 600;
        int CataDef= 200;
        int CataLoad= 75;
        int CataFoodCity = 250; //per hour
        int CataSpeed= 80;
        int CataRange= 1500;
        //int CataQuest = { qtyCatas:10, award:{ food:500, lumber:1500, iron:100 } };
        #endregion

        #region City
        string ThPrereq = "Walls Level Th-2";
        int ThFoodReq = 400;
        int ThStoneReq = 5000;
        int ThWoodReq = 6000;
        int ThIronReq = 200;
        int ThTimeReq = 30 * 60;

        string AcademyPrereq = "Town Hall Level 2";
        int AcademyFoodReq= 120; //reward prim
        int AcademyStoneReq= 1500;
        int AcademyWoodReq= 2500;
        int AcademyIronReq= 200;
        int AcademyTimeReq= 8 * 60;
        //lvl2 = lvl1*(2^currentlevel), etc. lvl 2 = 120 *2^2 =120*4
        //academyUnlocks
        //lvl1 = Agg, lumbering,MS
        //lvl2 = Masonry, mining,MT
        //lvl3=Metal Cast, info, Iron work
        //lvl4 Logistics, Compass,arch
        //lvl5 HBR, intruction
        //lvl6 stockpile, medicine
        //lvl7 ...nothing
        //lvl8 engineering
        //lvl9 machinery
        //lvl10 privatering.
        //GET quest reward for every table updrage.
        //lvl1 = prim guidline food 150,lumber 2500, stone 1500,iron 200 all * 2^currentlvl

        //Cotts
        //req after lvl2, requires (cottLvl-1) > Th.level
        int CottFoodReq= 100;//lvl2 = lvl1*(2^currentlevel)...lvl10 = 100 *2^9 ..100*12512
        int CottStoneReq = 100;
        int CottWoodReq= 500;
        int CottIronReq = 50;
        int CottTimeReq = 75;
        //int[] CottPopulationSupported =  [0, 100, 300, 600, 1000, 1500, 2100, 2800, 3600, 4500, 5500];
        //quest lvl1 = beg guid, amulet food:200 stone:200 wood:1000,iron:200, pop:30
        //quest lvl2=food:500 stone:500 wood:2000,iron:500,pop:50

        //warehouse
        int WareFoodReq= 100;//lvl2 = lvl1*(2^currentlevel)...
        int WareStoneReq = 1000;
        int WareWoodReq= 1500;
        int WareIronReq = 300;
        int WareTimeReq = 10 * 60;
        //int WareStorage = [0, 10000, 30000, 60000, 100000, 150000, 210000, 280000, 360000, 450000, 550000];
        //int wareQuest = //lv1 food:500 wood 2000, stone 1500,iron 500

        string InnPrereq = "Cottage Level 2";
        int InnFoodReq= 300;//lvl2 = lvl1*(2^currentlevel)...
        int InnStoneReq = 1000;
        int InnWoodReq= 2000;
        int InnIronReq = 400;
        int InnTimeReq = 4 * 60;
        //QtyHeros = lvl of inn
        //quest lvl1 food:500,wood:2000,stone1000,iron500

        //shows heros, click to view or delete, view has rename,exp,upgrade,redist,reward
        string FeastPrereq = "Inn Level 1";
        int FeastFoodReq= 400;//lvl2 = lvl1*(2^currentlevel)...
        int FeastStoneReq = 1200;
        int FeastWoodReq= 2500;
        int FeastIronReq = 700;
        int FeastTimeReq = 6 * 60;
        //heroqty=lvl
        //quest lvl1 gold1000,food1000,lumber3000, stone 1500, iron 1000

        string EmbassyPrereq = "Town Hall Level 2";
        int EmbassyFoodReq= 200;
        int EmbassyStoneReq = 500;
        int EmbassyWoodReq= 2000;
        int EmbassyIronReq = 300;
        int EmbassyTimeReq = 12 * 60;
        //Garrisons allowed = lvl
        //Alliance limit = lvl*10
        //quest lvl1 food500, wood2000,stone1000,iron 500

        string MarketPrereq = "";
        int MarketFoodReq= 1000;
        int MarketStoneReq = 1000;
        int MarketWoodReq= 1000;
        int MarketIronReq = 1000;
        int MarketTimeReq = 12 * 60 + 30;
        //transaction qty = lvl
        //quest lvl1 5cents, food1500,wood 1500,stone1500 iron 1500


        string RallyPrereq = "";
        int RallyFoodReq= 100;
        int RallyStoneReq = 2000;
        int RallyWoodReq= 600;
        int RallyIronReq = 150;
        int RallyTimeReq = 2 * 60 + 30;
        //int RallyMaxTroopsSent = lvl*10000;
        //no quest

        string BarrPrereq = "Rally Spot Level 1";
        int BarrFoodReq= 250;
        int BarrStoneReq = 1500;
        int BarrWoodReq= 1200;
        int BarrIronReq = 500;
        int BarrTimeReq = 5 * 60;
        //Unlocks= lvl1 = worker, warr; lvl2=scout,pike, lvl3 Sword
        //lvl4arch, lvl5 cab, lvl6 trans, lvl7 cata
        //lvl8 nothingk, lvl9 balls, battRam, lvl10 cata
        //Quest = award for every lvl upgrade

        string BeaconPrereq = "Barracks Level 1";
        int BeaconFoodReq= 150;
        int BeaintoneReq = 3000;
        int BeaconWoodReq= 1000;
        int BeaconIronReq = 500;
        int BeaconTimeReq = 7 * 60 + 30;
        //Unlocks= lvl1 = Pre inform invasion; lvl2= analysis of enemy purpose, lvl3 Show arrival time of enemy
        //lvl4 see enemy lord status lvl5 depart location of enemy lvl6 gives arms branch of enemy lvl7 approx numbers of enemy
        //lvl8 exact # of enemy lvl9 enemy lvl hero, lvl10 gives enemy MT
        //Quest = award for every lvl upgrade

        string ForgePrereq = "Iron Mine Level 3";
        int ForgeFoodReq= 125;
        int ForgeStoneReq = 600;
        int ForgeWoodReq= 1000;
        int ForgeIronReq = 1200;
        int ForgeTimeReq = 3 * 60;
        //quest lvl1 f500,w1500, w2000, i 1500

        string StablePrereq = "Farm Level 5";
        int StableFoodReq= 1200;
        int StableStoneReq = 800;
        int StableWoodReq= 2000;
        int StableIronReq = 1000;
        int StableTimeReq = 4 * 60 + 30;
        //each lvl = HBRlvl ..for barrs
        //quest lvl1 f1000,w5000, s3000, i 2500

        string WorkshopPrereq = "Forge Level 2";
        int WorkshopFoodReq= 150;
        int WorkshopStoneReq = 500;
        int WorkshopWoodReq= 1500;
        int WorkshopIronReq = 1500;
        int WorkshopTimeReq = 9 * 60;
        //each lvl = hero star lvl max
        //quest lvl1 f1500,s5000, w5000, i 5000

        string ReliefPrereq = "Stable Level 1 and HBR level = level";
        int ReliefFoodReq= 1500;
        int ReliefStoneReq = 4500;
        int ReliefWoodReq= 5000;
        int ReliefIronReq = 500;
        int ReliefTimeReq = 60 * 60;
        //lvl1-2 = 2x speed, lvl3 =3x, 5-7 4x, 8-9 = 5x, 10=6x
        //quest lvl1 f2000,s5000, w5000, i 1000


        #endregion

        //walls...


        #region Research
        string AggPrereq = "Academy lvl1, farm Level = level";//lvl3 needs a farm lvl3
        int AggWoodReq= 500; //lvl3 500*2*2
        int AggGoldReq = 1000;
        int AggTimeReq = 6 * 60 + 40;
        //each lvl = 10% inc in food , lvl5 = 50%

        string LumberingPrereq = "Academy lvl1, wood Level = level";//lvl3 needs a farm lvl3
        int LumberingWoodReq= 500; //lvl3 500*2*2
        int LumberingIronReq= 100; //lvl3 500*2*2
        int LumberingGoldReq = 1200;
        int LumberingTimeReq = 8 * 60 + 20;
        //each lvl = 10% inc in food , lvl5 = 50%

        string MasonryPrereq = "Academy lvl2, wood Level = level";//lvl3 needs a farm lvl3
        int MasonryStoneReq= 500; //lvl3 500*2*2
        int MasonryIronReq= 200; //lvl3 500*2*2
        int MasonryGoldReq = 1500;
        int MasonryTimeReq = 10 * 60;
        //each lvl = 10% inc in food , lvl5 = 50%

        string MiningPrereq = "Academy lvl2, Masonry Level 1";//lvl3 needs a ironmine lvl3
        int MiningIronReq= 800; //lvl3 500*2*2
        int MiningGoldReq = 2000;
        int MiningTimeReq = 11 * 60 + 40;
        //each lvl = 10% inc in food , lvl5 = 50%


        string MetalCastingPrereq = "Academy lvl3, Mining Level 2";
        int MetalCastingWoodReq= 500;
        int MetalCastingIronReq= 500; //lvl3 500*2*2
        int MetalCastingGoldReq = 5000;
        int MetalCastingTimeReq = 15 * 60;
        //each lvl = 10% inc in production of Mechs (balls, rams, etc)

        #endregion

        


        #endregion

    }

    
}
