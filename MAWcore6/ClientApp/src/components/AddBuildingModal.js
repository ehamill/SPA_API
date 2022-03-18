import React, { Component } from 'react';
import {Row,Col, Button, Modal, Card, CardImg, CardBody, CardTitle, CardSubtitle, CardText, ModalBody, ModalFooter, Table } from 'reactstrap';

export class AddBuildingModal extends Component {
    
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
        let buildings = this.props.newBuildings.filter(function (el) {
            return el.farm == false;
        });
        //console.log('this.props.activeSlot ' + this.props.activeSlot)
        if (this.props.activeSlot > 23) {
           buildings = this.props.newBuildings.filter(function (el) {
                return el.farm == true;
            });
            
        }

        return (
                <Modal
                    isOpen={this.props.showModal}
                    toggle={this.props.toggleAddBuildingModal}
                >
                <ModalBody>
                {buildings.map((b) =>
                    <Card key={ b.typeString}>
                  {/*<CardImg top width="100%" src="/assets/318x180.svg" alt={b.type} />*/}
                  <CardBody>
                    <CardTitle tag="h5" className="text-center">
                        {b.typeString}
                    </CardTitle>
                        <CardSubtitle tag="h6" className="mb-2 text-muted">
                        </CardSubtitle>
                            <CardText>
                                <Row>
                                    <Col md="2">
                                        <img src={b.type + ".jpg"} alt="a" width="55px" />
                                    </Col>
                                    <Col md="6">
                                        <div>
                                            {b.description}
                                        </div>
                                        <div className="danger">
                                            {b.reqMet ? "" : "Requires: " + b.preReq}
                                        </div>
                                        <div>
                                            slot: {this.props.activeSlot} id: buildingID: {this.props.activeBuildingId}
                                        </div>
                                    </Col>
                                    <Col md="4">
                                        <div>
                                            Build Time {this.showTime(b.time)}
                                        </div>
                                        <div>
                                            <Button color="primary"
                                                disabled={b.reqMet ? false : true}
                                                data-building_type={b.typeString}
                                                data-level="1"
                                                data-building_id={this.props.activeBuildingId}
                                                onClick={this.props.handleClickBuildWhat}>Build</Button>
                                        </div>
                                    </Col>
                                </Row>
                        <div>
                            <Table bordered={true}>
                                <tbody>
                                    <tr>
                                        <th> Required</th>
                                        <th> Needed</th>
                                        <th> You Own </th>
                                    </tr>
                                    <tr>
                                        <td>Food </td>
                                        <td>{b.food}</td>
                                        <td className={(this.props.food < b.food) ? "text-danger" : "text-success"} >
                                            {this.props.food} </td>
                                    </tr>
                                    <tr>
                                        <th>Stone </th>
                                        <th>{b.stone}</th>
                                        <th className={(this.props.stone < b.stone) ? "text-danger" : "text-success"} >
                                            {this.props.stone} </th>
                                    </tr>
                                    <tr>
                                        <th>Wood </th>
                                        <th>{b.wood}</th>
                                        <th className={(this.props.wood < b.wood) ? "text-danger" : "text-success"} >
                                            {this.props.wood} 
                                        </th>
                                    </tr>
                                    <tr>
                                        <th>Iron </th>
                                        <th>{b.iron}</th>
                                        <th className={(this.props.iron < b.iron) ? "text-danger" : "text-success"} >
                                            {this.props.iron} 
                                        </th>
                                    </tr>
                                </tbody>
                            </Table>

                      </div>
                    </CardText>
                            
                  </CardBody>
                </Card>
                )}
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