import * as React from 'react';
import Stack from '@mui/material/Stack';
import Button from '@mui/material/Button';
import Snackbar from '@mui/material/Snackbar';
import MuiAlert, {AlertProps} from '@mui/material/Alert';

function Alert(props) {
    return <MuiAlert elevation={6} variant="filled" {...props} />;
}

function CustomSnackbar({open, message, severity}) {

    return (<Snackbar
        open={open}
        autoHideDuration={6000}>
        <Alert severity="success" sx={{width: '100%'}}>
            {message}
        </Alert>
    </Snackbar>);
}

export default CustomSnackbar;
