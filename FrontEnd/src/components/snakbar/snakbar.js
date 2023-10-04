import React, { useContext } from 'react';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert from '@mui/material/Alert';
import { SnackbarContext } from './snackbarProvider'; // Import the SnackbarContext

function Alert(props) {
    return <MuiAlert elevation={6} variant="filled" {...props} />;
}

const DynamicSnackbar = () => {
    const { snackbarOpen, snackbarMessage, snackbarSeverity, closeSnackbar } = useContext(SnackbarContext);

    return (
        <Snackbar
            open={snackbarOpen}
            autoHideDuration={3000}
            onClose={closeSnackbar}
        >
            <Alert onClose={closeSnackbar} severity={snackbarSeverity}>
                {snackbarMessage}
            </Alert>
        </Snackbar>
    );
};

export default DynamicSnackbar;

