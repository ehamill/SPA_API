import React, { Component } from 'react';
import {Table, Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

//use this for intellisense sdfjlja s dfl jlsajfdl

export class RecruitModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            intelNeeded : 0,
            foodNeeded : 0,
            stoneNeeded: 0,
            woodNeeded: 0,
            ironNeeded: 0,
            stoneNeeded: 0,
            timeNeeded: 0,
            trainQty: "",
            duration: 0,
        };

        this.getMaximumTroops = this.getMaximumTroops.bind(this);
        this.handleTroopQtyChange = this.handleTroopQtyChange.bind(this);
        this.showTime = this.showTime.bind(this);
        this.toggleRecruitModal = this.toggleRecruitModal.bind(this);
        this.closeModal = this.closeModal.bind(this);
    }

    handleTroopQtyChange(e) {
        //changed from troopTypeInt to typeInt
        //console.log('at handleTroopQtyChange. recruit modal this.props.troopTypeInt22: ' + this.props.typeInt,' this.props.troops: ' , this.props.troops, )
        const troop = (this.props.typeInt === 0) ? this.props.city.troops[0] : this.props.city.troops.find((x) => x.typeInt === this.props.typeInt);
        const qty = parseFloat(e.target.value);
        console.log('qty', qty);
        if (!Number.isNaN(qty)) {
            this.setState({
                trainQty: qty,
                foodNeeded: qty * troop.foodCost, 
                stoneNeeded: qty * troop.stoneCost,
                woodNeeded: qty * troop.woodCost,
                ironNeeded: qty * troop.ironCost,
                timeNeeded: this.showTime(qty * troop.timeCost),
            });
        } else {
            this.setState({
                trainQty: "",
                foodNeeded: 0,
                woodNeeded: 0,
                ironNeeded: 0,
                timeNeeded: 0,
            });
        }
    }

    getMaximumTroops() {
        const troop = (this.props.typeInt === 0) ? this.props.city.troops[0] : this.props.city.troops.find((x) => x.typeInt === this.props.typeInt);
        const food = this.props.city.food;
        const stone = this.props.city.stone;
        const wood = this.props.city.wood;
        const iron = this.props.city.iron;
        let max = Math.floor(food / troop.foodCost);
        max = max < Math.floor(stone / troop.stoneCost) ? max : Math.floor(stone / troop.stoneCost);
        max = max < Math.floor(wood / troop.woodCost) ? max : Math.floor(wood / troop.woodCost);
        max = max < Math.floor(iron / troop.ironCost) ? max : Math.floor(iron / troop.ironCost);
        this.setState({
            trainQty: max,
            foodNeeded: max * troop.foodCost,
            stoneNeeded: max * troop.stoneCost,
            woodNeeded: max * troop.woodCost,
            ironNeeded: max * troop.ironCost,
            timeNeeded: this.showTime(max * troop.timeCost),
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

    
    resetAndCallTrainTroops(troopTypeInt) {
        //console.log('recruit modal mounted ...wood:' + this.props.wood);
        this.setState({
            trainQty: "",
            foodNeeded: 0,
            woodNeeded: 0,
            ironNeeded: 0,
            timeNeeded: 0,
        });
        this.props.trainTroops(troopTypeInt, this.state.trainQty)
    }
    componentDidMount() {
    }

    componentWillUnmount() {
       // console.log('recruit modal..componentWillUnmount...');
    }

    toggleRecruitModal() {
        this.setState({
            trainQty: "",
            foodNeeded: 0,
            woodNeeded: 0,
            ironNeeded: 0,
            timeNeeded: 0,
        });
        this.props.toggleRecruitModal();
    }
    closeModal() {
        this.setState({
            trainQty: "",
            foodNeeded: 0,
            woodNeeded: 0,
            ironNeeded: 0,
            timeNeeded: 0,
        });
        this.props.closeModal();
    }
    
    render() {
       console.log('this.props.troopTypeInt: '+ this.props.typeInt)
        const troop = (this.props.typeInt === 0) ? this.props.city.troops[0] : this.props.city.troops.find((x) => x.typeInt === this.props.typeInt);
        console.log('at recruit ...troop is: ' + JSON.stringify(troop))
        //const activeBuildingId = this.props.activeBuildingId;
        //const activeBuilding = (activeBuildingId <= 0) ? city.buildings[0] : city.buildings.find((x) => x.buildingId === activeBuildingId);
        const food = this.props.city.food;
        const stone = this.props.city.stone;
        const wood = this.props.city.wood;
        const iron = this.props.city.iron;
        const foodDanger = this.state.foodNeeded > food && "text-danger";
        const stoneDanger = this.state.stoneNeeded > stone && "text-danger";
        const woodDanger = this.state.woodNeeded > wood && "text-danger";
        const ironDanger = this.state.ironNeeded > iron && "text-danger";

        return (
            <Modal
                isOpen={this.props.showModal}
                toggle={this.toggleRecruitModal}
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
                        <Table size="sm">
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
                        <Table size="sm">
                            <thead>
                                <tr>
                                    <th> Required</th>
                                    <th> Needed</th>
                                    <th> You Own </th>
                                </tr>
                            </thead>
                            <tbody>
                                <tr>
                                    <th>Food </th>
                                    <th>{this.state.foodNeeded}</th>
                                    <th className={foodDanger}>{this.props.city.food} </th>
                                </tr>
                                {this.state.stoneNeeded > 0 &&
                                    <tr>
                                        <th>Stone </th>
                                        <th>{this.state.stoneNeeded}</th>
                                    <th className={stoneDanger}>{this.props.city.stone} </th>
                                    </tr>
                                }
                                
                                <tr>
                                    <th>Wood </th>
                                    <th>{this.state.woodNeeded}</th>
                                    <th className={woodDanger}>{this.props.city.wood} </th>
                                </tr>
                                <tr>
                                    <th>Iron </th>
                                    <th>{this.state.ironNeeded}</th>
                                    <th className={ironDanger}>{this.props.city.iron} </th>
                                </tr>
                                <tr>
                                    <td colSpan="2">
                                        You have {troop.qty} {troop.typeString}s.
                                    </td>
                                    <td colSpan="1" className="text-right">
                                        Train:
                                        <input type="text" id="troopQty" className="width-50 text-right"  value={this.state.trainQty} onChange={this.handleTroopQtyChange} />
                                        <button onClick={this.getMaximumTroops} >max </button>
                                        <button hidden >min </button>
                                    </td>
                                </tr>
                                <tr>
                                    <td colSpan="3" className="text-right">
                                        <div>Duration {this.state.timeNeeded}</div>
                                    </td>
                                </tr>
                                <tr>
                                    <td colSpan="3" className="text-right">
                                        <Button  onClick={this.closeModal} color="secondary" >
                                            Cancel
                                        </Button>
                                        <Button onClick={() => this.resetAndCallTrainTroops(this.props.typeInt) } color="primary" >
                                            Train
                                        </Button>
                                    </td>
                                </tr>
                            </tbody>
                        </Table>
                    </div>

                </ModalBody>
            </Modal>



            


                
            
                
                
                
                
        );
    }


}