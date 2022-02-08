import React, { Component,Fragment } from 'react';
import authService from './api-authorization/AuthorizeService';
import { Container, Button, Table } from 'reactstrap';
import { BottomNav } from './BottomNav';
import { Building } from './Building';
import { BuildingTimer } from './BuildingTimer';
import { AddBuildingModal } from './AddBuildingModal';
import { UpgradeModal } from './UpgradeModal';


export class City extends Component {

    constructor(props) {
        super(props);
        this.state = {
            city: {},
            userResearch: {},
            newBuildingsCost: {},
            loading: true,
            showModal: false,
            activeSlot: -1,
            activeBuildingId : -1,
            buildWhat: "",
            showBuilding1Timer: false,
            build1Time: 0,
        //    newBuildings: [{ type: "cottage", reqMet: false, preReq: "TH level 2", food: 100, stone: 100, wood: 500, iron: 50, time: 75 },
        //{ type: "inn", reqMet: false, preReq: "lvl 2 Cott", food: 300, stone: 1000, wood: 2000, iron: 400, time: 240 },
        //{ type: "barrack", reqMet: false, preReq: "Rally Spot Level 1", food: 250, stone: 1500, wood: 1200, iron: 500, time: 300 }],
        };

        this.openModal = this.openModal.bind(this);
        this.closeModal = this.closeModal.bind(this);
        this.closeUpgradeModal = this.closeUpgradeModal.bind(this);
        this.handleClickBuildWhat = this.handleClickBuildWhat.bind(this);
        this.testClick = this.testClick.bind(this);
        this.renderCity = this.renderCity.bind(this);
    }

    componentDidMount() {
        this.getCityData();
    }

    testClick() {
        console.log('at testClick');
    }

    openModal(slot) {
        let b = this.state.city.buildings.find((x) => x.location === slot);
        ///console.log('clicked on b level...: ' + b.level);
        if (b.level === 0) {
            this.setState({ showModal: !this.state.showModal, activeSlot: slot, activeBuildingId: b.buildingId });
        } else {
            this.setState({ showUpgradeModal: !this.state.showUpgradeModal, activeSlot: slot });
        }
    }

    closeModal() {
        this.setState({ showModal: false });
    }
    closeUpgradeModal() {
        this.setState({ showUpgradeModal: false });
    }
    handleClickBuildWhat(e) {
        let type = e.target.dataset.building_type;
        let foodCost = e.target.dataset.food_cost;
        let time = e.target.dataset.time;
        let level = e.target.dataset.level;
        let buildingId = e.target.dataset.building_id;
        let slot = e.target.dataset.active_slot;
        //console.log('handleClickBuildWhat  ... foodCost: ' + foodCost + ' buildingId: ' + buildingId + '  buildingtype: '+ type);
        //let b = this.state.buildings.find((x) => x.location === this.state.activeSlot);
        var buildings = [...this.state.city.buildings];
        var b = buildings.find((x) => x.buildingId == buildingId);
        //console.log('handleClickBuildWhat b: ' + JSON.stringify(b))
        b.buildingType = type;
        b.level = level;
        //this.state.city.buildings.find((x) => x.id === buildingId).type = type;
        //this.state.city.buildings.find((x) => x.id === buildingId).level = level;
        this.setState({
            buildWhat: type,
            showModal: false,
            showBuilding1Timer: true,
            build1Time: time,
            buildings: buildings,
        });
        //this.updateCityData();
     console.log("building.." + JSON.stringify(this.state.city.buildings.find((x) => x.id === buildingId)));
    }

  renderCity() {
      let building0 = this.state.city.buildings.find((x) => x.location === 0);
      let building1 = this.state.city.buildings.find((x) => x.location === 1);
        let lvlb = building0.level;
        
      return (
          <Container>
              <AddBuildingModal
                  newBuildings={this.state.newBuildingsCost}
                  activeSlot={this.state.activeSlot}
                  activeBuildingId={this.state.activeBuildingId}
                  handleClickBuildWhat={this.handleClickBuildWhat}
                  food={this.state.food}
                  wood={this.state.wood}
                  stone={this.state.stone}
                  iron={this.state.iron}
                  showModal={this.state.showModal} closeModal={this.closeModal} />
              <UpgradeModal
                  handleClickUpgradeBuilding={this.handleClickBuildWhat}
                  showModal={this.state.showUpgradeModal}
                  closeModal={this.closeUpgradeModal} food={this.state.food} wood={this.state.wood}
                  stone={this.state.stone} iron={this.state.iron}
                  activeTroop="Warr"
              />
              {this.state.showBuilding1Timer ? <BuildingTimer time={this.state.build1Time} isHidden={this.state.showBuilding1Timer } /> : ''}
              
              <div style={{ marginTop: "20px" }} onClick={this.testClick}>
                  Build Where: {this.state.activeSlot}
                  Build What: {this.state.buildWhat}
              </div>

              <div>
                  <Table bordered={true}>
                      <tbody>
                          <tr>
                              <td>
                                  <Building onClick={() => this.openModal(building0.location)} b={building0}>empty </Building>
                              </td>
                              <td>
                                  <Building onClick={() => this.openModal(building1.location)} b={building1}>empty </Building>
                              </td>
                              <td colSpan="2" rowSpan="2">
                                  <Button color="primary" onClick={this.testClick}>town hall</Button>
                              </td>
                              <td>
                                  <Button color="success">open</Button>
                              </td>
                              <td>
                                  <Button color="success">open</Button>
                              </td>
                          </tr>
                      </tbody>
                  </Table>

                  <div>
                      food:{this.state.food}
                      wood:{this.state.wood}
                      stone:{this.state.stone}
                      iron:{this.state.iron}
                  </div>
                  
              </div>
              {/*<BottomNav />*/}
          </Container>

            
        );
    }

    render() {
        let showLoading = this.state.loading ? <p><em>Loading...</em></p> : this.renderCity(); 

        return (
            <div>
                {showLoading}
            </div>
        );
    }

    async updateCityData() {

       // console.log('at update city data..');
        var test = { id2: 55, fuck: 'fuck yoo' }; 
        const token = await authService.getAccessToken();
        const response = await fetch('city/Fucker', {
            method: 'POST',
            headers: !token ? { 'Content-Type': 'application/json' } : { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
            body: JSON.stringify(test), 
        });
        const data = await response.json();
        //console.log('at updateCityData..'+ JSON.stringify(data.newBuildingsCost));
        this.setState({ city: data.city, userResearch: data.userResearch, newBuildingsCost: data.newBuildingsCost, loading: false });
    }

    async getCityData() {
        const token = await authService.getAccessToken();
        const response = await fetch('city', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        console.log('at getCityData..'+ JSON.stringify(data.newBuildingsCost));
        this.setState({ city: data.city, userResearch: data.userResearch, newBuildingsCost: data.newBuildingsCost, loading: false });
    }

}