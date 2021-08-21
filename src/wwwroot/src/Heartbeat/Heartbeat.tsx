import React, { useEffect, useRef, useState } from 'react';
import './Heartbeat.css';

const DEFAULT_TIMEOUT = 30000;

enum Status {
    Unknown,
    Ok,
    ServerError,
    ConnectionError,
}

export default function Heartbeat() {
    const [status, setStatus] = useState(Status.Unknown);
    const timerHandle = useRef<number | undefined>(undefined);

    useEffect(() => {
        const timerHandler = async () => {
            setStatus(await getHealthAsync());
            timerHandle.current = window.setTimeout(timerHandler, DEFAULT_TIMEOUT);
        };

        timerHandler();

        return () => window.clearInterval(timerHandle.current);
    }, []);

    return (
        <span
            className="heartbeat"
            data-status={status}
            title={StatusNames.get(status)}
        />
    );
}

const HEALTH_URL = '/health';

const REQUEST: RequestInit = {
    cache: 'no-store',
    method: 'GET',
};

async function getHealthAsync() {
    try {
        const response = await window.fetch(HEALTH_URL, REQUEST);

        if (response.ok) {
            return Status.Ok;
        } else {
            return Status.ServerError;
        }
    } catch {
        return Status.ConnectionError;
    }
}

const StatusNames = new Map<Status, string>([
    [Status.Unknown, 'Unknown'],
    [Status.Ok, 'OK'],
    [Status.ServerError, 'Server error'],
    [Status.ConnectionError, 'Connection error'],
]);
