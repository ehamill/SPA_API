import React, { Component } from 'react';
import { Button } from 'reactstrap';

export class Building extends Component {
    
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
            <Button color="success">
                <div onClick={this.props.onBuildingClick}>
                    Type: {this.props.b.typeString}
                    Level: {this.props.b.level}<br/>
                    Img: {this.props.b.image}
                </div>
            </Button>
        );
    }
    
}