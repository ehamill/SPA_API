import React, { Component } from 'react';
import { Col, Button, Modal, ModalHeader, ModalBody, ModalFooter, Table } from 'reactstrap';


export class TownHallModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            //activeSlot : this.props.activeSlot,
        };
        this.showTime = this.showTime.bind(this);
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

    componentDidMount() { }

    componentWillUnmount() { }

    render() {
       //const className = "jlsaldfjl";
        //const toggle = "";
        //const buildings = this.props.newBuildings;

        return (
            <Modal
            isOpen={this.props.showModal}
            toggle={this.props.toggleTownHallModal}
            >
                <ModalHeader className="text-right">
                    <div className="text-warning">Town Hall</div>
                    
                </ModalHeader>
                <ModalBody>
                    <Col>
                        Img
                        <Button>Upgrade</Button> <Button className="float-right">Demo</Button>
                    </Col>
                    <Col>
                        Loyalty = 100% with 0 tax rate. updates every 6 minutes.
                    </Col>
                    <Col>
                    Overview - show all buildings and levels. Production rates. how much is made.
                       <Table size="sm">

                            <tbody>
                                <tr>
                                    <th>Resource</th>
                                    <th>Rate </th>
                                </tr>
                                <tr>
                                    <td>Food</td>
                                    <td>{this.props.city.foodRate}</td>
                                </tr>
                                <tr>
                                    <td>Stone</td>
                                    <td>{this.props.city.stoneRate}</td>
                                </tr>
                                <tr>
                                    <td>Wood</td>
                                    <td>{this.props.city.woodRate}</td>
                                </tr>
                                <tr>
                                    <td>Iron</td>
                                    <td>{this.props.city.ironRate}</td>
                                </tr>
                            </tbody>
                        </Table>

                    </Col>
                    <Col>Levy button -
                        borrow resourses from city..decreases loyalty alot
                    </Col>
                    <Col>
                        Comforting button - donate resources to increase loyalty.

                    </Col>
                    <Col>
                        Valleys. show valleys occupied
                    </Col>
                    <Col>
                        If you are asking yourself "What's the best tax rate for my city?", then listen up. I will give you some advice. The highest tax rate you can set to give you the most money without doing anything else is 50%. Here are some numbers for you to look at. The efficiency of the tax rate can be modeled by the function tax revenue = x(1-x)p where x is the tax rate and p is the population limit.
                        Population max is 20,000.
                        Adjust tax rate..

                        Tax rate	30%	40%	50%	60%	70%
                        Loyalty	70%	60%	50%	40%	30%
                        Population	14,000	12,000	10,000	8,000	6,000
                        Income	4,200	4,800	5,000	4800	4,200

                    </Col>


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