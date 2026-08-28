import {
    ArrowBack,
    CheckCircle,
    LockOutlined,
    ReceiptLong,
    ShoppingCartOutlined,
} from "@mui/icons-material";
import {
    Alert,
    Box,
    Button,
    Card,
    CardContent,
    CircularProgress,
    Grid,
    Stack,
    Typography,
} from "@mui/material";
import { useRef, useState, } from "react";
import { Link, } from "react-router-dom";
import { createOrder, type Order, } from "../../api/orderApi";
import CheckoutSummary from "../../components/checkout/CheckoutSummary";
import EmptyState from "../../components/common/EmptyState";
import PageHeader from "../../components/common/PageHeader";
import { useAuth, } from "../../context/AuthContext";
import { useCart, } from "../../context/CartContext";
import { formatPrice } from "../../utils/formatPrice";

const CheckoutPage = () => {
    const {
        cart,
        loading: cartLoading,
        refreshCart,
    } = useCart();

    const {
        user,
    } = useAuth();

    const [placingOrder, setPlacingOrder] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    const [createdOrder, setCreatedOrder] =
        useState<Order | null>(null);

    const idempotencyKey = useRef(
        crypto.randomUUID()
    );

    const handlePlaceOrder = async () => {
        if (
            placingOrder ||
            !cart ||
            cart.items.length === 0
        ) {
            return;
        }

        try {
            setPlacingOrder(true);
            setError(null);

            const order = await createOrder(
                idempotencyKey.current
            );

            setCreatedOrder(order);

            await refreshCart();
        } catch {
            setError(
                "The order could not be created. Please try again."
            );
        } finally {
            setPlacingOrder(false);
        }
    };

    if (cartLoading && !cart && !createdOrder) {
        return (
            <Box
                sx={{
                    py: 10,
                    display: "flex",
                    justifyContent: "center",
                }}
            >
                <CircularProgress
                    aria-label="Loading checkout"
                />
            </Box>
        );
    }

    if (createdOrder) {
        return (
            <OrderConfirmation
                order={createdOrder}
            />
        );
    }

    const isEmpty =
        !cart || cart.items.length === 0;

    if (isEmpty) {
        return (
            <Box>
                <PageHeader
                    title="Checkout"
                    subtitle={
                        "Review and confirm your order."
                    }
                />

                <EmptyState
                    title="Your cart is empty"
                    description={
                        "Add products to your cart before proceeding to checkout."
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
            </Box>
        );
    }

    return (
        <Box>
            <PageHeader
                title="Checkout"
                subtitle={
                    "Review your products before placing the order."
                }
                action={
                    <Button
                        component={Link}
                        to="/cart"
                        startIcon={<ArrowBack />}
                    >
                        Back to cart
                    </Button>
                }
            />

            {error && (
                <Alert
                    severity="error"
                    onClose={() =>
                        setError(null)
                    }
                    sx={{
                        mb: 3,
                    }}
                >
                    {error}
                </Alert>
            )}

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
                    <CheckoutSummary
                        cart={cart}
                    />
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        lg: 4,
                    }}
                >
                    <Card
                        variant="outlined"
                        sx={{
                            borderRadius: 3,
                            position: {
                                xs: "static",
                                lg: "sticky",
                            },
                            top: {
                                lg: 104,
                            },
                        }}
                    >
                        <CardContent
                            sx={{
                                p: 3,

                                "&:last-child": {
                                    pb: 3,
                                },
                            }}
                        >
                            <Typography
                                variant="h5"
                                component="h2"
                                sx={{
                                    fontWeight: 800,
                                }}
                            >
                                Confirm order
                            </Typography>

                            <Stack
                                spacing={2.5}
                                sx={{
                                    mt: 3,
                                }}
                            >
                                <Box>
                                    <Typography
                                        variant="body2"
                                        color="text.secondary"
                                    >
                                        Ordering as
                                    </Typography>

                                    <Typography
                                        sx={{
                                            mt: 0.5,
                                            fontWeight: 700,
                                            overflowWrap:
                                                "anywhere",
                                        }}
                                    >
                                        {user?.email ??
                                            "Business customer"}
                                    </Typography>
                                </Box>

                                <Alert
                                    severity="info"
                                    icon={
                                        <LockOutlined />
                                    }
                                >
                                    Your order will be
                                    created using your
                                    company account and
                                    current company prices.
                                </Alert>

                                <Box>
                                    <Typography
                                        variant="body2"
                                        color="text.secondary"
                                    >
                                        Total excluding VAT
                                    </Typography>

                                    <Typography
                                        variant="h4"
                                        sx={{
                                            mt: 0.5,
                                            fontWeight: 800,
                                        }}
                                    >
                                        {formatPrice(
                                            cart.total
                                        )}
                                    </Typography>
                                </Box>

                                <Button
                                    variant="contained"
                                    size="large"
                                    disabled={
                                        placingOrder
                                    }
                                    onClick={() =>
                                        void handlePlaceOrder()
                                    }
                                    sx={{
                                        py: 1.4,
                                        borderRadius: 2.5,
                                    }}
                                >
                                    {placingOrder
                                        ? "Placing order..."
                                        : "Place order"}
                                </Button>

                                <Box
                                    sx={{
                                        display: "flex",
                                        alignItems:
                                            "center",
                                        justifyContent:
                                            "center",
                                        gap: 0.75,
                                        color:
                                            "text.secondary",
                                    }}
                                >
                                    <LockOutlined
                                        sx={{
                                            fontSize: 17,
                                        }}
                                    />

                                    <Typography
                                        variant="caption"
                                        sx={{
                                            textAlign:
                                                "center",
                                        }}
                                    >
                                        The order can only
                                        be submitted once.
                                    </Typography>
                                </Box>
                            </Stack>
                        </CardContent>
                    </Card>
                </Grid>
            </Grid>
        </Box>
    );
};

