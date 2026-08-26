import { ReceiptLong, Refresh, RestartAlt, Visibility, } from "@mui/icons-material";
import { Alert, Box, Button, Pagination, Paper, Stack, Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography, } from "@mui/material";
import { useCallback, useEffect, useState, } from "react";
import { Link, useLocation, } from "react-router-dom";
import { getAdminOrders, type AdminOrderListItem, } from "../../../api/orderApi";
import AdminOrderFilters, { type AdminOrderFilterValues, } from "../../../components/admin/orders/AdminOrderFilters";
import AdminOrderStatusChip from "../../../components/admin/orders/AdminOrderStatusChip";
import EmptyState from "../../../components/common/EmptyState";
import LoadingSpinner from "../../../components/common/LoadingSpinner";
import PageHeader from "../../../components/common/PageHeader";
import { formatPrice } from "../../../utils/formatPrice";

const pageSize = 20;

const defaultFilters: AdminOrderFilterValues = {
    status: "",
    companyId: "",
    fromDate: "",
    toDate: "",
};

type AdminOrdersLocationState = {
    filters?: AdminOrderFilterValues;
    page?: number;
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

const AdminOrdersPage = () => {
    const location = useLocation();

    const restoredState =
        location.state as
        | AdminOrdersLocationState
        | null;

    const [orders, setOrders] =
        useState<AdminOrderListItem[]>([]);

    const [page, setPage] =
        useState(() => {
            const restoredPage =
                restoredState?.page;

            return restoredPage &&
                Number.isInteger(restoredPage) &&
                restoredPage > 0
                ? restoredPage
                : 1;
        });

    const [totalCount, setTotalCount] =
        useState(0);

    const [filters, setFilters] =
        useState<AdminOrderFilterValues>(
            () => ({
                ...defaultFilters,
                ...restoredState?.filters,
            })
        );

    const [appliedFilters, setAppliedFilters,] =
        useState<AdminOrderFilterValues>(
            () => ({
                ...defaultFilters,
                ...restoredState?.filters,
            })
        );

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    const loadOrders = useCallback(async () => {
        setLoading(true);
        setError(null);

        try {
            const companyId =
                appliedFilters.companyId.trim()
                    ? Number(appliedFilters.companyId)
                    : undefined;

            const result =
                await getAdminOrders({
                    page,
                    pageSize,
                    status:
                        appliedFilters.status ||
                        undefined,
                    companyId:
                        companyId &&
                            Number.isInteger(companyId) &&
                            companyId > 0
                            ? companyId
                            : undefined,
                    fromDate:
                        appliedFilters.fromDate
                            ? `${appliedFilters.fromDate}T00:00:00`
                            : undefined,
                    toDate:
                        appliedFilters.toDate
                            ? `${appliedFilters.toDate}T23:59:59.999`
                            : undefined,
                });

            setOrders(result.items);
            setTotalCount(
                result.totalCount
            );
        } catch (error) {
            console.error(
                "Failed to load admin orders",
                error
            );

            setError(
                "The orders could not be loaded. Please try again."
            );
        } finally {
            setLoading(false);
        }
    }, [page, appliedFilters]);

    useEffect(() => {
        void loadOrders();
    }, [loadOrders]);

    const handleApplyFilters = () => {
        setPage(1);

        setAppliedFilters({
            ...filters,
        });
    };

    const handleResetFilters = () => {
        setFilters({
            ...defaultFilters,
        });

        setAppliedFilters({
            ...defaultFilters,
        });

        setPage(1);
    };

    const hasActiveFilters =
        Boolean(
            appliedFilters.status ||
            appliedFilters.companyId.trim() ||
            appliedFilters.fromDate ||
            appliedFilters.toDate
        );

    const pageCount = Math.max(
        1,
        Math.ceil(totalCount / pageSize)
    );

    const firstOrderNumber =
        totalCount === 0
            ? 0
            : (page - 1) * pageSize + 1;

    const lastOrderNumber =
        Math.min(
            page * pageSize,
            totalCount
        );

    return (
        <Box>
            <PageHeader
                title="Orders"
                subtitle="Review customer orders and follow their current processing status."
                action={
                    <Button
                        variant="outlined"
                        startIcon={<Refresh />}
                        onClick={() =>
                            void loadOrders()
                        }
                        disabled={loading}
                    >
                        Refresh
                    </Button>
                }
            />

            <AdminOrderFilters
                values={filters}
                loading={loading}
                onChange={setFilters}
                onApply={handleApplyFilters}
                onReset={handleResetFilters}
            />

            {error && (
                <Alert
                    severity="error"
                    sx={{ mb: 3 }}
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
                >
                    {error}
                </Alert>
            )}

            {loading && (
                <LoadingSpinner text="Loading orders..." />
            )}

            {!loading &&
                !error &&
                orders.length === 0 && (
                    <EmptyState
                        icon={<ReceiptLong />}
                        title={
                            hasActiveFilters
                                ? "No orders match your filters"
                                : "No orders found"
                        }
                        description={
                            hasActiveFilters
                                ? "Try changing or clearing the selected filters."
                                : "There are currently no customer orders to display."
                        }
                        action={
                            hasActiveFilters ? (
                                <Button
                                    variant="outlined"
                                    startIcon={<RestartAlt />}
                                    onClick={handleResetFilters}
                                >
                                    Clear filters
                                </Button>
                            ) : (
                                <Button
                                    variant="outlined"
                                    startIcon={<Refresh />}
                                    onClick={() =>
                                        void loadOrders()
                                    }
                                >
                                    Check again
                                </Button>
                            )
                        }
                    />
                )}

            {!loading &&
                orders.length > 0 && (
                    <>
                        <Stack
                            direction={{
                                xs: "column",
                                sm: "row",
                            }}
                            spacing={1}
                            sx={{
                                mb: 2,
                                alignItems: {
                                    xs: "flex-start",
                                    sm: "center",
                                },
                                justifyContent:
                                    "space-between",
                            }}
                        >
                            <Typography
                                variant="h5"
                                component="h2"
                                sx={{
                                    fontWeight: 800,
                                }}
                            >
                                Customer orders
                            </Typography>

                            <Typography
                                color="text.secondary"
                            >
                                {totalCount}{" "}
                                {totalCount === 1
                                    ? "order"
                                    : "orders"}
                            </Typography>
                        </Stack>

                        <TableContainer
                            component={Paper}
                            variant="outlined"
                        >
                            <Table
                                sx={{ minWidth: 850 }}
                            >
                                <TableHead>
                                    <TableRow>
                                        <TableCell>
                                            Order
                                        </TableCell>

                                        <TableCell>
                                            Company
                                        </TableCell>

                                        <TableCell>
                                            Created
                                        </TableCell>

                                        <TableCell>
                                            Status
                                        </TableCell>

                                        <TableCell align="right">
                                            Total
                                        </TableCell>

                                        <TableCell align="right">
                                            Actions
                                        </TableCell>
                                    </TableRow>
                                </TableHead>

                                <TableBody>
                                    {orders.map(
                                        (order) => (
                                            <TableRow
                                                key={
                                                    order.id
                                                }
                                                hover
                                            >
                                                <TableCell>
                                                    <Typography
                                                        sx={{
                                                            fontWeight: 700,
                                                        }}
                                                    >
                                                        #
                                                        {
                                                            order.id
                                                        }
                                                    </Typography>
                                                </TableCell>

                                                <TableCell>
                                                    Company #
                                                    {
                                                        order.companyId
                                                    }
                                                </TableCell>

                                                <TableCell>
                                                    {formatOrderDate(
                                                        order.createdAt
                                                    )}
                                                </TableCell>

                                                <TableCell>
                                                    <AdminOrderStatusChip
                                                        status={
                                                            order.status
                                                        }
                                                    />
                                                </TableCell>

                                                <TableCell
                                                    align="right"
                                                    sx={{
                                                        fontWeight: 700,
                                                        whiteSpace:
                                                            "nowrap",
                                                    }}
                                                >
                                                    {formatPrice(
                                                        order.total
                                                    )}
                                                </TableCell>

                                                <TableCell
                                                    align="right"
                                                >
                                                    <Button
                                                        component={Link}
                                                        to={`/admin/orders/${order.id}`}
                                                        state={{
                                                            filters: appliedFilters,
                                                            page,
                                                        }}
                                                        size="small"
                                                        startIcon={
                                                            <Visibility />
                                                        }
                                                    >
                                                        View
                                                    </Button>
                                                </TableCell>
                                            </TableRow>
                                        )
                                    )}
                                </TableBody>
                            </Table>
                        </TableContainer>

                        <Stack
                            direction={{
                                xs: "column",
                                sm: "row",
                            }}
                            spacing={2}
                            sx={{
                                mt: 3,
                                alignItems: "center",
                                justifyContent:
                                    "space-between",
                            }}
                        >
                            <Typography
                                color="text.secondary"
                                variant="body2"
                            >
                                Showing{" "}
                                {firstOrderNumber}–
                                {lastOrderNumber} of{" "}
                                {totalCount}
                            </Typography>

                            {pageCount > 1 && (
                                <Pagination
                                    page={page}
                                    count={pageCount}
                                    color="secondary"
                                    onChange={(
                                        _event,
                                        newPage
                                    ) => {
                                        setPage(
                                            newPage
                                        );
                                    }}
                                />
                            )}
                        </Stack>
                    </>
                )}
        </Box>
    );
};

export default AdminOrdersPage;