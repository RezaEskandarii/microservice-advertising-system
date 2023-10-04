import React from 'react';
import {AppBar, Toolbar, Typography, Container} from '@mui/material';
import {Link} from 'react-router-dom';
import {GetAuthToken} from "../helpers/tokenHelper";
import jwt from "jwt-decode";

const Header = () => {

    const token = GetAuthToken;
    let fullName = "";

    try {
        const decodedToken = jwt(token);
        fullName = `${decodedToken.firstName} ${decodedToken.lastName}`;

    } catch (error) {

        console.error('Error decoding JWT:', error);
    }

    return (<AppBar position="fixed">
        <Toolbar>
            <Container>

                <nav>
                    {fullName && (<>
                        <Link style={{marginRight: 10, color: 'white', textDecoration: 'none'}}>
                            welcome {fullName}
                        </Link>
                    </>)}
                    <Link to="/" style={{marginRight: 10, color: 'white', textDecoration: 'none'}}>
                        Home
                    </Link>
                    {!token && (<>
                        <Link to="Login" style={{marginRight: 10, color: 'white', textDecoration: 'none'}}>
                            Login
                        </Link>
                        <Link to="Registration" style={{marginRight: 10, color: 'white', textDecoration: 'none'}}>
                            Register
                        </Link>
                    </>)}
                    {token ? <Link to="/login" style={{color: 'white', textDecoration: 'none'}}>
                        Profile
                    </Link> : null}
                </nav>
            </Container>
        </Toolbar>
    </AppBar>);
}

export default Header;