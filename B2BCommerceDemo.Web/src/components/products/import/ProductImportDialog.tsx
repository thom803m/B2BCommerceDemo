import {
    Alert,
    Box,
    Button,
    CircularProgress,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Stack,
    Tab,
    Tabs,
    Typography,
} from "@mui/material";
import { UploadFile } from "@mui/icons-material";
import { type ChangeEvent, useState } from "react";
import { importDeliveryDates, importProducts, type ImportResult, } from "../../../api/importApi";

type ImportType =
    | "products"
    | "deliveryDates";

type ProductImportDialogProps = {
    open: boolean;
    onClose: () => void;
    onImportCompleted: () =>
        void | Promise<void>;
};

const ProductImportDialog = ({
    open,
    onClose,
    onImportCompleted,
}: ProductImportDialogProps) => {
    const [importType, setImportType] =
        useState<ImportType>("products");

    const [selectedFile, setSelectedFile] =
        useState<File | null>(null);

    const [fileError, setFileError] =
        useState<string | null>(null);

    const [importing, setImporting] =
        useState(false);

    const [result, setResult] =
        useState<ImportResult | null>(null);

    const [importError, setImportError] =
        useState<string | null>(null);

    const handleFileChange = (
        event: ChangeEvent<HTMLInputElement>
    ) => {
        const file =
            event.target.files?.[0] ?? null;

        event.target.value = "";

        if (!file) {
            return;
        }

        if (
            !file.name
                .toLowerCase()
                .endsWith(".csv")
        ) {
            setSelectedFile(null);
            setFileError(
                "Select a CSV file."
            );
            return;
        }

        if (file.size === 0) {
            setSelectedFile(null);
            setFileError(
                "The selected file is empty."
            );
            return;
        }

        setResult(null);
        setImportError(null);
        setSelectedFile(file);
        setFileError(null);
    };

    const handleImport = async () => {
        if (!selectedFile) {
            setFileError(
                "Select a CSV file."
            );
            return;
        }

        try {
            setImporting(true);
            setImportError(null);
            setResult(null);

            const importResult =
                importType === "products"
                    ? await importProducts(
                        selectedFile
                    )
                    : await importDeliveryDates(
                        selectedFile
                    );

            setResult(importResult);
            setSelectedFile(null);

            await onImportCompleted();
        } catch {
            setImportError(
                "The file could not be imported."
            );
        } finally {
            setImporting(false);
        }
    };

    const handleClose = () => {
        if (importing) {
            return;
        }

        setImportType("products");
        setSelectedFile(null);
        setFileError(null);
        setResult(null);
        setImportError(null);

        onClose();
    };

    return (
        <Dialog
            open={open}
            onClose={handleClose}
            fullWidth
            maxWidth="sm"
        >
            <DialogTitle>
                Import data
            </DialogTitle>

            <DialogContent dividers>
                <Stack spacing={3}>
                    <Tabs
                        value={importType}
                        onChange={(
                            _,
                            newValue: ImportType
                        ) => {
                            setImportType(newValue);
                            setSelectedFile(null);
                            setFileError(null);
                            setResult(null);
                            setImportError(null);
                        }}
                        variant="fullWidth"
                    >
                        <Tab
                            value="products"
                            label="Products"
                            disabled={importing}
                        />

                        <Tab
                            value="deliveryDates"
                            label="Delivery dates"
                            disabled={importing}
                        />
                    </Tabs>

                    <Stack spacing={1}>
                        <Typography
                            variant="h6"
                            sx={{ fontWeight: 800 }}
                        >
                            {importType === "products"
                                ? "Import products"
                                : "Import delivery dates"}
                        </Typography>

                        <Typography color="text.secondary">
                            {importType === "products"
                                ? "Upload a CSV file to create or update products."
                                : "Upload a CSV file containing incoming quantities and expected delivery dates."}
                        </Typography>
                    </Stack>

                    {importError && (
                        <Alert severity="error">
                            {importError}
                        </Alert>
                    )}

                    {!result && (
                        <Box
                            component="label"
                            sx={{
                                minHeight: 190,
                                border: "2px dashed",
                                borderColor: fileError
                                    ? "error.main"
                                    : selectedFile
                                        ? "primary.main"
                                        : "divider",
                                borderRadius: 3,
                                bgcolor: selectedFile
                                    ? "action.selected"
                                    : "background.default",
                                cursor: importing
                                    ? "default"
                                    : "pointer",
                                display: "flex",
                                alignItems: "center",
                                justifyContent: "center",
                                px: 3,
                                py: 4,
                                textAlign: "center",
                                transition:
                                    "border-color 160ms ease, background-color 160ms ease",
                                opacity: importing
                                    ? 0.7
                                    : 1,
                                "&:hover": {
                                    borderColor: importing
                                        ? undefined
                                        : "primary.main",
                                    bgcolor: importing
                                        ? undefined
                                        : "action.hover",
                                },
                            }}
                        >
                            <input
                                type="file"
                                accept=".csv,text/csv"
                                hidden
                                disabled={importing}
                                onChange={handleFileChange}
                            />

                            <Stack
                                spacing={1}
                                sx={{ alignItems: "center" }}
                            >
                                <UploadFile
                                    color={
                                        fileError
                                            ? "error"
                                            : "primary"
                                    }
                                    sx={{ fontSize: 42 }}
                                />

                                <Typography
                                    sx={{ fontWeight: 700 }}
                                >
                                    {selectedFile
                                        ? selectedFile.name
                                        : "Select CSV file"}
                                </Typography>

                                <Typography
                                    variant="body2"
                                    color={
                                        fileError
                                            ? "error.main"
                                            : "text.secondary"
                                    }
                                >
                                    {fileError ??
                                        (
                                            selectedFile
                                                ? "Click to select a different file."
                                                : "Click here to choose a file from your computer."
                                        )}
                                </Typography>
                            </Stack>
                        </Box>
                    )}

                    {result && (
                        <Stack spacing={2.5}>
                            <Alert severity="success">
                                The import was completed.
                            </Alert>

                            <Stack
                                direction={{
                                    xs: "column",
                                    sm: "row",
                                }}
                                spacing={2}
                            >
                                <Box
                                    sx={{
                                        flex: 1,
                                        p: 2,
                                        borderRadius: 2,
                                        bgcolor: "success.light",
                                        color: "success.contrastText",
                                        textAlign: "center",
                                    }}
                                >
                                    <Typography
                                        variant="h4"
                                        sx={{ fontWeight: 800 }}
                                    >
                                        {result.created}
                                    </Typography>

                                    <Typography>
                                        Created
                                    </Typography>
                                </Box>

                                <Box
                                    sx={{
                                        flex: 1,
                                        p: 2,
                                        borderRadius: 2,
                                        bgcolor: "primary.light",
                                        color: "primary.contrastText",
                                        textAlign: "center",
                                    }}
                                >
                                    <Typography
                                        variant="h4"
                                        sx={{ fontWeight: 800 }}
                                    >
                                        {result.updated}
                                    </Typography>

                                    <Typography>
                                        Updated
                                    </Typography>
                                </Box>

                                <Box
                                    sx={{
                                        flex: 1,
                                        p: 2,
                                        borderRadius: 2,
                                        bgcolor: "warning.light",
                                        color: "warning.contrastText",
                                        textAlign: "center",
                                    }}
                                >
                                    <Typography
                                        variant="h4"
                                        sx={{ fontWeight: 800 }}
                                    >
                                        {result.skipped}
                                    </Typography>

                                    <Typography>
                                        Skipped
                                    </Typography>
                                </Box>
                            </Stack>

                            {result.warnings.length > 0 && (
                                <Alert
                                    severity="warning"
                                    sx={{
                                        alignItems: "flex-start",
                                    }}
                                >
                                    <Typography
                                        sx={{
                                            mb: 1,
                                            fontWeight: 700,
                                        }}
                                    >
                                        Import warnings
                                    </Typography>

                                    <Stack
                                        component="ul"
                                        spacing={0.75}
                                        sx={{
                                            m: 0,
                                            pl: 2.5,
                                            maxHeight: 220,
                                            overflowY: "auto",
                                        }}
                                    >
                                        {result.warnings.map(
                                            (
                                                warning,
                                                index
                                            ) => (
                                                <Typography
                                                    component="li"
                                                    variant="body2"
                                                    key={`${index}-${warning}`}
                                                    sx={{
                                                        overflowWrap:
                                                            "anywhere",
                                                    }}
                                                >
                                                    {warning}
                                                </Typography>
                                            )
                                        )}
                                    </Stack>
                                </Alert>
                            )}

                            <Button
                                variant="outlined"
                                onClick={() => {
                                    setResult(null);
                                    setImportError(null);
                                    setSelectedFile(null);
                                    setFileError(null);
                                }}
                            >
                                Import another file
                            </Button>
                        </Stack>
                    )}
                </Stack>
            </DialogContent>

            <DialogActions
                sx={{
                    px: 3,
                    py: 2,
                }}
            >
                <Button
                    color="inherit"
                    onClick={handleClose}
                    disabled={importing}
                >
                    {result ? "Close" : "Cancel"}
                </Button>

                {!result && (
                    <Button
                        variant="contained"
                        startIcon={
                            importing
                                ? (
                                    <CircularProgress
                                        size={18}
                                        color="inherit"
                                    />
                                )
                                : <UploadFile />
                        }
                        disabled={
                            !selectedFile ||
                            importing
                        }
                        onClick={() =>
                            void handleImport()
                        }
                    >
                        {importing
                            ? "Importing..."
                            : "Import"}
                    </Button>
                )}
            </DialogActions>
        </Dialog>
    );
};

export default ProductImportDialog;