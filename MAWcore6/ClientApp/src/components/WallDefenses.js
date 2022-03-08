import React, { Component } from 'react';
import {Row,Col, Table, Button } from 'reactstrap';

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
                        List of WallDefenseQueue  new(buildtype,qty,timeStarted, buildtime)

                    </div>
                    
                </Col>
                <Col md-9>
                    <CardGroup>
                        <Card>
                            <CardBody>
                                <CardTitle tag="h5">
                                    <CardImg
                                        alt=""
                                        src="https://picsum.photos/318/180"
                                        top
                                        width="100px"
                                    />
                                    Arch
                                </CardTitle>
                                <CardText>
                                    This is a wider card with supporting text below as a natural lead-in to additional content. This content is a little bit longer.
                                </CardText>
                                <Button>
                                    Recruit
                                </Button>
                            </CardBody>
                        </Card>
                        <Card>
                            <CardImg
                                alt="Card image cap"
                                src="https://picsum.photos/318/180"
                                top
                                width="100%"
                            />
                            <CardBody>
                                <CardTitle tag="h5">
                                    Card title
                                </CardTitle>
                                <CardSubtitle
                                    className="mb-2 text-muted"
                                    tag="h6"
                                >
                                    Card subtitle
                                </CardSubtitle>
                                <CardText>
                                    This card has supporting text below as a natural lead-in to additional content.
                                </CardText>
                                <Button>
                                    Button
                                </Button>
                            </CardBody>
                        </Card>
                        <Card>
                            <CardImg
                                alt="Card image cap"
                                src="https://picsum.photos/318/180"
                                top
                                width="100%"
                            />
                            <CardBody>
                                <CardTitle tag="h5">
                                    Card title
                                </CardTitle>
                                <CardSubtitle
                                    className="mb-2 text-muted"
                                    tag="h6"
                                >
                                    Card subtitle
                                </CardSubtitle>
                                <CardText>
                                    This is a wider card with supporting text below as a natural lead-in to additional content. This card has even longer content than the first to show that equal height action.
                                </CardText>
                                <Button>
                                    Button
                                </Button>
                            </CardBody>
                        </Card>
                    </CardGroup>
                </Col>

                <Col md="9" >
                    <Table bordered={true}>
                        <thead>
                            <tr>
                                <th colSpan="3"> AT</th>
                            </tr>
                        </thead>
                        <tbody>
                            <tr>
                                <th> Required</th>
                                <th> Needed</th>
                                <th> You Own </th>
                            </tr>
                            <tr>
                                <th> Food</th>
                                <th> 1500</th>
                                <th> 3500 </th>
                            </tr>
                            {/*<tr>*/}
                            {/*    <td>Food </td>*/}
                            {/*    <td>{b.food}</td>*/}
                            {/*    <td className={(this.props.food < b.food) ? "text-danger" : "text-success"} >*/}
                            {/*        {this.props.food} </td>*/}
                            {/*</tr>*/}
                            {/*<tr>*/}
                            {/*    <th>Stone </th>*/}
                            {/*    <th>{b.stone}</th>*/}
                            {/*    <th className={(this.props.stone < b.stone) ? "text-danger" : "text-success"} >*/}
                            {/*        {this.props.stone} </th>*/}
                            {/*</tr>*/}
                            {/*<tr>*/}
                            {/*    <th>Wood </th>*/}
                            {/*    <th>{b.wood}</th>*/}
                            {/*    <th className={(this.props.wood < b.wood) ? "text-danger" : "text-success"} >*/}
                            {/*        {this.props.wood} */}
                            {/*    </th>*/}
                            {/*</tr>*/}
                            {/*<tr>*/}
                            {/*    <th>Iron </th>*/}
                            {/*    <th>{b.iron}</th>*/}
                            {/*    <th className={(this.props.iron < b.iron) ? "text-danger" : "text-success"} >*/}
                            {/*        {this.props.iron} */}
                            {/*    </th>*/}
                            {/*</tr>*/}
                        </tbody>
                    </Table>
                </Col>
            </Row>
            
            
        );
    }
    
}