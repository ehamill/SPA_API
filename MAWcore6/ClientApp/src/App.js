import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import { City } from './components/City';
import AuthorizeRoute from './components/api-authorization/AuthorizeRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import './custom.css'
//import { WorldMap } from './components/WorldMap';
//import { TownHallModal } from './components/TownHallModal';
import 'bootstrap/css/bootstrap.min.css';
import 'bootstrap/js/bootstrap.min.js';

export default class App extends Component {
  static displayName = App.name;

    render() {
        
    return (
      <Layout>
        <Route exact path='/' component={Home} />
        <Route path='/counter' component={Counter} />
        <AuthorizeRoute path='/City' component={City} /> {/*City just url text..must also match  to="/City" in NavMenu..Also add component here*/}
            {/*<AuthorizeRoute path='/WorldMap' component={WorldMap} />*/}
            <AuthorizeRoute path='/fetch-data' component={FetchData} />
            {/*<AuthorizeRoute path='/townhall' component={TownHallModal} />*/}
        <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
      </Layout>
    );
  }
}
