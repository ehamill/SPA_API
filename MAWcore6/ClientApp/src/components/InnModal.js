import React, { Component } from 'react';
import {Col, Row, Button, Modal, ModalHeader, ModalBody, ModalFooter, Table } from 'reactstrap';

//use this for intelli sense

export class InnModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            //activeSlot : this.props.activeSlot,
        };
        
        //this.showTime = this.showTime.bind(this);
    }

    

    render() {
        const className = "jlsaldfjl";
        //console.log('at in Inn modal. props:' + JSON.stringify(this.props))
        //console.log('at in modal. heros:' + JSON.stringify(this.props.heros))
        //const innBuildingID = this.props.activeBuildingId;
        //let level = this.props.city.buildings[innBuildingID].image;
        //console.log('at INN modal.s level:' + level + " innBuildingID " + innBuildingID);
        let activeBuildingId = this.props.activeBuildingId;
        let city = this.props.city;
        const innData = (activeBuildingId <= 0) ? city.buildings[0] : city.buildings.find((x) => x.buildingId === activeBuildingId);
        const herosForHire = this.props.city.heros.filter(function (attr) {
            return attr.isHired === false;
        });
        return (
            <Modal
                size="lg"
                isOpen={this.props.showModal}
                toggle={this.props.toggleModal}
            >
                <ModalHeader className="text-center" style={{ 'display': 'block' }}>
                    Inn level { innData.level}
                    <Button close className="float-right" onClick={this.props.toggleModal}></Button>
                </ModalHeader>
                <ModalBody>
                    <div>
                        Img = { innData.image}
                        <Button className="float-right">Upgrade</Button> 
                        <Button className="float-right mr-2">Demo</Button>
                    </div>
                    <hr/>
                    <Row>
                       <Table dark size="sm">
                            <tbody>
                                <tr>
                                    <th>Hero</th>
                                    <th>Level</th>
                                    <th>Politics</th>
                                    <th>Attack</th>
                                    <th>Intelligence</th>
                                    <th>Loyalty</th>
                                    <th>Fee</th>
                                    <th>Action</th>
                                </tr>
                                {herosForHire.map((hero, index) =>
                                    <tr key={index}>
                                        <td>{hero.name}</td>
                                        <td>{hero.level}</td>
                                        <td>{hero.politics}</td>
                                        <td>{hero.attack}</td>
                                        <td>{hero.intelligence}</td>
                                        <td>{hero.loyalty}</td>
                                        <td>{hero.level * 1000}</td>
                                        <td>
                                            <Button onClick={() => this.props.hireHero(hero.heroId )}>Hire {hero.heroId}</Button>
                                        </td>
                                    </tr>
                                )}
                                <tr>
                                    <td colSpan="8">
                                        <span>Gold : { city.gold}</span>
                                    </td>
                                </tr>
                            </tbody>
                        </Table>

                    </Row>


                </ModalBody>
                <ModalFooter>
         
                    <Button color="secondary" onClick={this.props.closeModal}>
                    Cancel
                    </Button>
                </ModalFooter>
            </Modal>

        );
    }
    
}