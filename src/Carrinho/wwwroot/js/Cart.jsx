var CartItem = React.createClass({
    getInitialState: function () {
        var item = this.props.model;
        return {
            SKU: item.sKU,
            SmallImagePath: item.smallImagePath,
            LargeImagePath: item.largeImagePath,
            Description: item.description,
            SoldAndDeliveredBy: item.soldAndDeliveredBy,
            Price: item.price,
            Quantity: item.quantity,
            Subtotal: item.subtotal
        };
    },
    updateState: function (change) {
        this.setState(Object.assign({}, this.state, change))
    },
    handleIncrement: function () {
        this.postQuantity(this.state.quantity + 1);
    },
    handleDecrement: function () {
        this.postQuantity(this.state.quantity - 1);
    },
    removeItem: function () {
        this.postQuantity(0);
    },
    postQuantity: function (quantity, callback) {
        $('.overlay').show();

        $.ajax({
            url: '/api/Cart',
            type: 'post',
            data: {
                SKU: this.props.model.SKU,
                Quantity: quantity,
                Price: this.props.model.Price
            },
            headers: {
                'RequestVerificationToken': this.props.TokenHeaderValue
            },
            dataType: 'json'
        }).done(function (data) {
            for (var item of data.CartItems) {
                if (item.SKU == this.props.model.SKU) {
                    this.updateState({ Quantity: item.Quantity, Subtotal: item.Subtotal });
                    this.props.handleCartChange(data, item);
                    return;
                }
            }
        }.bind(this))
        .always(function () {
            $('.overlay').hide();
        });;
    },
    handleQuantityChanged: function (event) {
        var newQty = 1;
        var val = event.target.value;
        if (val && !isNaN(val))
            newQty = parseInt(val);
        this.postQuantity(newQty);
    },
    render: function () {
        return (
            <Row className="vertical-align">
                <Column md={2} className="justify-left">
                    <Row className="fullwidth">
                        <Column md={3}>
                            <img src={'../' + this.state.SmallImagePath} width="80" height="80" />
                        </Column>
                    </Row>
                </Column>
                <Column md={4} className="justify-left">
                    <Row className="fullwidth">
                        <Column md={9}>
                            <span>{this.state.Description}</span>
                        </Column>
                    </Row>
                </Column>
                <Column md={2} className="green justify-center">
                    <Dollars val={this.state.Price } />
                </Column>
                <Column md={2} className="justify-center">
                    <div className="text-center">
                        <ButtonGroup>
                            <input type="button" className="btn btn-default" value="-" onClick={this.handleDecrement} />
                            <input type="text" className="btn" value={this.state.Quantity} onChange={this.handleQuantityChanged } />
                            <input type="button" className="btn btn-default" value="+" onClick={this.handleIncrement} />
                        </ButtonGroup>
                        <a onClick={this.removeItem} className="remove pointer">Remove</a>
                    </div>
                </Column>
                <Column md={2} className="green justify-right">
                    <Dollars val={this.state.Subtotal} />
                </Column>
            </Row>
        );
    }
})

class CartView extends React.Component {

    constructor(props) {
        super(props);
        this.state = {};
        var items = [];

        var item;
        for (var i = 0; i < (this.props.model.cartItems || []).length; i++) {
            item = this.props.model.cartItems[i];
            //items.push(item);

            items.push({
                id: item.id,
                sku: item.sku,
                smallImagePath: item.smallImagePath,
                largeImagePath: item.largeImagePath,
                description: item.description,
                soldAndDeliveredBy: item.soldAndDeliveredBy,
                price: item.price,
                quantity: item.quantity,
                subtotal: item.subtotal
            });
        }

        this.state = {
            canFinishOrder: true,
            items: items,
            Subtotal: this.props.model.Subtotal,
            DiscountRate: this.props.model.DiscountRate,
            DiscountValue: this.props.model.DiscountValue,
            Total: this.props.model.Total
        };
    }

    handleCartChange(cart, cartItem) {
        var newState = Object.assign({}, this.state, {
            Subtotal: cart.Subtotal,
            DiscountRate: cart.DiscountRate,
            DiscountValue: cart.DiscountValue,
            Total: cart.Total
        });
        if (cartItem.Quantity == 0) {
            newState.items.splice(newState.items.findIndex(i =>
                i.SKU == cartItem.SKU), 1);
        }
        this.setState(newState);
    }