type OrderConfirmationProps = {
    order: Order;
};

const OrderConfirmation = ({
    order,
}: OrderConfirmationProps) => {
    return (
        <Box
            sx={{
                maxWidth: 760,
                mx: "auto",
                py: {
                    xs: 3,
                    md: 6,
                },
            }}
        >
            <Card
                variant="outlined"
                sx={{
                    borderRadius: 4,
                    textAlign: "center",
                }}
            >
                <CardContent
                    sx={{
                        p: {
                            xs: 3,
                            sm: 5,
                        },

                        "&:last-child": {
                            pb: {
                                xs: 3,
                                sm: 5,
                            },
                        },
                    }}
                >
                    <CheckCircle
                        color="success"
                        sx={{
                            fontSize: 72,
                        }}
                    />

                    <Typography
                        variant="h3"
                        component="h1"
                        sx={{
                            mt: 2,
                            fontWeight: 800,
                        }}
                    >
                        Order received
                    </Typography>

                    <Typography
                        color="text.secondary"
                        sx={{
                            mt: 1.5,
                        }}
                    >
                        Thank you. Your order has been
                        created successfully.
                    </Typography>

                    <Box
                        sx={{
                            mt: 4,
                            p: 3,
                            bgcolor: "grey.50",
                            borderRadius: 3,
                        }}
                    >
                        <Stack spacing={1.5}>
                            <Stack
                                direction="row"
                                sx={{
                                    justifyContent:
                                        "space-between",
                                    gap: 2,
                                }}
                            >
                                <Typography
                                    color="text.secondary"
                                >
                                    Order number
                                </Typography>

                                <Typography
                                    sx={{
                                        fontWeight: 750,
                                    }}
                                >
                                    #{order.id}
                                </Typography>
                            </Stack>

                            <Stack
                                direction="row"
                                sx={{
                                    justifyContent:
                                        "space-between",
                                    gap: 2,
                                }}
                            >
                                <Typography
                                    color="text.secondary"
                                >
                                    Status
                                </Typography>

                                <Typography
                                    sx={{
                                        fontWeight: 750,
                                    }}
                                >
                                    {order.status ??
                                        "Pending"}
                                </Typography>
                            </Stack>

                            <Stack
                                direction="row"
                                sx={{
                                    justifyContent:
                                        "space-between",
                                    gap: 2,
                                }}
                            >
                                <Typography
                                    color="text.secondary"
                                >
                                    Total
                                </Typography>

                                <Typography
                                    sx={{
                                        fontWeight: 750,
                                    }}
                                >
                                    {formatPrice(
                                        order.total
                                    )}
                                </Typography>
                            </Stack>
                        </Stack>
                    </Box>

                    <Stack
                        direction={{
                            xs: "column",
                            sm: "row",
                        }}
                        spacing={1.5}
                        sx={{
                            mt: 4,
                            justifyContent: "center",
                        }}
                    >
                        <Button
                            component={Link}
                            to="/orders"
                            variant="contained"
                            startIcon={<ReceiptLong />}
                        >
                            View my orders
                        </Button>

                        <Button
                            component={Link}
                            to="/products"
                            variant="outlined"
                        >
                            Continue shopping
                        </Button>

                        <Button
                            component={Link}
                            to="/"
                            variant="text"
                        >
                            Back to home
                        </Button>
                    </Stack>
                </CardContent>
            </Card>
        </Box>
    );
};

export default CheckoutPage;