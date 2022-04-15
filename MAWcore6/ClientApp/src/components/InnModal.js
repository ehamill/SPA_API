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
        //const className = "jlsaldfjl";
        //console.log('at in modal. heros:' + JSON.stringify(this.props.heros))
        const herosForHire = this.props.heros.filter(function (attr) {
            return attr.isHired === false;
        });
        return (
            <Modal
                size="lg"
                isOpen={this.props.showModal}
                toggle={this.props.toggleModal}
            >
                <ModalHeader className="text-center" style={{ 'display': 'block' }}>
                    Inn
                    <Button close  onClick={this.props.toggleModal}>X</Button>
                </ModalHeader>
                <ModalBody>
                    <div>
                        Img
                        <Button className="float-right">Upgrade</Button> 
                        <Button className="float-right">Demo</Button>
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