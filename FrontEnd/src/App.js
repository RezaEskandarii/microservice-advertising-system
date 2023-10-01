import './App.css';
import {BrowserRouter, Route, Router, Routes} from "react-router-dom";
import RegistrationPage from "./components/auth/register";
import React from "react";
import Header from "./components/header";

function NoPage() {
    return (<div><h3>not found</h3></div>);
}

const App = () => {
    return (<BrowserRouter>
        <Header/>
        <Routes>
            <Route path="/">
                <Route/>
                <Route path="*" element={<NoPage/>}/>
                <Route path="Registration" element={<RegistrationPage/>}/>
            </Route>
        </Routes>
    </BrowserRouter>);
};


export default App;
