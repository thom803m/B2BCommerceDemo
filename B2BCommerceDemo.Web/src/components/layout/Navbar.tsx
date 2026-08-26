import { useState } from "react";
import { AppBar, Badge, Box, Button, IconButton, Menu, MenuItem, Stack, Toolbar, Typography, } from "@mui/material";
import { AdminPanelSettings, Inventory2, Login, LockResetOutlined, Logout, Menu as MenuIcon, ContactSupportOutlined, ReceiptLong, ShoppingCart, } from "@mui/icons-material";
import { Link } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { useCart, } from "../../context/CartContext";

const Navbar = () => {
    const { isAuthenticated, isAdmin, logout } = useAuth();

    const { itemCount, } = useCart();

    const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

    const isMenuOpen = Boolean(anchorEl);

    const handleMenuOpen = (event: React.MouseEvent<HTMLElement>) => {
        setAnchorEl(event.currentTarget);
    };

    const handleMenuClose = () => {
        setAnchorEl(null);
    };

    const handleLogout = () => {
        handleMenuClose();
        logout();
    };

    return (
        <AppBar
            position="sticky"
            elevation={0}
            sx={{
                top: 0,
                zIndex: (theme) => theme.zIndex.appBar,
                bgcolor: "primary.main",
                borderBottom: "1px solid rgba(255,255,255,0.1)",
            }}
        >
            <Toolbar
                sx={{
                    minHeight: { xs: 64, md: 72 },
                    gap: 2,
                    px: { xs: 2, sm: 3 },
                }}
            >
                <Typography
                    component={Link}
                    to="/"
                    variant="h6"
                    sx={{
                        mr: "auto",
                        color: "white",
                        textDecoration: "none",
                        fontWeight: 800,
                        letterSpacing: 1,
                    }}
                >
                    B2B COMMERCE DEMO
                </Typography>

                <Stack
                    direction="row"
                    spacing={1}
                    sx={{
                        alignItems: "center",
                        display: { xs: "none", lg: "flex" },
                    }}
                >
                    <Button
                        component={Link}
                        to="/products"
                        color="inherit"
                        startIcon={<Inventory2 />}
                    >
                        Products
                    </Button>

                    {isAuthenticated && !isAdmin && (
                        <Button
                            component={Link}
                            to="/cart"
                            color="inherit"
                            aria-label={
                                `Cart with ${itemCount} ${itemCount === 1
                                    ? "item"
                                    : "items"
                                }`
                            }
                            startIcon={
                                <Badge
                                    badgeContent={itemCount}
                                    color="secondary"
                                    max={99}
                                >
                                    <ShoppingCart />
                                </Badge>
                            }
                        >
                            Cart
                        </Button>
                    )}

                    {isAuthenticated && !isAdmin && (
                        <Button
                            component={Link}
                            to="/orders"
                            color="inherit"
                            startIcon={<ReceiptLong />}
                        >
                            My orders
                        </Button>
                    )}

                    {isAdmin && (
                        <Button
                            component={Link}
                            to="/admin"
                            color="inherit"
                            startIcon={<AdminPanelSettings />}
                        >
                            Admin
                        </Button>
                    )}

                    <Button
                        component={Link}
                        to="/contact"
                        color="inherit"
                        startIcon={<ContactSupportOutlined />}
                    >
                        Contact
                    </Button>

                    {isAuthenticated && (
                        <Button
                            component={Link}
                            to="/change-password"
                            color="inherit"
                            startIcon={
                                <LockResetOutlined />
                            }
                        >
                            Change password
                        </Button>
                    )}

                    {isAuthenticated ? (
                        <Button
                            onClick={logout}
                            color="inherit"
                            startIcon={<Logout />}
                        >
                            Logout
                        </Button>
                    ) : (
                        <Button
                            component={Link}
                            to="/login"
                            color="inherit"
                            startIcon={<Login />}
                        >
                            Login
                        </Button>
                    )}
                </Stack>

                <Box sx={{ display: { xs: "block", lg: "none" } }}>
                    <IconButton
                        onClick={handleMenuOpen}
                        color="inherit"
                        aria-label="Open navigation menu"
                        aria-controls={
                            isMenuOpen ? "mobile-navigation-menu" : undefined
                        }
                        aria-haspopup="true"
                        aria-expanded={isMenuOpen ? "true" : undefined}
                    >
                        <MenuIcon />
                    </IconButton>

                    <Menu
                        id="mobile-navigation-menu"
                        anchorEl={anchorEl}
                        open={isMenuOpen}
                        onClose={handleMenuClose}
                        anchorOrigin={{
                            vertical: "bottom",
                            horizontal: "right",
                        }}
                        transformOrigin={{
                            vertical: "top",
                            horizontal: "right",
                        }}
                        slotProps={{
                            paper: {
                                sx: {
                                    mt: 1,
                                    minWidth: 220,
                                    borderRadius: 2,
                                },
                            },
                        }}
                    >
                        <MenuItem
                            component={Link}
                            to="/products"
                            onClick={handleMenuClose}
                        >
                            <Inventory2 sx={{ mr: 1.5 }} />
                            Products
                        </MenuItem>

                        {isAuthenticated && !isAdmin && (
                            <MenuItem
                                component={Link}
                                to="/cart"
                                onClick={handleMenuClose}
                                aria-label={
                                    `Cart with ${itemCount} ${itemCount === 1
                                        ? "item"
                                        : "items"
                                    }`
                                }
                            >
                                <Badge
                                    badgeContent={itemCount}
                                    color="secondary"
                                    max={99}
                                    sx={{
                                        mr: 1.5,
                                    }}
                                >
                                    <ShoppingCart />
                                </Badge>

                                Cart
                            </MenuItem>
                        )}

                        {isAuthenticated && !isAdmin && (
                            <MenuItem
                                component={Link}
                                to="/orders"
                                onClick={handleMenuClose}
                            >
                                <ReceiptLong
                                    sx={{
                                        mr: 1.5,
                                    }}
                                />

                                My orders
                            </MenuItem>
                        )}

                        {isAdmin && (
                            <MenuItem
                                component={Link}
                                to="/admin"
                                onClick={handleMenuClose}
                            >
                                <AdminPanelSettings sx={{ mr: 1.5 }} />
                                Admin
                            </MenuItem>
                        )}

                        <MenuItem
                            component={Link}
                            to="/contact"
                            onClick={handleMenuClose}
                        >
                            <ContactSupportOutlined
                                sx={{ mr: 1.5 }}
                            />

                            Contact
                        </MenuItem>

                        {isAuthenticated && (
                            <MenuItem
                                component={Link}
                                to="/change-password"
                                onClick={handleMenuClose}
                            >
                                <LockResetOutlined
                                    sx={{ mr: 1.5 }}
                                />

                                Change password
                            </MenuItem>
                        )}

                        {isAuthenticated ? (
                            <MenuItem onClick={handleLogout}>
                                <Logout sx={{ mr: 1.5 }} />
                                Logout
                            </MenuItem>
                        ) : (
                            <MenuItem
                                component={Link}
                                to="/login"
                                onClick={handleMenuClose}
                            >
                                <Login sx={{ mr: 1.5 }} />
                                Login
                            </MenuItem>
                        )}
                    </Menu>
                </Box>
            </Toolbar>
        </AppBar>
    );
};

export default Navbar;