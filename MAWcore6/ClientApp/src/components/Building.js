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
        const userName = "testing";


        return (
            <Button color="success">
                <div onClick={this.props.onClick}>
                    Type: {this.props.b.type}
                    Level: {this.props.b.level}
                </div>
            </Button>
        );
    }
    
}