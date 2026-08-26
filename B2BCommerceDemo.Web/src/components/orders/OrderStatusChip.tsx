import {
    Chip,
    type ChipProps,
} from "@mui/material";

type OrderStatusChipProps = {
    status?: string | null;
};

type StatusDisplay = {
    label: string;
    color: ChipProps["color"];
};

const OrderStatusChip = ({
    status,
}: OrderStatusChipProps) => {
    const display = getStatusDisplay(status);

    return (
        <Chip
            label={display.label}
            color={display.color}
            size="small"
            sx={{
                fontWeight: 700,
                borderRadius: 2,
            }}
        />
    );
};

const getStatusDisplay = (
    status?: string | null
): StatusDisplay => {
    switch (status?.toLowerCase()) {
        case "pending":
            return {
                label: "Pending",
                color: "warning",
            };

        case "confirmed":
            return {
                label: "Confirmed",
                color: "info",
            };

        case "processing":
            return {
                label: "Processing",
                color: "info",
            };

        case "shipped":
            return {
                label: "Shipped",
                color: "primary",
            };

        case "completed":
            return {
                label: "Completed",
                color: "success",
            };

        case "cancelled":
            return {
                label: "Cancelled",
                color: "error",
            };

        default:
            return {
                label: status || "Unknown",
                color: "default",
            };
    }
};

export default OrderStatusChip;