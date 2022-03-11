import React, { Component } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
//import logo from './Images/Cottage.jpg' ;
import { WallDefenses } from './WallDefenses';
import { Troops } from './Troops';
import { RecruitModal } from './RecruitModal';

export class UpgradeModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            showTroops : true,
            showRecruitModal: false,
            showWallDefenses: true,
            recruitTroopType : 0,
        };

        this.getMaximumTroops = this.getMaximumTroops.bind(this);
        this.handleTroopQtyChange = this.handleTroopQtyChange.bind(this);
        this.showTime = this.showTime.bind(this);
        //this.handleRecruitClick = this.handleRecruitClick.bind(this);
        this.WarrFoodCost = 80;
        this.WarrWoodCost = 100;
        this.WarrIronCost = 50;
        this.WarrTimeCost = 25;
        this.WarrPopCost = 1;
        //this.upgradeBuildingClick = this.upgradeBuildingClick.bind(this);
        this.showRecruitModal = this.showRecruitModal.bind(this);
        this.hideRecruitModal = this.hideRecruitModal.bind(this);
    }

   
    hideRecruitModal() {
        this.setState({
            showRecruitModal: false,
        });
    }

    showRecruitModal(troopTypeInt) {
        const troop = this.props.troops.find((x) => x.typeInt === troopTypeInt);
        this.setState({
            showRecruitModal: true,
            recruitTroopType: troopTypeInt,
        });
        console.log('at handleRecruitClick ...troopTypeInt: ' + troopTypeInt);
        //TrainTroops();
    }
    toggleRecruitModal = () => {
        this.setState(prevState => ({
            showRecruitModal: !prevState.showRecruitModal
        }));
    };


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
        //const activeBuildingId = this.props.activeBuildingId;
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
                return "Beacon_Tower";
            case 4:
                return "Cottage";
            case 5:
                return "Embassy";
            case 6:
                return "Feasting_Hall";
            case 7:
                return "Forge";
            case 8:
                return "Inn";
            case 9:
                return "Marketplace";
            case 21:
                return "Walls";

            default:
                return "Error";
        }
        
            //Rally_Spot = 10,
            //Relief_Station = 11,
            //Stable = 12,
            //Town_Hall = 13,
            //Warehouse = 14,
            //Workshop = 15,
            //Farm = 16,
            //Iron_Mill = 17,
            //Sawmill = 18,
            //Iron_Mine = 19,
            //Quarry = 20,
            //Not_Found = 21,
        
    }
    
    render() {
        const city = this.props.city;
        const activeBuildingId = this.props.activeBuildingId;
        const activeBuilding = (activeBuildingId <= 0) ? city.buildings[0] : city.buildings.find((x) => x.buildingId === activeBuildingId);
        const buildingLevel = activeBuilding.level;
        const upgradeLevel = activeBuilding.level + 1;
        const demoLevel = activeBuilding.level - 1;
        const buildingType = this.GetBuildingType(activeBuilding.buildingType);
        const buildingImage = "Images/" + buildingType + ".jpg";
        //if (activeBuilding.buildingType === 21) {
        //    this.setState({
        //        showWallDefenses: true,
        //    });
        //} else if (activeBuilding.buildingType === 2) {
        //    this.setState({
        //        showTroops: true,
        //    });
        //}
        //const nextBuildingType = (demoLevel == 0) ? "empty" : buildingType;
        
        //console.log('active: ' + this.props.activeBuildingId+'testing ...' + JSON.stringify(this.props.troops));

        return (
            


            <Modal isOpen={this.props.showUpgradeModal}
                toggle={this.props.toggleUpdateModal}
                size="lg"
            >
                <ModalHeader >{buildingType} Level {buildingLevel }</ModalHeader>
                <ModalBody>
                    {/*<Fragment>*/}

                    {/*</Fragment>*/}
                    <RecruitModal
                        food={this.props.city.food}
                        wood={this.props.city.wood}
                        stone={this.props.city.stone}
                        iron={this.props.city.iron}
                        showModal={this.state.showRecruitModal} closeModal={this.hideRecruitModal}
                        toggleRecruitModal={this.toggleRecruitModal}
                        troopType={this.state.recruitTroopType}
                        troops={this.props.troops} />

                    <div className="row">
                        <div className="col-md-2">
                            <img src={buildingImage}  width="55px"/>
                        </div>
                        <div className="col-md-8">
                            <p>
                                {activeBuilding.description}
                            </p>
                            <div>
                                Upgrading {activeBuilding.image} at {activeBuilding.location} || builidingID {activeBuildingId} ||
                                Building Type: {buildingType} || activeBuilding.buildingType: {activeBuilding.buildingType} ||
                                image: {activeBuilding.image} || Level: {buildingLevel}
                            </div>
                            
                        </div>
                        <div className="col-md-2">
                            <Button className="float-right mr-2 mb-2" onClick={() => this.props.upgradeBuilding(activeBuildingId, buildingType, upgradeLevel)} >
                                Upgrade
                            </Button>
                            <Button className="float-right mr-2" onClick={() => this.props.upgradeBuilding(activeBuildingId, buildingType, demoLevel)} >
                                Demo
                            </Button>
                        </div>
                    </div>
                    <div>
                        {activeBuilding.buildingType === 21 && <WallDefenses city={this.props.city} />}
                        {activeBuilding.buildingType === 2  && <Troops handleRecruitClick={this.showRecruitModal } troops={this.props.troops} city={this.props.city}  />}
                        
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
                <ModalFooter>
                    <Button color="primary" data-building="feast" onClick={this.props.handleClickBuildWhat}>
                        Make Warrs
                    </Button>
                    <Button color="secondary" onClick={this.props.closeModal}>
                        Cancel
                    </Button>
                </ModalFooter>
            </Modal>
        );
    }


}