import React from 'react';
import {AppBar, Toolbar, Typography, Container} from '@mui/material';
import {Link} from 'react-router-dom';

function Header() {
    return (
        <AppBar position="fixed">
            <Toolbar>
                <Container>

                    <nav>
                        <Link to="/" style={{marginRight: 10, color: 'white', textDecoration: 'none'}}>
                            Home
                        </Link>
                        <Link to="Login" style={{marginRight: 10, color: 'white', textDecoration: 'none'}}>
                            Login
                        </Link>
                        <Link to="Registration" style={{marginRight: 10, color: 'white', textDecoration: 'none'}}>
                            Register
                        </Link>
                        <Link to="/login" style={{color: 'white', textDecoration: 'none'}}>
                            Login
                        </Link>
                    </nav>
                </Container>
            </Toolbar>
        </AppBar>
    );
}

export default Header;