import React, { Component,Fragment } from 'react';
import authService from './api-authorization/AuthorizeService';
import { Fade, Container, Button, Table, ListGroup, ListGroupItem,Toast,ToastHeader,ToastBody, Modal, ModalHeader, ModalBody, ModalFooter} from 'reactstrap';
//import { BottomNav } from './BottomNav'; will do this later....TEsting...
import { Building } from './Building';
import { BuildingTimer } from './BuildingTimer';
import { AddBuildingModal } from './AddBuildingModal';
import { UpgradeModal } from './UpgradeModal';
import { TownHallModal } from './TownHallModal';
import { InnModal } from './InnModal';
import { FeastModal } from './FeastModal';
//import testing form the beging...sdlf ads

export class City extends Component { 

    constructor(props) {
        super(props);
        this.state = {
            intelNeeded : 0,
            city: {},
            troops: {},
            heros: {},
            wallDefenses: {},
            userResearch: {},
            troopQueues: {},
            newBuildingsCost: {},
            loading: true,
            showModal: false,
            activeSlot: -1,
            activeBuildingId : -1,
            buildWhat: "",
            buildTypeInt : 0,
            showBuilding1Timer: false,
            build1Time: 0,
            buildLevel: 0,
            showTownHallModal: false,
            showInnModal: false,
            showFeastModal: false,
            showTestModal: false,
            showErrorMessage: false,
            errorMessage: "",
        };

        this.openModal = this.openModal.bind(this);
        this.closeModal = this.closeModal.bind(this);
        this.closeUpgradeModal = this.closeUpgradeModal.bind(this);
        this.closeTownHallModal = this.closeTownHallModal.bind(this);
        this.closeInnModal = this.closeInnModal.bind(this);
        this.closeFeastModal = this.closeFeastModal.bind(this);
        this.handleClickBuildWhat = this.handleClickBuildWhat.bind(this);
        this.testClick = this.testClick.bind(this);
        this.renderCity = this.renderCity.bind(this);
        this.buildingDone = this.buildingDone.bind(this);
        this.speedUpClick = this.speedUpClick.bind(this);
        this.upgradeBuilding = this.upgradeBuilding.bind(this);
        this.showTestModalClick = this.showTestModalClick.bind(this);
        this.trainTroops = this.trainTroops.bind(this);
        this.hireHero = this.hireHero.bind(this);
        this.next = this.next.bind(this);
    }

    componentDidMount() {
        this.getCityData();
    }

    testClick() {
        console.log('at testClick');
    }

