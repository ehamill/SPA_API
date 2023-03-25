import React, { Component } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
//import logo from './Images/Cottages.jpg' ; Testing for the java
import { WallDefenses } from './WallDefenses';
import { Troops } from './Troops';


export class UpgradeModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            intelNeeded : 0,
            showTroops : true,
            showWallDefenses: true,
            
        };

        this.showTime = this.showTime.bind(this);
        //this.handleRecruitClick = this.handleRecruitClick.bind(this);
       //test 
        //this.upgradeBuildingClick = this.upgradeBuildingClick.bind(this);
    }

   

    showTime(secs) {
        let d = Math.floor(secs / (60 * 60 * 24));
        let h = Math.floor((secs % (60 * 60 * 24)) / (60 * 60));
        let m = Math.floor((secs % (60 * 60)) / 60);
        let s = Math.floor(secs % 60);
        if (d >= 1) {
            return (d + "d " + h + "h");
        } else if (h >= 1) {
            return (h + "h " + m + "m ");
        } else {
            return (m + "m " + s + "s");
        }
    }
    componentDidMount() {
        //const city = this.props.city;
        //const activeBuilding = (activeBuildingId <= 0) ? city.buildings[0] : city.buildings.find((x) => x.buildingId === activeBuildingId);

        //console.log("comp did mount activeBuilding.buildingType: " + activeBuilding.buildingType)
        //console.log("comp did mount upgrademodal troops: " + JSON.stringify(this.props.troops[0]) )
        //if (activeBuilding.buildingType === 21) {
        //    this.setState({
        //        showWallDefenses: true,
        //    });
        //} else if (activeBuilding.buildingType === 2) {
        //    this.setState({
        //        showTroops: true,
        //    });
        //}
       // console.log('upgrade modal mounted ...');
    }

    componentWillUnmount() { }

    GetBuildingType(id) {
        switch (id) {
            case 0:
                return "Empty";
            case 1:
                return "Academy";
            case 2:
                return "Barrack";
            case 3:
                return "Beacon Tower";
            case 4:
                return "Cottage";
            case 5:
                return "Embassy";
            case 6:
                return "Feasting Hall";
            case 7:
                return "Forge";
            case 8:
                return "Inn";
            case 9:
                return "Marketplace";
            case 10:
                return "Rally Spot";
            case 11:
                return "Relief Station";
            case 12:
                return "Stable";
            case 13:
                return "Town Hall";
            case 14:
                return "Warehouse";
            case 15:
                return "Workshop";
            case 16:
                return "Farm";
            case 17:
                return "Iron Mine";
            case 18:
                return "Sawmill";
            case 19:
                return "Iron Mine";
            case 20:
                return "Quarry";
            case 21:
                return "Walls";
            default:
                return "Not Found";
        }
    }

    GetBuildingDescription(id) {
        switch (id) {
            case 0:
                return "Empty - build something here.";
            case 1:
                return "Academy - do your reasearch.";
            case 2:
                return "Barrack - Train new troops.";
            case 3:
                return "Beacon Tower - warns you when enemy approaching.";
            case 4:
                return "Cottage - increase your population.";
            case 5:
                return "Embassy - house ally troops.";
            case 6:
                return "Feasting Hall - Store your heros.";
            case 7:
                return "Forge - Create new weapons";
            case 8:
                return "Inn - Hire new heros.";
            case 9:
                return "Marketplace - trade with others.";
            case 10:
                return "Rally Spot - Send troops.";
            case 11:
                return "Relief Station - Increase sharing speed.";
            case 12:
                return "Stable - train horses and ballistae.";
            case 13:
                return "Town Hall - Manage your city.";
            case 14:
                return "Warehouse - keep resources safe.";
            case 15:
                return "Workshop - odod work";
            case 16:
                return "Farm - make food.";
            case 17:
                return "Iron Mine - make iron.";
            case 18:
                return "Sawmill - make wood.";
            case 19:
                return "Iron Mine - make iron.";
            case 20:
                return "Quarry - make stone";
            case 21:
                return "Walls - keep safe";
            default:
                return "Not Found";
        }
    }
    render() {
        const city = this.props.city;
        const activeBuildingId = this.props.activeBuildingId;
        const activeBuilding = (activeBuildingId <= 0) ? city.buildings[0] : city.buildings.find((x) => x.buildingId === activeBuildingId);
        const buildingLevel = activeBuilding.level;
        const upgradeLevel = activeBuilding.level + 1;
        const demoLevel = activeBuilding.level - 1;
        const buildingTypeString = this.GetBuildingType(activeBuilding.buildingType);
        let buildingDescription = this.GetBuildingDescription(activeBuilding.buildingType);
        const buildingTypeInt = activeBuilding.buildingType;
        const buildingImage = "Images/" + buildingTypeString + "Lvl" + buildingLevel +".jpg";
        
        //console.log('upgrade modal building: ' + JSON.stringify(activeBuilding));

        return (

            <Modal
                isOpen={this.props.showUpgradeModal}
                toggle={this.props.toggleUpdateModal}
                size="lg"
            >
                <ModalHeader toggle={this.props.toggleUpdateModal}>
                    {buildingTypeString} Level {buildingLevel}
                </ModalHeader>
                <ModalBody>
                    
                    <div className="row">
                        <div className="col-md-2">
                            
                            <img src={buildingImage}  width="55px"/>
                        </div>
                        <div className="col-md-8">
                            <p>
                                {buildingImage}
                            </p>
                            <p>
                                {buildingDescription}
                            </p>
                            <div>
                                Upgrading {activeBuilding.image} at Location {activeBuilding.location} || builidingID {activeBuildingId} ||
                                BuildingTypeString: {buildingTypeString} || buildingTypeInt: {buildingTypeInt} ||
                                image: {activeBuilding.image} || Level: {buildingLevel}
                            </div>
                            
                        </div>
                        <div className="col-md-2">
                            <Button className="float-right mr-2 mb-2" onClick={() => this.props.upgradeBuilding(activeBuildingId, buildingTypeInt, upgradeLevel)} >
                                Upgrade
                            </Button>
                            <Button className="float-right mr-2" onClick={() => this.props.upgradeBuilding(activeBuildingId, buildingTypeInt, demoLevel)} >
                                Demo
                            </Button>
                        </div>
                    </div>
                    <div>
                        {/*{activeBuilding.buildingType === 21 && <WallDefenses city={this.props.city} />}*/}
                        {activeBuilding.buildingType === 21 && <Troops trainTroops={this.props.trainTroops} troops={this.props.wallDefenses} troopQueues={this.props.troopQueues} city={this.props.city} activeBuildingId={activeBuildingId} />}
                        {activeBuilding.buildingType === 2 && <Troops trainTroops={ this.props.trainTroops } troops={this.props.troops} troopQueues={this.props.troopQueues} city={this.props.city} activeBuildingId={activeBuildingId} />}
                        
                    </div>
                    <div className="row" hidden>
                        <div className="col-md-6">
                            On left show a list of troops in training, or queue of walls being built, or research.
                            On Right Show all troops, wall defenses, research..use cards. each one has a recruit button.
                            On click shows archer pic with his stats..range population used
                            speed, defence, etc.
                        </div>
                        <div className="col-md-6" hidden></div>
                        On left show a list of troops in training.
                        On Right Show all troops..use cards: type: qty, each one has a recruit button.
                        On troop click: open another modal: Type pic ..troop name.
                        table with his stats..range population used
                        speed, defence, life,etc.
                        below that table, another table: th Res, required, youHave
                        Below that table: You Own #, max#, Train input..max and min buttons,
                        Below that checkbox - split to all barrs
                        below that checkbox - share only w/ idle barrs
                    </div>

                    <div hidden>
                        On barracks click have a make troops button.
                        Have a modal pop up. On left show a list of troops in training. On Right Show all troops..use cards. each one has a recruit button.
                        On click shows archer pic with his stats..range population used
                        speed, defence, etc.

                        You own: 22 arch        input number  button Train below this Duration
                        Also limited by Populatioin
                    </div>
                    

                </ModalBody>
            </Modal>
        );
    }


}