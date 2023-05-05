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
                <div hidden>
                    TypeInt: {this.props.b.buildingType}
                </div>
                <div onClick={this.props.onBuildingClick}>
                    {this.props.b.image}<br />
                    Level: {this.props.b.level}
                </div>
            </Button>
        );
    }
    
}