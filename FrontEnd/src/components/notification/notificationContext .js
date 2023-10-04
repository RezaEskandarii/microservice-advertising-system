import React from 'react';
import {Alert, Snackbar} from "@mui/material";

export const NotificationContext = React.createContext();

export const NotificationProvider = ({children}) => {
    const [open, setOpen] = React.useState(false);
    const [message, setMessage] = React.useState('');
    const [severity, setSeverity] = React.useState('info');

    const showNotification = (newMessage, newSeverity = 'info') => {
        setMessage(newMessage);
        setSeverity(newSeverity);
        setOpen(true);
    };

    const hideNotification = () => {
        setOpen(false);
    };

    return (<NotificationContext.Provider value={{showNotification, hideNotification}}>
        {children}
        <Snackbar open={open} autoHideDuration={6000} onClose={hideNotification}>
            <Alert onClose={hideNotification} severity={severity} sx={{width: '100%'}}>
                {message}
            </Alert>
        </Snackbar>
    </NotificationContext.Provider>);
};
