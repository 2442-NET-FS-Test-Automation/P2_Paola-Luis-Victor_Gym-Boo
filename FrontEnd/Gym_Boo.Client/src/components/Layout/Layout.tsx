import { Outlet } from "react-router-dom";
import Sidebar from "../SideBar/Sidebar";
import "./Layout.css";

const Layout = () => {
    return (
        <div className="app-layout">
            <Sidebar />
            <main className="app-layout__content">
                <Outlet />
            </main>
        </div>
    );
};

export default Layout;