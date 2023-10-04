import {createContext, useContext} from "react";
export const SnackbarContext = createContext({});

export default function useSnackbar () {
    return useContext(SnackbarContext);
};