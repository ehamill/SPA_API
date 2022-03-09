import React, { Component } from 'react';
import { Button, CardGroup,Card,CardBody,CardTitle,CardImg,CardText, } from 'reactstrap';

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
            //{ troops.map((troop) =>
            //        <Card key={b.type}>
                        
            //}
            <CardGroup>
                { troops.map((troop) =>
                    <Card key={troop.type}>
                        <CardBody>
                            <CardTitle tag="h5">
                                Arch
                            </CardTitle>
                            <CardText>
                                <CardImg
                                    alt=""
                                    src="https://picsum.photos/318/180"
                                    top
                                    width="100px"
                                />
                                <span>153 = qty</span>
                                <div>onclick go to recruit troops(type,costs,details: attk,def,speed,etc)</div>
                                <Button className="float-bottom-right" >
                                    Recruit
                                </Button>
                            </CardText>

                        </CardBody>
                    </Card>

                )}
                    
            </CardGroup>
            
        );
    }
    
}