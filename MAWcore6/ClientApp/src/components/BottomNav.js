import React, { Component } from "react";
import {Button, Container, Row, Col, List } from 'reactstrap';

export class BottomNav extends Component {
    //static displayName = NavMenu.name;

    componentDidMount() { }

    componentWillUnmount() { }

    render() {
        const isAuthenticated = true;

        if (!isAuthenticated) {
            return (
                <nav className="navbar fixed-bottom navbar-light bg-light">
                    <a className="navbar-brand" href="#jlj">
                        Not logged in Bottom Nav
                    </a>
                </nav>
            );
        } else {
            return (
                <nav className="navbar  navbar-light bg-light">
                    <Container style={{ display: "block", height:"auto", maxWidth: "100%" }} className="ml-1 mr-1">
                        <Row className="">
                            <Col md="3" >
                                <Row >
                                    <Col md="6" className="text-left text-uppercase">
                                        user pic
                                    </Col>
                                    <Col md="6" id="serverTime">
                                        14:22
                                    </Col>
                                    <Col md="6" >
                                        <Button>Items</Button>
                                    </Col>
                                    <Col md="6" >
                                        <Button>Friends</Button>
                                    </Col>
                                    <Col md="6" >
                                        <span className=" ">Title : Knight</span>
                                    </Col>
                                    <Col md="6" >
                                        <span> Rank: Liutenant</span>
                                    </Col>
                                    <Col md="6" >
                                        <span className="prestige"> 1055454</span>
                                    </Col>
                                    <Col md="6" >
                                        <span className="honor"> 10000</span>
                                    </Col>
                                    <Col md="6" >
                                        <span>Silly_Alliance</span>
                                    </Col>
                                    <Col md="6" >
                                        <span className="server-ranking"> 1533</span>
                                    </Col>
                                </Row>
                            </Col>
                            <Col md="6" >
                                <Row>
                                    <Col md="2" >
                                        <Button>A</Button>
                                        <Button>W</Button>
                                    </Col>
                                    <Col md="10" >
                                        <List id="chatRoom" style={{ display: "block", maxWidth: "100%", backgroundColor: 'azure', height: "144px" }} >
                                            <li>user1: Lorem ipsum dolor sit amet</li>
                                            <li>user2: Consectetur adipiscing elit</li>
                                            <li>user1: Integer molestie lorem at massa</li>
                                        </List>
                                        <div>
                                            <input type="text" id="usersMessage" name="name" value="" />
                                            <Button id="submitMessage"> -- </Button>
                                        </div>
                                    </Col>
                                </Row>

                            </Col>
                            <Col md="3" >
                                <Button>Buy coins</Button>
                                <Button> Wheel</Button>
                                <Button>Shop</Button>
                                <Button>Alliance</Button>
                                <Button>Quest</Button>
                                <Button>Reports</Button>
                                <Button>Options</Button>
                                <Button>Achieve</Button>
                                <Button>Stats</Button>
                                <Button>Mail</Button>
                            </Col>
                        </Row>
                    </Container>
                </nav>
            );
        }
    }
}
