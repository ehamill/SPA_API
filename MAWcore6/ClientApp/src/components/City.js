import React, { Component,Fragment } from 'react';
import authService from './api-authorization/AuthorizeService';
import { Container, Button, Table, ListGroup, ListGroupItem,Toast,ToastHeader,ToastBody, Modal, ModalHeader, ModalBody, ModalFooter} from 'reactstrap';
//import { Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';
//import { BottomNav } from './BottomNav';
import { Building } from './Building';
import { BuildingTimer } from './BuildingTimer';
import { AddBuildingModal } from './AddBuildingModal';
import { UpgradeModal } from './UpgradeModal';
import { TownHallModal } from './TownHallModal';

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
            showTownHallModal: false,
            showTestModal: false,
            showErrorMessage: false,
            errorMessage: false,
        };

        this.openModal = this.openModal.bind(this);
        this.closeModal = this.closeModal.bind(this);
        this.closeUpgradeModal = this.closeUpgradeModal.bind(this);
        this.closeTownHallModal = this.closeTownHallModal.bind(this);
        this.handleClickBuildWhat = this.handleClickBuildWhat.bind(this);
        this.testClick = this.testClick.bind(this);
        this.renderCity = this.renderCity.bind(this);
        this.buildingDone = this.buildingDone.bind(this);
        this.speedUpClick = this.speedUpClick.bind(this);
        this.upgradeBuilding = this.upgradeBuilding.bind(this);
        this.showTestModalClick = this.showTestModalClick.bind(this);
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
        console.log('clicked on b level...: ' + b.level +' locatoin:'+ slot);
        this.setState({
            activeSlot: slot,
            activeBuildingId: b.buildingId,
        });
        if (b.location === 3) {
            //console.log('  show thmodal..: ');
            this.setState({ showTownHallModal: !this.state.showTownHallModal });
        } else if (b.location === 0) {
            this.setState({ showUpgradeModal: !this.state.showUpgradeModal });
        } else if (b.level === 0) {
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
    toggleUpdateModal = () => {
        this.setState(prevState => ({
            showUpgradeModal: !prevState.showUpgradeModal
        }));
    };
    toggleAddBuildingModal = () => {
        this.setState(prevState => ({
            showModal: !prevState.showModal
        }));
    };
    closeTownHallModal() {
        this.setState({ showTownHallModal: false });
    }
    toggleTownHallModal = () => {
        this.setState(prevState => ({
            showTownHallModal: !prevState.showTownHallModal
        }));
    };
    toggleErrorMessage = () => {
        this.setState(prevState => ({
            showErrorMessage: !prevState.showErrorMessage
        }));
    };

    handleClickBuildWhat(e) {
        let buildingId = e.target.dataset.building_id;
        let type = e.target.dataset.building_type;
        let level = e.target.dataset.level;
        
        this.setState({
            buildWhat: type, //gets passed to timer
            buildLevel: level, //gets passed to timer
            showModal: false,
            //showBuilding1Timer: true,
            //build1Time: time,
            //city: newCity,
        });
        this.updateCityData(buildingId, type, level);
        //sconsole.log("handleClickBuildWhat: cityid= " + this.state.city.cityId);
    }
    showTestModalClick() {
        this.setState({ showTestModal: !this.state.showTestModal });
    }
    
    
    //toggleModal = () => {
    //    this.setState(prevState => ({
    //        showTestModal: !prevState.showTestModal
    //    }));
    //};
        
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
                return "Town Hall";

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

    buildingDone( location, type, level) {
        console.log("building done at loc: " + location + " type:" + type + " lvL: " + level);
        if (level === 0) {
            type = "Empty";
        }
        //Might want to update right away, avoid any server lag.
        //var newCity = this.state.city;
        //var b = newCity.buildings.find((x) => x.location == location);
        //b.buildingType = type;
        //b.level = level;
        //b.image = type +"lvl" + level + ".jpg";
        this.setState({
            //showBuilding1Timer: false,
            //city: newCity,
        });
        this.fetchBuildingDone(location, type, level);
    }
    speedUpClick() {
        this.speedUpUsed();
    }

    upgradeBuilding(buildingId, type ,level) {
        console.log('at upgradeBuilding buildingid: ' + buildingId +' type: '+ type+ " level: "+ level);
        this.closeUpgradeModal();
        this.updateCityData(buildingId, type, level);
    }

    renderCity() {
         
        let building0 = this.state.city.buildings.find((x) => x.location === 0);
        let building1 = this.state.city.buildings.find((x) => x.location === 1);
        let building2 = this.state.city.buildings.find((x) => x.location === 2);
        let building3 = this.state.city.buildings.find((x) => x.location === 3);
        let building4 = this.state.city.buildings.find((x) => x.location === 4);
        let building5 = this.state.city.buildings.find((x) => x.location === 5);
        

      return (
          <Container>
              <Toast isOpen={this.state.showErrorMessage}>
                  <ToastHeader toggle={this.toggleErrorMessage}>
                     error { this.state.errorMessage}
                  </ToastHeader>
                  <ToastBody>
                      Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
                  </ToastBody>
              </Toast>
              <TownHallModal
                  activeBuildingId={this.state.activeBuildingId}
                  city={this.state.city}
                  showModal={this.state.showTownHallModal}
                  closeModal={this.closeTownHallModal}
                  toggleTownHallModal={this.toggleTownHallModal}
              />
              <AddBuildingModal
                  newBuildings={this.state.newBuildingsCost}
                  activeSlot={this.state.activeSlot}
                  activeBuildingId={this.state.activeBuildingId}
                  handleClickBuildWhat={this.handleClickBuildWhat}
                  food={this.state.city.food}
                  wood={this.state.city.wood}
                  stone={this.state.city.stone}
                  iron={this.state.city.iron}
                  showModal={this.state.showModal} closeModal={this.closeModal}
                  toggleAddBuildingModal={this.toggleAddBuildingModal}
              />

               <UpgradeModal
                  handleClickUpgradeBuilding={this.handleClickBuildWhat}
                  upgradeBuilding={this.upgradeBuilding}
                  demoBuilding={this.upgradeBuilding}
                  showUpgradeModal={this.state.showUpgradeModal}
                  closeModal={this.closeUpgradeModal}
                  city={this.state.city}
                  activeBuildingId={this.state.activeBuildingId}
                  activeTroop="Warr"
                  toggleUpdateModal={ this.toggleUpdateModal}
              /> 

              {/*<BuildingTimer buildingDone={this.buildingDone} speedUpClick={this.speedUpClick}  buildWhat={this.state.buildWhat} location={this.state.activeSlot} level={this.state.buildLevel} time={this.state.city.builder1Time} builder1Busy={this.state.city.builder1Busy} /> */}
              {this.state.city.builder1Busy ? <BuildingTimer buildingDone={this.buildingDone} speedUpClick={this.speedUpClick} buildWhat={this.state.buildWhat} location={this.state.activeSlot } level={this.state.buildLevel} time={this.state.city.builder1Time} builder1Busy={this.state.city.builder1Busy} /> : ''}
              
              <div style={{ marginTop: "20px" }} onClick={this.toggleErrorMessage}>
                  show error message
                  Build Where: {this.state.activeSlot}
                  Build What: {this.state.buildWhat}
              </div>

              <div>
                  <Table bordered={true}>
                      <tbody>
                          <tr>
                              <td colSpan="6">
                                  <Building onBuildingClick={() => this.openModal(building0.location)} b={building0}>Walls </Building>
                              </td>
                          </tr>
                          <tr>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building1.location)} b={building1}>empty </Building>
                              </td>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building2.location)} b={building2}>empty </Building>
                              </td>
                              <td colSpan="2" rowSpan="2">
                                  <Button color="primary" onClick={() => this.openModal(building3.location)} b={building2}>town hall</Button>
                              </td>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building4.location)} b={building4}>empty </Building>
                              </td>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building5.location)} b={building5}>empty </Building>
                              </td>
                          </tr>
                      </tbody>
                  </Table>

                  {/*<div>*/}
                  {/*    <Button*/}
                  {/*        color="danger"*/}
                  {/*        onClick={this.showTestModalClick}*/}
                  {/*    >*/}
                  {/*        Click Me*/}
                  {/*    </Button>*/}
                  {/*    <Modal isOpen={this.state.showTestModal}*/}
                  {/*        toggle={this.toggleModal}*/}
                  {/*    >*/}
                  {/*        <ModalHeader />*/}
                  {/*        <ModalBody className='text-center'>*/}
                  {/*            <h5 className='mb-3'>Text Example</h5>*/}
                  {/*        </ModalBody>*/}
                  {/*    </Modal>*/}
                  {/*</div>*/}


                  <ListGroup className="fixed-bottom" >
                      <ListGroupItem>
                          food {this.state.city.food} || wood {this.state.city.wood} || stone {this.state.city.stone} || iron {this.state.city.iron}
                      </ListGroupItem>
                      <ListGroupItem>
                          builder1Busy: {this.state.city.builder1Busy.toString()} || BuldingID: {this.state.city.construction1BuildingId}
                          activeSlot: {this.state.activeSlot} || activeBuildingId {this.state.activeBuildingId} || Type: {this.state.city.buildingWhat}
                          || builder1Time: {this.state.city.builder1Time}
                      </ListGroupItem>
                  </ListGroup>

                  
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

    async fetchBuildingDone(location, buildingType, level) {

        console.log('at fetchBuildingDone..');
        var updateModel = { cityId: this.state.city.cityId, location: parseInt(location), buildingType, level: parseInt(level) };
        //console.log('updateModel: ' + JSON.stringify(updateModel));
        const token = await authService.getAccessToken();
        const response = await fetch('city/BuildingDone', {
            method: 'POST',
            headers: !token ? { 'Content-Type': 'application/json' } : { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
            body: JSON.stringify(updateModel),
        });
        const data = await response.json();
        //console.log('at updateCityData..returned: cityID: '+ JSON.stringify(data));
        if (data.message !== 'ok') {
            alert('oops fetchBuildingDone..' + data.message)
        } else {
            this.setState({ city: data.city });
        }
    }

    async speedUpUsed() {
        //console.log('at speedup used ..cityid: ' + this.state.city.cityId);
        var speedUpModel = { cityId: this.state.city.cityId, speedUp5min: true, usedOn : "builder1" };
        //console.log('updateModel: ' + JSON.stringify(updateModel));
        const token = await authService.getAccessToken();
        const response = await fetch('city/SpeedUpUsed', {
            method: 'POST',
            headers: !token ? { 'Content-Type': 'application/json' } : { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
            body: JSON.stringify(speedUpModel),
        });
        const data = await response.json();
        //console.log('at speedUpUsed..returned: ' + JSON.stringify(data));
        if (data.message !== 'ok') {
            alert('oops speedUpUsed..' + data.message)
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
        //console.log('at updateCityData..returned: cityID: '+ JSON.stringify(data));
        if (data.message !== 'ok') {
            alert('oops updateCityData..' + data.message)
        } else {
            this.setState({ city: data.city});
        }
    }

    async getCityData() {
        const token = await authService.getAccessToken();
        const response = await fetch('city', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        console.log('at getCityData..loading city: ' + JSON.stringify(data.newBuildingsCost));
        //console.log('at getCityData..loading city: ' + JSON.stringify(data.city));
        //console.log('at getCityData..loading city: ' + JSON.stringify(data.city.buildings[0]));
        this.setState({ city: data.city, userResearch: data.userResearch, newBuildingsCost: data.newBuildingsCost, loading: false });
    }
    

}