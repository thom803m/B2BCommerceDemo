import {
    ShoppingCartOutlined,
} from "@mui/icons-material";

import {
    Alert,
    Box,
    Button,
    CircularProgress,
    Grid,
    Stack,
} from "@mui/material";

import { Link } from "react-router-dom";

import CartItemRow from
    "../../components/cart/CartItemRow";

import CartSummary from
    "../../components/cart/CartSummary";

import EmptyState from
    "../../components/common/EmptyState";

import PageHeader from
    "../../components/common/PageHeader";

import { useCart } from
    "../../context/CartContext";

const CartPage = () => {
    const {
        cart,
        itemCount,
        loading,
        error,
        refreshCart,
        updateItem,
        removeItem,
        clearError,
    } = useCart();

    if (loading && !cart) {
        return (
            <Box
                sx={{
                    py: 10,
                    display: "flex",
                    justifyContent: "center",
                }}
            >
                <CircularProgress
                    aria-label="Loading cart"
                />
            </Box>
        );
    }

    const isEmpty =
        !cart || cart.items.length === 0;

    const subtitle = isEmpty
        ? "Your selected products will appear here."
        : `${itemCount} ${itemCount === 1
            ? "item"
            : "items"
        } ready for checkout.`;

    return (
        <Box>
            <PageHeader
                title="Your cart"
                subtitle={subtitle}
            />

            {error && (
                <Alert
                    severity="error"
                    onClose={clearError}
                    action={
                        <Button
                            color="inherit"
                            size="small"
                            onClick={() =>
                                void refreshCart()
                            }
                        >
                            Try again
                        </Button>
                    }
                    sx={{
                        mb: 3,
                    }}
                >
                    {error}
                </Alert>
            )}

            {isEmpty ? (
                <EmptyState
                    title="Your cart is empty"
                    description={
                        "Browse the catalogue and add the products your company needs."
                    }
                    action={
                        <Button
                            component={Link}
                            to="/products"
                            variant="contained"
                            startIcon={
                                <ShoppingCartOutlined />
                            }
                        >
                            Browse products
                        </Button>
                    }
                />
            ) : (
                <Grid
                    container
                    spacing={{
                        xs: 3,
                        lg: 4,
                    }}
                    sx={{
                        alignItems: "flex-start",
                    }}
                >
                    <Grid
                        size={{
                            xs: 12,
                            lg: 8,
                        }}
                    >
                        <Stack spacing={2}>
                            {cart.items.map(
                                (item) => (
                                    <CartItemRow
                                        key={item.id}
                                        item={item}
                                        disabled={
                                            loading
                                        }
                                        onUpdateQuantity={
                                            updateItem
                                        }
                                        onRemove={
                                            removeItem
                                        }
                                    />
                                )
                            )}
                        </Stack>
                    </Grid>

                    <Grid
                        size={{
                            xs: 12,
                            lg: 4,
                        }}
                    >
                        <CartSummary
                            total={cart.total}
                            itemCount={itemCount}
                        />
                    </Grid>
                </Grid>
            )}
        </Box>
    );
};

export default CartPage;