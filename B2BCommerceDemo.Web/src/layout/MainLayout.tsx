import { Box, Container } from "@mui/material";
import { Outlet } from "react-router-dom";
import Navbar from "../components/layout/Navbar";
import Footer from "../components/layout/Footer";

const MainLayout = () => {
    return (
        <Box
            sx={{
                minHeight: "100vh",
                bgcolor: "background.default",
                display: "flex",
                flexDirection: "column",
            }}
        >
            <Navbar />

            <Container
                component="main"
                maxWidth="xl"
                sx={{
                    py: 4,
                    flexGrow: 1,
                }}
            >
                <Outlet />
            </Container>

            <Footer />
        </Box>
    );
};

export default MainLayout;