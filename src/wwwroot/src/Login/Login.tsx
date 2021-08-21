import React, { useCallback, useState } from 'react';
import { Redirect, RouteChildrenProps } from 'react-router';
import { Auth } from '../auth';
import './Login.css';

export default function Login(props: RouteChildrenProps) {
    const [redirectToReferrer, setRedirectToReferrer] = useState(false);
    const [userName, setUserName] = useState('');
    const handleUserNameChange = useCallback((event) => setUserName(event.target.value), []);
    const submit = useCallback(async () => {
        await Auth.signInAsync(userName);
        return setRedirectToReferrer(true);
    }, [userName]);

    if (redirectToReferrer) {
        const state = props.location.state as any;
        const { from } = state || { from: { pathname: '/' } };

        if (from.pathname === '/login') {
            from.pathname = '/';
        }

        return <Redirect to={from} />;
    }

    if (Auth.isAuthenticated) {
        setRedirectToReferrer(true);
    }

    return (
        <div>
            <form onSubmit={submit}>
                <p>
                    Please sign in to continue
                </p>

                <div>
                    <input
                        type="text"
                        value={userName}
                        onChange={handleUserNameChange}
                        placeholder="Pick a user name"
                        autoCapitalize="none"
                        autoCorrect="off"
                        autoFocus={true}
                    />
                </div>

                <div>
                    <button
                        className="login-submit"
                        type="submit"
                        disabled={userName.length === 0}
                    >
                        Sign in
                    </button>
                </div>
            </form>
        </div>
    );
}
