import React, { Component } from 'react';
import { Container } from 'reactstrap';
import { NavMenu } from './NavMenu';
import { BottomNav } from './BottomNav';
import { Home } from './Home';
import { Route } from 'react-router';
import authService from './api-authorization/AuthorizeService';
import { ApplicationPaths } from './api-authorization/ApiAuthorizationConstants';
import ApiAuthorizationRoutes from './api-authorization/ApiAuthorizationRoutes';
import AuthorizeRoute from './api-authorization/AuthorizeRoute';

export class Layout extends Component {
    static displayName = Layout.name;
    
    //constructor(props) {
    //    super(props);

    //    this.state = {
    //        isAuthenticated: false,
    //        userName: null
    //    };
    //}

    //componentDidMount() {
    //    this._subscription = authService.subscribe(() => this.populateState());
    //    this.populateState();
    //}

    //componentWillUnmount() {
    //    authService.unsubscribe(this._subscription);
    //}

    //async populateState() {
    //    const [isAuthenticated, user] = await Promise.all([authService.isAuthenticated(), authService.getUser()])
    //    this.setState({
    //        isAuthenticated,
    //        userName: user && user.name
    //    });
    //}

    render () {
    return (
      <div>
        <NavMenu />
        <Container>
          {this.props.children}
        </Container>
      </div>
    );
  }


    //render() {
    //    //const { isAuthenticated, userName } = this.state;
    //    if (!isAuthenticated) {
    //        //const registerPath = `${ApplicationPaths.Register}`;
    //        //const loginPath = `${ApplicationPaths.Login}`;
    //        return this.anonymousView();
    //    } else {
    //        //const profilePath = `${ApplicationPaths.Profile}`;
    //        //const logoutPath = { pathname: `${ApplicationPaths.LogOut}`, state: { local: true } };
    //        return this.authenticatedView(userName);
    //    }
    //}

    //authenticatedView(userName) {
    //    return (
    //        <div>
                
    //            <NavMenu auth={this.isAuthenticated} name={this.userName} />
    //            <Container>
    //                <div>{ this.userName} Logged in</div>
    //            </Container>
    //            <BottomNav />
    //        </div>
    //    );

    //}

    //anonymousView() {
    //    return (
    //        <div>
    //        <NavMenu auth={this.isAuthenticated} name={this.userName}/>
    //            <Container>
    //                <Route exact path='/' component={Home} />
    //                <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
    //            <div>Home page..not logged in</div>
    //        </Container>
    //        </div>
    
    //        //<Fragment>
    //        //<NavItem>
    //        //    <NavLink tag={Link} className="text-dark" to={registerPath}>Register</NavLink>
    //        //</NavItem>
    //        //<NavItem>
    //        //    <NavLink tag={Link} className="text-dark" to={loginPath}>Login</NavLink>
    //        //</NavItem>
    //        //</Fragment>
    //    );
    //}


  
}
