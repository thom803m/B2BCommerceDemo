import { FilterAlt, RestartAlt, } from "@mui/icons-material";
import { Button, Grid, MenuItem, Paper, Stack, TextField, Typography, } from "@mui/material";
import type { FormEvent, } from "react";
import type { OrderStatus, } from "../../../api/orderApi";

export type AdminOrderFilterValues = {
    status: OrderStatus | "";
    companyId: string;
    fromDate: string;
    toDate: string;
};

type AdminOrderFiltersProps = {
    values: AdminOrderFilterValues;
    loading?: boolean;
    onChange: (
        values: AdminOrderFilterValues
    ) => void;
    onApply: () => void;
    onReset: () => void;
};

const orderStatuses: OrderStatus[] = [
    "Pending",
    "Confirmed",
    "Processing",
    "Shipped",
    "Completed",
    "Cancelled",
];

const AdminOrderFilters = ({
    values,
    loading = false,
    onChange,
    onApply,
    onReset,
}: AdminOrderFiltersProps) => {
    const hasInvalidDateRange =
        Boolean(
            values.fromDate &&
            values.toDate &&
            values.fromDate > values.toDate
        );

    const handleSubmit = (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();
        onApply();
    };

    return (
        <Paper
            component="form"
            variant="outlined"
            onSubmit={handleSubmit}
            sx={{
                mb: 3,
                p: {
                    xs: 2,
                    md: 3,
                },
            }}
        >
            <Typography
                variant="h6"
                component="h2"
                sx={{
                    mb: 2,
                    fontWeight: 800,
                }}
            >
                Filter orders
            </Typography>

            <Grid
                container
                spacing={2}
                sx={{ alignItems: "flex-start" }}
            >
                <Grid
                    size={{
                        xs: 12,
                        sm: 6,
                        lg: 3,
                    }}
                >
                    <TextField
                        select
                        fullWidth
                        label="Status"
                        value={values.status}
                        onChange={(event) =>
                            onChange({
                                ...values,
                                status:
                                    event.target
                                        .value as
                                    | OrderStatus
                                    | "",
                            })
                        }
                        disabled={loading}
                    >
                        <MenuItem value="">
                            All statuses
                        </MenuItem>

                        {orderStatuses.map(
                            (status) => (
                                <MenuItem
                                    key={status}
                                    value={status}
                                >
                                    {status}
                                </MenuItem>
                            )
                        )}
                    </TextField>
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        sm: 6,
                        lg: 3,
                    }}
                >
                    <TextField
                        fullWidth
                        type="number"
                        label="Company ID"
                        value={values.companyId}
                        onChange={(event) =>
                            onChange({
                                ...values,
                                companyId:
                                    event.target.value,
                            })
                        }
                        disabled={loading}
                        slotProps={{
                            htmlInput: {
                                min: 1,
                                step: 1,
                            },
                        }}
                    />
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        sm: 6,
                        lg: 3,
                    }}
                >
                    <TextField
                        fullWidth
                        type="date"
                        label="From date"
                        value={values.fromDate}
                        onChange={(event) =>
                            onChange({
                                ...values,
                                fromDate:
                                    event.target.value,
                            })
                        }
                        disabled={loading}
                        slotProps={{
                            inputLabel: {
                                shrink: true,
                            },
                            htmlInput: {
                                max:
                                    values.toDate ||
                                    undefined,
                            },
                        }}
                    />
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        sm: 6,
                        lg: 3,
                    }}
                >
                    <TextField
                        fullWidth
                        type="date"
                        label="To date"
                        value={values.toDate}
                        onChange={(event) =>
                            onChange({
                                ...values,
                                toDate:
                                    event.target.value,
                            })
                        }
                        disabled={loading}
                        error={hasInvalidDateRange}
                        helperText={
                            hasInvalidDateRange
                                ? "To date must be on or after From date."
                                : undefined
                        }
                        slotProps={{
                            inputLabel: {
                                shrink: true,
                            },
                            htmlInput: {
                                min:
                                    values.fromDate ||
                                    undefined,
                            },
                        }}
                    />
                </Grid>
            </Grid>

            <Stack
                direction={{
                    xs: "column",
                    sm: "row",
                }}
                spacing={1.5}
                sx={{
                    mt: 2,
                    justifyContent: "flex-end",
                }}
            >
                <Button
                    type="button"
                    variant="text"
                    startIcon={<RestartAlt />}
                    onClick={onReset}
                    disabled={loading}
                >
                    Reset filters
                </Button>

                <Button
                    type="submit"
                    variant="contained"
                    startIcon={<FilterAlt />}
                    disabled={loading || hasInvalidDateRange}
                >
                    Apply filters
                </Button>
            </Stack>
        </Paper>
    );
};

export default AdminOrderFilters;