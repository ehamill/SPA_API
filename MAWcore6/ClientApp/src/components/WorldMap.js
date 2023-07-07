import React, { Component } from 'react';
import { Col, Container, Row, Button, Table } from 'reactstrap';
import { AttackModal } from './AttackModal';
import authService from './api-authorization/AuthorizeService';

export class WorldMap extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            hidden: false,
            showAttackModal: false,
            coordX: 5,
            coordY: 5,
        };
        //this.showTime = this.showTime.bind(this);
        //this.setActiveTab = this.setActiveTab.bind(this);
        this.openAttackModal = this.openAttackModal.bind(this);
    }

    
    openAttackModal(x, y) {
        //console.log('clicked on map at (', x, ',', y, ")"); 
        this.setState({
            coordX: x, 
            coordY: y,
            showAttackModal: !this.state.showAttackModal,
        });
    }
    closeAttackModal() {
        this.setState({ showAttackModal: false });
    }
    toggleAttackModal = () => {
        this.setState(prevState => ({
            showAttackModal: !prevState.showAttackModal
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
        //scrolll to center of map.
        //let c = document.getElementById("cell2017");//select upperleft corner cell to scroll to
        //let x = c.offsetLeft;
        //let y = c.offsetTop;
        ////console.log('x' + x + ' y ' + y);
        //const element = document.getElementById("portal");
        //element.scrollLeft = x+5;
        //element.scrollTop = y;
        //this.getWorldData();
        //let x = document.getElementById("cell2017").offsetTop;
        //console.log('cell2017 offsetTop', x);
    }

    componentWillUnmount() {
    }

    render() {
        //let townHall = this.props.city.buildings.find((x) => x.buildingType === 13);
        var rows = [];
        let rowWidth = 49;
        for (let y = rowWidth; y >= 0; y--) {
            rows.push({ row: rowWidth - y, coords: [] });
            for (let x = 0; x <= rowWidth; x++) {
                rows[rowWidth - y].coords.push({ x: x, y: y });
            }
        }
        
        //console.log('rows: ', JSON.stringify(rows));
        //(1,0),(1,1)
        //(0,0),(1,0)
        
        return (
            <Container hidden={!this.props.showModal} id="world-map"style={{ }}>
                <div>
                    <Table size="sm2" className="table-bordered " id="world-map-table">
                        <tbody>
                            {rows.map((row, index) =>
                                <tr key={index}>
                                    {row.coords.map((coord, index2) =>
                                        <td key={index2} id={"cell" + index + index2}
                                            onClick={() => this.openAttackModal(coord.x, coord.y)}
                                        >
                                            ({coord.x},{coord.y})
                                        </td>
                                    )}
                                </tr>
                            )}

                        </tbody>
                    </Table>
                </div>
                <AttackModal
                    coordX={this.state.coordX}
                    coordY={this.state.coordY}
                    city={this.state.city}
                    showModal={this.state.showAttackModal}
                    closeModal={this.closeAttackModal}
                    toggleModal={this.toggleAttackModal}
                />
            </Container>
            );
        
    }



    async getWorldData() {
        const token = await authService.getAccessToken();
        const response = await fetch('city/World', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        this.setState({
            city: data.city,
            heros: data.heros,
            troopQueues: data.troopQueues,
            troops: data.troops,
            wallDefenses: data.wallDefenses,
            userResearch: data.userResearch,
            userItems: data.userItems,
            newBuildingsCost: data.newBuildingsCost,
            loading: false
        });
    }

}