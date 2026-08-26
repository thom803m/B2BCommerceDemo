import {
    ArrowBack,
    Business,
    CalendarToday,
    Save,
} from "@mui/icons-material";
import {
    Alert,
    Box,
    Button,
    Chip,
    Divider,
    Grid,
    MenuItem,
    Paper,
    Snackbar,
    Stack,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    TextField,
    Typography,
} from "@mui/material";
import { useCallback, useEffect, useState, } from "react";
import { Link, useLocation, useParams, } from "react-router-dom";
import {
    getAdminOrderById,
    updateOrderStatus,
    type Order,
    type OrderStatus,
    type RackbeatSyncStatus,
} from "../../../api/orderApi";
import AdminOrderStatusChip from "../../../components/admin/orders/AdminOrderStatusChip";
import LoadingSpinner from "../../../components/common/LoadingSpinner";
import PageHeader from "../../../components/common/PageHeader";
import { formatPrice } from "../../../utils/formatPrice";

const allowedStatusTransitions: Record<
    OrderStatus,
    OrderStatus[]
> = {
    Pending: [
        "Confirmed",
        "Cancelled",
    ],
    Confirmed: [
        "Processing",
        "Cancelled",
    ],
    Processing: [
        "Shipped",
    ],
    Shipped: [
        "Completed",
    ],
    Completed: [],
    Cancelled: [],
};

const formatOrderDate = (
    value: string
) => {
    const date = new Date(value);

    if (Number.isNaN(date.getTime())) {
        return "Unknown date";
    }

    return new Intl.DateTimeFormat(
        "en-GB",
        {
            dateStyle: "medium",
            timeStyle: "short",
        }
    ).format(date);
};

const getRackbeatChipColor = (
    status?: RackbeatSyncStatus | null
) => {
    switch (status) {
        case "Synced":
            return "success";

        case "Failed":
            return "error";

        case "Pending":
            return "warning";

        default:
            return "default";
    }
};

