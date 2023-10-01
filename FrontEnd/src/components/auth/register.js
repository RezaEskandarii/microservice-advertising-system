import React, {useEffect, useState} from 'react';
import {Box, Button, Container, Grid, InputLabel, MenuItem, Select, TextField, Typography} from "@mui/material";
import axios from "axios";
import {ApiRoutes} from "../../constants/apiRoutes";

const RegistrationPage = () => {

    const handleRegistration = () => {
        console.log(formData);
    };

    const [formData, setFormData] = useState({
        firstName: '', lastName: '', cellNumber: '', address: {
            street: '', city: '', state: '', postalCode: '', country: '',
        }, email: '', phoneNumber: '', password: '', confirmPassword: '',
    });


    const [locations, setLocations] = useState([]);
    const [selectedCountry, setSelectedCountry] = useState('');
    const [selectedCity, setSelectedCity] = useState('');
    const [cities, setCities] = useState([]);

    useEffect(() => {

        axios.get(ApiRoutes.Locations)
            .then(response => {
                setLocations(response.data);
            })
            .catch(error => {
                console.error('Error fetching locations:', error);
            });

    }, []);


    const handleChange = (event) => {
        const {name, value} = event.target;

        if (name.startsWith('address.')) {
            const addressField = name.split('.')[1];
            setFormData((prevFormData) => ({
                ...prevFormData, address: {
                    ...prevFormData.address, [addressField]: value,
                },
            }));
        } else {
            setFormData((prevFormData) => ({
                ...prevFormData, [name]: value,
            }));
        }
    };

    const handleCountryChange = (event) => {
        const selectedCountry = event.target.value;
        setSelectedCountry(selectedCountry);

        const countryLocations = locations[selectedCountry] || [];
        const cityNames = countryLocations.map(location => location.city_name);
        setCities(cityNames);
        formData.address.country = selectedCountry;
    };


    const handleCityChange = (event) => {
        const selectedCity = event.target.value;
        setSelectedCity(selectedCity);
        formData.address.city = selectedCity;
    };

    return (<>
        <Container sx={{display: 'flex', justifyContent: 'center', mt: 4}}>
            <Box
                sx={{
                    backgroundColor: 'white',
                    padding: '20px',
                    width: '100%',
                    maxWidth: '70%',
                    alignContent: 'center',
                    boxShadow: '0px 0px 10px rgba(0, 0, 0, 0.1)',
                }}
            >
                <div style={{backgroundColor: 'white', padding: '20px', width: '70%', margin: 'center'}}>
                    <Typography variant="h2" sx={{mb: 4}}>
                        Registration
                    </Typography>
                    <form>

                        <Grid container spacing={2}>
                            <Grid item xs={6}>
                                <TextField
                                    label="First Name"
                                    variant="outlined"
                                    size="small"
                                    name="firstName"
                                    value={formData.firstName}
                                    onChange={handleChange}
                                    fullWidth
                                />
                            </Grid>
                            <Grid item xs={6}>
                                <TextField
                                    label="Last Name"
                                    variant="outlined"
                                    size="small"
                                    name="lastName"
                                    value={formData.lastName}
                                    onChange={handleChange}
                                    fullWidth
                                />
                            </Grid>
                        </Grid>

                        <TextField
                            label="Cell Number"
                            variant="outlined"
                            size="small"
                            name="cellNumber"
                            value={formData.cellNumber}
                            onChange={handleChange}
                            fullWidth
                            sx={{mt: 2}}
                        />

                        <TextField
                            label="Street"
                            variant="outlined"
                            size="small"
                            name="street"
                            value={formData.address.street}
                            onChange={handleChange}
                            fullWidth
                            sx={{mt: 2}}
                        />


                        <Grid container spacing={2}>
                            <Grid item xs={6}>

                                <InputLabel id="country-label">Country</InputLabel>
                                <Select
                                    labelId="country-label"
                                    id="country"
                                    value={selectedCountry}
                                    onChange={handleCountryChange}
                                    label="Country"
                                    size="small"
                                    fullWidth
                                >
                                    <MenuItem value="">
                                        <em>Select Country</em>
                                    </MenuItem>
                                    {Object.keys(locations).map(country => (
                                        <MenuItem key={country} value={country}>{country}</MenuItem>
                                    ))}
                                </Select>

                            </Grid>
                            <Grid item xs={6}>

                                <InputLabel id="city-label">City</InputLabel>
                                <Select
                                    labelId="city-label"
                                    id="city"
                                    value={selectedCity}
                                    onChange={handleCityChange}
                                    label="City"
                                    size="small"
                                    fullWidth
                                >
                                    <MenuItem value="">
                                        <em>Select City</em>
                                    </MenuItem>
                                    {cities.map(city => (
                                        <MenuItem key={city} value={city}>{city}</MenuItem>
                                    ))}
                                </Select>

                            </Grid>
                        </Grid>

                        <TextField
                            label="State"
                            variant="outlined"
                            size="small"
                            name="state"
                            value={formData.address.state}
                            onChange={handleChange}
                            fullWidth
                            sx={{mt: 2}}
                        />

                        <TextField
                            label="Postal Code"
                            variant="outlined"
                            size="small"
                            name="postalCode"
                            value={formData.address.postalCode}
                            onChange={handleChange}
                            fullWidth
                            sx={{mt: 2}}
                        />


                        <TextField
                            label="Email"
                            variant="outlined"
                            size="small"
                            name="email"
                            value={formData.email}
                            onChange={handleChange}
                            fullWidth
                            sx={{mt: 2}}
                        />

                        <TextField
                            label="Phone Number"
                            variant="outlined"
                            size="small"
                            name="phoneNumber"
                            value={formData.phoneNumber}
                            onChange={handleChange}
                            fullWidth
                            sx={{mt: 2}}
                        />

                        <TextField
                            label="Password"
                            variant="outlined"
                            size="small"
                            type="password"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            fullWidth
                            sx={{mt: 2}}
                        />

                        <TextField
                            label="Confirm Password"
                            variant="outlined"
                            size="small"
                            type="password"
                            name="confirmPassword"
                            value={formData.confirmPassword}
                            onChange={handleChange}
                            fullWidth
                            sx={{mt: 2}}
                        />

                        <Button variant="contained" onClick={handleRegistration} sx={{mt: 4}}>
                            Register
                        </Button>
                    </form>
                </div>
            </Box>
        </Container>
    </>);
};

export default RegistrationPage;