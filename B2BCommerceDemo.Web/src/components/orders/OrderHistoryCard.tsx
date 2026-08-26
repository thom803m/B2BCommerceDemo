import {
    ExpandLess,
    ExpandMore,
    Inventory2,
    ReceiptLong,
} from "@mui/icons-material";
import {
    Box,
    Button,
    Card,
    CardContent,
    Collapse,
    Divider,
    Stack,
    Typography,
} from "@mui/material";
import { useState, } from "react";
import type { Order, OrderItem, } from "../../api/orderApi";
import { formatPrice } from "../../utils/formatPrice";
import OrderStatusChip from "./OrderStatusChip";

type OrderHistoryCardProps = {
    order: Order;
};

const OrderHistoryCard = ({
    order,
}: OrderHistoryCardProps) => {
    const [detailsOpen, setDetailsOpen] =
        useState(false);

    const itemCount =
        order.items.reduce(
            (total, item) =>
                total + item.quantity,
            0
        );

    const detailsId =
        `order-${order.id}-details`;

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
                <Stack
                    direction={{
                        xs: "column",
                        md: "row",
                    }}
                    spacing={{
                        xs: 2,
                        md: 3,
                    }}
                    sx={{
                        alignItems: {
                            xs: "stretch",
                            md: "center",
                        },
                    }}
                >
                    <Box
                        sx={{
                            width: 52,
                            height: 52,
                            flexShrink: 0,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            bgcolor: "grey.100",
                            borderRadius: 2.5,
                        }}
                    >
                        <ReceiptLong
                            sx={{
                                color: "primary.main",
                            }}
                        />
                    </Box>

                    <Box
                        sx={{
                            minWidth: 0,
                            flexGrow: 1,
                        }}
                    >
                        <Stack
                            direction={{
                                xs: "column",
                                sm: "row",
                            }}
                            spacing={1}
                            sx={{
                                alignItems: {
                                    xs: "flex-start",
                                    sm: "center",
                                },
                            }}
                        >
                            <Typography
                                variant="h6"
                                component="h2"
                                sx={{
                                    fontWeight: 800,
                                }}
                            >
                                Order #{order.id}
                            </Typography>

                            <OrderStatusChip
                                status={order.status}
                            />
                        </Stack>

                        <Typography
                            variant="body2"
                            color="text.secondary"
                            sx={{
                                mt: 0.75,
                            }}
                        >
                            {formatDate(
                                order.createdAt
                            )}
                            {" · "}
                            {itemCount}{" "}
                            {itemCount === 1
                                ? "item"
                                : "items"}
                        </Typography>
                    </Box>

                    <Box
                        sx={{
                            minWidth: {
                                md: 150,
                            },
                            textAlign: {
                                xs: "left",
                                md: "right",
                            },
                        }}
                    >
                        <Typography
                            variant="body2"
                            color="text.secondary"
                        >
                            Order total
                        </Typography>

                        <Typography
                            variant="h6"
                            sx={{
                                mt: 0.25,
                                fontWeight: 800,
                            }}
                        >
                            {formatPrice(
                                order.total
                            )}
                        </Typography>
                    </Box>

                    <Button
                        variant="outlined"
                        endIcon={
                            detailsOpen
                                ? <ExpandLess />
                                : <ExpandMore />
                        }
                        aria-expanded={
                            detailsOpen
                        }
                        aria-controls={
                            detailsId
                        }
                        onClick={() =>
                            setDetailsOpen(
                                (current) =>
                                    !current
                            )
                        }
                        sx={{
                            flexShrink: 0,
                        }}
                    >
                        {detailsOpen
                            ? "Hide details"
                            : "View details"}
                    </Button>
                </Stack>

                <Collapse
                    in={detailsOpen}
                    timeout="auto"
                    unmountOnExit
                >
                    <Box
                        id={detailsId}
                        sx={{
                            mt: 3,
                        }}
                    >
                        <Divider />

                        <Typography
                            variant="subtitle1"
                            sx={{
                                mt: 3,
                                mb: 1,
                                fontWeight: 800,
                            }}
                        >
                            Products
                        </Typography>

                        <Stack
                            divider={
                                <Divider flexItem />
                            }
                        >
                            {order.items.map(
                                (item) => (
                                    <OrderItemRow
                                        key={
                                            item.productId
                                        }
                                        item={item}
                                    />
                                )
                            )}
                        </Stack>
                    </Box>
                </Collapse>
            </CardContent>
        </Card>
    );
};

type OrderItemRowProps = {
    item: OrderItem;
};

const OrderItemRow = ({
    item,
}: OrderItemRowProps) => {
    return (
        <Stack
            direction="row"
            spacing={2}
            sx={{
                py: 2,
                alignItems: "center",
            }}
        >
            <Box
                sx={{
                    width: 44,
                    height: 44,
                    flexShrink: 0,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    bgcolor: "grey.50",
                    border: "1px solid",
                    borderColor: "divider",
                    borderRadius: 2,
                }}
            >
                <Inventory2
                    sx={{
                        fontSize: 22,
                        color: "grey.400",
                    }}
                />
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
                    }}
                >
                    {item.productName ??
                        "Product"}
                </Typography>

                <Typography
                    variant="body2"
                    color="text.secondary"
                    sx={{
                        mt: 0.25,
                    }}
                >
                    {item.sku
                        ? `SKU: ${item.sku} · `
                        : ""}
                    Quantity: {item.quantity}
                    {" · "}
                    {formatPrice(
                        item.unitPrice
                    )}
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
                {formatPrice(
                    item.lineTotal
                )}
            </Typography>
        </Stack>
    );
};

const formatDate = (
    value: string
) => {
    return new Intl.DateTimeFormat(
        "en-DK",
        {
            dateStyle: "medium",
            timeStyle: "short",
        }
    ).format(new Date(value));
};

export default OrderHistoryCard;