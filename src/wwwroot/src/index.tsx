import React from 'react';
import ReactDOM from 'react-dom';
import { BrowserRouter } from 'react-router-dom';
import App from './App';
import './index.css';

ReactDOM.render((
    <BrowserRouter>
        <App />
    </BrowserRouter>
), document.getElementById('root'));

// Global unhandled error event handler:
declare global {
    interface Window { 
        env: {
            api: URL;
        };
    }
}

const LOG_URL = `${window.env.api}/logs`;

window.addEventListener('error', async (event) => {
    // Cannot serialize Error from event so we convert it to a plain object first.
    // Spread operator does not work (i.e. {...error}).
    const error = event.error;

    const plain = {
        columnNumber: error.columnNumber,
        fileName: error.fileName,
        lineNumber: error.lineNumber,
        message: error.message,
        stack: error.stack,
    };

    try {
        const response = await window.fetch(LOG_URL, {
            body: JSON.stringify(plain),
            headers: { 'Content-Type': 'application/json' },
            method: 'POST',
        });

        if (!response.ok) {
            console.warn('Failed to log', response.status, response.statusText);
        }
    } catch (error) {
        // Log to prevent unhandled error handler being called again.
        console.error(error);
    }
});

// Number.isSafeInteger polyfill:
Number.isSafeInteger = Number.isSafeInteger || ((value: number) => {
    return Number.isInteger(value) && Math.abs(value) <= Number.MAX_SAFE_INTEGER;
});
