import React, { Component } from 'react';
import {Row, Col, Button, CardGroup, Card, CardBody, CardTitle, CardImg, CardText, CardFooter } from 'reactstrap';

export class Troops extends Component {
    
    constructor(props) {
        super(props);

        this.state = {

        };
    }

    componentDidMount() {
    }

    componentWillUnmount() {
    }

    render() {
        //const userName = "testing";


        return (
            <Row>
                <Col md="4">
                    <div>
                        Training troop1 time left 2d 12hr 23 minutes
                    </div>
                    <div>
                        Training troop2 time left 2d 12hr 23 minutes
                    </div>
                    <div>
                        Training troop3 time left 2d 12hr 23 minutes
                    </div>
                </Col>
                <Col md="8">
                    <CardGroup>
                { this.props.troops.map((troop) =>
                    <Card key={troop.typeInt}>
                        <CardBody>
                            <CardTitle tag="h5">
                                {troop.typeString}
                            </CardTitle>
                            <CardText>
                                image <br/> Qty: {troop.qty}
                            </CardText>
                        </CardBody>
                        <div className="text-center">
                            <Button className="width-80" onClick={() => this.props.handleRecruitClick(troop.typeInt)} className="float-bottom-right" >
                                Recruit
                            </Button>
                        </div>
                        
                    </Card>

                )}
                    
            </CardGroup>
                </Col>
            </Row>


            
            
        );
    }
    
}