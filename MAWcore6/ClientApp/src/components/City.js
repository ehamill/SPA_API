import React, { Component,Fragment } from 'react';
import authService from './api-authorization/AuthorizeService';
import { Container, Button, Table } from 'reactstrap';
//import { BottomNav } from './BottomNav';
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
            //userItems: {},
            newBuildingsCost: {},
            loading: true,
            showModal: false,
            activeSlot: -1,
            activeBuildingId : -1,
            buildWhat: "",
            showBuilding1Timer: false,
            build1Time: 0,
            buildLevel: 0,
        };

        this.openModal = this.openModal.bind(this);
        this.closeModal = this.closeModal.bind(this);
        this.closeUpgradeModal = this.closeUpgradeModal.bind(this);
        this.handleClickBuildWhat = this.handleClickBuildWhat.bind(this);
        this.testClick = this.testClick.bind(this);
        this.renderCity = this.renderCity.bind(this);
        this.buildingDone = this.buildingDone.bind(this);
        this.speedUpClick = this.speedUpClick.bind(this);
    }

    componentDidMount() {
        this.getCityData();
        //if (this.state.activeBuildingId === -1) {
        //    this.setState({
        //        activeBuildingId: this.state.city.buildings[0].buildingId,
        //    });
        //}
    }

    testClick() {
        console.log('at testClick');
    }

    openModal(slot) {
        let b = this.state.city.buildings.find((x) => x.location === slot);
        console.log('clicked on b level...: ' + b.level);
        this.setState({
            activeSlot: slot,
            activeBuildingId: b.buildingId,
        });
        if (b.level === 0) {
            this.setState({ showModal: !this.state.showModal });
        } else {
            this.setState({ showUpgradeModal: !this.state.showUpgradeModal});
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
        //let foodCost = e.target.dataset.food_cost;
        //let time = e.target.dataset.time;
        let level = e.target.dataset.level;
        let buildingId = e.target.dataset.building_id;
        //let slot = e.target.dataset.active_slot;
        //console.log('handleClickBuildWhat  ... buildingid:  ' + buildingId + '  buildingtype: '+ type);
        //var newCity = this.state.city;
        //let b = newCity.buildings.find((x) => x.buildingId == buildingId);
        //console.log("b: " + JSON.stringify(b));
        //b.image = "Upgrading" + type + ".jpg";
        this.setState({
            buildWhat: type, //gets passed to timer
            buildLevel: level, //gets passed to timer
            showModal: false,
            //showBuilding1Timer: true,
            //build1Time: time,
            //city: newCity,
        });
        this.updateCityData(buildingId, type, level);
    }

    buildingDone(location, type, level) {
        console.log("building done at loc: " + location + " type:" + type + " lvL: " + level);
        var newCity = this.state.city;
        var b = newCity.buildings.find((x) => x.location == location);
        b.buildingType = type;
        b.level = level;
        b.image = type +"lvl" + level + ".jpg";
        this.setState({
            //city.buildings: buildings,
            city: newCity,
        });
    }
    speedUpClick( ) {
        this.speedUpUsed();
    }

    renderCity() {
        
      let building0 = this.state.city.buildings.find((x) => x.location === 0);
      let building1 = this.state.city.buildings.find((x) => x.location === 1);
     
        
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
              
              {this.state.activeBuildingId != -1 ? <UpgradeModal
                  handleClickUpgradeBuilding={this.handleClickBuildWhat}
                  showUpgradeModal={this.state.showUpgradeModal}
                  closeModal={this.closeUpgradeModal}
                  city={this.state.city}
                  activeBuildingId={this.state.activeBuildingId}
                  activeTroop="Warr"
              /> : ''}
              {/*{this.state.showBuilding1Timer ? <BuildingTimer time={this.state.build1Time} isHidden={this.state.showBuilding1Timer} /> : ''}*/}
              {this.state.city.builder1Busy ? <BuildingTimer buildingDone={this.buildingDone} speedUpClick={ this.speedUpUsed} buildWhat={this.state.buildWhat} location={this.state.activeSlot } level={this.state.buildLevel} time={this.state.city.builder1Time} isHidden={this.state.city.builder1Busy} /> : ''}
              
              <div style={{ marginTop: "20px" }} onClick={this.testClick}>
                  Build Where: {this.state.activeSlot}
                  Build What: {this.state.buildWhat}
              </div>

              <div>
                  <Table bordered={true}>
                      <tbody>
                          <tr>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building0.location)} b={building0}>empty </Building>
                              </td>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building1.location)} b={building1}>empty </Building>
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
                      food:{this.state.city.food}
                      wood:{this.state.city.wood}
                      stone:{this.state.city.stone}
                      iron:{this.state.city.iron}
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

    async speedUpUsed() {

        console.log('at speedup used ..cityid: ' + this.state.city.cityId);
        var speedUpModel = { cityId: this.state.city.cityId, speedUp5min: true, usedOn : "builder1" };
        //console.log('updateModel: ' + JSON.stringify(updateModel));
        const token = await authService.getAccessToken();
        const response = await fetch('city/SpeedUpUsed', {
            method: 'POST',
            headers: !token ? { 'Content-Type': 'application/json' } : { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
            body: JSON.stringify(speedUpModel),
        });
        const data = await response.json();
        console.log('at speedUpUsed..returned: ' + JSON.stringify(data));
        if (data.message !== 'ok') {
            alert('oops ..' + data.message)
        } else {
            this.setState({ city: data.city, });
        }

    }


    async updateCityData(buildingId, buildingType, level) {

       // console.log('at update city data..');
        var updateModel = { cityId: this.state.city.cityId, buildingId: parseInt(buildingId), buildingType, level: parseInt(level) };
        //console.log('updateModel: ' + JSON.stringify(updateModel));
        const token = await authService.getAccessToken();
        const response = await fetch('city/UpdateCity', {
            method: 'POST',
            headers: !token ? { 'Content-Type': 'application/json' } : { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
            body: JSON.stringify(updateModel),
        });
        const data = await response.json();
        console.log('at updateCityData..returned: ' + JSON.stringify(data));
        if (data.message !== 'ok') {
            alert('oops ..' + data.message)
        } else {
            this.setState({ city: data.city, });
        }
        
        //if (data.city.builder1Busy === true) {
        //    this.setState({
        //        showBuilding1Timer: true,
        //        build1Time: data.city.build1Time,
        //    });
        //}

        //this.setState({ city: data.city, userResearch: data.userResearch, newBuildingsCost: data.newBuildingsCost, loading: false });
    }

    async getCityData() {
        const token = await authService.getAccessToken();
        const response = await fetch('city', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        //console.log('at getCityData..loading city: '+ JSON.stringify(data.city));
        this.setState({ city: data.city, userResearch: data.userResearch, newBuildingsCost: data.newBuildingsCost, loading: false });
    }

}