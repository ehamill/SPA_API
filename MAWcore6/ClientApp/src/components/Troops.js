import authService from './api-authorization/AuthorizeService';
import React, { Component } from 'react';
import { Row, Col, Button, Card, CardBody, CardTitle, CardImg, CardText, CardFooter } from 'reactstrap';
import { RecruitModal } from './RecruitModal';

export class Troops extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            recruitTroopType: 0,
            showRecruitModal: false,
        };
        //this.showRecruitModalClick = this.showRecruitModalClick.bind(this);
        this.hideRecruitModal = this.hideRecruitModal.bind(this);
       // this.trainTroops = this.trainTroops.bind(this);
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
        this.setState({
            showRecruitModal: true,
            recruitTroopType: troopTypeInt,
        });
        //console.log('at handleRecruitClick ...troopTypeInt: ' + troopTypeInt);
    }

    //trainTroops(troopTypeInt, qty) {
    //    this.setState({
    //        showRecruitModal: false,
    //    });
    //    console.log('at troops. train troops. typeInt: ' + troopTypeInt + ' qty: ' + qty);
    //    //this.postTrainTroops(troopTypeInt, qty, this.props.activeBuildingId);
    //    this.props.TrainTroops(troopTypeInt, qty, this.props.activeBuildingId);
    //}

    componentDidMount() {
    }

    componentWillUnmount() {
    }

    

    render() {
        const userName = "testing";


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
                        troopType={this.state.recruitTroopType}
                        troops={this.props.troops}
                        trainTroops={this.props.trainTroops}
                    />
                </div>
                <Col md="4">
                    {this.props.troopQueues.map((queue) =>
                        <div>
                            Starts: {queue.starts}, Ends: {queue.ends} qty: {queue.qty}
                            type: {queue.troopType} buidlingId: {queue.buildingId}
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
                                            Recruit
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