import React, { Component } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';

export class UpgradeModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            activeBuildingId : this.props.activeBuildingId,
            //city: this.props.city,
        };

        this.getMaximumTroops = this.getMaximumTroops.bind(this);
        this.handleTroopQtyChange = this.handleTroopQtyChange.bind(this);
        this.showTime = this.showTime.bind(this);
        this.WarrFoodCost = 80;
        this.WarrWoodCost = 100;
        this.WarrIronCost = 50;
        this.WarrTimeCost = 25;
        this.WarrPopCost = 1;
    }

    handleTroopQtyChange(e) {
        const qty = parseFloat(e.target.value);
        //if (Number.isNaN(qty)) {
        //this.setState({ 
        //troopQty: 0 
        //});
        //=}
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
        //if (this.props.activeBuildingId == -1) {
        //    console.log('is equal to -1')
        //    this.setState({
        //        //city.buildings: buildings,
        //        activeBuildingId : this.props.city.buildings[0].buildingId,
        //    });
        //}
        //this.setState({
        //    activeBuildingId: 55,
        //});
        //console.log('upgrade modal compdidmount,  activeBuildingId:' + this.state.activeBuildingId);
    }

    componentWillUnmount() { }

    render() {
        //const showModal = this.props.modal;
        //const className = "jlsaldfjl";
        //const toggle = "";
        const city = this.props.city;
        const activeBuilding = city.buildings.find((x) => x.buildingId === this.props.activeBuildingId);
        //console.log('active: ' + this.props.activeBuildingId+'testing ...' + JSON.stringify(city));

        return (
            <Modal isOpen={this.props.showUpgradeModal} >
                <ModalHeader >Upgrading</ModalHeader>
                <ModalBody>
                    <div>
                        Upgrading {activeBuilding.image} at {activeBuilding.location} builidingID {this.props.activeBuildingId }
                    </div>
                    <div className="row" hidden>
                        <div className="col-md-6">
                            On left show a list of troops in training. On Right Show all troops..use cards. each one has a recruit button.
                            On click shows archer pic with his stats..range population used
                            speed, defence, etc.
                        </div>
                        <div className="col-md-6" hidden></div>
                        On left show a list of troops in training. On Right Show all troops..use cards. each one has a recruit button.
                        On click shows archer pic with his stats..range population used
                        speed, defence, etc.
                    </div>

                    <div hidden>
                        On barracks click have a make troops button.
                        Have a modal pop up. On left show a list of troops in training. On Right Show all troops..use cards. each one has a recruit button.
                        On click shows archer pic with his stats..range population used
                        speed, defence, etc.

                        You own: 22 arch        input number  button Train below this Duration
                        Also limited by Populatioin
                    </div>
                    <table hidden>
                        <tr>
                            <th> Required</th>
                            <th> Needed</th>
                            <th> You Own </th>
                        </tr>
                        <tr>
                            <th>Food </th>
                            <th>{this.state.foodNeeded}</th>
                            <th>{this.props.food} </th>
                        </tr>
                        <tr>
                            <th>Wood </th>
                            <th>{this.state.woodNeeded}</th>
                            <th>{this.props.wood} </th>
                        </tr>
                        <tr>
                            <th>Iron </th>
                            <th>{this.state.ironNeeded}</th>
                            <th>{this.props.iron} </th>
                        </tr>
                    </table>
                    <div>
                        You have {this.props.activeTroop}
                        <input type="text" id="troopQty" value={this.state.troopQty} onChange={this.handleTroopQtyChange} />
                        <button onClick={this.getMaximumTroops} >max {this.state.troopQty}</button>
                    </div>
                    <div >
                        Duration {this.state.duration}


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