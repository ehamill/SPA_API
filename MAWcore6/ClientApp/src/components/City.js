import React, { Component,Fragment } from 'react';
import authService from './api-authorization/AuthorizeService';
import { Container, Button, Table } from 'reactstrap';
import { BottomNav } from './BottomNav';

export class City extends Component {
    //static displayName = Home.name;

    constructor(props) {
        super(props);
        this.state = { city: {}, loading: true };
    }

    componentDidMount() {
        this.getCityData();
    }

    static renderCity(city) {
        return (
            <Fragment>
            <div>Getting City data, cityName {city.cityName} {city.cityId}
                server {city.serverId} userID { city.userId}
                <h1>cityName {city.cityName} {city.cityId}
                    server {city.serverId}
                </h1>
            </div>

            {/*<Table bordered="true">*/}
            {/*  <tbody>*/}
            {/*    <tr>*/}
            {/*      <td><Building info={buildings.find(x => x.location === 7)}>open</Building></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td colSpan="2" rowSpan="2"><Button color="success">town hall</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*    </tr>*/}
            {/*    <tr>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*    </tr>*/}
            {/*    <tr>*/}
            {/*    <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*    </tr>*/}
            {/*    <tr>*/}
            {/*    <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*    </tr>*/}
            {/*    <tr>*/}
            {/*    <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*      <td><Button color="success">open</Button></td>*/}
            {/*    </tr>*/}
            {/*  </tbody>*/}
            {/*</Table>*/}
            </Fragment>
        );
    }

    render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : City.renderCity(this.state.city);

        return (
            <div>
                <h1 className="cityView" >City</h1>
                <p>Getting city from server.</p>
                {contents}
            </div>
        );
    }



    async getCityData() {
        const token = await authService.getAccessToken();
        const response = await fetch('city', {
            headers: !token ? {} : { 'Authorization': `Bearer ${token}` }
        });
        const data = await response.json();
        this.setState({ city: data.city, loading: false });
    }

}