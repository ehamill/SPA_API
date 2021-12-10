import React, { Component } from "react";
import { Container, Row, Col, List } from 'reactstrap';

export class BottomNav extends Component {
    //static displayName = NavMenu.name;

    componentDidMount() { }

    componentWillUnmount() { }

    render() {
        const isAuthenticated = true;
        const userName = "erc sjd";

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
                <nav className="navbar fixed-bottom navbar-light bg-light">

                    <Container style={{ display: "block", maxWidth: "100%" }} className="ml-1 mr-1">
                        <Row className="">
                            <Col md="4" className="text-left text-uppercase">
                                <div className="">
                                    Server Time: 21:33
                                </div>
                            </Col>
                            <Col md="4" >
                                <div className="">
                                    <List>
                                        <li>user1: Lorem ipsum dolor sit amet</li>
                                        <li>user2: Consectetur adipiscing elit</li>
                                        <li>user1: Integer molestie lorem at massa</li>
                                    </List>
                                </div>
                            </Col>
                            <Col md="4" style={{}}>
                                <div>
                                    Email
                                </div>

                            </Col>
                        </Row>
                    </Container>
                </nav>
            );
        }
    }
}