    openModal(slot) {
        let b = this.state.city.buildings.find((x) => x.location === slot);
        console.log('clicked on building: ' + JSON.stringify(b) +' slot:'+ slot);
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
            ///Show AddBUildingModal..ie add new building modal
            this.setState({ showModal: !this.state.showModal });
        }
        else if (b.buildingType === 6) {
            this.setState({ showFeastModal: !this.state.showFeastModal });
        }
        else if (b.buildingType === 8) {
            this.setState({ showInnModal: !this.state.showInnModal });
        }
        else {
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
    closeFeastModal() {
        this.setState({ showFeastModal: false });
    }
    toggleFeastModal = () => {
        this.setState(prevState => ({
            showFeastModal: !prevState.showFeastModal
        }));
    };

    closeInnModal() {
        this.setState({ showInnModal: false });
    }
    toggleInnModal = () => {
        this.setState(prevState => ({
            showInnModal: !prevState.showInnModal
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
        setTimeout(
            () => this.setState(prevState => ({
                showErrorMessage: !prevState.showErrorMessage
            })),
            3000
        );
    };

    handleClickBuildWhat(e) {
        let buildingId = e.target.dataset.building_id;
        let typeString = e.target.dataset.building_type;
        let typeInt = e.target.dataset.building_type_int;
        let level = e.target.dataset.level;
        //alert('typeInt: ' + typeInt);
        this.setState({
            buildWhat: typeString, //gets passed to timer
            buildLevel: level, //gets passed to timer
            buildTypeInt: typeInt,
            showModal: false,
            //showBuilding1Timer: true,
            //build1Time: time,
            //city: newCity,
        });
        this.updateCityData(buildingId, typeInt, level);
        //sconsole.log("handleClickBuildWhat: cityid= " + this.state.city.cityId);
    }
    showTestModalClick() {
        this.setState({ showTestModal: !this.state.showTestModal });
    }
    
    trainTroops(troopTypeInt, qty) {
        //this.setState({
        //    showRecruitModal: false,
        //});
        console.log('at City. train troops. typeInt: ' + troopTypeInt + ' qty: ' + qty );
       this.postTrainTroops(troopTypeInt, qty);
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
            case 10:
                return "Rally_Spot";
            case 11:
                return "Relief_Station";
            case 12:
                return "Stable";
            case 13:
                return "Town_Hall";
            case 14:
                return "Warehouse";
            case 15:
                return "Workshop";
            case 16:
                return "Farm";
            case 17:
                return "Iron_Mine";
            case 18:
                return "Sawmill";
            case 19:
                return "Iron_Mine";
            case 20:
                return "Quarry";
            case 21:
                return "Walls";
            default:
                return "Not_Found";
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


    hireHero(heroId) {
        console.log("cityjs at hireHero fn. heroid: " + heroId );
       
        this.setState({
            //showBuilding1Timer: false,
            //city: newCity,
        });
        this.fetchHireHero(heroId);
    }


    buildingDone( location, type, level) {
        console.log("building done at loc: " + location + " type:" + type + " lvL: " + level);
        //if (level === 0) {
        //    type = "Empty";
        //}
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
        //console.log('at upgradeBuilding buildingid: ' + buildingId +' type: '+ type+ " level: "+ level);
        this.closeUpgradeModal();
        this.updateCityData(buildingId, type, level);
    }

    next() {
        console.log('city js at next...');
    }

    renderCity() {
         
        let building0 = this.state.city.buildings.find((x) => x.location === 0);
        let building1 = this.state.city.buildings.find((x) => x.location === 1);
        let building2 = this.state.city.buildings.find((x) => x.location === 2);
        let building3 = this.state.city.buildings.find((x) => x.location === 3);
        let building4 = this.state.city.buildings.find((x) => x.location === 4);
        let building5 = this.state.city.buildings.find((x) => x.location === 5);
        let building6 = this.state.city.buildings.find((x) => x.location === 6);
        let building7 = this.state.city.buildings.find((x) => x.location === 7);
        let building24 = this.state.city.buildings.find((x) => x.location === 24);
        let building25 = this.state.city.buildings.find((x) => x.location === 25);
        

      return (
          <Container>
              <Fade>
                  <Toast isOpen={this.state.showErrorMessage} className="error-toaster">
                      <ToastHeader toggle={this.toggleErrorMessage}>
                          Error
                          {/*<Button className="btn-close float-right" onClick={this.toggleErrorMessage}></Button>*/}
                  </ToastHeader>
                  <ToastBody>
                      {this.state.errorMessage}
                  </ToastBody>
              </Toast>
              </Fade>

              <FeastModal
                  activeBuildingId={this.state.activeBuildingId}
                  city={this.state.city}
                  heros={this.state.heros}
                  showModal={this.state.showFeastModal}
                  closeModal={this.closeFeastModal}
                  toggleModal={this.toggleFeastModal}
                />

              <InnModal
                  activeBuildingId={this.state.activeBuildingId}
                  city={this.state.city}
                  heros={this.state.heros}
                  showModal={this.state.showInnModal}
                  closeModal={this.closeInnModal}
                  toggleModal={this.toggleInnModal}
                  hireHero={ this.hireHero}
              />

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
                  toggleUpdateModal={this.toggleUpdateModal}
                  troops={this.state.troops}
                  troopQueues={this.state.troopQueues}
                  trainTroops={this.trainTroops}
                  wallDefenses={this.state.wallDefenses}
              /> 

              {/*<BuildingTimer buildingDone={this.buildingDone} speedUpClick={this.speedUpClick}  buildWhat={this.state.buildWhat} location={this.state.activeSlot} level={this.state.buildLevel} time={this.state.city.builder1Time} builder1Busy={this.state.city.builder1Busy} /> */}
              {this.state.city.builder1Busy ? <BuildingTimer buildingDone={this.buildingDone} speedUpClick={this.speedUpClick} buildTypeInt={ this.state.buildTypeInt} buildWhat={this.state.buildWhat} location={this.state.activeSlot } level={this.state.buildLevel} time={this.state.city.builder1Time} builder1Busy={this.state.city.builder1Busy} /> : ''}
              
              <div style={{ marginTop: "20px" }} className="mb-6" onClick={this.toggleErrorMessage}>
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
                                  <Building onBuildingClick={() => this.openModal(building3.location)} b={building3} />
                              </td>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building4.location)} b={building4}>empty </Building>
                              </td>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building5.location)} b={building5}>empty </Building>
                              </td>
                          </tr>
                          <tr>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building6.location)} b={building6}>empty </Building>
                               </td>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building7.location)} b={building7}>empty </Building>

                              </td>
                              <td colSpan="2" rowSpan="2">
                               </td>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building24.location)} b={building24}>empty </Building>
                              </td>
                              <td>
                                  <Building onBuildingClick={() => this.openModal(building25.location)} b={building25}>empty </Building>
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

    async fetchHireHero(heroId) {
        var hireHeroModel = { CityId: this.state.city.cityId, HeroId: parseInt(heroId) };
        const token = await authService.getAccessToken();
        const response = await fetch('city/HireHero', {
            method: 'POST',
            headers: !token ? { 'Content-Type': 'application/json' } : { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
            body: JSON.stringify(hireHeroModel),
        });
        const data = await response.json();
        //console.log('at updateCityData..returned: cityID: '+ JSON.stringify(data));
        if (data.message !== 'ok') {
            //this.setState({ errorMessage: JSON.stringify(data.errors) + JSON.stringify(data), showErrorMessage: true, });
            console.log('data.message: ' + data.message)
            this.setState({ errorMessage: data.message, showErrorMessage: true, });
            setTimeout(
                () => this.setState(prevState => ({
                    showErrorMessage: !prevState.showErrorMessage
                })),
                6000000
            );
        } else {
            this.setState({ city: data.city, newBuildingsCost: data.newBuildingsCost, });
        }

    }


    async fetchBuildingDone(location, buildingTypeInt, level) {

        //console.log('at fetchBuildingDone..buildingTypeInt: ' + buildingTypeInt);
        var updateModel = { cityId: this.state.city.cityId, location: parseInt(location), buildingTypeInt: parseInt(buildingTypeInt), level: parseInt(level) };
        console.log('at fetchBuildingDone.. updateModel: ' + JSON.stringify(updateModel));
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
            this.setState({ city: data.city, newBuildingsCost: data.newBuildingsCost,});
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


    async updateCityData(buildingId, buildingTypeInt, level) {
        var updateModel = { cityId: this.state.city.cityId, buildingId: parseInt(buildingId), buildingTypeInt: parseInt(buildingTypeInt), level: parseInt(level) };
        const token = await authService.getAccessToken();
        const response = await fetch('city/UpdateCity', {
            method: 'POST',
            headers: !token ? { 'Content-Type': 'application/json' } : { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
            body: JSON.stringify(updateModel),
        });
        const data = await response.json();
        //console.log('at updateCityData..returned: cityID: '+ JSON.stringify(data));
        //console.log('at update city data ..' + JSON.stringify(data));
        if (data.message !== 'ok') {
            //this.setState({ errorMessage: JSON.stringify(data.errors) + JSON.stringify(data), showErrorMessage: true, });
            this.setState({ errorMessage: data.message, showErrorMessage: true, });
            setTimeout(
                () => this.setState(prevState => ({
                    showErrorMessage: !prevState.showErrorMessage
                })),
                6000
            );
        } else {
            let buildWhat = this.GetBuildingType(parseInt(buildingTypeInt));
            this.setState({ city: data.city, buildTypeInt: buildingTypeInt, buildWhat: buildWhat });
        }
    }

    async postTrainTroops(troopTypeInt, qty) {

        console.log('at postTrainTroops..troopTypeInt: '+ troopTypeInt +" qty:" + qty +" buildingId: "+ this.state.activeBuildingId);

        var updateModel = { cityId: this.state.city.cityId, buildingId: this.state.activeBuildingId, troopTypeInt: parseInt(troopTypeInt), qty };
        //console.log('updateModel: ' + JSON.stringify(updateModel));
        const token = await authService.getAccessToken();
        const response = await fetch('city/TrainTroops', {
            method: 'POST',
            headers: !token ? { 'Content-Type': 'application/json' } : { 'Authorization': `Bearer ${token}`, 'Content-Type': 'application/json' },
            body: JSON.stringify(updateModel),
        });
        const data = await response.json();
        //console.log('at postTrainTroops..returned: cityID: '+ JSON.stringify(data));
        if (data.message !== 'ok') {
            let message = 'oops error at city.js postTrainTroops..' + data.message;
            this.setState({ errorMessage: message, showErrorMessage: true, });
        } else {
            this.setState({ city: data.city, troopsQueue: data.troopQueues, });
        }
    }

    async getCityData() {
        const token = await authService.getAccessToken();
        const response = await fetch('city', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        //console.log('at getCityData..loading city: ' + JSON.stringify(data.newBuildingsCost));
        //console.log('at getCityData..loading city: ' + JSON.stringify(data.city));
        //console.log('at getCityData..loading city: ' + JSON.stringify(data.city.buildings[0]));
        //console.log('at getCityData..loading troops: ' + JSON.stringify(data.troops));
        //console.log('at getCityData..loading troopqueue: ' + JSON.stringify(data.troopQueues));
        //console.log('at getCityData..loading wallDefenses: ' + JSON.stringify(data.wallDefenses));
        this.setState({
            city: data.city,
            heros: data.heros,
            troopQueues: data.troopQueues,
            troops: data.troops,
            wallDefenses: data.wallDefenses,
            userResearch: data.userResearch,
            newBuildingsCost: data.newBuildingsCost,
            loading: false
        });
    }
    

}