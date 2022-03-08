import React, { Component } from 'react';
import { Button, Modal, Card, CardImg, CardBody, CardTitle, CardSubtitle, CardText, ModalBody, ModalFooter, Table } from 'reactstrap';

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
        const buildings = this.props.newBuildings;

        return (
                <Modal
                    isOpen={this.props.showModal}
                toggle={this.props.toggleAddBuildingModal}
                >
                <ModalBody>
                {buildings.map((b) =>
                    <Card key={ b.type}>
                  <CardImg top width="100%" src="/assets/318x180.svg" alt={b.type} />
                  <CardBody>
                    <CardTitle tag="h5">{b.type}</CardTitle>
                            <CardSubtitle tag="h6" className="mb-2 text-muted">
                                <div className="row">
                                    <div className="col-md-2">
                                        <img src={b.type + ".jpg"} alt="a" width="55px" />
                                    </div>
                                    <div className="col-md-8">
                                        <div className="danger">Requires {b.reqMet ? "" : b.preReq} </div>
                                        <div>
                                            {b.description}
                                        </div>
                                        <div>
                                            slot: {this.props.activeSlot} id: buildingID: {this.props.activeBuildingId}
                                        </div>
                                    </div>
                                    <div className="col-md-2">
                                        <div>
                                            Build Time {this.showTime(b.time)}
                                        </div>
                                        <div>
                                            <Button color="primary"
                                                disabled={b.reqMet ? true : false}
                                                data-building_type={b.type}
                                                data-level="1"
                                                data-building_id={this.props.activeBuildingId}
                                                onClick={this.props.handleClickBuildWhat}>Build</Button>
                                        </div>
                                    </div>
                                </div>
                                
                                
                    </CardSubtitle>
                    <CardText>
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
          



                {/*  <Card>*/}
                {/*  <CardImg top width="100%" src="/assets/318x180.svg" alt="Cottage" />*/}
                {/*  <CardBody>*/}
                {/*    <CardTitle tag="h5">Cottage</CardTitle>*/}
                {/*    <CardSubtitle tag="h6" className="mb-2 text-muted">*/}
                {/*      <div className="danger">Requires {this.props.cottage.reqMet ? "" : this.props.cottage.preReq }</div>*/}
                {/*      Food: {this.props.cottage.food} Stone: {this.props.cottage.stone} Wood: {this.props.cottage.wood} Iron: {this.props.cottage.iron}*/}
                {/*    </CardSubtitle>*/}
                {/*    <CardText> Increase your population. */}
                {/*      <div>*/}
                {/*        Build Time {this.showTime(this.props.cottage.time)}*/}
                {/*      </div>*/}
                {/*    </CardText>*/}
                {/*    <Button color="primary" data-building="cottage" onClick={this.props.handleClickBuildWhat}>Build</Button>*/}
                {/*  </CardBody>*/}
                {/*</Card>*/}
                {/*<Card>*/}
                {/*  <CardImg top width="100%" src="/assets/318x180.svg" alt="Barrack" />*/}
                {/*  <CardBody>*/}
                {/*    <CardTitle tag="h5">Barrack</CardTitle>*/}
                {/*    <CardSubtitle tag="h6" className="mb-2 text-muted">*/}
                {/*      <div className="danger">Requires {this.props.barrack.reqMet ? "" : this.props.barrack.preReq }</div>*/}
                {/*      Food: {this.props.barrack.food} Stone: {this.props.barrack.stone} Wood: {this.props.barrack.wood} Iron: {this.props.barrack.iron}*/}
                {/*    </CardSubtitle>*/}
                {/*    <CardText> Train your troops. */}
                {/*      <div>*/}
                {/*        Build Time {this.showTime(this.props.barrack.time)}*/}
                {/*      </div>*/}
                {/*    </CardText>*/}
                {/*    <Button color="primary" data-building="barrack" onClick={this.props.handleClickBuildWhat}>Build</Button>*/}
                {/*  </CardBody>*/}
                {/*</Card>*/}
                {/*<Card>*/}
                {/*  <CardImg top width="100%" src="/assets/318x180.svg" alt="Inn" />*/}
                {/*  <CardBody>*/}
                {/*    <CardTitle tag="h5">Inn</CardTitle>*/}
                {/*    <CardSubtitle tag="h6" className="mb-2 text-muted">*/}
                {/*      <div className="danger">Requires {this.props.inn.reqMet ? "" : this.props.inn.preReq }</div>*/}
                {/*      Food: {this.props.inn.food} Stone: {this.props.inn.stone} Wood: {this.props.inn.wood} Iron: {this.props.inn.iron}*/}
                {/*    </CardSubtitle>*/}
                {/*    <CardText> Recruit new heros. The higher the level, the more heros to choose from and the higher the level heros. */}
                {/*    <div>*/}
                {/*        Build Time {this.showTime(this.props.inn.time)}*/}
                {/*      </div>*/}
                {/*    </CardText>*/}
                {/*    <Button color="primary" data-building="inn" onClick={this.props.handleClickBuildWhat}>Build</Button>*/}
                {/*  </CardBody>*/}
                {/*</Card>*/}



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