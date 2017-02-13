var path = require('path');

module.exports = {
    entry: 
    {
        javascript: [
            //'./wwwroot/lib/react/react-dom.js',
            //'./wwwroot/lib/react/react-with-addons.js',
            //'./wwwroot/js/react-bootstrap.js',
            './wwwroot/js/Cart.jsx',
            './wwwroot/js/CheckoutSuccess.jsx',
            './wwwroot/js/Components.jsx'
            //'./wwwroot/lib/react/react-dom.js',
            //'./wwwroot/lib/react/react.js'
            //'./wwwroot/lib/react/react-with-addons.js'
        ]
    },
    output: {
        path: path.join(__dirname, 'wwwroot/build'),
        filename: 'bundle.js'
    },
    devtool: 'inline-source-map',
    module: {
        loaders: [
            {
                test: /.jsx?$/,
                loader: 'babel-loader',
                exclude: /node_modules/,
                query: {
                    presets: ['es2015', 'react']
                }
            }
        ]
    }
};