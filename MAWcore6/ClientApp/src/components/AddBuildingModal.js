import React, { Component } from 'react';
import { Button, Modal, Card, CardImg, CardBody, CardTitle, CardSubtitle, CardText, ModalBody, ModalFooter } from 'reactstrap';


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
        const className = "jlsaldfjl";
        const toggle = "";
        const buildings = this.props.newBuildings;

        return (
              <Modal
        isOpen={this.props.showModal}
        toggle={toggle}
        className={className}
      >
        <ModalBody>
        {buildings.map((b) =>
          <Card>
          <CardImg top width="100%" src="/assets/318x180.svg" alt={b.type} />
          <CardBody>
            <CardTitle tag="h5">{b.type}</CardTitle>
            <CardSubtitle tag="h6" className="mb-2 text-muted">
              <div className="danger">Requires {b.reqMet ? "" : b.preReq }</div>
                        Food: {b.food} Stone: {b.stone} Wood: {b.wood} Iron: {b.iron}
                        <div>slot: {this.props.activeSlot} id: buildingID: {this.props.activeBuildingId}</div>
                        
            </CardSubtitle>
            <CardText> b.description 
            <div>
                Build Time {this.showTime(b.time)}
              </div>
            </CardText>
                    <Button color="primary"
                        data-building_type={ b.type}
                        data-time={b.time}
                        data-food_cost={b.food}
                        data-stoneCost={b.stone}
                        data-woodCost={b.wood}
                        data-active_slot={this.props.activeSlot}
                        data-level="1"
                        data-building_id={this.props.activeBuildingId}
                        onClick={this.props.handleClickBuildWhat}>Build</Button>
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