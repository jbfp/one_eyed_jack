// Polyfills:
// Array.prototype.flat does not yet exist in Edge.
import 'array.prototype.flat/auto';

import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import * as serviceWorker from './custom-service-worker';
import './index.css';

ReactDOM.render(<App />, document.getElementById('root'));

serviceWorker.register();
