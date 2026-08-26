import {
    Refresh,
    ShoppingCartOutlined,
} from "@mui/icons-material";

import {
    Alert,
    Box,
    Button,
    Stack,
} from "@mui/material";

import {
    useCallback,
    useEffect,
    useState,
} from "react";

import {
    Link,
} from "react-router-dom";

import {
    getOrders,
    type Order,
} from "../../api/orderApi";

import EmptyState from
    "../../components/common/EmptyState";

import LoadingSpinner from
    "../../components/common/LoadingSpinner";

import PageHeader from
    "../../components/common/PageHeader";

import OrderHistoryCard from
    "../../components/orders/OrderHistoryCard";

const OrderHistoryPage = () => {
    const [orders, setOrders] =
        useState<Order[]>([]);

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    const loadOrders =
        useCallback(async () => {
            try {
                setLoading(true);
                setError(null);

                const orderData =
                    await getOrders();

                setOrders(orderData);
            } catch {
                setError(
                    "Your orders could not be loaded. Please try again."
                );
            } finally {
                setLoading(false);
            }
        }, []);

    useEffect(() => {
        void loadOrders();
    }, [loadOrders]);

    const hasOrders =
        orders.length > 0;

    return (
        <Box>
            <PageHeader
                title="Order history"
                subtitle={
                    "View your previous orders and follow their current status."
                }
                action={
                    hasOrders ? (
                        <Button
                            variant="outlined"
                            startIcon={<Refresh />}
                            disabled={loading}
                            onClick={() =>
                                void loadOrders()
                            }
                        >
                            {loading
                                ? "Refreshing..."
                                : "Refresh"}
                        </Button>
                    ) : undefined
                }
            />

            {error && (
                <Alert
                    severity="error"
                    action={
                        <Button
                            color="inherit"
                            size="small"
                            onClick={() =>
                                void loadOrders()
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

            {loading && !hasOrders ? (
                <LoadingSpinner
                    text="Loading orders..."
                />
            ) : !hasOrders && !error ? (
                <EmptyState
                    title="No orders yet"
                    description={
                        "Orders placed through the webshop will appear here."
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
                <Stack spacing={2}>
                    {orders.map((order) => (
                        <OrderHistoryCard
                            key={order.id}
                            order={order}
                        />
                    ))}
                </Stack>
            )}
        </Box>
    );
};

export default OrderHistoryPage;