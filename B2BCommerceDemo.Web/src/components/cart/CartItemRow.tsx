import {
    Add,
    DeleteOutlined,
    Inventory2,
    Remove,
} from "@mui/icons-material";
import {
    Box,
    Card,
    IconButton,
    Stack,
    Typography,
} from "@mui/material";
import { useState } from "react";
import { Link } from "react-router-dom";
import type { CartItem, } from "../../api/cartApi";
import { formatPrice } from "../../utils/formatPrice";

type CartItemRowProps = {
    item: CartItem;
    disabled?: boolean;

    onUpdateQuantity: (
        itemId: number,
        quantity: number
    ) => Promise<boolean>;

    onRemove: (
        itemId: number
    ) => Promise<boolean>;
};

const CartItemRow = ({
    item,
    disabled = false,
    onUpdateQuantity,
    onRemove,
}: CartItemRowProps) => {
    const [imageError, setImageError] =
        useState(false);

    const showImage =
        Boolean(item.imageUrl) && !imageError;

    const decreaseQuantity = async () => {
        if (item.quantity <= 1) {
            await onRemove(item.id);
            return;
        }

        await onUpdateQuantity(
            item.id,
            item.quantity - 1
        );
    };

    const increaseQuantity = async () => {
        await onUpdateQuantity(
            item.id,
            item.quantity + 1
        );
    };

    const removeItem = async () => {
        await onRemove(item.id);
    };

    return (
        <Card
            variant="outlined"
            sx={{
                p: {
                    xs: 2,
                    sm: 2.5,
                },
                borderRadius: 3,
            }}
        >
            <Stack
                direction={{
                    xs: "column",
                    sm: "row",
                }}
                spacing={{
                    xs: 2,
                    sm: 2.5,
                }}
                sx={{
                    alignItems: {
                        xs: "stretch",
                        sm: "center",
                    },
                }}
            >
                <Box
                    component={Link}
                    to={`/products/${item.productId}`}
                    sx={{
                        width: {
                            xs: "100%",
                            sm: 112,
                        },
                        height: {
                            xs: 180,
                            sm: 112,
                        },
                        flexShrink: 0,
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        bgcolor: "grey.50",
                        border: "1px solid",
                        borderColor: "divider",
                        borderRadius: 2.5,
                        overflow: "hidden",
                    }}
                >
                    {showImage ? (
                        <Box
                            component="img"
                            src={item.imageUrl!}
                            alt={item.productName}
                            onError={() =>
                                setImageError(true)
                            }
                            sx={{
                                width: "100%",
                                height: "100%",
                                objectFit: "contain",
                                p: 1.5,
                            }}
                        />
                    ) : (
                        <Inventory2
                            sx={{
                                fontSize: 44,
                                color: "grey.400",
                            }}
                        />
                    )}
                </Box>

                <Box
                    sx={{
                        minWidth: 0,
                        flexGrow: 1,
                    }}
                >
                    <Typography
                        component={Link}
                        to={`/products/${item.productId}`}
                        variant="h6"
                        sx={{
                            display: "inline-block",
                            color: "text.primary",
                            textDecoration: "none",
                            fontWeight: 750,
                            lineHeight: 1.35,

                            "&:hover": {
                                color: "secondary.main",
                            },
                        }}
                    >
                        {item.productName}
                    </Typography>

                    <Typography
                        color="text.secondary"
                        sx={{
                            mt: 0.75,
                        }}
                    >
                        {formatPrice(item.unitPrice)}
                        {" per item"}
                    </Typography>

                    <Stack
                        direction="row"
                        spacing={1}
                        sx={{
                            mt: 2,
                            alignItems: "center",
                        }}
                    >
                        <IconButton
                            onClick={() =>
                                void decreaseQuantity()
                            }
                            disabled={disabled}
                            aria-label={
                                item.quantity === 1
                                    ? `Remove ${item.productName}`
                                    : `Decrease quantity of ${item.productName}`
                            }
                            size="small"
                            sx={quantityButtonStyles}
                        >
                            <Remove fontSize="small" />
                        </IconButton>

                        <Typography
                            aria-label={
                                `Quantity: ${item.quantity}`
                            }
                            sx={{
                                minWidth: 36,
                                textAlign: "center",
                                fontWeight: 700,
                            }}
                        >
                            {item.quantity}
                        </Typography>

                        <IconButton
                            onClick={() =>
                                void increaseQuantity()
                            }
                            disabled={disabled}
                            aria-label={
                                `Increase quantity of ${item.productName}`
                            }
                            size="small"
                            sx={quantityButtonStyles}
                        >
                            <Add fontSize="small" />
                        </IconButton>

                        <IconButton
                            onClick={() =>
                                void removeItem()
                            }
                            disabled={disabled}
                            aria-label={
                                `Remove ${item.productName} from cart`
                            }
                            size="small"
                            color="error"
                            sx={{
                                ml: "8px !important",
                            }}
                        >
                            <DeleteOutlined fontSize="small" />
                        </IconButton>
                    </Stack>
                </Box>

                <Box
                    sx={{
                        minWidth: {
                            sm: 130,
                        },
                        textAlign: {
                            xs: "left",
                            sm: "right",
                        },
                    }}
                >
                    <Typography
                        variant="body2"
                        color="text.secondary"
                    >
                        Line total
                    </Typography>

                    <Typography
                        variant="h6"
                        sx={{
                            mt: 0.25,
                            fontWeight: 800,
                        }}
                    >
                        {formatPrice(item.total)}
                    </Typography>
                </Box>
            </Stack>
        </Card>
    );
};

const quantityButtonStyles = {
    width: 36,
    height: 36,
    border: "1px solid",
    borderColor: "divider",
    borderRadius: 2,
};

export default CartItemRow;