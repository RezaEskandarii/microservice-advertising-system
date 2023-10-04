import {Box, Button, Container, Grid, TextField, Typography} from "@mui/material";
import React, {useState} from "react";


const LoginPage = () => {
    const [authParams, setAuthParams] = useState({
        username: '', password: ''
    });

    const handleChange = (event) => {
        const {name, value} = event.target;

        setAuthParams((prevAuthParams) => ({
            ...prevAuthParams, [name]: value,
        }));

    };

    const handleLoginForm = () => {

    };

    return (<>

        <Container sx={{display: 'flex', justifyContent: 'center', mt: 1}}>
            <Box
                sx={{
                    backgroundColor: 'white',
                    padding: '20px',
                    width: '100%',
                    maxWidth: '70%',
                    alignContent: 'center',
                    boxShadow: '0px 0px 10px rgba(0, 0, 0, 0.1)',
                    marginTop: '80px',
                    display: 'flex',
                }}
            >
                <Box sx={{flex: '1', backgroundImage: 'url(/images/login.jpg)', backgroundSize: 'cover'}}/>
                <Box sx={{flex: '1', padding: '20px'}}>
                    <Typography variant="h5" sx={{mb: 24}}>
                        Login
                    </Typography>
                    <form>
                        <Grid container spacing={2}>
                            <Grid item xs={12}>
                                <TextField
                                    label="Username"
                                    variant="outlined"
                                    size="small"
                                    name="username"
                                    value={authParams.username}
                                    onChange={handleChange}
                                    fullWidth
                                    required={true}
                                />
                            </Grid>
                            <Grid item xs={12}>
                                <TextField
                                    label="Password"
                                    variant="outlined"
                                    size="small"
                                    name="password"
                                    type="password"
                                    value={authParams.password}
                                    onChange={handleChange}
                                    fullWidth
                                    required={true}
                                />
                            </Grid>
                        </Grid>
                        <Button variant="contained" onClick={() => handleLoginForm()}
                                sx={{mt: 4}}>
                            Login
                        </Button>
                    </form>
                </Box>
            </Box>
        </Container>
    </>);
};

export default LoginPage;