const AdminOrderDetailPage = () => {
    const { id } = useParams();

    const location = useLocation();

    const orderId = Number(id);

    const [order, setOrder] =
        useState<Order | null>(null);

    const [
        selectedStatus,
        setSelectedStatus,
    ] = useState<OrderStatus | "">("");

    const [loading, setLoading] =
        useState(true);

    const [updating, setUpdating] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    const [
        successMessage,
        setSuccessMessage,
    ] = useState<string | null>(null);

    const loadOrder = useCallback(async () => {
        if (
            !Number.isInteger(orderId) ||
            orderId <= 0
        ) {
            setError("The order ID is invalid.");
            setLoading(false);
            return;
        }

        setLoading(true);
        setError(null);

        try {
            const result =
                await getAdminOrderById(
                    orderId
                );

            setOrder(result);
            setSelectedStatus(
                result.status ?? ""
            );
        } catch (error) {
            console.error(
                "Failed to load admin order",
                error
            );

            setError(
                "The order could not be loaded. It may no longer exist."
            );
        } finally {
            setLoading(false);
        }
    }, [orderId]);

    useEffect(() => {
        void loadOrder();
    }, [loadOrder]);

    const currentStatus =
        order?.status ?? null;

    const availableStatuses =
        currentStatus
            ? allowedStatusTransitions[
            currentStatus
            ]
            : [];

    const handleUpdateStatus =
        async () => {
            if (
                !order ||
                !selectedStatus ||
                selectedStatus ===
                order.status
            ) {
                return;
            }

            setUpdating(true);
            setError(null);

            try {
                const updatedOrder =
                    await updateOrderStatus(
                        order.id,
                        selectedStatus
                    );

                setOrder(updatedOrder);

                setSelectedStatus(
                    updatedOrder.status ?? ""
                );

                setSuccessMessage(
                    `Order #${order.id} was updated to ${selectedStatus}.`
                );
            } catch (error) {
                console.error(
                    "Failed to update order status",
                    error
                );

                setError(
                    "The order status could not be updated. Please try again."
                );
            } finally {
                setUpdating(false);
            }
        };

    if (loading) {
        return (
            <LoadingSpinner text="Loading order..." />
        );
    }

    if (!order) {
        return (
            <Box>
                <PageHeader
                    title="Order not found"
                    action={
                        <Button
                            component={Link}
                            to="/admin/orders"
                            state={location.state}
                            variant="outlined"
                            startIcon={
                                <ArrowBack />
                            }
                        >
                            Back to orders
                        </Button>
                    }
                />

                <Alert severity="error">
                    {error ??
                        "The requested order could not be found."}
                </Alert>
            </Box>
        );
    }

    return (
        <Box>
            <PageHeader
                title={`Order #${order.id}`}
                subtitle="Review the order details and update its processing status."
                action={
                    <Button
                        component={Link}
                        to="/admin/orders"
                        state={location.state}
                        variant="outlined"
                        startIcon={<ArrowBack />}
                    >
                        Back to orders
                    </Button>
                }
            />

            {error && (
                <Alert
                    severity="error"
                    sx={{ mb: 3 }}
                >
                    {error}
                </Alert>
            )}

            <Grid container spacing={3}>
                <Grid
                    size={{
                        xs: 12,
                        lg: 7,
                    }}
                >
                    <Paper
                        variant="outlined"
                        sx={{ p: 3 }}
                    >
                        <Typography
                            variant="h6"
                            component="h2"
                            sx={{
                                mb: 3,
                                fontWeight: 800,
                            }}
                        >
                            Order information
                        </Typography>

                        <Stack spacing={2.5}>
                            <Stack
                                direction="row"
                                spacing={1.5}
                                sx={{
                                    alignItems:
                                        "center",
                                }}
                            >
                                <Business color="action" />

                                <Box>
                                    <Typography
                                        variant="body2"
                                        color="text.secondary"
                                    >
                                        Company
                                    </Typography>

                                    <Typography
                                        sx={{
                                            fontWeight: 700,
                                        }}
                                    >
                                        Company #
                                        {order.companyId}
                                    </Typography>
                                </Box>
                            </Stack>

                            <Stack
                                direction="row"
                                spacing={1.5}
                                sx={{
                                    alignItems:
                                        "center",
                                }}
                            >
                                <CalendarToday color="action" />

                                <Box>
                                    <Typography
                                        variant="body2"
                                        color="text.secondary"
                                    >
                                        Created
                                    </Typography>

                                    <Typography
                                        sx={{
                                            fontWeight: 700,
                                        }}
                                    >
                                        {formatOrderDate(
                                            order.createdAt
                                        )}
                                    </Typography>
                                </Box>
                            </Stack>

                            <Box>
                                <Typography
                                    variant="body2"
                                    color="text.secondary"
                                    sx={{ mb: 1 }}
                                >
                                    Current status
                                </Typography>

                                <AdminOrderStatusChip
                                    status={order.status}
                                />
                            </Box>
                        </Stack>
                    </Paper>
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        lg: 5,
                    }}
                >
                    <Paper
                        variant="outlined"
                        sx={{
                            p: 3,
                            height: "100%",
                        }}
                    >
                        <Typography
                            variant="h6"
                            component="h2"
                            sx={{
                                mb: 1,
                                fontWeight: 800,
                            }}
                        >
                            Update status
                        </Typography>

                        <Typography
                            color="text.secondary"
                            sx={{ mb: 3 }}
                        >
                            Move the order to its next
                            processing stage.
                        </Typography>

                        {availableStatuses.length >
                            0 ? (
                            <Stack spacing={2}>
                                <TextField
                                    select
                                    fullWidth
                                    label="New status"
                                    value={
                                        selectedStatus ===
                                            currentStatus
                                            ? ""
                                            : selectedStatus
                                    }
                                    onChange={(
                                        event
                                    ) =>
                                        setSelectedStatus(
                                            event.target
                                                .value as OrderStatus
                                        )
                                    }
                                    disabled={updating}
                                >
                                    <MenuItem
                                        value=""
                                        disabled
                                    >
                                        Select new status
                                    </MenuItem>

                                    {availableStatuses.map(
                                        (status) => (
                                            <MenuItem
                                                key={
                                                    status
                                                }
                                                value={
                                                    status
                                                }
                                            >
                                                {
                                                    status
                                                }
                                            </MenuItem>
                                        )
                                    )}
                                </TextField>

                                <Button
                                    variant="contained"
                                    startIcon={<Save />}
                                    onClick={() =>
                                        void handleUpdateStatus()
                                    }
                                    disabled={
                                        updating ||
                                        !selectedStatus ||
                                        selectedStatus ===
                                        currentStatus
                                    }
                                >
                                    {updating
                                        ? "Updating..."
                                        : "Update status"}
                                </Button>
                            </Stack>
                        ) : (
                            <Alert severity="info">
                                This order has reached a
                                final status and cannot be
                                changed.
                            </Alert>
                        )}
                    </Paper>
                </Grid>
            </Grid>

            <Paper
                variant="outlined"
                sx={{
                    mt: 3,
                    overflow: "hidden",
                }}
            >
                <Box sx={{ p: 3 }}>
                    <Typography
                        variant="h6"
                        component="h2"
                        sx={{ fontWeight: 800 }}
                    >
                        Rackbeat synchronization
                    </Typography>
                </Box>

                <Divider />

                <Stack
                    direction={{
                        xs: "column",
                        md: "row",
                    }}
                    spacing={3}
                    sx={{ p: 3 }}
                >
                    <Box sx={{ flex: 1 }}>
                        <Typography
                            variant="body2"
                            color="text.secondary"
                        >
                            Sync status
                        </Typography>

                        <Chip
                            label={
                                order.rackbeatSyncStatus ??
                                "Unknown"
                            }
                            color={getRackbeatChipColor(
                                order.rackbeatSyncStatus
                            )}
                            size="small"
                            sx={{
                                mt: 1,
                                fontWeight: 700,
                            }}
                        />
                    </Box>

                    <Box sx={{ flex: 1 }}>
                        <Typography
                            variant="body2"
                            color="text.secondary"
                        >
                            Rackbeat order number
                        </Typography>

                        <Typography
                            sx={{
                                mt: 1,
                                fontWeight: 700,
                            }}
                        >
                            {order.rackbeatOrderNumber ??
                                "Not assigned"}
                        </Typography>
                    </Box>

                    <Box sx={{ flex: 1 }}>
                        <Typography
                            variant="body2"
                            color="text.secondary"
                        >
                            Last synchronized
                        </Typography>

                        <Typography
                            sx={{
                                mt: 1,
                                fontWeight: 700,
                            }}
                        >
                            {order.rackbeatSyncedAt
                                ? formatOrderDate(
                                    order.rackbeatSyncedAt
                                )
                                : "Not synchronized"}
                        </Typography>
                    </Box>
                </Stack>

                {order.rackbeatSyncError && (
                    <Alert
                        severity="error"
                        sx={{
                            mx: 3,
                            mb: 3,
                        }}
                    >
                        {order.rackbeatSyncError}
                    </Alert>
                )}
            </Paper>

            <Paper
                variant="outlined"
                sx={{
                    mt: 3,
                    overflow: "hidden",
                }}
            >
                <Box sx={{ p: 3 }}>
                    <Typography
                        variant="h6"
                        component="h2"
                        sx={{ fontWeight: 800 }}
                    >
                        Order items
                    </Typography>
                </Box>

                <Divider />

                <TableContainer>
                    <Table sx={{ minWidth: 720 }}>
                        <TableHead>
                            <TableRow>
                                <TableCell>
                                    Product
                                </TableCell>

                                <TableCell>
                                    SKU
                                </TableCell>

                                <TableCell align="right">
                                    Quantity
                                </TableCell>

                                <TableCell align="right">
                                    Unit price
                                </TableCell>

                                <TableCell align="right">
                                    Line total
                                </TableCell>
                            </TableRow>
                        </TableHead>

                        <TableBody>
                            {order.items.map(
                                (item) => (
                                    <TableRow
                                        key={`${item.productId}-${item.sku}`}
                                    >
                                        <TableCell>
                                            {item.productName ??
                                                `Product #${item.productId}`}
                                        </TableCell>

                                        <TableCell>
                                            {item.sku ??
                                                "Unknown"}
                                        </TableCell>

                                        <TableCell align="right">
                                            {item.quantity}
                                        </TableCell>

                                        <TableCell align="right">
                                            {formatPrice(
                                                item.unitPrice
                                            )}
                                        </TableCell>

                                        <TableCell
                                            align="right"
                                            sx={{
                                                fontWeight: 700,
                                            }}
                                        >
                                            {formatPrice(
                                                item.lineTotal
                                            )}
                                        </TableCell>
                                    </TableRow>
                                )
                            )}

                            <TableRow>
                                <TableCell
                                    colSpan={4}
                                    align="right"
                                    sx={{
                                        fontWeight: 800,
                                        fontSize: "1rem",
                                    }}
                                >
                                    Order total
                                </TableCell>

                                <TableCell
                                    align="right"
                                    sx={{
                                        fontWeight: 800,
                                        fontSize: "1rem",
                                    }}
                                >
                                    {formatPrice(
                                        order.total
                                    )}
                                </TableCell>
                            </TableRow>
                        </TableBody>
                    </Table>
                </TableContainer>
            </Paper>

            <Snackbar
                open={successMessage !== null}
                autoHideDuration={5000}
                onClose={() =>
                    setSuccessMessage(null)
                }
                anchorOrigin={{
                    vertical: "bottom",
                    horizontal: "center",
                }}
            >
                <Alert
                    severity="success"
                    variant="filled"
                    onClose={() =>
                        setSuccessMessage(null)
                    }
                >
                    {successMessage}
                </Alert>
            </Snackbar>
        </Box>
    );
};

export default AdminOrderDetailPage;