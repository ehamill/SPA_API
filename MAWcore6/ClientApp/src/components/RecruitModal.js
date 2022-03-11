﻿import React, { Component } from 'react';
import {Table, Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

export class RecruitModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            foodNeeded : 0,
            stoneNeeded: 0,
            woodNeed: 0,
            ironNeeded: 0,
            stoneNeeded: 0,
            trainQty: 0,
            duration: 0,
        };

        this.getMaximumTroops = this.getMaximumTroops.bind(this);
        this.handleTroopQtyChange = this.handleTroopQtyChange.bind(this);
        this.showTime = this.showTime.bind(this);
    }

    handleTroopQtyChange(e) {
        const qty = parseFloat(e.target.value);
        if (!Number.isNaN(qty)) {
            this.setState({
                troopQty: qty,
                foodNeeded: qty * this.WarrFoodCost,
                woodNeeded: qty * this.WarrWoodCost,
                ironNeeded: qty * this.WarrIronCost,
            });
        } else {
            this.setState({
                troopQty: "",
                foodNeeded: 0,
                woodNeeded: 0,
                ironNeeded: 0,
            });
        }
    }

    getMaximumTroops() {
        const food = this.props.food;
        const stone = this.props.stone;
        const wood = this.props.wood;
        const iron = this.props.iron;
        //const population = this.props.population;
        let max = Math.floor(food / this.WarrFoodCost);
        max = max < Math.floor(wood / this.WarrWoodCost) ? max : Math.floor(wood / this.WarrWoodCost);
        max = max < Math.floor(iron / this.WarrIronCost) ? max : Math.floor(iron / this.WarrIronCost);
        //max = max > Math.Floor(population/WarrPopCost) ? max : Math.Floor(wood/WarrPopCost);
        this.setState({
            troopQty: max,
            foodNeeded: max * this.WarrFoodCost,
            woodNeeded: max * this.WarrWoodCost,
            ironNeeded: max * this.WarrIronCost,
            duration: this.showTime(max * this.WarrTimeCost),
        });
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
        const troop = (this.props.troopType === 0) ? this.props.troops[0] : this.props.troops.find((x) => x.typeInt === this.props.troopType);
        //const city = this.props.city;
        //const activeBuildingId = this.props.activeBuildingId;
        //const activeBuilding = (activeBuildingId <= 0) ? city.buildings[0] : city.buildings.find((x) => x.buildingId === activeBuildingId);
        //const buildingLevel = activeBuilding.level;
        //const upgradeLevel = activeBuilding.level + 1;
        //const demoLevel = activeBuilding.level - 1;
        //const buildingType = this.GetBuildingType(activeBuilding.buildingType);
        //const buildingImage = "Images/" + buildingType + ".jpg";
        

        return (
            <Modal
                isOpen={this.props.showModal}
                toggle={this.props.toggleRecruitModal}
            >
                <ModalHeader className="text-right">
                    Recruit {troop.typeString} 
                </ModalHeader>
                <ModalBody>
                    <div>
                        Image...
                        {troop.description}
                    </div>
                    <div>
                        {troop.preReq}
                    </div>
                    <div>
                        <Table>
                            <tbody>
                                <tr>
                                    <th>Attack {troop.attack} </th>
                                    <th>Defense {troop.defense} </th>
                                    <th>Range {troop.range} </th>
                                </tr>
                                <tr>
                                    <th>Speed {troop.speed} </th>
                                    <th>Load {troop.load}</th>
                                    <th>Life {troop.life}</th>
                                </tr>
                                <tr>
                                    <th>Food 5</th>
                                    <th>Population 6</th>
                                    <th> </th>
                                </tr>
                            </tbody>
                        </Table>
                        <Table>
                            <thead>
                                <tr>
                                    <th> Required</th>
                                    <th> Needed</th>
                                    <th> You Own </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <td>Food </td>
                                    <th>{this.state.foodNeeded}</th>
                                    <th>{this.props.food} </th>
                                </tr>
                                <tr>
                                    <th>Wood </th>
                                    <th>{this.state.stoneNeeded}</th>
                                    <th>{this.props.stone} </th>
                                </tr>
                                <tr>
                                    <th>Iron </th>
                                    <th>{this.state.woodNeeded}</th>
                                    <th>{this.props.wood} </th>
                                </tr>
                                <tr>
                                    <th>Iron </th>
                                    <th>{this.state.ironNeeded}</th>
                                    <th>{this.props.iron} </th>
                                </tr>
                                <tr>
                                    <td colSpan="3">
                                        You have 100 warrs this.props.activeTroop
                                        <input type="text" id="troopQty" value={this.state.trainQty} onChange={this.handleTroopQtyChange} />
                                        <button onClick={this.getMaximumTroops} >max </button>
                                        <button hidden >min </button>
                                    </td>
                                </tr>
                                <tr>
                                    <td colSpan="3" className="float-right">
                                        Duration {this.state.duration}
                                    </td>
                                </tr>
                                <tr>
                                    <td colSpan="3">
                                        <Button className="float-right" onClick={this.props.trainTroops} color="primary" >
                                            Train
                                        </Button>
                                        <Button className="float-right" onClick={this.props.closeModal} color="secondary" >
                                            Cancel
                                        </Button>
                                    </td>
                                </tr>
                            </tbody>
                        </Table>
                    </div>

                </ModalBody>
                <ModalFooter>
                    <Button className="">
                        Train
                    </Button>
                    <Button color="secondary" onClick={this.props.closeRecruitModal}>
                    Cancel
                    </Button>
                </ModalFooter>
            </Modal>



            


                
            
                
                
                
                
        );
    }


}