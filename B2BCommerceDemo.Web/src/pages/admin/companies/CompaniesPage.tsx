import { Refresh } from "@mui/icons-material";
import { Alert, Box, Button, Grid, Snackbar, Stack, Typography, } from "@mui/material";
import { useCallback, useEffect, useState, } from "react";
import {
    approveCompany,
    getAdminCompanies,
    getPendingCompanies,
    rejectCompany,
    reactivateCompany,
    suspendCompany,
    updateCompanyPriceGroup,
    type ApproveCompanyRequest,
    type Company,
} from "../../../api/companyApi";
import { getPriceGroups, type PriceGroup, } from "../../../api/priceGroupApi";
import AdminCompanyTable from "../../../components/admin/companies/AdminCompanyTable";
import PendingCompanyCard from "../../../components/admin/companies/PendingCompanyCard";
import ConfirmDialog from "../../../components/common/ConfirmDialog";
import EmptyState from "../../../components/common/EmptyState";
import LoadingSpinner from "../../../components/common/LoadingSpinner";
import PageHeader from "../../../components/common/PageHeader";

const CompaniesPage = () => {
    const [
        pendingCompanies,
        setPendingCompanies,
    ] = useState<Company[]>([]);

    const [companies, setCompanies] =
        useState<Company[]>([]);

    const [priceGroups, setPriceGroups] =
        useState<PriceGroup[]>([]);

    const [loading, setLoading] =
        useState(true);

    const [error, setError] =
        useState<string | null>(null);

    const [
        processingCompanyId,
        setProcessingCompanyId,
    ] = useState<number | null>(null);

    const [
        companyToReject,
        setCompanyToReject,
    ] = useState<Company | null>(null);

    const [
        companyToSuspend,
        setCompanyToSuspend,
    ] = useState<Company | null>(null);

    const [
        companyToReactivate,
        setCompanyToReactivate,
    ] = useState<Company | null>(null);

    const [
        successMessage,
        setSuccessMessage,
    ] = useState<string | null>(null);

    const loadData = useCallback(async () => {
        setLoading(true);
        setError(null);

        try {
            const [
                allCompanies,
                pendingRegistrations,
                availablePriceGroups,
            ] = await Promise.all([
                getAdminCompanies(),
                getPendingCompanies(),
                getPriceGroups(),
            ]);

            setCompanies(allCompanies);

            setPendingCompanies(
                pendingRegistrations
            );

            setPriceGroups(
                availablePriceGroups
            );
        } catch (error) {
            console.error(
                "Failed to load company administration data",
                error
            );

            setError(
                "The company administration data could not be loaded. Please try again."
            );
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        void loadData();
    }, [loadData]);

    const handleApprove = async (
        companyId: number,
        request: ApproveCompanyRequest
    ) => {
        const company =
            pendingCompanies.find(
                (item) =>
                    item.id === companyId
            );

        setProcessingCompanyId(companyId);
        setError(null);

        try {
            await approveCompany(
                companyId,
                request
            );

            setSuccessMessage(
                `${company?.name ?? "The company"} was approved successfully.`
            );

            await loadData();
        } catch (error) {
            console.error(
                "Failed to approve company",
                error
            );

            setError(
                "The company could not be approved. Please check the information and try again."
            );
        } finally {
            setProcessingCompanyId(null);
        }
    };

    const handleRejectRequest = (
        companyId: number
    ) => {
        const company =
            pendingCompanies.find(
                (item) =>
                    item.id === companyId
            );

        if (company) {
            setCompanyToReject(company);
        }
    };

    const handleConfirmReject =
        async () => {
            if (!companyToReject) {
                return;
            }

            const company =
                companyToReject;

            setProcessingCompanyId(
                company.id
            );

            setError(null);

            try {
                await rejectCompany(
                    company.id
                );

                setCompanyToReject(null);

                setSuccessMessage(
                    `${company.name} was rejected.`
                );

                await loadData();
            } catch (error) {
                console.error(
                    "Failed to reject company",
                    error
                );

                setCompanyToReject(null);

                setError(
                    "The company could not be rejected. Please try again."
                );
            } finally {
                setProcessingCompanyId(
                    null
                );
            }
        };

    const handleUpdatePriceGroup =
        async (
            companyId: number,
            priceGroupId: number
        ) => {
            if (
                !Number.isInteger(
                    priceGroupId
                ) ||
                priceGroupId <= 0
            ) {
                return;
            }

            setProcessingCompanyId(
                companyId
            );

            setError(null);

            try {
                await updateCompanyPriceGroup(
                    companyId,
                    {
                        priceGroupId,
                    }
                );

                const selectedPriceGroup =
                    priceGroups.find(
                        (priceGroup) =>
                            priceGroup.id ===
                            priceGroupId
                    );

                setCompanies((current) =>
                    current.map(
                        (company) =>
                            company.id ===
                                companyId
                                ? {
                                    ...company,
                                    priceGroup:
                                        selectedPriceGroup ??
                                        company.priceGroup,
                                }
                                : company
                    )
                );

                const company =
                    companies.find(
                        (item) =>
                            item.id ===
                            companyId
                    );

                setSuccessMessage(
                    `The price group for ${company?.name ?? "the company"} was updated.`
                );
            } catch (error) {
                console.error(
                    "Failed to update company price group",
                    error
                );

                setError(
                    "The company price group could not be updated. Please try again."
                );
            } finally {
                setProcessingCompanyId(
                    null
                );
            }
        };

    const handleSuspendRequest = (
        company: Company
    ) => {
        setCompanyToSuspend(company);
    };

    const handleConfirmSuspend =
        async () => {
            if (!companyToSuspend) {
                return;
            }

            const company =
                companyToSuspend;

            setProcessingCompanyId(
                company.id
            );

            setError(null);

            try {
                await suspendCompany(
                    company.id
                );

                setCompanies((current) =>
                    current.map((item) =>
                        item.id === company.id
                            ? {
                                ...item,
                                status: "Suspended",
                            }
                            : item
                    )
                );

                setCompanyToSuspend(null);

                setSuccessMessage(
                    `${company.name} was suspended successfully.`
                );
            } catch (error) {
                console.error(
                    "Failed to suspend company",
                    error
                );

                setCompanyToSuspend(null);

                setError(
                    "The company could not be suspended. Please try again."
                );
            } finally {
                setProcessingCompanyId(
                    null
                );
            }
        };

    const handleRequestReactivate = (
        company: Company
    ) => {
        setCompanyToReactivate(company);
    };

    const handleConfirmReactivate =
        async () => {
            if (!companyToReactivate) {
                return;
            }

            const company =
                companyToReactivate;

            setProcessingCompanyId(
                company.id
            );

            setError(null);

            try {
                await reactivateCompany(
                    company.id
                );

                setCompanies((current) =>
                    current.map((item) =>
                        item.id === company.id
                            ? {
                                ...item,
                                status: "Active",
                            }
                            : item
                    )
                );

                setCompanyToReactivate(null);

                setSuccessMessage(
                    `${company.name} was reactivated successfully.`
                );
            } catch (error) {
                console.error(
                    "Failed to reactivate company",
                    error
                );

                setCompanyToReactivate(null);

                setError(
                    "The company could not be reactivated. Please try again."
                );
            } finally {
                setProcessingCompanyId(null);
            }
        };

    const managedCompanies =
        companies.filter(
            (company) =>
                company.status !==
                "Pending"
        );

    return (
        <Box>
            <PageHeader
                title="Companies"
                subtitle="Review registrations and manage approved business customers."
                action={
                    <Button
                        variant="outlined"
                        startIcon={
                            <Refresh />
                        }
                        onClick={() =>
                            void loadData()
                        }
                        disabled={loading}
                    >
                        Refresh
                    </Button>
                }
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
                                void loadData()
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
                <LoadingSpinner text="Loading companies..." />
            )}

            {!loading && (
                <Stack spacing={5}>
                    <Box
                        id="pending-companies"
                        sx={{
                            scrollMarginTop: 120,
                        }}
                    >
                        <Stack
                            direction={{
                                xs: "column",
                                sm: "row",
                            }}
                            spacing={1}
                            sx={{
                                mb: 3,
                                alignItems: {
                                    xs: "flex-start",
                                    sm: "center",
                                },
                                justifyContent:
                                    "space-between",
                            }}
                        >
                            <Box>
                                <Typography
                                    variant="h5"
                                    component="h2"
                                    sx={{
                                        fontWeight: 800,
                                    }}
                                >
                                    Pending
                                    registrations
                                </Typography>

                                <Typography
                                    color="text.secondary"
                                    sx={{ mt: 0.5 }}
                                >
                                    Approve or reject
                                    companies waiting
                                    for webshop access.
                                </Typography>
                            </Box>

                            <Typography
                                color="text.secondary"
                            >
                                {
                                    pendingCompanies.length
                                }{" "}
                                {pendingCompanies.length ===
                                    1
                                    ? "company"
                                    : "companies"}
                            </Typography>
                        </Stack>

                        {pendingCompanies.length ===
                            0 ? (
                            <Alert severity="info">
                                There are currently
                                no business
                                registrations waiting
                                for approval.
                            </Alert>
                        ) : (
                            <Grid
                                container
                                spacing={3}
                            >
                                {pendingCompanies.map(
                                    (
                                        company
                                    ) => (
                                        <Grid
                                            key={
                                                company.id
                                            }
                                            size={{
                                                xs: 12,
                                                xl: 6,
                                            }}
                                        >
                                            <PendingCompanyCard
                                                company={
                                                    company
                                                }
                                                priceGroups={
                                                    priceGroups
                                                }
                                                processing={
                                                    processingCompanyId ===
                                                    company.id
                                                }
                                                onApprove={
                                                    handleApprove
                                                }
                                                onReject={
                                                    handleRejectRequest
                                                }
                                            />
                                        </Grid>
                                    )
                                )}
                            </Grid>
                        )}
                    </Box>

                    <Box
                        id="managed-companies"
                        sx={{
                            scrollMarginTop: 120,
                        }}
                    >
                        <Stack
                            direction={{
                                xs: "column",
                                sm: "row",
                            }}
                            spacing={1}
                            sx={{
                                mb: 3,
                                alignItems: {
                                    xs: "flex-start",
                                    sm: "center",
                                },
                                justifyContent:
                                    "space-between",
                            }}
                        >
                            <Box>
                                <Typography
                                    variant="h5"
                                    component="h2"
                                    sx={{
                                        fontWeight: 800,
                                    }}
                                >
                                    Managed companies
                                </Typography>

                                <Typography
                                    color="text.secondary"
                                    sx={{ mt: 0.5 }}
                                >
                                    Review company
                                    status, Rackbeat
                                    details and price
                                    groups.
                                </Typography>
                            </Box>

                            <Typography
                                color="text.secondary"
                            >
                                {
                                    managedCompanies.length
                                }{" "}
                                {managedCompanies.length ===
                                    1
                                    ? "company"
                                    : "companies"}
                            </Typography>
                        </Stack>

                        {managedCompanies.length ===
                            0 ? (
                            <EmptyState
                                title="No managed companies"
                                description="There are currently no approved or rejected companies to display."
                            />
                        ) : (
                            <AdminCompanyTable
                                companies={
                                    managedCompanies
                                }
                                priceGroups={
                                    priceGroups
                                }
                                processingCompanyId={
                                    processingCompanyId
                                }
                                onUpdatePriceGroup={
                                    handleUpdatePriceGroup
                                }
                                onSuspend={
                                    handleSuspendRequest
                                }
                                onReactivate={
                                    handleRequestReactivate
                                }
                            />
                        )}
                    </Box>
                </Stack>
            )}

            <ConfirmDialog
                open={
                    companyToReject !== null
                }
                title="Reject company?"
                description={
                    companyToReject
                        ? `Rejecting "${companyToReject.name}" will prevent the company from accessing the webshop.`
                        : ""
                }
                confirmLabel="Reject company"
                loading={
                    processingCompanyId ===
                    companyToReject?.id
                }
                onClose={() =>
                    setCompanyToReject(null)
                }
                onConfirm={() =>
                    void handleConfirmReject()
                }
            />

            <ConfirmDialog
                open={
                    companyToSuspend !== null
                }
                title="Suspend company?"
                description={
                    companyToSuspend
                        ? `Suspending "${companyToSuspend.name}" will remove its access to the webshop.`
                        : ""
                }
                confirmLabel="Suspend company"
                loading={
                    processingCompanyId ===
                    companyToSuspend?.id
                }
                onClose={() =>
                    setCompanyToSuspend(null)
                }
                onConfirm={() =>
                    void handleConfirmSuspend()
                }
            />

            <ConfirmDialog
                open={
                    companyToReactivate !== null
                }
                title="Reactivate company?"
                description={
                    companyToReactivate
                        ? `You are about to reactivate "${companyToReactivate.name}". The company will regain access to the webshop.`
                        : ""
                }
                confirmLabel="Reactivate company"
                loading={
                    processingCompanyId ===
                    companyToReactivate?.id
                }
                onClose={() =>
                    setCompanyToReactivate(null)
                }
                onConfirm={() =>
                    void handleConfirmReactivate()
                }
            />

            <Snackbar
                open={
                    successMessage !== null
                }
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

export default CompaniesPage;