import { BlockOutlined, BusinessOutlined, RestartAlt, } from "@mui/icons-material";
import {
    Box,
    Button,
    MenuItem,
    Paper,
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
import type { Company, } from "../../../api/companyApi";
import type { PriceGroup, } from "../../../api/priceGroupApi";
import AdminCompanyStatusChip from "./AdminCompanyStatusChip";

type AdminCompanyTableProps = {
    companies: Company[];
    priceGroups: PriceGroup[];
    processingCompanyId?: number | null;
    onUpdatePriceGroup: (
        companyId: number,
        priceGroupId: number
    ) => void;
    onSuspend: (
        company: Company
    ) => void;
    onReactivate: (
        company: Company
    ) => void;
};

const AdminCompanyTable = ({
    companies,
    priceGroups,
    processingCompanyId = null,
    onUpdatePriceGroup,
    onSuspend,
    onReactivate,
}: AdminCompanyTableProps) => {
    return (
        <TableContainer
            component={Paper}
            variant="outlined"
        >
            <Table sx={{ minWidth: 1000 }}>
                <TableHead>
                    <TableRow>
                        <TableCell>
                            Company
                        </TableCell>

                        <TableCell>
                            Status
                        </TableCell>

                        <TableCell>
                            Price group
                        </TableCell>

                        <TableCell>
                            Adjustment
                        </TableCell>

                        <TableCell>
                            Rackbeat customer
                        </TableCell>

                        <TableCell align="right">
                            Actions
                        </TableCell>
                    </TableRow>
                </TableHead>

                <TableBody>
                    {companies.map(
                        (company) => {
                            const processing =
                                processingCompanyId ===
                                company.id;

                            const canEdit =
                                company.status ===
                                "Active";

                            return (
                                <TableRow
                                    key={
                                        company.id
                                    }
                                    hover
                                >
                                    <TableCell>
                                        <Stack
                                            direction="row"
                                            spacing={1.5}
                                            sx={{
                                                alignItems:
                                                    "center",
                                                minWidth:
                                                    220,
                                            }}
                                        >
                                            <Box
                                                sx={{
                                                    width: 42,
                                                    height: 42,
                                                    borderRadius:
                                                        "50%",
                                                    display:
                                                        "grid",
                                                    placeItems:
                                                        "center",
                                                    bgcolor:
                                                        "action.hover",
                                                    color:
                                                        "text.secondary",
                                                    flexShrink: 0,
                                                }}
                                            >
                                                <BusinessOutlined />
                                            </Box>

                                            <Box>
                                                <Typography
                                                    sx={{
                                                        fontWeight: 700,
                                                    }}
                                                >
                                                    {
                                                        company.name
                                                    }
                                                </Typography>

                                                <Typography
                                                    variant="body2"
                                                    color="text.secondary"
                                                >
                                                    ID:{" "}
                                                    {
                                                        company.id
                                                    }
                                                </Typography>
                                            </Box>
                                        </Stack>
                                    </TableCell>

                                    <TableCell>
                                        <AdminCompanyStatusChip
                                            status={
                                                company.status
                                            }
                                        />
                                    </TableCell>

                                    <TableCell>
                                        {canEdit ? (
                                            <TextField
                                                select
                                                size="small"
                                                value={
                                                    company
                                                        .priceGroup
                                                        ?.id ??
                                                    ""
                                                }
                                                onChange={(
                                                    event
                                                ) =>
                                                    onUpdatePriceGroup(
                                                        company.id,
                                                        Number(
                                                            event
                                                                .target
                                                                .value
                                                        )
                                                    )
                                                }
                                                disabled={
                                                    processing
                                                }
                                                sx={{
                                                    minWidth:
                                                        180,
                                                }}
                                            >
                                                <MenuItem
                                                    value=""
                                                    disabled
                                                >
                                                    Select
                                                    price
                                                    group
                                                </MenuItem>

                                                {priceGroups.map(
                                                    (
                                                        priceGroup
                                                    ) => (
                                                        <MenuItem
                                                            key={
                                                                priceGroup.id
                                                            }
                                                            value={
                                                                priceGroup.id
                                                            }
                                                        >
                                                            {
                                                                priceGroup.name
                                                            }
                                                        </MenuItem>
                                                    )
                                                )}
                                            </TextField>
                                        ) : (
                                            company
                                                .priceGroup
                                                ?.name ??
                                            "Not assigned"
                                        )}
                                    </TableCell>

                                    <TableCell>
                                        {company.priceGroup
                                            ? `${company.priceGroup.percentageAdjustment}%`
                                            : "—"}
                                    </TableCell>

                                    <TableCell>
                                        {company.rackbeatCustomerNumber ??
                                            "Not assigned"}
                                    </TableCell>

                                    <TableCell align="right">
                                        {company.status === "Active" && (
                                            <Button
                                                color="error"
                                                size="small"
                                                startIcon={
                                                    <BlockOutlined />
                                                }
                                                onClick={() =>
                                                    onSuspend(company)
                                                }
                                                disabled={processing}
                                            >
                                                Suspend
                                            </Button>
                                        )}

                                        {company.status ===
                                            "Suspended" && (
                                                <Button
                                                    color="success"
                                                    size="small"
                                                    startIcon={<RestartAlt />}
                                                    onClick={() =>
                                                        onReactivate(company)
                                                    }
                                                    disabled={processing}
                                                >
                                                    Reactivate
                                                </Button>
                                            )}

                                        {company.status !== "Active" &&
                                            company.status !==
                                            "Suspended" && (
                                                <Typography
                                                    variant="body2"
                                                    color="text.secondary"
                                                >
                                                    No actions
                                                </Typography>
                                            )}
                                    </TableCell>
                                </TableRow>
                            );
                        }
                    )}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default AdminCompanyTable;