import { formatPrice } from "../../utils/formatPrice";
import { Link, } from "react-router-dom";
import {
    Button,
    Card,
    CardContent,
    Divider,
    Stack,
    Typography,
} from "@mui/material";

type CartSummaryProps = {
    total: number;
    itemCount: number;
};

const CartSummary = ({
    total,
    itemCount,
}: CartSummaryProps) => {
    return (
        <Card
            variant="outlined"
            sx={{
                borderRadius: 3,
                position: {
                    xs: "static",
                    lg: "sticky",
                },
                top: {
                    lg: 104,
                },
            }}
        >
            <CardContent
                sx={{
                    p: 3,

                    "&:last-child": {
                        pb: 3,
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
                    Order summary
                </Typography>

                <Stack
                    spacing={2}
                    sx={{
                        mt: 3,
                    }}
                >
                    <Stack
                        direction="row"
                        sx={{
                            justifyContent:
                                "space-between",
                        }}
                    >
                        <Typography
                            color="text.secondary"
                        >
                            Items ({itemCount})
                        </Typography>

                        <Typography
                            sx={{
                                fontWeight: 650,
                            }}
                        >
                            {formatPrice(total)}
                        </Typography>
                    </Stack>

                    <Divider />

                    <Stack
                        direction="row"
                        sx={{
                            justifyContent:
                                "space-between",
                        }}
                    >
                        <Typography
                            variant="h6"
                            sx={{
                                fontWeight: 800,
                            }}
                        >
                            Total
                        </Typography>

                        <Typography
                            variant="h6"
                            sx={{
                                fontWeight: 800,
                            }}
                        >
                            {formatPrice(total)}
                        </Typography>
                    </Stack>

                    <Typography
                        variant="body2"
                        color="text.secondary"
                    >
                        Prices are shown excluding VAT.
                        Delivery and final order details
                        are confirmed during checkout.
                    </Typography>

                    <Button
                        component={Link}
                        to="/checkout"
                        variant="contained"
                        size="large"
                        sx={{
                            mt: 1,
                            py: 1.35,
                            borderRadius: 2.5,
                        }}
                    >
                        Proceed to checkout
                    </Button>
                </Stack>
            </CardContent>
        </Card>
    );
};

export default CartSummary;