import React, { Component } from 'react';
import {Col, Row, Button, Modal, ModalHeader, ModalBody, ModalFooter, Table } from 'reactstrap';

// use this to upgrade to the next thing

export class FeastModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            //activeSlot : this.props.activeSlot,
        };
        
        //this.showTime = this.showTime.bind(this);
    }

    

    render() {
        //const className = "jlsaldfjl";
        //console.log('at in modal. heros:' + JSON.stringify(this.props.heros))
        const heros = this.props.heros.filter(function (attr) {
            return attr.isHired === true;
        });
        return (
            <Modal
                size="lg"
                isOpen={this.props.showModal}
                toggle={this.props.toggleModal}
            >
                <ModalHeader className="text-center" style={{ 'display': 'block' }}>
                    Feasting Hall
                    <Button close  onClick={this.props.toggleModal}>X</Button>
                </ModalHeader>
                <ModalBody>
                    <div>
                        Img
                        <Button className="float-right">Upgrade</Button> 
                        <Button className="float-right">Demo</Button>
                    </div>
                    <hr/>
                    <Row>
                       <Table dark size="sm">
                            <tbody>
                                <tr>
                                    <th>Hero</th>
                                    <th>Level</th>
                                    <th>Politics</th>
                                    <th>Attack</th>
                                    <th>Intelligence</th>
                                    <th>Loyalty</th>
                                    <th>Status</th>
                                    <th>Recall</th>
<th>Action</th>
                                </tr>
                                {heros.map((hero, index) =>
                                    <tr key={index}>
                                        <td>{hero.name}</td>
                                        <td>{hero.level}</td>
                                        <td>{hero.politics}</td>
                                        <td>{hero.attack}</td>
                                        <td>{hero.intelligence}</td>
                                        <td>{hero.loyalty}</td>
                                        <td>
{hero.isMayor ? 'Mayor' : 'Idle'}
            
                                        </td>
                                        <td>
                                            <Button>Recall</Button>
                                        </td>
                                        <td>
                                            <Button>View</Button>
                                        </td>
                                    </tr>
                                )}
                            </tbody>
                        </Table>

                    </Row>
<Row>
View Hero Modal
</Row>
<Row>
 <Col md="4">
    her.image
    <Button>Dismiss</Button>
</Col>
<Col md="8">
    Name: _____hero.name_______
  Level: _______hero.level_____
    Experience ____hero.experience/1000_____  + (click for more attributes)
    <Button>Redistrubute</Button>  <Button>Reward</Button>
    <Button>Rename</Button>   <Button>Upgrade</Button>
</Col>
</Row>
<Row>
 <Col md="6">
    Politics: _____hero.name_______ +
  Attack: _______hero.level_____ +
    Intel ____hero.experience/1000_____  + 
    Attribute: ____0_____
  Energy: __100/100___ +
  Salary: __1040___
</Col>
<Col md="6">
    Layalty: _____hero.name_______ 
  Leadership: _______hero.level_____ 
    Speed ____0_____  
   
    <Button>Appoint Mayor</Button>  
    <Button>Save</Button>   
</Col>
</Row>
<Row>
List of Heros
Hero   Level Status
</Row>
<Row>
Show Gear
One Click change button
But Items button
</Row>
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