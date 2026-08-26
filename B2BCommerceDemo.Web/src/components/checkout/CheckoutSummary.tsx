import { Inventory2, } from "@mui/icons-material";
import {
    Box,
    Card,
    CardContent,
    Divider,
    Stack,
    Typography,
} from "@mui/material";
import { useState } from "react";
import type { Cart, CartItem, } from "../../api/cartApi";
import { formatPrice } from "../../utils/formatPrice";

type CheckoutSummaryProps = {
    cart: Cart;
};

const CheckoutSummary = ({
    cart,
}: CheckoutSummaryProps) => {
    const itemCount =
        cart.items.reduce(
            (total, item) =>
                total + item.quantity,
            0
        );

    return (
        <Card
            variant="outlined"
            sx={{
                borderRadius: 3,
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
                <Typography
                    variant="h5"
                    component="h2"
                    sx={{
                        fontWeight: 800,
                    }}
                >
                    Review your order
                </Typography>

                <Typography
                    color="text.secondary"
                    sx={{
                        mt: 0.75,
                    }}
                >
                    {itemCount}{" "}
                    {itemCount === 1
                        ? "item"
                        : "items"}
                </Typography>

                <Stack
                    divider={<Divider flexItem />}
                    spacing={0}
                    sx={{
                        mt: 3,
                    }}
                >
                    {cart.items.map((item) => (
                        <CheckoutItem
                            key={item.id}
                            item={item}
                        />
                    ))}
                </Stack>

                <Divider sx={{ my: 3 }} />

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
                            Subtotal
                        </Typography>

                        <Typography
                            sx={{
                                fontWeight: 650,
                            }}
                        >
                            {formatPrice(cart.total)}
                        </Typography>
                    </Stack>

                    <Divider />

                    <Stack
                        direction="row"
                        sx={{
                            justifyContent:
                                "space-between",
                            alignItems: "center",
                            gap: 2,
                        }}
                    >
                        <Typography
                            variant="h6"
                            sx={{
                                fontWeight: 800,
                            }}
                        >
                            Order total
                        </Typography>

                        <Typography
                            variant="h5"
                            sx={{
                                fontWeight: 800,
                            }}
                        >
                            {formatPrice(cart.total)}
                        </Typography>
                    </Stack>

                    <Typography
                        variant="body2"
                        color="text.secondary"
                    >
                        Prices are shown excluding VAT.
                    </Typography>
                </Stack>
            </CardContent>
        </Card>
    );
};

type CheckoutItemProps = {
    item: CartItem;
};

const CheckoutItem = ({
    item,
}: CheckoutItemProps) => {
    const [imageError, setImageError] =
        useState(false);

    const showImage =
        Boolean(item.imageUrl) && !imageError;

    return (
        <Stack
            direction="row"
            spacing={2}
            sx={{
                py: 2.5,
                alignItems: "center",

                "&:first-of-type": {
                    pt: 0,
                },

                "&:last-of-type": {
                    pb: 0,
                },
            }}
        >
            <Box
                sx={{
                    width: {
                        xs: 64,
                        sm: 80,
                    },
                    height: {
                        xs: 64,
                        sm: 80,
                    },
                    flexShrink: 0,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    bgcolor: "grey.50",
                    border: "1px solid",
                    borderColor: "divider",
                    borderRadius: 2,
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
                            p: 1,
                        }}
                    />
                ) : (
                    <Inventory2
                        sx={{
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
                    sx={{
                        fontWeight: 700,
                        lineHeight: 1.35,
                    }}
                >
                    {item.productName}
                </Typography>

                <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{
                        mt: 0.5,
                    }}
                >
                    Quantity: {item.quantity}
                    {" · "}
                    {formatPrice(item.unitPrice)}
                    {" per item"}
                </Typography>
            </Box>

            <Typography
                sx={{
                    flexShrink: 0,
                    textAlign: "right",
                    fontWeight: 750,
                }}
            >
                {formatPrice(item.total)}
            </Typography>
        </Stack>
    );
};

export default CheckoutSummary;