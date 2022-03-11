import React, { Component } from 'react';
import { Row, Col, Button, CardGroup,Card,CardBody,CardTitle,CardImg,CardText, } from 'reactstrap';

export class WallDefenses extends Component {
    
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
                <Col md="3" >
                    <div>
                        Total spaces: calculated by Wall lvl
                        Spaces used: Calculated by defQty
                        Spaces Available: Difference
                    </div>
                    <div>
                        Walls queue: show what is building, qty being build, time left.
                        Only first will be building.
                        Cancel buttons on all of them.
                        City will need: TrapQty, AtQty, TrebQty, AbbatisQty, RollLogQty,
                        ListOfWallDefenses: type, costs, description
                        List of WallDefenseQueue  new(buildtype,qty,timeStarted, buildtime)
                        ListOfWallDefStats: at - attck, range, life, etc.
                    </div>
                    
                </Col>
                <Col md="9">
                    <CardGroup>
                        <Card>
                            <CardBody>
                                <CardTitle tag="h5">
                                    Traps
                                </CardTitle>
                                <CardText>
                                    <div hidden>
                                        description
                                        Fortified Unit Type	Vacant Space Per Unit	Food	Lumber	Stone	Iron	Time per Unit
                                        Trap	1 Space	50	500	100	50	1m
                                        Abatis	2 Spaces	100	1,200	0	150	2m
                                        Archer's Tower	3 Spaces	200	2,000	1,500	500	3m
                                        Rolling log	4 Spaces	300	6,000	0	0	6m
                                        Defensive Trebuchet	5 Spaces	600	0	8,000	0	10m

                                        Walls Req: Quarry Lv.2 Workshop Lv.1
                                        Level	Prerequisite	Food	Lumber	Stone	Iron	Costs Time	Durability	Fortified
                                        Spaces	Allows
                                        
                                        1	3,000	1,500	10,000	500	30m 00s	10,000	1,000	Trap and level 3 Town_Hall
                                        2	6,000	3,000	20,000	1,000	1h 00m 00s	30,000	3,000	Abatis and level 4 TH
                                        3	12,000	6,000	40,000	2,000	2h 00m 00s	60,000	6,000	Archer's Tower and level 5 TH
                                        4	24,000	12,000	80,000	4,000	4h 00m 00s	100,000	10,000	level 6 TH
                                        5	48,000	24,000	160,000	8,000	8h 00m 00s	150,000	15,000	Rollinglog and level 7 TH
                                        6	96,000	48,000	320,000	16,000	16h 00m 00s	210,000	21,000	level 8 TH
                                        7	192,000	96,000	640,000	32,000	32h 00m 00s	280,000	28,000	Defensive Trebuchet and level 9 TH
                                        8	384,000	192,000	1,280,000	64,000	64h 00m 00s	360,000	36,000	level 10


                                    </div>
                                    </CardText>
                                <Button >
                                    Build
                                </Button>
                            </CardBody>
                        </Card>
                    </CardGroup>
                </Col>

            </Row>
            
            
        );
    }
    
}