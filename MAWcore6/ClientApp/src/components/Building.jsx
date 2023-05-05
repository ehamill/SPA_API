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

    GetBuildingType(id) {
        switch (id) {
            case 0:
                return "Empty";
            case 1:
                return "Academy";
            case 2:
                return "Barrack";
            case 3:
                return "Beacon Tower";
            case 4:
                return "Cottage";
            case 5:
                return "Embassy";
            case 6:
                return "Feasting Hall";
            case 7:
                return "Forge";
            case 8:
                return "Inn";
            case 9:
                return "Marketplace";
            case 10:
                return "Rally Spot";
            case 11:
                return "Relief Station";
            case 12:
                return "Stable";
            case 13:
                return "Town Hall";
            case 14:
                return "Warehouse";
            case 15:
                return "Workshop";
            case 16:
                return "Farm";
            case 17:
                return "Iron Mine";
            case 18:
                return "Sawmill";
            case 19:
                return "Iron Mine";
            case 20:
                return "Quarry";
            case 21:
                return "Walls";
            default:
                return "Not Found";
        }
    }

    render() {
        const imageUrl = this.GetBuildingType(this.props.b.buildingType) + "Lvl" + this.props.b.level + ".jpg";


        return (
            <Button color="success">
                <div hidden>
                    TypeInt: {this.GetBuildingType(this.props.b.buildingType)}
                </div>
                <div onClick={this.props.onBuildingClick}>
                    {imageUrl}<br />
                    Level: {this.props.b.level}
                </div>
            </Button>
        );
    }
    
}