import { Alert, Button, Portal, Snackbar, Stack, } from "@mui/material";
import { ArrowForward, ShoppingCart, } from "@mui/icons-material";
import { useState } from "react";
import { Link, useLocation, useNavigate, } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import { useCart } from "../../context/CartContext";

type ProductActionsProps = {
    productId: number;
    productName: string;
    disabled?: boolean;
};

const ProductActions = ({
    productId,
    productName,
    disabled = false,
}: ProductActionsProps) => {
    const [addingToCart, setAddingToCart] =
        useState(false);

    const [feedback, setFeedback] = useState<{
        severity: "success" | "error";
        message: string;
    } | null>(null);

    const location = useLocation();

    const productListUrl =
        `${location.pathname}${location.search}`;

    const navigate = useNavigate();

    const {
        isAuthenticated,
        user,
    } = useAuth();

    const {
        addItem,
    } = useCart();

    const isCustomerAccount =
        !isAuthenticated ||
        user?.role === "User";

    const canAddToCart =
        !disabled &&
        isCustomerAccount &&
        !addingToCart;

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
                productId,
                1
            );

            if (succeeded) {
                setFeedback({
                    severity: "success",
                    message: `1 × ${productName} added to your cart.`,
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

    const buttonText = disabled
        ? "Out of stock"
        : !isCustomerAccount
            ? "Customer accounts only"
            : addingToCart
                ? "Adding..."
                : "Add to cart";

    return (
        <>
            <Stack
                direction="column"
                spacing={1}
                sx={{ mt: 1 }}
            >
                <Button
                    component={Link}
                    to={`/products/${productId}`}
                    state={{ productListUrl }}
                    variant="outlined"
                    endIcon={<ArrowForward />}
                    sx={{
                        minWidth: { sm: 100 },
                        whiteSpace: "nowrap",
                        flexShrink: 0,
                    }}
                >
                    View
                </Button>

                <Button
                    variant="contained"
                    disabled={!canAddToCart}
                    startIcon={
                        canAddToCart
                            ? <ShoppingCart />
                            : undefined
                    }
                    onClick={() =>
                        void handleAddToCart()
                    }
                    sx={{
                        flexGrow: 1,
                        whiteSpace: "nowrap",
                        fontSize: !isCustomerAccount
                            ? "0.75rem"
                            : undefined,
                        px: !isCustomerAccount
                            ? 1
                            : 2,
                    }}
                >
                    {buttonText}
                </Button>
            </Stack>

            <Portal>
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
                        sx={{ width: "100%" }}
                    >
                        {feedback?.message}
                    </Alert>
                </Snackbar>
            </Portal>
        </>
    );
};

export default ProductActions;