    render() {
        const header = (<Row className="vertical-align">
                                    <Column md={6} className="justify-left">item(s)</Column>
                                    <Column md={2} className="justify-center">unit price</Column>
                                    <Column md={2} className="justify-center">quantity</Column>
                                    <Column md={2} className="justify-right">subtotal</Column>
        </Row>);

        const body = (this.state.items.map(item => {
            return <CartItem key={item.SKU} model={item}
                             handleCartChange={this.handleCartChange.bind(this)}
                             TokenHeaderValue={this.props.TokenHeaderValue} />;
        }
        ));

        const footer = (<Row>
                            <Column md={7}></Column>
                            <Column md={5} className="my-children-have-dividers">
                                <Row className="vertical-align">
                                    <Column md={8} className="justify-right">
                                        Subtotal ({this.state.items.length}<Pluralize value={this.state.items.length} singular="item" plural="items" />):
                                    </Column>
                                    <Column md={4} className="green justify-right">
                                        <span>
                                            <Dollars val={this.state.Subtotal} />
                                        </span>
                                    </Column>
                                </Row>
                                { this.state.discountRate
                                ?
                                    <Row className="vertical-align">
                                        <Column md={8} className="justify-right">
                                            Discount (<span>{this.state.discountRate}</span>%):
                                        </Column>
                                    <Column md={4} className="green justify-right">
                                        <span>
                                            <Dollars val={this.state.discountValue} />
                                        </span>
                                    </Column>
                                    </Row>
                                    : null
                                }
                                <Row className="vertical-align">
                                    <Column md={12} className="justify-right">
                                    <h3>
                                        Total:&nbsp;
                                        <span className="green">
                                            <Dollars val={this.state.total} />
                                        </span>
                                    </h3>
                                    </Column>
                                </Row>
                            </Column>
        </Row>);

        return (
                <div className="cart">
                    {
                        this.state.items.length == 0 ? null :
                        <div>
                        {/* TITLE */}
                        <h3>Your shopping cart ({ this.state.items.length}<Pluralize value={this.state.items.length} singular="item" plural="items" />)</h3>
                        {/* NAVIGATION BUTTONS */}
                        <Row>
                            <Column md={3}>
                                <a href={this.props.urlNewProduct}>
                                    <button type="button" className="btn btn-success">Add new product</button>
                                </a>
                            </Column>
                            <Column md={3} className="pull-right">
                                <a href={this.props.urlCheckoutSuccess}>
                                    <button type="button" className="btn btn-success pull-right">Proceed to checkout</button>
                                </a>
                            </Column>
                        </Row>
                        {/* NAVIGATION BUTTONS */}
                        <br />
                        {/* CART PANEL */}
                        <Panel header={header} footer={footer}>
                            {body}
                        </Panel>
                        {/* CART PANEL */}

                        {/* NAVIGATION BUTTONS */}
                        <Row>
                            <Column md={3}>
                                <a href={this.props.urlNewProduct}>
                                    <button type="button" className="btn btn-success">Add new product</button>
                                </a>
                            </Column>
                            <Column md={3} className="pull-right">
                                <a href={this.props.urlCheckoutSuccess}>
                                    <button type="button" className="btn btn-success pull-right">Proceed to checkout</button>
                                </a>
                            </Column>
                        </Row>
                        {/* NAVIGATION BUTTONS */}
                        </div>
                    }
                    {
                    this.state.items.length > 0
                    ? null
                    :
                        <div>
                            <h1><br /><br />:(</h1>
                            <div>
                                <h1>
                                    Oops! Your shopping cart is empty.
                                </h1>
                                <br />
                                <div className="empty-cart-content-message">
                                    Enter more products and resume shopping.
                                </div>
                                <br />
                                <div>
                                    {
                                        this.state.canFinishOrder
                                        ?
                                        <a href={this.props.urlNewProduct}>
                                            <button type="button" className="btn btn-success">Enter new product</button>
                                        </a>
                                        : null
                                    }
                                </div>
                            </div>
                        </div>
                    }
                </div>
      );
    }
}
