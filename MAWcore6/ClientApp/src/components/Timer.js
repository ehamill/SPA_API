import React, { Component } from 'react';
import { Button, Col, Text } from 'reactstrap';

export class Timer extends Component {
    
    constructor(props) {
        super(props);
        this.state = {
             intelNeeded : 0,
            timeLeft: this.props.seconds,
            time: {},
            seconds: this.props.seconds,
            //isHidden: this.props.isHidden,
            width: "100%",
            //buildType: this.props.buildWhat,
            //location: this.props.location,
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
        //console.log('building timer mount => timeSent: ' + this.props.time + ' buildWhat: '
        //    + this.props.buildWhat + ' location: ' + this.props.location+ ' level: '+this.props.level);
        
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
            //console.log('training done at timer.js');
            this.props.buildingDone(this.state.location, this.props.buildTypeInt, this.state.level);
        }
    }

    speedUp() {
        //console.log('at speedUp: ' + this.state.seconds);
        //this.props.speedUpClick();
        let secs = this.state.seconds - 30;
        if (secs <= 0) {
            this.setState({
                seconds: 0,
                //isHidden: true,
            });
            //console.log('training done at timer.js');
            this.props.buildingDone(this.state.location, this.props.buildTypeInt, this.state.level);
        } else {
            this.setState({
                seconds: secs
            });
        }
    }

    render() {
        //const timerClassName = (this.props.builder1Busy) ? "" : "hidden";

        return (
            <Col>
                <div>
                    {this.props.making} typeint: {this.props.buildTypeInt} <ShowTime time={this.state.time} />
                    <Button onClick={this.speedUp} className="btn-sm btn-primary speedUpButton float-right">
                        SpeedUp
                    </Button>
                </div>
                <div className="progress" style={{ height: "20px" }} >
                    <div className="progress-bar" role="progressbar" style={{ width: this.state.width  }} aria-valuenow="25"
                        aria-valuemin="0" aria-valuemax="100">
                    </div>
                </div>
            </Col>
            
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