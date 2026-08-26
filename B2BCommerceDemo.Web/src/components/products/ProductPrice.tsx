import { formatPrice } from "../../utils/formatPrice";
import { Box, Typography } from "@mui/material";

type ProductPriceProps = {
    amount: number;
    currency?: string;
};

const ProductPrice = ({
    amount,
    currency = "EUR",
}: ProductPriceProps) => {
    const formattedPrice = formatPrice(
        amount,
        currency
    );

    return (
        <Box>
            <Typography
                variant="h6"
                sx={{
                    fontWeight: 700,
                    color: "primary.main",
                }}
            >
                {formattedPrice}
            </Typography>

            <Typography
                variant="caption"
                color="text.secondary"
            >
                Excl. VAT
            </Typography>
        </Box>
    );
};

export default ProductPrice;