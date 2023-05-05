import React, { Component } from 'react';
import {Col, Row, Button,  } from 'reactstrap';

// use this to upgrade to the 

export class HeroDetail extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            activeHero : 0,
        };
        
        this.clickViewHero = this.clickViewHero.bind(this);
    }

    clickViewHero(id) {
        // Changing state
        this.setState({ activeHero: id })
    }

    render() {
        const hero = this.props.hero;

        return (
            <div>
                <Row>
                    <Col md="4">
                        <div>
                            hero.image
                        </div>
                        <div>
                            HeroID: {hero.heroId}
                        </div>
                        <div>
                            <Button>Dismiss</Button>
                        </div>
                    </Col>
                    <Col md="8">
                        <div>
                            Name: {hero.name}
                        </div>
                        <div>
                            Level: {hero.level}
                        </div>
                        <div>
                            Experience ____hero.experience/1000_____  + (click for more attributes)
                        </div>
                        <div>
                            <Button>Redistrubute</Button>  <Button>Reward</Button>
                            </div>
                        <div>
                            <Button>Rename</Button>   <Button>Upgrade</Button>
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col md="6">
                        <div>
                            Politics: {hero.politics} +
                        </div>
                        <div>
                            Attack: {hero.attack} +
                        </div>
                        <div>
                            Intel: {hero.intelligence} +
                        </div>
                        <div>
                            Attribute: __0__
                        </div>
                        <div>
                            Energy: __100/100___ +
                        </div>
                        <div>
                            Salary: __1040___
                        </div>
                    </Col>
                    <Col md="6">
                        <div>
                            Layalty: {hero.loyalty}
                        </div>
                        <div>
                            Leadership: {hero.level}
                        </div>
                        <div>
                            Speed __0__
                        </div>
                        <div>
                            <Button>Appoint Mayor</Button>
                        </div>
                        <div>
                            <Button>Save</Button>
                        </div>
                    </Col>
                </Row>
                <Row>
                    <Col>
                        Show Gear |
                        One Click change button |
                        But Items button
                    </Col>
                </Row>
            </div>

        );
    }
    
}