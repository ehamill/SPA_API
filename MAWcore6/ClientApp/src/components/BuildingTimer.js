﻿import React, { Component } from 'react';
import { Button } from 'reactstrap';

export class BuildingTimer extends Component {
    
    constructor(props) {
        super(props);
        this.state = {
            timeLeft: this.props.time,
            time: {},
            seconds: this.props.time,
            //visible: " hidden ",
            width: "100%",
            buildType: this.props.buildWhat,
            location: this.props.location,
            level: this.props.level,
            
        };
        this.timer = 0;
        this.startTimer = this.startTimer.bind(this);
        this.countDown = this.countDown.bind(this);
        this.speedUp = this.speedUp.bind(this);
    }

    secondsToTime(secs) {
        let days = Math.floor(secs / (60 * 60 * 24));
        let hours = Math.floor((secs % (60 * 60 * 24)) / (60 * 60));
        let minutes = Math.floor((secs % (60 * 60)) / 60);
        let seconds = Math.floor(secs % 60);
        let obj = {
            d: days,
            h: hours,
            m: minutes,
            s: seconds
        };
        let startTime = this.state.timeLeft;
        let percentNotDone = (secs / startTime) * 100 + "%";
        this.setState({ width: percentNotDone });
        return obj;
    }

    componentDidMount() {
        console.log('building timer mount => timeSent: ' + this.props.time +
            " props.buildTypeInt: " + this.props.buildTypeInt + ' buildWhat: ' + this.props.buildWhat +
            ' location: ' + this.props.location + ' level: ' + this.props.level);
        //if (this.props.builder1Busy) {
        //    let timeLeftVar = this.secondsToTime(this.state.seconds);
        //    this.setState({
        //        time: timeLeftVar,
        //    });
        //    this.startTimer();
        //}
        let timeLeftVar = this.secondsToTime(this.state.seconds);
        this.setState({
            time: timeLeftVar,
        });
        this.startTimer();
    }
    startTimer() {
        if (this.timer === 0 && this.state.seconds > 0) {
            this.timer = setInterval(this.countDown, 1000);
        }
    }

    countDown() {
        // Remove one second, set state so a re-render happens.
        let seconds = this.state.seconds - 1;
        this.setState({
            time: this.secondsToTime(seconds),
            seconds: seconds
        });

        // Check if we're at zero.
        if (seconds <= 0) {
            clearInterval(this.timer);
            this.setState({
                //isHidden: true,
            });
            this.props.buildingDone(this.state.location, this.props.buildTypeInt, this.state.level );
        }
    }

    speedUp() {
        console.log('at speedUp: ' + this.state.seconds);
        this.props.speedUpClick();
        let secs = this.state.seconds - 30;
        if (secs <= 0) {
            this.setState({
                seconds: 0,
                //isHidden: true,
            });
            this.props.buildingDone(this.state.location, this.props.buildTypeInt, this.state.level);
        } else {
            this.setState({
                seconds: secs
            });
        }
    }

    render() {
        const timerClassName = (this.props.builder1Busy) ? "" : "hidden";

        return (
            <div id="buildingTime" className={timerClassName}>
                <div id="buildingProgress">
                    <span id="buildingWhat" className="buildingWhat">
                        {this.props.buildWhat} <ShowTime time={this.state.time} /> typeInt:  {this.props.buildTypeInt}
                    </span>
                    <div id="buildingBar" style={{ width: this.state.width }}>
                        {" "}
                    </div>
                    <span id="upgradeTimer" className="upgradeTimer"></span>
                </div>
                <Button onClick={this.speedUp} className="speedUpBtn btn-success">
                    SpeedUp 
                </Button>
                {/*{this.props.builder1Busy.toString()}*/}
            </div>
        );
    }
}

function ShowTime(props) {
    let d = props.time.d;
    let h = props.time.h;
    let m = props.time.m;
    let s = props.time.s;
    if (d >= 1) {
        return d + "d " + h + "h";
    } else if (h >= 1) {
        return h + "h " + m + "m ";
    } else {
        return m + "m " + s + "s";
    }

    
}