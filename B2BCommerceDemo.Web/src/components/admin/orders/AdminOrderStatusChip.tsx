import { Autorenew, Cancel, CheckCircle, LocalShipping, Schedule, TaskAlt, } from "@mui/icons-material";
import { Chip, type ChipProps, } from "@mui/material";
import type { ReactElement, } from "react";
import type { OrderStatus, } from "../../../api/orderApi";

type AdminOrderStatusChipProps = {
    status?: OrderStatus | null;
};

type StatusConfiguration = {
    color: ChipProps["color"];
    icon: ReactElement;
};

const statusConfigurations: Record<
    OrderStatus,
    StatusConfiguration
> = {
    Pending: {
        color: "warning",
        icon: <Schedule />,
    },
    Confirmed: {
        color: "info",
        icon: <CheckCircle />,
    },
    Processing: {
        color: "secondary",
        icon: <Autorenew />,
    },
    Shipped: {
        color: "primary",
        icon: <LocalShipping />,
    },
    Completed: {
        color: "success",
        icon: <TaskAlt />,
    },
    Cancelled: {
        color: "error",
        icon: <Cancel />,
    },
};

const AdminOrderStatusChip = ({
    status,
}: AdminOrderStatusChipProps) => {
    if (!status) {
        return (
            <Chip
                label="Unknown"
                size="small"
                variant="outlined"
            />
        );
    }

    const configuration =
        statusConfigurations[status];

    return (
        <Chip
            label={status}
            color={configuration.color}
            icon={configuration.icon}
            size="small"
            variant="outlined"
            sx={{
                fontWeight: 700,

                "& .MuiChip-icon": {
                    fontSize: 18,
                },
            }}
        />
    );
};

export default AdminOrderStatusChip;