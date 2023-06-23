import React, { Component } from 'react';
import { Col, Container, Row, Button, Table } from 'reactstrap';


export class WorldMap extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            hidden: false,
            showAttackModal: false,
        };
        //this.showTime = this.showTime.bind(this);
        //this.setActiveTab = this.setActiveTab.bind(this);
        this.openAttackModal = this.openAttackModal.bind(this);
    }

    
    openAttackModal(x, y) {
        console.log('clicked on map at (', x, ',', y, ")"); 
        this.setState({ showAttackModal: !this.state.showAttackModal });
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
        //console.log('this.props.showModal', this.props.showModal);
    }

    componentWillUnmount() { }

    render() {
        //let townHall = this.props.city.buildings.find((x) => x.buildingType === 13);
        var rows = [];
        for (let y = 9; y >= 0; y--) {
            rows.push({ row: 9 - y, coords: [] });
            for (let x = 0; x <= 9; x++) {
                rows[9 - y].coords.push({ x: x, y: y });
            }
        }
        //console.log('rows: ', JSON.stringify(rows));
        //(1,0),(1,1)
        //(0,0),(1,0)
        
        return (
            <Container hidden={this.state.hidden}>
                <Row>
                    <Col className="" xs="12">
                        <Table size="sm" className="table-bordered table-sm">
                            <tbody>
                                {rows.map((row, index) =>
                                    <tr key={index}>
                                        {row.coords.map((coord, index2) =>
                                            <td key={index2}
                                                onClick={() => this.openAttackModal(coord.x, coord.y)}
                                            >
                                                ({coord.x},{coord.y})
                                            </td>
                                        )}
                                    </tr>
                                )}

                            </tbody>
                        </Table>
                    </Col>
                </Row>
                <AttackModal
                   // activeBuildingId={this.state.activeBuildingId}
                    city={this.state.city}
                    //heros={this.state.heros}
                    showModal={this.state.showAttackModal}
                    closeModal={this.closeAttackModal}
                    toggleModal={this.toggleAttackModal}
                />
            </Container>
            );
        
    }
}