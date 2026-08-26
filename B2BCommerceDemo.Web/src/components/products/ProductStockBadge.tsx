import { Chip } from "@mui/material";
import { CheckCircle, Error, Schedule, } from "@mui/icons-material";

type ProductStockBadgeProps = {
    availableStock: number;
    purchasedQuantity?: number;
    expectedDeliveryDate?: string | null;
};

const ProductStockBadge = ({
    availableStock,
    purchasedQuantity = 0,
    expectedDeliveryDate,
}: ProductStockBadgeProps) => {
    const normalizedStock = Math.max(availableStock, 0);
    const normalizedIncoming = Math.max(purchasedQuantity, 0);

    if (normalizedStock > 0) {
        const stockLabel =
            normalizedStock > 100
                ? "100+"
                : normalizedStock.toString();

        return (
            <Chip
                size="small"
                color="success"
                icon={<CheckCircle />}
                label={`In stock · ${stockLabel}`}
            />
        );
    }

    if (normalizedIncoming > 0) {
        const formattedDate =
            formatExpectedDeliveryDate(expectedDeliveryDate);

        return (
            <Chip
                size="small"
                color="warning"
                icon={<Schedule />}
                label={
                    formattedDate
                        ? `Expected ${formattedDate}`
                        : "Expected · TBC"
                }
            />
        );
    }

    return (
        <Chip
            size="small"
            color="error"
            icon={<Error />}
            label="Out of stock"
        />
    );
};

const formatExpectedDeliveryDate = (
    expectedDeliveryDate?: string | null
): string | null => {
    if (!expectedDeliveryDate) {
        return null;
    }

    const parsedDate = new Date(expectedDeliveryDate);

    if (Number.isNaN(parsedDate.getTime())) {
        return null;
    }

    const today = new Date();
    today.setHours(0, 0, 0, 0);

    parsedDate.setHours(0, 0, 0, 0);

    if (parsedDate < today) {
        return null;
    }

    return parsedDate.toLocaleDateString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    });
};

export default ProductStockBadge;