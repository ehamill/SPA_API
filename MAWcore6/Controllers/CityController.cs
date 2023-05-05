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
            //var u = await _userManager.FindByIdAsync(UserId);
            City UserCity = new City();
            UserItems UserItems = new UserItems();
            UserResearch userResearch = new UserResearch();
            List<TroopQueue> TroopQueues = new List<TroopQueue>();

            try
            {
                UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? await CreateCity(UserId);
                UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
                userResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync();
                TroopQueues = await db.TroopQueues.Where(c => c.CityId == UserCity.CityId && c.Complete == false).ToListAsync();
            }
            catch (Exception ex)
            {
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
            if (UserCity.Builder1Busy)
            {
                await CheckBuilder1(UserCity);
            }
            //GetUpgradeBuildings..only need one for each, can calculate costs off of it
            //Sleep doesn't work..
            //System.Diagnostics.Debug.WriteLine("Testing ... ");
            //Attack(UserCity.CityId);

            return new JsonResult(new { city = UserCity, heros = Heros, troops = Troops, troopQueues = TroopQueues, wallDefenses = WallDefenses, userItems = UserItems, userResearch = userResearch, newBuildingsCost = ListOfBuildingsCost });
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

        [HttpPost("TrainTroops")]
        public async Task<JsonResult> TrainTroops([FromBody] TrainTroopsModel update)
        {
            var message = "ok";

            string UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == update.cityId).FirstOrDefaultAsync() ?? new City();
            //Update Resources...
            UserResearch UserResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserResearch();
            List<TroopQueue> TroopQueues = await db.TroopQueues.Where(c => c.CityId == UserCity.CityId).ToListAsync();
            await CheckTroopQueues(TroopQueues, UserCity);
            var Building = UserCity.Buildings.Where(c => c.BuildingId == update.buildingId).FirstOrDefault();
            TroopType TroopType = (TroopType)update.troopTypeInt;
            Result check = CheckTroopRequirements(Building, TroopType, UserResearch, TroopQueues.Count()); //, barrack lvl and research, check if queue full
            if (check.Failed)
            {
                return new JsonResult(new { message = check.Message });
            }

            check = CheckResources(UserCity, UserResearch, TroopType, update.qty);
            if (check.Failed)
            {
                return new JsonResult(new { message = check.Message });
            }
            //Get cost of troop, b/c used in Remove resources and troop queue
            await RemoveResourcesForTroops(UserCity, UserResearch, TroopType, update.qty);
            await TroopQueueAdd(update, TroopQueues, UserCity, UserResearch);


            return new JsonResult(new { message = message, city = UserCity, troopQueues = TroopQueues });
        }

        [HttpPost("UpdateCity")]
        public async Task<JsonResult> UpdateCity([FromBody] UpdateCityModel update)
        {
            string message = CheckForUpdateErrors(update);

            if (message != "ok")
            {
                return new JsonResult(new { message = message });
            }

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

            BuildingCost BuildingCost = GetUpgradeCostOfBuilding(update, UserCity, UserResearch);

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

            City UserCity = await db.Cities.Include(c => c.Buildings).Where(c => c.CityId == model.cityId).FirstOrDefaultAsync() ?? new City();
            //UserResearch UserResearch = await db.UserResearch.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserResearch();
            UserItems UserItems = await db.UserItems.Where(c => c.UserId == UserId).FirstOrDefaultAsync() ?? new UserItems();

            if (model.usedOn == "builder1")
            {
                if (model.speedUp5min)
                {
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

            if (UserCity.UserId != UserId)
            {
                message = "You dont own this city.";
                return new JsonResult(new { message = message });
            }
            var heros = await db.Heros.Where(c => c.CityId == model.CityId).ToListAsync();

            var HiredHero = heros.Where(c => c.HeroId == model.HeroId).FirstOrDefault();

            var HeroCostInGold = HiredHero.Level * 1000;

            //await UpdateResources(UserCity);

            var GotEnoughGold = UserCity.Gold - HeroCostInGold;

            if (GotEnoughGold < 0)
            {
                return new JsonResult(new { message = "Need more gold for this hero." });
            }
            var removeGold = new Resources()
            {
                Gold = HeroCostInGold,
            };

            await RemoveResourcesFromCity(removeGold, UserCity);

            HiredHero.IsHired = true;
            await db.SaveChangesAsync();

            var newHero = await CreateHeros(1, UserCity.CityId);

            heros.Add(newHero.FirstOrDefault());

            return new JsonResult(new { message = message, city = UserCity });
        }

        private List<Troop> GetTroops(City city, UserResearch research)
        {
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
                TypeString = TroopType.Rolling_Log.ToString().Replace("_", " "),
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
                typeString = TroopType.Worker.ToString(),
                troopType = TroopType.Worker,
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
                typeString = TroopType.Warrior.ToString(),
                troopType = TroopType.Warrior,
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
                typeString = TroopType.Scout.ToString(),
                troopType = TroopType.Scout,
                preReq = Constants.ScoutBuildReq,
                reqMet = false,
                food = Constants.ScoutFoodCost,
                stone = 0,
                wood = Constants.ScoutWoodCost,
                iron = Constants.ScoutIronCost,
                time = Constants.ScoutTimeCost,
            };
            cost.Add(scout);

            var pike = new BuildingCost()
            {
                typeString = TroopType.Pikeman.ToString(),
                troopType = TroopType.Pikeman,
                preReq = Constants.PikeBuildReq,
                reqMet = false,
                food = Constants.PikeFoodCost,
                stone = 0,
                wood = Constants.PikeWoodCost,
                iron = Constants.PikeIronCost,
                time = Constants.PikeTimeCost,
            };
            cost.Add(pike);

            var arch = new BuildingCost()
            {
                typeString = TroopType.Archer.ToString(),
                troopType = TroopType.Archer,
                preReq = Constants.ArchBuildReq,
                reqMet = false,
                food = Constants.ArchFoodCost,
                stone = 0,
                wood = Constants.ArchWoodCost,
                iron = Constants.ArchIronCost,
                time = Constants.ArchTimeCost,
            };
            cost.Add(arch);

            var cav = new BuildingCost()
            {
                typeString = TroopType.Cavalry.ToString(),
                troopType = TroopType.Cavalry,
                preReq = Constants.CavBuildReq,
                reqMet = false,
                food = Constants.CavFoodCost,
                stone = 0,
                wood = Constants.CavWoodCost,
                iron = Constants.CavIronCost,
                time = Constants.CavTimeCost,
            };
            cost.Add(cav);

            var ball = new BuildingCost()
            {
                typeString = TroopType.Ballista.ToString(),
                troopType = TroopType.Ballista,
                preReq = Constants.BallBuildReq,
                reqMet = false,
                food = Constants.BallFoodCost,
                stone = 0,
                wood = Constants.BallWoodCost,
                iron = Constants.BallIronCost,
                time = Constants.BallTimeCost,
            };
            cost.Add(ball);

            var cata = new BuildingCost()
            {
                typeString = TroopType.Catapult.ToString(),
                troopType = TroopType.Catapult,
                preReq = Constants.CataBuildReq,
                reqMet = false,
                food = Constants.CataFoodCost,
                stone = Constants.CataStoneCost,
                wood = Constants.CataWoodCost,
                iron = Constants.CataIronCost,
                time = Constants.CataTimeCost,
            };
            cost.Add(cata);

            var Trap = new BuildingCost()
            {
                typeString = TroopType.Trap.ToString(),
                troopType = TroopType.Trap,
                preReq = "Requires Walls level 1.",
                reqMet = false,
                food = Constants.TrapFoodReq,
                stone = Constants.TrapStoneReq,
                wood = Constants.TrapWoodReq,
                iron = Constants.TrapIronReq,
                time = Constants.TrapTimeReq,
            };
            cost.Add(Trap);


            var Abatis = new BuildingCost()
            {
                typeString = TroopType.Abatis.ToString().Replace("_", " "),
                troopType = TroopType.Abatis,
                preReq = "Requires Walls level 2.",
                reqMet = false,
                food = Constants.AbatisFoodReq,
                stone = Constants.AbatisStoneReq,
                wood = Constants.AbatisWoodReq,
                iron = Constants.AbatisIronReq,
                time = Constants.AbatisTimeReq,
            };
            cost.Add(Abatis);

            var AT = new BuildingCost()
            {
                typeString = TroopType.Archers_Tower.ToString().Replace("_", " "),
                troopType = TroopType.Archers_Tower,
                preReq = "Requires Walls level 3.",
                reqMet = false,
                food = Constants.ATFoodReq,
                stone = Constants.ATStoneReq,
                wood = Constants.ATWoodReq,
                iron = Constants.ATIronReq,
                time = Constants.ATTimeReq,
            };
            cost.Add(AT);

            var rl = new BuildingCost()
            {
                typeString = TroopType.Rolling_Log.ToString().Replace("_", " "),
                troopType = TroopType.Rolling_Log,
                preReq = "Requires Walls level 5.",
                reqMet = false,
                food = Constants.RollLogFoodReq,
                stone = Constants.RollLogStoneReq,
                wood = Constants.RollLogWoodReq,
                iron = Constants.RollLogIronReq,
                time = Constants.RollLogTimeReq,
            };
            cost.Add(rl);

            var treb = new BuildingCost()
            {
                typeString = TroopType.Defensive_Trebuchet.ToString(),
                troopType = TroopType.Defensive_Trebuchet,
                preReq = "Requires Walls level 7.",
                reqMet = false,
                food = Constants.TrebFoodReq,
                stone = Constants.TrebStoneReq,
                wood = Constants.TrebWoodReq,
                iron = Constants.TrebIronReq,
                time = Constants.TrebTimeReq,
            };
            cost.Add(treb);



            return cost;
        }
        private void GetHeroPercentages(List<Hero> NewHeros)
        {
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


        public async Task<List<Hero>> CreateHeros(int Qty, int CityId)
        {
            List<Hero> NewHeros = new List<Hero>();
            //System.Diagnostics.Debug.WriteLine("hero" + i + ": Pol:" + PolPoints + " Attck: " + AttkPoints + " intel: " + IntelPoints);

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
                NewHero.Name = "George" + random.Next(1, 300);

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

        public async Task<List<Hero>> GetHeros(City userCity)
        {
            var CityHeros = await db.Heros.Where(c => c.CityId == userCity.CityId).ToListAsync();

            if (CityHeros.Where(c => c.IsHired == false).Count() < 5)
            {
                int Qty = 5 - CityHeros.Where(c => c.IsHired == false).Count();
                List<Hero> newHeros = await CreateHeros(Qty, userCity.CityId);
                foreach (var hero in newHeros)
                {
                    CityHeros.Add(hero);
                }
            }
            //System.Diagnostics.Debug.WriteLine("hero" + i + ": Pol:" + PolPoints + " Attck: " + AttkPoints + " intel: " + IntelPoints);

            //GetHeroPercentages(NewHeros);

            return CityHeros;

        }


        public async Task CheckBuilder1(City userCity)
        {
            DateTime TimeNow = DateTime.UtcNow;
            DateTime ConstructionEnds = userCity.Construction1Ends;

            if (userCity.Builder1Busy == false)
            {
                return;
            }
            //Can be busy in number of ways:
            // 1. Click build a building, and Construction has not ended yet. recalculate time.
            // 2. Building was done a long time ago. TimeNow > ConstructionEnds
            if (TimeNow >= ConstructionEnds && userCity.Construction1BuildingId != -1)
            {
                userCity.Builder1Busy = false;

                var b = userCity.Buildings.Where(c => c.BuildingId == userCity.Construction1BuildingId).FirstOrDefault();
                if (b.Level - userCity.Construction1BuildingLevel > 0)
                {
                    //downgrading.. add res
                }

                b.Level = userCity.Construction1BuildingLevel;
                BuildingType BuildingType = GetBuildingType(userCity.BuildingWhat);
                if (b.Level == 0)
                {
                    BuildingType = BuildingType.Empty;
                    b.BuildingType = BuildingType.Empty;
                }
                b.Image = BuildingType.ToString() + "lvl" + b.Level + ".jpg";
                userCity.Construction1BuildingId = -1; //Building Complete.
                userCity.Construction1BuildingLevel = -1;
                userCity.Builder1Time = -1;
            }
            else
            {
                userCity.Builder1Time = Convert.ToInt32(Math.Floor((ConstructionEnds - TimeNow).TotalSeconds));
            }
            await db.SaveChangesAsync();
        }

        public async Task UpdateResources(City userCity)
        {
            DateTime TimeNow = DateTime.UtcNow;

            int duration = Convert.ToInt32(Math.Floor((DateTime.UtcNow - userCity.ResourcesLastUpdated).TotalMinutes));
            duration = 6;
            if (duration < 5)
            {
                return;
            }
            int FoodRate = 100;
            int StoneRate = 100;
            int WoodRate = 100;
            int IronRate = 100;
            int GoldRate = 0;

            var farms = userCity.Buildings.Where(c => c.BuildingType == BuildingType.Farm).ToList();
            if (farms.Count() > 0)
            {
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

            await db.SaveChangesAsync();
        }

        public class UpdateCityModel
        {
            public int cityId { get; set; }
            public int buildingId { get; set; }
            public string? buildingTypeString { get; set; }
            public int buildingTypeInt { get; set; }
            public int level { get; set; }
            public int location { get; set; } = -1;
        }

        public class TroopPreReqCheck
        {
            public string buildingType { get; set; }
            public bool reqMet { get; set; }
        }

        private BuildingCost GetUpgradeCostOfBuilding(UpdateCityModel update, City userCity, UserResearch userResearch)
        {
            var building = userCity.Buildings.Where(c => c.BuildingId == update.buildingId).FirstOrDefault();
            //if downgrading, time is current level.
            var level = update.level;
            if (building.Level > update.level)
            {
                level++;
            }
            BuildingCost bc = new BuildingCost();

            //BuildingType BuildingType = GetBuildingType(update.buildingType);
            //int buildingTypeInt = update.buildingTypeInt;
            switch ((BuildingType)update.buildingTypeInt)
            {
                case BuildingType.Academy:
                    bc.typeString = BuildingType.Academy.ToString();
                    bc.buildingTypeInt = BuildingType.Academy;
                    bc.food = Constants.AcademyFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.AcademyStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.AcademyWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.AcademyIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.AcademyTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;

                case BuildingType.Barrack:
                    bc.typeString = BuildingType.Barrack.ToString();
                    bc.buildingTypeInt = BuildingType.Barrack;
                    bc.food = Constants.BarrFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.BarrStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.BarrWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.BarrIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.BarrTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Cottage:
                    bc.typeString = BuildingType.Cottage.ToString();
                    bc.buildingTypeInt = BuildingType.Cottage;
                    bc.food = Constants.CottFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.CottStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.CottWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.CottIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.CottTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Feasting_Hall:
                    bc.typeString = BuildingType.Feasting_Hall.ToString().Replace("_", " ");
                    bc.buildingTypeInt = BuildingType.Feasting_Hall;
                    bc.food = Constants.FeastFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.FeastStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.FeastWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.FeastIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.FeastTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Forge:
                    bc.typeString = BuildingType.Forge.ToString();
                    bc.buildingTypeInt = BuildingType.Forge;
                    bc.food = Constants.ForgeFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.ForgeStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.ForgeWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.ForgeIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.ForgeTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Inn:
                    bc.typeString = BuildingType.Inn.ToString();
                    bc.buildingTypeInt = BuildingType.Inn;
                    bc.food = Constants.InnFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.InnStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.InnWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.InnIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.InnTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Rally_Spot:
                    bc.typeString = BuildingType.Rally_Spot.ToString().Replace("_", " ");
                    bc.buildingTypeInt = BuildingType.Rally_Spot;
                    bc.food = Constants.RallyFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.RallyStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.RallyWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.RallyIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.RallyTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Town_Hall:
                    bc.typeString = BuildingType.Town_Hall.ToString();
                    bc.buildingTypeInt = BuildingType.Town_Hall;
                    bc.food = Constants.ThFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.ThStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.ThWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.ThIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.ThTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Walls:
                    bc.typeString = BuildingType.Walls.ToString();
                    bc.buildingTypeInt = BuildingType.Walls;
                    bc.food = Constants.ThFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.ThStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.ThWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.ThIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.ThTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Farm:
                    bc.typeString = BuildingType.Farm.ToString();
                    bc.buildingTypeInt = BuildingType.Farm;
                    bc.food = Constants.FarmFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.FarmStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.FarmWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.FarmIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.FarmTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
                case BuildingType.Quarry:
                    bc.typeString = BuildingType.Quarry.ToString();
                    bc.buildingTypeInt = BuildingType.Quarry;
                    bc.food = Constants.QuarryFoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.stone = Constants.QuarryStoneReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.wood = Constants.QuarryWoodReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.iron = Constants.QuarryIronReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    bc.time = Constants.QuarryTimeReq * Convert.ToInt32(Math.Pow(2, level - 1));
                    break;
            }
            return bc;

        }

        private string CheckIfBuildingPreReqMet(City city, UpdateCityModel update)
        {
            string res = "ok";
            //var updateBuilding = city.Buildings.Where(c => c.BuildingId == update.buildingId).FirstOrDefault();
            BuildingType buildingType = (BuildingType)update.buildingTypeInt;
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
                if (update.level - 1 > th.Level)
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

        public class TrainTroopsModel
        {
            public int cityId { get; set; }
            public int buildingId { get; set; }
            public int troopTypeInt { get; set; }
            public int qty { get; set; }
        }

        private async Task AddTroopsToCity(TroopQueue queue, City city)
        {

            switch (queue.TroopTypeInt)
            {
                case TroopType.Worker:
                    city.WorkerQty = city.WorkerQty + queue.Qty;
                    return;
                case TroopType.Warrior:
                    city.WarriorQty = city.WarriorQty + queue.Qty;
                    return;
                case TroopType.Scout:
                    city.ScoutQty = city.ScoutQty + queue.Qty;
                    return;
                case TroopType.Pikeman:
                    city.PikemanQty = city.PikemanQty + queue.Qty;
                    return;
                case (TroopType.Swordsman):
                    city.SwordsmanQty += queue.Qty;
                    break;
                case TroopType.Transporter:
                    city.TransporterQty = city.TransporterQty + queue.Qty;
                    return;
                case TroopType.Archer:
                    city.ArcherQty = city.ArcherQty + queue.Qty;
                    return;
                case TroopType.Cavalry:
                    city.CavalierQty = city.CavalierQty + queue.Qty;
                    return;
                case TroopType.Cataphract:
                    city.CataphractQty = city.CataphractQty + queue.Qty;
                    return;
                case TroopType.Ballista:
                    city.BallistaQty = city.BallistaQty + queue.Qty;
                    return;
                case TroopType.Catapult:
                    city.CatapultQty = city.CatapultQty + queue.Qty;
                    return;

                case TroopType.Trap:
                    city.TrapQty = city.TrapQty + queue.Qty;
                    return;
                case TroopType.Abatis:
                    city.AbatisQty = city.AbatisQty + queue.Qty;
                    return;
                case TroopType.Archers_Tower:
                    city.ArcherTowerQty = city.ArcherTowerQty + queue.Qty;
                    return;
                case TroopType.Rolling_Log:
                    city.RollingLogQty = city.RollingLogQty + queue.Qty;
                    return;
                case TroopType.Defensive_Trebuchet:
                    city.TrebuchetQty = city.TrebuchetQty + queue.Qty;
                    return;
                default:
                    //log error..
                    return;
            }

            queue.Complete = true;

            await db.SaveChangesAsync();
        }
        private async Task DeleteTroopQueues(List<int> queueIds)
        {
            foreach (var id in queueIds)
            {
                var q = await db.TroopQueues.Where(c => c.TroopQueueId == id).FirstOrDefaultAsync();
                db.TroopQueues.Remove(q);
            }
            await db.SaveChangesAsync();
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
        }
        private async Task TroopQueueAdd(TrainTroopsModel update, List<TroopQueue> troopQueues, City city, UserResearch userResearch)
        {
            var costOfTroops = GetCostOfTroops(city, userResearch);
            BuildingCost singleTroopCost = costOfTroops.Where(c => c.troopType == (TroopType)update.troopTypeInt).FirstOrDefault();

            bool walls = (update.troopTypeInt >= 13) ? true : false;

            var troopQueue = new TroopQueue()
            {
                Starts = DateTime.UtcNow,
                Ends = DateTime.UtcNow.AddSeconds(singleTroopCost.time * update.qty),
                Qty = update.qty,
                BuildingId = update.buildingId,
                CityId = city.CityId,
                TroopTypeInt = (TroopType)update.troopTypeInt,
                TroopTypeString = ((TroopType)update.troopTypeInt).ToString(),
                TimeLeft = singleTroopCost.time * update.qty,
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
            BuildingCost singleTroopCost = costOfTroops.Where(c => c.troopType == type).FirstOrDefault();

            city.Food = city.Food - singleTroopCost.food * qty;
            city.Stone = city.Stone - singleTroopCost.stone * qty;
            city.Wood = city.Wood - singleTroopCost.wood * qty;
            city.Iron = city.Iron - singleTroopCost.iron * qty;

            await db.SaveChangesAsync();
        }
        public class Result
        {
            public bool Failed { get; set; }
            public string Message { get; set; }
        }
        private Result CheckResources(City city, UserResearch userResearch, TroopType type, int qty)
        {
            Result result = new Result() { Failed = false, Message = "" };

            var costOfTroops = GetCostOfTroops(city, userResearch);
            var singleTroopCost = costOfTroops.Where(c => c.troopType == type).FirstOrDefault();

            if (city.Food - singleTroopCost.food * qty < 0)
            {
                result.Failed = true;
                result.Message += "Requires " + singleTroopCost.food * qty + " food. ";
            }
            if (city.Stone - singleTroopCost.stone * qty < 0)
            {
                result.Failed = true;
                result.Message += "Requires " + singleTroopCost.stone * qty + " stone. ";
            }
            if (city.Wood - singleTroopCost.wood * qty < 0)
            {
                result.Failed = true;
                result.Message += "Requires " + singleTroopCost.wood * qty + " wood. ";
            }
            if (city.Iron - singleTroopCost.iron * qty < 0)
            {
                result.Failed = true;
                result.Message += "Requires " + singleTroopCost.iron * qty + " iron. ";
            }

            return result;
        }

        private Result CheckTroopRequirements(Building building, TroopType type, UserResearch research, int queueCount)
        {
            Result result = new Result() { Failed = false, Message = "" };

            if (building.BuildingType == BuildingType.Walls)
            {
                return result;
            }

            if (queueCount >= building.Level)
            {
                result.Failed = true;
                result.Message = "Queue is full. Upgrade level or cancel troops to add to queue.";
            }

            if (type == TroopType.Worker || type == TroopType.Warrior)
            {
                if (building.Level < 1)
                {
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

        private string CheckForUpdateErrors(UpdateCityModel update)
        {
            string result = "ok";
            if (update.cityId == 0)
            {
                result += "City not found. update.cityId == " + update.cityId.ToString();
            }
            if (update.buildingId == 0)
            {
                result += "No buildingId found. update.buildingId == " + update.buildingId.ToString();
            }
            if (update.buildingTypeInt == 0)
            {
                result += "No building found. update.buildingTypeInt == " + update.buildingTypeInt.ToString();
            }

            return result;
        }


        public class SpeedUpModel
        {
            public int cityId { get; set; }
            public bool speedUp5min { get; set; }
            public string usedOn { get; set; }
        }



        public class HireHeroModel
        {
            public int CityId { get; set; }
            public int HeroId { get; set; }
        }



        public class Resources
        {
            public int Food { get; set; } = 0;
            public int Stone { get; set; } = 0;
            public int Wood { get; set; } = 0;
            public int Iron { get; set; } = 0;
            public int Gold { get; set; } = 0;
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

        public BuildingType GetBuildingType(string name)
        {
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
            else
            {
                return BuildingType.Not_Found;
            }

        }

        private async Task RemoveResourcesAndUpdateConstructionFromCity(City UserCity, UpdateCityModel update, BuildingCost BuildingCost)
        {

            BuildingType buildingType = (BuildingType)update.buildingTypeInt; //GetBuildingType(update.buildingType);

            var b = UserCity.Buildings.Where(c => c.BuildingId == update.buildingId).FirstOrDefault();
            string upgrading = "upgrading";
            if (update.level - b.Level < 0)
            {
                upgrading = "downgrading";
                //if (update.level == 0) {
                //    buildingType = BuildingType.Empty;
                //}
            }
            if (upgrading == "upgrading")
            {
                UserCity.Food = UserCity.Food - BuildingCost.food;
                UserCity.Stone = UserCity.Stone - BuildingCost.stone;
                UserCity.Wood = UserCity.Wood - BuildingCost.wood;
                UserCity.Iron = UserCity.Iron - BuildingCost.iron;
            }
            else
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


            b.Image = upgrading + buildingType.ToString() + "lvl" + update.level;
            b.BuildingType = buildingType;
            b.Description = GetBuildingDescription(buildingType);

            UserCity.BuildingWhat = buildingType.ToString();

            await db.SaveChangesAsync();
        }
        public async Task<City> CreateCity(string UserID)
        {

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

            UserItems NewUserItems = new UserItems()
            {
                UserId = UserID
            };
            await db.UserItems.AddAsync(NewUserItems);

            UserResearch newUR = new UserResearch()
            {
                UserId = UserID,
            };
            await db.UserResearch.AddAsync(newUR);

            await db.SaveChangesAsync();

            for (int i = 0; i <= 25; i++)
            {
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

        private string GetBuildingDescription(BuildingType buildingType)
        {
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
                        + " and the higher level of hero.";
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

        private List<BuildingCost> GetNewBuildingsCost(City userCity, UserResearch userResearch)
        {
            List<BuildingCost> lbc = new List<BuildingCost>();
            //[Base Building Time] *(0.9) ^[Construction Level]
            int time = Convert.ToInt32(Math.Ceiling(60 * Math.Pow(0.9, userResearch.Building)));

            UpdateCityModel update = new UpdateCityModel()
            {
                buildingTypeString = "Cottage",
                buildingTypeInt = (int)BuildingType.Cottage,
            };
            string TestingResult = "ok";
            bool requirementsMet = true;
            if (TestingResult != "ok")
            {
                requirementsMet = false;
            }
            BuildingCost cott = new BuildingCost()
            {
                typeString = BuildingType.Cottage.ToString(),
                buildingTypeInt = BuildingType.Cottage,
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
                update.buildingTypeString = "Academy";
                update.buildingTypeInt = (int)BuildingType.Academy;
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost Academy = new BuildingCost()
                {
                    typeString = BuildingType.Academy.ToString().Replace("_", " "),
                    buildingTypeInt = BuildingType.Academy,
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

            update.buildingTypeString = "Barrack";
            update.buildingTypeInt = (int)BuildingType.Barrack;
            TestingResult = CheckIfBuildingPreReqMet(userCity, update);
            requirementsMet = true;
            if (TestingResult != "ok")
            {
                requirementsMet = false;
            }
            BuildingCost barr = new BuildingCost()
            {
                typeString = BuildingType.Barrack.ToString(),
                buildingTypeInt = BuildingType.Barrack,
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
            if (BuildingCount == 0)
            {
                update.buildingTypeString = "Feasting";
                update.buildingTypeInt = (int)BuildingType.Feasting_Hall;
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost Feast = new BuildingCost()
                {
                    typeString = BuildingType.Feasting_Hall.ToString().Replace("_", " "),
                    buildingTypeInt = BuildingType.Feasting_Hall,
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
                update.buildingTypeString = "Forge";
                update.buildingTypeInt = (int)BuildingType.Forge;
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost Forge = new BuildingCost()
                {
                    typeString = BuildingType.Forge.ToString().Replace("_", " "),
                    buildingTypeInt = BuildingType.Forge,
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
                update.buildingTypeString = "Inn";
                update.buildingTypeInt = (int)BuildingType.Inn;
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost inn = new BuildingCost()
                {
                    typeString = BuildingType.Inn.ToString(),
                    buildingTypeInt = BuildingType.Inn,
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
                update.buildingTypeString = "Rally";
                update.buildingTypeInt = (int)BuildingType.Rally_Spot;
                TestingResult = CheckIfBuildingPreReqMet(userCity, update);
                requirementsMet = true;
                if (TestingResult != "ok")
                {
                    requirementsMet = false;
                }
                BuildingCost rally = new BuildingCost()
                {
                    typeString = BuildingType.Rally_Spot.ToString().Replace("_", " "),
                    buildingTypeInt = BuildingType.Rally_Spot,
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

            BuildingCost farm = new BuildingCost()
            {
                typeString = BuildingType.Farm.ToString(),
                buildingTypeInt = BuildingType.Farm,
                preReq = "",
                reqMet = true,
                food = Constants.FarmFoodReq,
                stone = Constants.FarmStoneReq,
                wood = Constants.FarmWoodReq,
                iron = Constants.FarmIronReq,
                time = Constants.FarmTimeReq,
                description = "Increases food production.",
                farm = true,
            };
            lbc.Add(farm);

            BuildingCost quarry = new BuildingCost()
            {
                typeString = BuildingType.Quarry.ToString(),
                buildingTypeInt = BuildingType.Sawmill,
                preReq = "",
                reqMet = true,
                food = Constants.QuarryFoodReq,
                stone = Constants.QuarryStoneReq,
                wood = Constants.QuarryWoodReq,
                iron = Constants.QuarryIronReq,
                time = Constants.QuarryTimeReq,
                description = "Increases stone production.",
                farm = true,
            };
            lbc.Add(quarry);

            BuildingCost sawmill = new BuildingCost()
            {
                typeString = BuildingType.Sawmill.ToString(),
                buildingTypeInt = BuildingType.Sawmill,
                preReq = "",
                reqMet = true,
                food = Constants.SawFoodReq,
                stone = Constants.SawStoneReq,
                wood = Constants.SawWoodReq,
                iron = Constants.SawIronReq,
                time = Constants.SawTimeReq,
                description = "Increases wood production.",
                farm = true,
            };
            lbc.Add(sawmill);

            BuildingCost ironMine = new BuildingCost()
            {
                typeString = BuildingType.Iron_Mine.ToString(),
                buildingTypeInt = BuildingType.Iron_Mine,
                preReq = "",
                reqMet = true,
                food = Constants.IronMineFoodReq,
                stone = Constants.IronMineStoneReq,
                wood = Constants.IronMineWoodReq,
                iron = Constants.IronMineIronReq,
                time = Constants.IronMineTimeReq,
                description = "Increases iron production.",
                farm = true,
            };
            lbc.Add(ironMine);
            return lbc;

        }



    }

    public class Troop
    {
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

}
