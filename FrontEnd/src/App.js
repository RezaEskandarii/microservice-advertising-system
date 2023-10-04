import './App.css';
import {BrowserRouter, Route, Routes} from "react-router-dom";
import RegistrationPage from "./components/auth/register";
import React from "react";
import Header from "./components/header";
import LoginPage from "./components/auth/login";
import {NotificationProvider} from "./components/notification/notificationContext ";


function NoPage() {
    return (<div><h3>not found</h3></div>);
}

const App = () => {
    return (<NotificationProvider>
            <BrowserRouter>
                <Header/>
                <Routes>
                    <Route path="/">
                        <Route/>
                        <Route path="*" element={<NoPage/>}/>
                        <Route path="Registration" element={<RegistrationPage/>}/>
                        <Route path="Login" element={<LoginPage/>}/>
                    </Route>
                </Routes>
            </BrowserRouter>
        </NotificationProvider>

    );
};


export default App;
