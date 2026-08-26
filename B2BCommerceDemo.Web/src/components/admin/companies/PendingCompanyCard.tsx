import { Business, CheckCircle, Close, } from "@mui/icons-material";
import {
    Box,
    Button,
    Card,
    CardActions,
    CardContent,
    Divider,
    MenuItem,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { type FormEvent, useState, } from "react";
import type { ApproveCompanyRequest, Company, } from "../../../api/companyApi";
import type { PriceGroup, } from "../../../api/priceGroupApi";

type PendingCompanyCardProps = {
    company: Company;
    priceGroups: PriceGroup[];
    processing?: boolean;
    onApprove: (
        companyId: number,
        request: ApproveCompanyRequest
    ) => Promise<void>;
    onReject: (
        companyId: number
    ) => void | Promise<void>;
};

const PendingCompanyCard = ({
    company,
    priceGroups,
    processing = false,
    onApprove,
    onReject,
}: PendingCompanyCardProps) => {
    const [priceGroupId, setPriceGroupId] =
        useState(0);

    const [
        rackbeatCustomerNumber,
        setRackbeatCustomerNumber,
    ] = useState("");

    const [submitted, setSubmitted] =
        useState(false);

    const trimmedCustomerNumber =
        rackbeatCustomerNumber.trim();

    const priceGroupError =
        submitted && priceGroupId <= 0;

    const customerNumberError =
        submitted &&
        !/^\d+$/.test(trimmedCustomerNumber);

    const handleSubmit = async (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();
        setSubmitted(true);

        if (
            priceGroupId <= 0 ||
            !/^\d+$/.test(trimmedCustomerNumber)
        ) {
            return;
        }

        await onApprove(company.id, {
            priceGroupId,
            rackbeatCustomerNumber:
                trimmedCustomerNumber,
        });
    };

    const handleReject = async () => {
        await onReject(company.id);
    };

    return (
        <Card
            component="article"
            variant="outlined"
            sx={{
                height: "100%",
                display: "flex",
                flexDirection: "column",
            }}
        >
            <CardContent sx={{ flexGrow: 1 }}>
                <Stack
                    direction="row"
                    spacing={2}
                    sx={{ alignItems: "center" }}
                >
                    <Box
                        sx={{
                            width: 48,
                            height: 48,
                            borderRadius: 2,
                            bgcolor: "action.hover",
                            color: "secondary.main",
                            display: "grid",
                            placeItems: "center",
                            flexShrink: 0,
                        }}
                    >
                        <Business />
                    </Box>

                    <Box sx={{ minWidth: 0 }}>
                        <Typography
                            variant="h6"
                            component="h2"
                            sx={{
                                fontWeight: 800,
                                overflowWrap: "anywhere",
                            }}
                        >
                            {company.name}
                        </Typography>

                        <Typography
                            variant="body2"
                            color="text.secondary"
                        >
                            Company ID: {company.id}
                        </Typography>
                    </Box>
                </Stack>

                <Divider sx={{ my: 3 }} />

                <Box
                    component="form"
                    id={`approve-company-${company.id}`}
                    onSubmit={handleSubmit}
                >
                    <Stack spacing={2}>
                        <TextField
                            select
                            fullWidth
                            label="Price group"
                            value={priceGroupId}
                            onChange={(event) =>
                                setPriceGroupId(
                                    Number(
                                        event.target.value
                                    )
                                )
                            }
                            error={priceGroupError}
                            helperText={
                                priceGroupError
                                    ? "Select a price group."
                                    : "The price group determines the company’s product prices."
                            }
                            disabled={processing}
                        >
                            <MenuItem value={0} disabled>
                                Select price group
                            </MenuItem>

                            {priceGroups.map(
                                (priceGroup) => (
                                    <MenuItem
                                        key={priceGroup.id}
                                        value={priceGroup.id}
                                    >
                                        {priceGroup.name}
                                    </MenuItem>
                                )
                            )}
                        </TextField>

                        <TextField
                            fullWidth
                            label="Rackbeat customer number"
                            value={rackbeatCustomerNumber}
                            onChange={(event) =>
                                setRackbeatCustomerNumber(
                                    event.target.value
                                )
                            }
                            error={customerNumberError}
                            helperText={
                                customerNumberError
                                    ? "Enter a customer number containing only digits."
                                    : "Enter the corresponding customer number from Rackbeat."
                            }
                            inputMode="numeric"
                            disabled={processing}
                        />
                    </Stack>
                </Box>
            </CardContent>

            <CardActions
                sx={{
                    px: 2,
                    pb: 2,
                    pt: 0,
                    display: "flex",
                    justifyContent: "space-between",
                    gap: 1,
                }}
            >
                <Button
                    color="error"
                    startIcon={<Close />}
                    onClick={handleReject}
                    disabled={processing}
                >
                    Reject
                </Button>

                <Button
                    type="submit"
                    form={`approve-company-${company.id}`}
                    variant="contained"
                    startIcon={<CheckCircle />}
                    disabled={
                        processing ||
                        priceGroups.length === 0
                    }
                >
                    {processing
                        ? "Processing..."
                        : "Approve"}
                </Button>
            </CardActions>
        </Card>
    );
};

export default PendingCompanyCard;