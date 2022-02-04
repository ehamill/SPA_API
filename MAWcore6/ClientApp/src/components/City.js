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
            activeSlot: 0,
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
            this.setState({ showModal: !this.state.showModal, activeSlot: slot });
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
        let type = e.target.dataset.building;
        let time = e.target.dataset.time;
        this.setState({
            buildWhat: type,
            showModal: false,
            showBuilding1Timer: true,
            build1Time: time
        });
    //console.log("building.." + this.state.buildWhat);
    }

  renderCity() {
      let building0 = this.state.city.buildings.find((x) => x.location === 0);
      let building1 = this.state.city.buildings.find((x) => x.location === 1);
        let lvlb = building0.level;
        
      return (
          <Container>
              <AddBuildingModal
                  newBuildings={this.state.newBuildingsCost}
                  handleClickBuildWhat={this.handleClickBuildWhat} showModal={this.state.showModal} closeModal={this.closeModal} />
              <UpgradeModal
                  handleClickUpgradeBuilding={this.handleClickBuildWhat}
                  showModal={this.state.showUpgradeModal}
                  closeModal={this.closeUpgradeModal} food={this.state.food} wood={this.state.wood}
                  stone={this.state.stone} iron={this.state.iron}
                  activeTroop="Warr"
              />
              {this.state.showBuilding1Timer ? <BuildingTimer time={this.state.build1Time} /> : ''}
              
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



    async getCityData() {
        const token = await authService.getAccessToken();
        const response = await fetch('city', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        console.log('testing..'+ JSON.stringify(data.newBuildingsCost));
        this.setState({ city: data.city, userResearch: data.userResearch, newBuildingsCost: data.newBuildingsCost, loading: false });
    }

}