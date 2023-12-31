﻿import React, { Component } from 'react';
import { Carousel, CarouselIndicators, CarouselItem, CarouselCaption, CarouselControl, Row,Col, Button, Modal, Card, CardImg, CardBody, CardTitle, CardSubtitle, CardText, ModalBody, ModalFooter, Table } from 'reactstrap';
//import { } from 'reactstrap';
//import crap forom crap those who said is the only way to do it

export class AddBuildingModal extends Component {
    
    constructor(props) {
        super(props);

        this.state = {
            activeIndex: 0, setActiveIndex: 0,
            animating: false, setAnimating: false,
            items : [],
        };
        this.showTime = this.showTime.bind(this);
        this.previous = this.previous.bind(this);
        this.next = this.next.bind(this);
        this.goToIndex = this.goToIndex.bind(this);
    }
    
    previous() {
        var itemsCount = this.state.items.length;
        const nextIndex = this.state.activeIndex === 0 ? itemsCount - 1 : this.state.activeIndex - 1;
        this.setState({ activeIndex: nextIndex });
        console.log('at prev..activeIndex' + this.state.activeIndex + ' items count: ' + itemsCount);
    }
    next() {
        var itemsCount = this.state.items.length;
        const nextIndex = this.state.activeIndex === itemsCount - 1 ? 0 : this.state.activeIndex + 1;
        this.setState({ activeIndex: nextIndex });
        //console.log('at nexyt..');
    }

    goToIndex(x) {
        this.setState({ activeIndex: x });
        //console.log('at goToIndex..index: '+ x);
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

    componentDidMount() {
        console.log('mounted add build modal');
        this.setState({
            items: [
                {
                altText: 'Slide1',
                caption: 'Cottage',
                key: 11,
                    src: 'https://picsum.photos/id/456/1200/600'
                },
                {
                    altText: 'Slide 2',
                    caption: 'Barrack',
                    key: 2,
                    
                    src: 'https://picsum.photos/id/123/1200/600'
                },
                {
                    altText: 'Slide 3',
                    caption: 'Farm',
                    key: 3,
                    src: 'https://picsum.photos/id/678/1200/600'
                },
                
                ],
        });
    }

    componentWillUnmount() { }

    render() {
        let buildings = this.props.city.listOfBuildingsCost.filter(function (el) {
            return el.farm == false;
        });
        //console.log('this.props.activeSlot ' + this.props.activeSlot)
        if (this.props.activeSlot > 23) {
            buildings = this.props.city.listOfBuildingsCost.filter(function (el) {
                return el.farm == true;
            });
            
        }
        const activeBuilding = (this.props.typeInt === 0) ? this.props.city.troops[0] : this.props.city.troops.find((x) => x.typeInt === this.props.typeInt);

        return (
           
                <Modal
                    isOpen={this.props.showModal}
                    toggle={this.props.toggleAddBuildingModal}
                >
                <ModalBody>
                    <Row hidden>

                        <div style={{
                           // display: 'block', width: "320",
                        }}>
                            <Carousel 
                                activeIndex={this.state.activeIndex}
                                next={this.next}
                                previous={this.previous}
                                slide={ false}
                            >
                                <CarouselIndicators
                                    activeIndex={this.state.activeIndex}
                                    items={this.state.items}
                                    onClickHandler={this.goToIndex}
                                />
                                {this.state.items.map(item => {
                                    return (
                                        <CarouselItem
                                            key={item.key}
                                            src={item.src}
                                            altText={item.altText}
                                        >
                                            <img
                                                alt="Slide 1"
                                                src={item.src}
                                                //height="100px"
                                                className="w-100"
                                            />
                                            <div>{item.altText }</div>
                                        <CarouselCaption
                                                captionHeader={item.caption}
                                                captionText={"ActiveState: " + this.state.activeIndex }
                                        />
                                        </CarouselItem>
                                    );
                                })}

                                <CarouselControl
                                    direction="prev"
                                    //directionText="Prev"
                                    onClickHandler={this.previous}
                                />
                                <CarouselControl
                                    direction="next"
                                    directionText="Next"
                                    onClickHandler={this.next}
                                />
                            </Carousel>
                        </div >
                    </Row>

                    <Row>
                        
                    </Row>


                {buildings.map((b) =>
                    <Card key={ b.typeString}>
                  {/*<CardImg top width="100%" src="/assets/318x180.svg" alt={b.type} />*/}
                  <CardBody>
                    <CardTitle tag="h5" className="text-center">
                                {b.typeString}
                    </CardTitle>
                        <CardSubtitle tag="h6" className="mb-2 text-muted">
                        </CardSubtitle>
                            <CardText>
                                <Row>
                                    <Col md="2">
                                        <img src={b.type + ".jpg"} alt="a" width="55px" />
                                    </Col>
                                    <Col md="6">
                                        <Row>
                                            {b.description}
                                        </Row>
                                        <Row className="danger">
                                            {b.reqMet ? "" : "Requires: " + b.preReq}
                                        </Row>
                                        <Row>
                                            BuildingTypeInt : {b.buildingTypeInt}
                                        </Row>
                                        <Row>
                                            slot: {this.props.activeSlot}  buildingID: {this.props.activeBuildingId}
                                        </Row>
                                    </Col>
                                    <Col md="4">
                                        <Row>
                                            Build Time {this.showTime(b.time)}
                                        </Row>
                                        <Row>
                                            <Button color="primary"
                                                disabled={b.reqMet ? false : true}
                                                data-building_type={b.typeString}
                                                data-building_type_int={b.buildingTypeInt}
                                                data-level="1"
                                                data-building_id={this.props.activeBuildingId}
                                                onClick={this.props.handleClickBuildWhat}>Build</Button>
                                        </Row>
                                    </Col>
                                </Row>
                        <Row>
                            <Table bordered={true}>
                                <tbody>
                                    <tr>
                                        <th> Required</th>
                                        <th> Needed</th>
                                        <th> You Own </th>
                                    </tr>
                                    <tr>
                                        <td>Food </td>
                                        <td>{b.food}</td>
                                        <td className={(this.props.city.food < b.food) ? "text-danger" : "text-success"} >
                                                    {this.props.city.food} </td>
                                    </tr>
                                    <tr>
                                        <th>Stone </th>
                                        <th>{b.stone}</th>
                                                <th className={(this.props.city.stone < b.stone) ? "text-danger" : "text-success"} >
                                                    {this.props.city.stone} </th>
                                    </tr>
                                    <tr>
                                        <th>Wood </th>
                                        <th>{b.wood}</th>
                                                <th className={(this.props.city.wood < b.wood) ? "text-danger" : "text-success"} >
                                                    {this.props.city.wood}
                                        </th>
                                    </tr>
                                    <tr>
                                        <th>Iron </th>
                                        <th>{b.iron}</th>
                                                <th className={(this.props.city.iron < b.iron) ? "text-danger" : "text-success"} >
                                                    {this.props.city.iron}
                                        </th>
                                    </tr>
                                </tbody>
                            </Table>

                      </Row>
                    </CardText>
                            
                  </CardBody>
                </Card>
                )}
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