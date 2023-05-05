import React, { Component } from 'react';
import {Col, Row, Button, Modal, ModalHeader, ModalBody, ModalFooter, Table } from 'reactstrap';
import { HeroDetail } from './HeroDetail';
// use this to upgrade to the next thing

export class FeastModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            activeHero : 0,
        };
        
        this.clickViewHero = this.clickViewHero.bind(this);
    }

    clickViewHero(id) {

        // Changing state
        this.setState({ activeHero: id })
    }

    render() {
        //console.log('at in modal. heros:' + JSON.stringify(this.props.heros))
        const activeHero = this.state.activeHero;
        const heros = this.props.city.heros.filter(function (attr) {
            return attr.isHired === true;
        });
        return (
            <Modal
                size="lg"
                isOpen={this.props.showModal}
                toggle={this.props.toggleModal}
            >
                <ModalHeader className="text-center" style={{ 'display': 'block' }}>
                    Feasting Hall
                    <Button close className="float-right" onClick={this.props.toggleModal}></Button>
                </ModalHeader>
                <ModalBody>

                    <Row>
                        <Col>
                            Img
                            <Button className="float-right">Upgrade</Button>
                            <Button className="float-right mr-2">Demo</Button>
                        </Col>
                    </Row>

                    <Row>
                        <Col>
                            <Table  size="sm">
                                <tbody>
                                    <tr>
                                        <th>Hero</th>
                                        <th>Level</th>
                                        <th>Politics</th>
                                        <th>Attack</th>
                                        <th>Intelligence</th>
                                        <th>Loyalty</th>
                                        <th>Status</th>
                                        <th>Recall</th>
                                        <th>Action</th>
                                    </tr>
                                    {heros.map((hero, index) =>
                                        <tr key={index}>
                                            <td>{hero.name}</td>
                                            <td>{hero.level}</td>
                                            <td>{hero.politics}</td>
                                            <td>{hero.attack}</td>
                                            <td>{hero.intelligence}</td>
                                            <td>{hero.loyalty}</td>
                                            <td>
                                                {hero.isMayor ? 'Mayor' : 'Idle'}

                                            </td>
                                            <td>
                                                <Button>Recall</Button>
                                            </td>
                                            <td>
                                                <Button onClick={() => this.clickViewHero(hero.heroId)}>View</Button>
                                            </td>
                                        </tr>
                                    )}
                                </tbody>
                            </Table>
                        </Col>
                    </Row>
                    <Row hidden>
                    View Hero Modal
                    </Row>
                    {activeHero > 0 && <HeroDetail hero={heros.find((x) => x.heroId === activeHero)} />}
                   

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