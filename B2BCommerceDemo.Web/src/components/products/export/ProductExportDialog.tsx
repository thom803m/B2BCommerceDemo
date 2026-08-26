import {
    Alert,
    Button,
    Checkbox,
    CircularProgress,
    Dialog,
    DialogActions,
    DialogContent,
    DialogTitle,
    Divider,
    FormControlLabel,
    Stack,
    Switch,
    TextField,
    Typography,
} from "@mui/material";
import { Download, } from "@mui/icons-material";
import { useEffect, useState, } from "react";
import { exportProducts, exportProductsWithMarkup, getExportFields, type ExportField, } from "../../../api/exportApi";
import { downloadBlob, } from "../../../utils/downloadBlob";
import { useAuth } from "../../../context/AuthContext";

type ProductExportDialogProps = {
    open: boolean;
    onClose: () => void;
};

const ProductExportDialog = ({
    open,
    onClose,
}: ProductExportDialogProps) => {
    const { isAdmin } = useAuth();

    const [applyMarkup, setApplyMarkup] =
        useState(false);

    const [markupPercentage, setMarkupPercentage] =
        useState("10");

    const [fields, setFields] =
        useState<ExportField[]>([]);

    const [
        selectedFields,
        setSelectedFields,
    ] = useState<string[]>([]);

    const [loadingFields, setLoadingFields] =
        useState(false);

    const [downloading, setDownloading] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    const parsedMarkupPercentage =
        Number(markupPercentage);

    const markupPercentageIsInvalid =
        isAdmin &&
        applyMarkup &&
        (
            markupPercentage.trim() === "" ||
            !Number.isFinite(
                parsedMarkupPercentage
            ) ||
            parsedMarkupPercentage <= 0
        );

    useEffect(() => {
        if (!open) {
            return;
        }

        setApplyMarkup(false);
        setMarkupPercentage("10");

        let active = true;

        const loadFields = async () => {
            try {
                setLoadingFields(true);
                setError(null);

                const availableFields =
                    await getExportFields();

                if (!active) {
                    return;
                }

                setFields(availableFields);

                setSelectedFields(
                    availableFields.map(
                        field => field.key
                    )
                );
            } catch {
                if (active) {
                    setError(
                        "The export fields could not be loaded."
                    );
                }
            } finally {
                if (active) {
                    setLoadingFields(false);
                }
            }
        };

        void loadFields();

        return () => {
            active = false;
        };
    }, [open]);

    const toggleField = (
        fieldKey: string
    ) => {
        setSelectedFields(current =>
            current.includes(fieldKey)
                ? current.filter(
                    key => key !== fieldKey
                )
                : [...current, fieldKey]
        );
    };

    const selectAllFields = () => {
        setSelectedFields(
            fields.map(field => field.key)
        );
    };

    const clearFields = () => {
        setSelectedFields([]);
    };

    const handleDownload = async () => {
        if (selectedFields.length === 0) {
            setError(
                "Select at least one field."
            );
            return;
        }

        const percentage =
            Number(markupPercentage);

        if (
            isAdmin &&
            applyMarkup &&
            (
                markupPercentage.trim() === "" ||
                !Number.isFinite(percentage) ||
                percentage <= 0
            )
        ) {
            setError(
                "Enter a percentage greater than 0."
            );
            return;
        }

        try {
            setDownloading(true);
            setError(null);

            const file =
                isAdmin && applyMarkup
                    ? await exportProductsWithMarkup(
                        selectedFields,
                        percentage
                    )
                    : await exportProducts(
                        selectedFields
                    );

            const fileName =
                isAdmin && applyMarkup
                    ? "products-with-markup.csv"
                    : "products.csv";

            downloadBlob(
                file,
                fileName
            );

            onClose();
        } catch {
            setError(
                "The product export could not be downloaded."
            );
        } finally {
            setDownloading(false);
        }
    };

    const handleClose = () => {
        if (!downloading) {
            onClose();
        }
    };

    return (
        <Dialog
            open={open}
            onClose={handleClose}
            fullWidth
            maxWidth="sm"
        >
            <DialogTitle>
                Export products
            </DialogTitle>

            <DialogContent dividers>
                <Stack spacing={2.5}>
                    <Typography
                        color="text.secondary"
                    >
                        Select the product fields
                        to include in the CSV file.
                    </Typography>

                    {error && (
                        <Alert severity="error">
                            {error}
                        </Alert>
                    )}

                    {loadingFields ? (
                        <Stack
                            direction="row"
                            spacing={1.5}
                            sx={{
                                alignItems: "center",
                                py: 2,
                            }}
                        >
                            <CircularProgress
                                size={22}
                            />

                            <Typography>
                                Loading export
                                fields...
                            </Typography>
                        </Stack>
                    ) : (
                        <>
                            <Stack
                                direction="row"
                                spacing={1}
                            >
                                <Button
                                    size="small"
                                    onClick={
                                        selectAllFields
                                    }
                                >
                                    Select all
                                </Button>

                                <Button
                                    size="small"
                                    color="inherit"
                                    onClick={
                                        clearFields
                                    }
                                >
                                    Clear
                                </Button>
                            </Stack>

                            <Stack spacing={0.5}>
                                {fields.map(field => (
                                    <FormControlLabel
                                        key={field.key}
                                        control={
                                            <Checkbox
                                                checked={
                                                    selectedFields
                                                        .includes(
                                                            field.key
                                                        )
                                                }
                                                onChange={() =>
                                                    toggleField(
                                                        field.key
                                                    )
                                                }
                                            />
                                        }
                                        label={field.label}
                                    />
                                ))}
                            </Stack>
                        </>
                    )}

                    {isAdmin && (
                        <>
                            <Divider />

                            <Stack spacing={1.5}>
                                <FormControlLabel
                                    control={
                                        <Switch
                                            checked={applyMarkup}
                                            disabled={downloading}
                                            onChange={event => {
                                                setApplyMarkup(
                                                    event.target.checked
                                                );

                                                setError(null);
                                            }}
                                        />
                                    }
                                    label="Apply percentage markup"
                                />

                                {applyMarkup && (
                                    <TextField
                                        label="Percentage markup"
                                        type="number"
                                        value={markupPercentage}
                                        onChange={event => {
                                            setMarkupPercentage(
                                                event.target.value
                                            );

                                            setError(null);
                                        }}
                                        slotProps={{
                                            htmlInput: {
                                                min: 0.1,
                                                step: 0.1,
                                            },
                                        }}
                                        error={markupPercentageIsInvalid}
                                        helperText={
                                            markupPercentageIsInvalid
                                                ? "Enter a percentage greater than 0."
                                                : "The percentage will be added to every exported price."
                                        }
                                        disabled={downloading}
                                        fullWidth
                                    />
                                )}
                            </Stack>
                        </>
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
                    disabled={downloading}
                >
                    Cancel
                </Button>

                <Button
                    variant="contained"
                    startIcon={
                        downloading
                            ? (
                                <CircularProgress
                                    size={18}
                                    color="inherit"
                                />
                            )
                            : <Download />
                    }
                    disabled={
                        loadingFields ||
                        downloading ||
                        selectedFields.length === 0 ||
                        markupPercentageIsInvalid
                    }
                    onClick={() =>
                        void handleDownload()
                    }
                >
                    {downloading
                        ? "Preparing..."
                        : "Download CSV"}
                </Button>
            </DialogActions>
        </Dialog>
    );
};

export default ProductExportDialog;