//import authService from './api-authorization/AuthorizeService';
import React, { Component } from 'react';
import { Row, Col, Button, Card, CardBody, CardTitle, CardText, CardFooter } from 'reactstrap';
import { RecruitModal } from './RecruitModal';
import { Timer } from './Timer';

export class Troops extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
             intelNeeded : 0,
            recruitTroopType: 0,
            showRecruitModal: false,
        };
        this.hideRecruitModal = this.hideRecruitModal.bind(this);
    }

    
    hideRecruitModal() {
        this.setState({
            showRecruitModal: false,
        });
    }

    toggleRecruitModal = () => {
        this.setState(prevState => ({
            showRecruitModal: !prevState.showRecruitModal
        }));
    };

    showRecruitModalClick(troopTypeInt) {
        //const troop = this.props.troops.find((x) => x.typeInt === troopTypeInt);
        //console.log('at handleRecruitClick ...troopTypeInt: ' + troopTypeInt);
        this.setState({
            showRecruitModal: true,
            recruitTroopType: troopTypeInt,
        });
        
    }


    render() {
        console.log('troops.js troops' + JSON.stringify(this.props.troops));
        const activeBuildingId = this.props.activeBuildingId;
        const troopQueue = this.props.troopQueues.filter(function (el) {
            return el.buildingId == activeBuildingId;
        });

        return (
            
            <Row>
                <div>
                    <RecruitModal
                        food={this.props.city.food}
                        wood={this.props.city.wood}
                        stone={this.props.city.stone}
                        iron={this.props.city.iron}
                        showModal={this.state.showRecruitModal}
                        closeModal={this.hideRecruitModal}
                        toggleRecruitModal={this.toggleRecruitModal}
                        typeInt={this.state.recruitTroopType}
                        troops={this.props.troops}
                        trainTroops={this.props.trainTroops}
                    />
                </div>
                <Col md="4">
                    {troopQueue.map((queue, index) =>
                        <div key={index}>
                            <div>
                                <Timer seconds={queue.timeLeft} making={queue.qty + ' ' + queue.typeString + 's'} timerExpired={this.timerExpired} />
                            </div>

                            <div >
                                this.props.activeBuildingId: {this.props.activeBuildingId}
                               troopQueueId {queue.troopQueueId}
                                Training {queue.qty} {queue.typeString}
                                Starts: {queue.starts}, Ends: {queue.ends} qty: {queue.qty}
                                type: {queue.typeString} typeInt: {queue.typeInt} buidlingId: {queue.buildingId} seconds: {queue.timeLeft}
                            </div>
                        </div>
                    )}
                </Col>
                <Col md="8">
                    <Row>
                        { this.props.troops.map((troop) =>
                            <Col md="4" key={troop.typeInt}>
                                <Card>
                                    <CardBody>
                                        <CardTitle tag="h5">
                                            {troop.typeString}
                                        </CardTitle>
                                        <CardText>
                                            image <br/> Qty: {troop.qty}
                                        </CardText>
                                    </CardBody>
                                    <div className="text-center">
                                        <Button className="width-80" onClick={() => this.showRecruitModalClick(troop.typeInt)} className="float-bottom-right" >
                                            Recruit troop.typeInt{troop.typeInt}
                                        </Button>
                                    </div>
                                </Card>
                            </Col>
                        )}
                    </Row>
                </Col>
            </Row>


            
            
        );
    }

    
}