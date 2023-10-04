import React, {createContext, useContext, useState} from 'react';
import {SnackbarContext} from "./provider";

const SnackbarProvider = ({children}) => {
    const [snackbarOpen, setSnackbarOpen] = useState(false);
    const [snackbarMessage, setSnackbarMessage] = useState('');
    const [snackbarSeverity, setSnackbarSeverity] = useState('success');

    const showSnackbar = (message, severity = 'success') => {
        setSnackbarMessage(message);
        setSnackbarSeverity(severity);
        setSnackbarOpen(true);
    };

    const closeSnackbar = () => {
        setSnackbarOpen(false);
    };

    return (
        <SnackbarContext.Provider value={{showSnackbar, closeSnackbar}}>
            {children}
        </SnackbarContext.Provider>
    );
};

export default SnackbarProvider;