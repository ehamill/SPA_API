import React, { Component } from 'react';
import { Button, Modal, ModalHeader, ModalBody, ModalFooter } from 'reactstrap';


export class TownHallModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            //activeSlot : this.props.activeSlot,
        };
        this.showTime = this.showTime.bind(this);
    }

    showTime(secs) {
        let d = Math.floor(secs / (60 * 60 * 24));
        let h = Math.floor((secs % (60 * 60 * 24)) / (60 * 60));
        let m = Math.floor((secs % (60 * 60)) / 60);
        let s = Math.floor(secs % 60);
        if (d >= 1) {
            return (d + "d " + h + "h");
        } else if (h >= 1) {
            return (h + "h " + m + "m ");
        } else {
            return (m + "m " + s + "s");
        }
    }

    componentDidMount() { }

    componentWillUnmount() { }

    render() {
       //const className = "jlsaldfjl";
        //const toggle = "";
        //const buildings = this.props.newBuildings;

        return (
            <Modal
            isOpen={this.props.showModal}
            toggle={this.props.toggleTownHallModal}
            >
                <ModalHeader className="text-right">
                    <div className="text-warning">Town Hall</div>
                    
                </ModalHeader>
                <ModalBody>
                            { }



                </ModalBody>
                <ModalFooter>
         
                    <Button color="secondary" onClick={this.props.closeModal}>
                    Cancel
                    </Button>
                </ModalFooter>
            </Modal>

        );
    }
    
}