import {
    Add,
    AddShoppingCart,
    Remove,
} from "@mui/icons-material";
import {
    Alert,
    Box,
    Button,
    Card,
    CardContent,
    Divider,
    IconButton,
    Snackbar,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../../../context/AuthContext";
import { useCart } from "../../../context/CartContext";
import type { Product } from "../../../api/productApi";
import ProductStockBadge from "../ProductStockBadge";
import { formatPrice } from "../../../utils/formatPrice";

type ProductPurchaseCardProps = {
    product: Product;
};

const ProductPurchaseCard = ({
    product,
}: ProductPurchaseCardProps) => {
    const [quantity, setQuantity] = useState(1);

    const [addingToCart, setAddingToCart] =
        useState(false);

    const [feedback, setFeedback] = useState<{
        severity: "success" | "error";
        message: string;
    } | null>(null);

    const navigate = useNavigate();

    const {
        isAuthenticated,
        user,
    } = useAuth();

    const {
        addItem,
    } = useCart();

    const maxQuantity =
        Math.max(0, product.availableStock);

    const isAvailable =
        maxQuantity > 0;

    const decreaseQuantity = () => {
        setQuantity((current) =>
            Math.max(1, current - 1)
        );
    };

    const increaseQuantity = () => {
        setQuantity((current) =>
            Math.min(maxQuantity, current + 1)
        );
    };

    const handleQuantityChange = (
        value: string
    ) => {
        if (value === "") {
            return;
        }

        const parsedValue = Number(value);

        if (!Number.isInteger(parsedValue)) {
            return;
        }

        setQuantity(
            Math.min(
                maxQuantity,
                Math.max(1, parsedValue)
            )
        );
    };

    const handleAddToCart = async () => {
        if (!isAuthenticated) {
            navigate("/login");
            return;
        }

        if (user?.role !== "User") {
            return;
        }

        try {
            setAddingToCart(true);

            const succeeded = await addItem(
                product.id,
                quantity
            );

            if (succeeded) {
                setFeedback({
                    severity: "success",
                    message:
                        `${quantity} × ${product.name} added to your cart.`,
                });
            } else {
                setFeedback({
                    severity: "error",
                    message:
                        "The product could not be added to your cart.",
                });
            }
        } finally {
            setAddingToCart(false);
        }
    };

    const isCustomerAccount =
        !isAuthenticated ||
        user?.role === "User";

    const canAddToCart =
        isAvailable &&
        isCustomerAccount &&
        !addingToCart;

    return (
        <Card
            variant="outlined"
            sx={{
                width: "100%",
                maxWidth: 430,
                mx: {
                    xs: "auto",
                    md: 0,
                },
                borderRadius: 4,
                bgcolor: "background.paper",
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
                    p: {
                        xs: 2.5,
                        sm: 3,
                    },
                    "&:last-child": {
                        pb: {
                            xs: 2.5,
                            sm: 3,
                        },
                    },
                }}
            >
                <Stack spacing={3}>
                    <Box>
                        <Typography
                            variant="body2"
                            color="text.secondary"
                            sx={{
                                mb: 0.5,
                                fontWeight: 500,
                            }}
                        >
                            Your price
                        </Typography>

                        <Typography
                            component="p"
                            sx={{
                                fontSize: {
                                    xs: "2.25rem",
                                    sm: "2.75rem",
                                },
                                lineHeight: 1.1,
                                fontWeight: 800,
                                letterSpacing: "-0.03em",
                            }}
                        >
                            {formatPrice(product.basePrice)}
                        </Typography>

                        <Typography
                            variant="caption"
                            color="text.secondary"
                            sx={{
                                display: "block",
                                mt: 1,
                            }}
                        >
                            Excluding VAT
                        </Typography>
                    </Box>

                    <Box
                        sx={{
                            display: "flex",
                            alignItems: "flex-start",
                        }}
                    >
                        <ProductStockBadge
                            availableStock={
                                product.availableStock
                            }
                            purchasedQuantity={
                                product.purchasedQuantity
                            }
                            expectedDeliveryDate={
                                product.expectedDeliveryDate
                            }
                        />
                    </Box>

                    {isAvailable && (
                        <>
                            <Divider />

                            <Box>
                                <Stack
                                    direction="row"
                                    sx={{
                                        mb: 1.25,
                                        alignItems: "center",
                                        justifyContent: "space-between",
                                        gap: 2,
                                    }}
                                >
                                    <Typography
                                        variant="body2"
                                        sx={{
                                            fontWeight: 700,
                                        }}
                                    >
                                        Quantity
                                    </Typography>

                                    <Typography
                                        variant="caption"
                                        color="text.secondary"
                                    >
                                        {maxQuantity} available
                                    </Typography>
                                </Stack>

                                <Stack
                                    direction="row"
                                    spacing={1.25}
                                    sx={{
                                        alignItems: "center",
                                    }}
                                >
                                    <IconButton
                                        onClick={decreaseQuantity}
                                        disabled={quantity <= 1}
                                        aria-label="Decrease quantity"
                                        sx={{
                                            width: 44,
                                            height: 44,
                                            border: "1px solid",
                                            borderColor: "divider",
                                            borderRadius: 2,
                                        }}
                                    >
                                        <Remove />
                                    </IconButton>

                                    <TextField
                                        value={quantity}
                                        onChange={(event) =>
                                            handleQuantityChange(
                                                event.target.value
                                            )
                                        }
                                        type="number"
                                        size="small"
                                        slotProps={{
                                            htmlInput: {
                                                min: 1,
                                                max: maxQuantity,
                                                "aria-label":
                                                    "Product quantity",
                                                style: {
                                                    textAlign: "center",
                                                },
                                            },
                                        }}
                                        sx={{
                                            width: 96,
                                            "& input": {
                                                height: 27,
                                                fontWeight: 600,
                                            },
                                            "& input::-webkit-outer-spin-button, & input::-webkit-inner-spin-button":
                                            {
                                                display: "none",
                                            },
                                        }}
                                    />

                                    <IconButton
                                        onClick={increaseQuantity}
                                        disabled={
                                            quantity >= maxQuantity
                                        }
                                        aria-label="Increase quantity"
                                        sx={{
                                            width: 44,
                                            height: 44,
                                            border: "1px solid",
                                            borderColor: "divider",
                                            borderRadius: 2,
                                        }}
                                    >
                                        <Add />
                                    </IconButton>
                                </Stack>
                            </Box>
                        </>
                    )}

                    <Button
                        variant="contained"
                        size="large"
                        startIcon={
                            canAddToCart
                                ? <AddShoppingCart />
                                : undefined
                        }
                        disabled={!canAddToCart}
                        onClick={() =>
                            void handleAddToCart()
                        }
                        sx={{
                            width: "100%",
                            maxWidth: 340,
                            alignSelf: "center",
                            borderRadius: 2.5,
                            py: 1.35,
                            fontWeight: 700,
                            textTransform: "none",

                            boxShadow: canAddToCart
                                ? "0 8px 18px rgba(15, 23, 42, 0.16)"
                                : "none",

                            "&:hover": {
                                boxShadow: canAddToCart
                                    ? "0 10px 22px rgba(15, 23, 42, 0.2)"
                                    : "none",
                            },
                        }}
                    >
                        {!isAvailable
                            ? "Out of stock"
                            : !isCustomerAccount
                                ? "Customer accounts only"
                                : addingToCart
                                    ? "Adding..."
                                    : "Add to cart"}
                    </Button>

                    {!isAvailable && (
                        <Typography
                            variant="body2"
                            color="text.secondary"
                            sx={{
                                textAlign: "center",
                            }}
                        >
                            This product is currently out of stock.
                        </Typography>
                    )}
                </Stack>
            </CardContent>

            <Snackbar
                open={feedback !== null}
                autoHideDuration={4500}
                onClose={() =>
                    setFeedback(null)
                }
                anchorOrigin={{
                    vertical: "bottom",
                    horizontal: "center",
                }}
            >
                <Alert
                    severity={
                        feedback?.severity ??
                        "success"
                    }
                    variant="filled"
                    onClose={() =>
                        setFeedback(null)
                    }
                    sx={{
                        width: "100%",
                    }}
                >
                    {feedback?.message}
                </Alert>
            </Snackbar>
        </Card>
    );
};

export default ProductPurchaseCard;