import React from 'react';
import Server from './server';

export const ServerContext = React.createContext<Server>(
    new Server(''),
);
