import { useState } from "react";
import {
    AdminPanelSettings,
    Business,
    Dashboard,
    Inventory2,
    Menu as MenuIcon,
    Paid,
    ReceiptLong,
    SettingsSuggest,
} from "@mui/icons-material";
import {
    Box,
    Button,
    Chip,
    Divider,
    Drawer,
    List,
    ListItemButton,
    ListItemIcon,
    ListItemText,
    Paper,
    Stack,
    Typography,
} from "@mui/material";
import { Link, Outlet, useLocation, } from "react-router-dom";

const drawerWidth = 260;

const navigationItems = [
    {
        label: "Dashboard",
        path: "/admin",
        icon: <Dashboard />,
        available: true,
    },
    {
        label: "Products",
        path: "/admin/products",
        icon: <Inventory2 />,
        available: true,
    },
    {
        label: "Companies",
        path: "/admin/companies",
        icon: <Business />,
        available: true,
    },
    {
        label: "Orders",
        path: "/admin/orders",
        icon: <ReceiptLong />,
        available: true,
    },
    {
        label: "Pricing",
        path: "/admin/pricing",
        icon: <Paid />,
        available: true,
    },
    {
        label: "Integrations",
        path: "/admin/integrations",
        icon: <SettingsSuggest />,
        available: true,
    },
];

const AdminLayout = () => {
    const [mobileOpen, setMobileOpen] =
        useState(false);

    const location = useLocation();

    const isSelected = (path: string) => {
        if (path === "/admin") {
            return location.pathname === path;
        }

        return location.pathname.startsWith(path);
    };

    const handleCloseMobileMenu = () => {
        setMobileOpen(false);
    };

    const navigation = (
        <Box sx={{ p: 2 }}>
            <Stack
                direction="row"
                spacing={1.5}
                sx={{
                    alignItems: "center",
                    px: 1,
                    py: 1.5,
                }}
            >
                <Box
                    sx={{
                        width: 42,
                        height: 42,
                        borderRadius: 2,
                        bgcolor: "secondary.main",
                        color: "secondary.contrastText",
                        display: "grid",
                        placeItems: "center",
                    }}
                >
                    <AdminPanelSettings />
                </Box>

                <Box>
                    <Typography sx={{ fontWeight: 800 }}>
                        Administration
                    </Typography>

                    <Typography
                        variant="body2"
                        color="text.secondary"
                    >
                        B2B Commerce Demo
                    </Typography>
                </Box>
            </Stack>

            <Divider sx={{ my: 2 }} />

            <List disablePadding>
                {navigationItems.map((item) => (
                    <ListItemButton
                        key={item.path}
                        component={
                            item.available
                                ? Link
                                : "div"
                        }
                        to={
                            item.available
                                ? item.path
                                : undefined
                        }
                        selected={
                            item.available &&
                            isSelected(item.path)
                        }
                        disabled={!item.available}
                        onClick={() => {
                            if (item.available) {
                                handleCloseMobileMenu();
                            }
                        }}
                        sx={{
                            mb: 0.75,
                            borderRadius: 2,

                            "&.Mui-selected": {
                                bgcolor: "secondary.main",
                                color:
                                    "secondary.contrastText",

                                "&:hover": {
                                    bgcolor: "secondary.dark",
                                },

                                "& .MuiListItemIcon-root": {
                                    color: "inherit",
                                },
                            },
                        }}
                    >
                        <ListItemIcon
                            sx={{ minWidth: 42 }}
                        >
                            {item.icon}
                        </ListItemIcon>

                        <ListItemText
                            primary={item.label}
                        />

                        {!item.available && (
                            <Chip
                                label="Soon"
                                size="small"
                            />
                        )}
                    </ListItemButton>
                ))}
            </List>
        </Box>
    );

    return (
        <Box>
            <Button
                variant="outlined"
                startIcon={<MenuIcon />}
                onClick={() => setMobileOpen(true)}
                sx={{
                    display: {
                        xs: "inline-flex",
                        md: "none",
                    },
                    mb: 3,
                }}
            >
                Admin menu
            </Button>

            <Box
                sx={{
                    display: "grid",
                    gridTemplateColumns: {
                        xs: "minmax(0, 1fr)",
                        md: `${drawerWidth}px minmax(0, 1fr)`,
                    },
                    gap: {
                        xs: 0,
                        md: 4,
                    },
                    alignItems: "start",
                }}
            >
                <Paper
                    component="aside"
                    variant="outlined"
                    sx={{
                        display: {
                            xs: "none",
                            md: "block",
                        },
                        position: "sticky",
                        top: 96,
                        overflow: "hidden",
                    }}
                >
                    {navigation}
                </Paper>

                <Box sx={{ minWidth: 0 }}>
                    <Outlet />
                </Box>
            </Box>

            <Drawer
                anchor="left"
                open={mobileOpen}
                onClose={handleCloseMobileMenu}
                sx={{
                    display: {
                        xs: "block",
                        md: "none",
                    },

                    "& .MuiDrawer-paper": {
                        width: drawerWidth,
                    },
                }}
            >
                {navigation}
            </Drawer>
        </Box>
    );
};

export default AdminLayout;