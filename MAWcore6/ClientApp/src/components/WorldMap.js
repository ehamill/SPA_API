import React, { Component } from 'react';
import { Nav,NavItem,NavLink,TabContent,TabPane, Col, Container,Row, Button, Table } from 'reactstrap';
//import { Link } from 'react-router-dom';

export class TownHallModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            //activeTab: 1,
        };
        //this.showTime = this.showTime.bind(this);
        //this.setActiveTab = this.setActiveTab.bind(this);
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

    

    componentDidMount() {
        //console.log('this.props.showModal', this.props.showModal);
    }

    componentWillUnmount() { }

    //setActiveTab(id) {
    //     this.setState({ activeTab: id });  
    //}

    render() {
        //let townHall = this.props.city.buildings.find((x) => x.buildingType === 13);

        return (
            <Container hidden={!this.props.showModal}>
                <Row>
                    <Col className="" xs="12">
                        <div>
                            <Table size="sm">
                                <tbody>
                                    
                                    <tr>
                                        <td>Food</td>
                                        
                                    </tr>
                                    <tr>
                                        <td>Stone</td>
                                        
                                    </tr>
                                   
                                </tbody>
                            </Table>
                        </div>
                    </Col>
                </Row>

            </Container>
            );
        
    }
}