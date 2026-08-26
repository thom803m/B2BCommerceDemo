import { Save } from "@mui/icons-material";
import {
    Alert,
    Button,
    CircularProgress,
    Divider,
    Grid,
    MenuItem,
    Paper,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { type ChangeEvent, type FormEvent, useEffect, useState, } from "react";
import axiosInstance from "../../api/axios";
import type {
    ProductWriteRequest,
} from "../../api/productApi";

type SelectOption = {
    id: number;
    name: string;
};

type ProductFormProps = {
    initialData?: Partial<ProductWriteRequest>;
    onSubmit: (
        values: ProductWriteRequest
    ) => void | Promise<void>;
};

type ProductFieldErrors = Partial<
    Record<keyof ProductWriteRequest, string>
>;

const defaultValues: ProductWriteRequest = {
    sku: "",
    name: "",
    ean: "",
    basePrice: 0,
    availableStock: 0,
    brandId: 0,
    categoryId: 0,
};

const ProductForm = ({
    initialData,
    onSubmit,
}: ProductFormProps) => {
    const [brands, setBrands] =
        useState<SelectOption[]>([]);

    const [categories, setCategories] =
        useState<SelectOption[]>([]);

    const [form, setForm] =
        useState<ProductWriteRequest>({
            ...defaultValues,
            ...initialData,
        });

    const [fieldErrors, setFieldErrors] =
        useState<ProductFieldErrors>({});

    const [optionsLoading, setOptionsLoading] =
        useState(true);

    const [optionsError, setOptionsError] =
        useState<string | null>(null);

    const [submitting, setSubmitting] =
        useState(false);

    const isEditing = Boolean(initialData);

    useEffect(() => {
        const loadOptions = async () => {
            setOptionsLoading(true);
            setOptionsError(null);

            try {
                const [
                    brandsResponse,
                    categoriesResponse,
                ] = await Promise.all([
                    axiosInstance.get<
                        SelectOption[]
                    >("/brands"),

                    axiosInstance.get<
                        SelectOption[]
                    >("/categories"),
                ]);

                setBrands(
                    brandsResponse.data
                );

                setCategories(
                    categoriesResponse.data
                );
            } catch (error) {
                console.error(
                    "Failed to load product form options",
                    error
                );

                setOptionsError(
                    "Brands and categories could not be loaded. Please refresh the page and try again."
                );
            } finally {
                setOptionsLoading(false);
            }
        };

        void loadOptions();
    }, []);

    useEffect(() => {
        setForm({
            ...defaultValues,
            ...initialData,
        });

        setFieldErrors({});
    }, [initialData]);

    const handleChange = (
        event: ChangeEvent<
            HTMLInputElement |
            HTMLTextAreaElement
        >
    ) => {
        const {
            name,
            value,
        } = event.target;

        const fieldName =
            name as keyof ProductWriteRequest;

        const numericFields:
            (keyof ProductWriteRequest)[] = [
                "basePrice",
                "availableStock",
                "brandId",
                "categoryId",
            ];

        setForm((current) => ({
            ...current,
            [fieldName]:
                numericFields.includes(
                    fieldName
                )
                    ? Number(value)
                    : value,
        }));

        setFieldErrors((current) => ({
            ...current,
            [fieldName]: undefined,
        }));
    };

    const validateForm = () => {
        const errors: ProductFieldErrors = {};

        if (!form.sku.trim()) {
            errors.sku =
                "SKU is required.";
        }

        if (!form.name.trim()) {
            errors.name =
                "Product name is required.";
        }

        if (!form.ean.trim()) {
            errors.ean =
                "EAN is required.";
        }

        if (
            !Number.isFinite(
                Number(form.basePrice)
            ) ||
            Number(form.basePrice) <= 0
        ) {
            errors.basePrice =
                "Base price must be greater than zero.";
        }

        if (
            !Number.isInteger(
                Number(form.availableStock)
            ) ||
            Number(form.availableStock) < 0
        ) {
            errors.availableStock =
                "Available stock must be a whole number of zero or greater.";
        }

        if (Number(form.brandId) <= 0) {
            errors.brandId =
                "Please select a brand.";
        }

        if (Number(form.categoryId) <= 0) {
            errors.categoryId =
                "Please select a category.";
        }

        setFieldErrors(errors);

        return Object.keys(errors).length === 0;
    };

    const handleSubmit = async (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        if (
            submitting ||
            !validateForm()
        ) {
            return;
        }

        setSubmitting(true);

        try {
            await onSubmit({
                ...form,
                sku: form.sku.trim(),
                name: form.name.trim(),
                ean: form.ean.trim(),
                basePrice: Number(
                    form.basePrice
                ),
                availableStock: Number(
                    form.availableStock
                ),
                brandId: Number(
                    form.brandId
                ),
                categoryId: Number(
                    form.categoryId
                ),
            });
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <Paper
            component="form"
            variant="outlined"
            onSubmit={handleSubmit}
            noValidate
            sx={{
                p: {
                    xs: 2,
                    md: 4,
                },
            }}
        >
            <Stack spacing={0.5}>
                <Typography
                    variant="h5"
                    component="h2"
                    sx={{
                        fontWeight: 800,
                    }}
                >
                    Product information
                </Typography>

                <Typography
                    color="text.secondary"
                >
                    Enter the catalogue,
                    pricing and stock
                    information for the
                    product.
                </Typography>
            </Stack>

            <Divider sx={{ my: 3 }} />

            {optionsError && (
                <Alert
                    severity="error"
                    sx={{ mb: 3 }}
                >
                    {optionsError}
                </Alert>
            )}

            <Grid
                container
                spacing={2.5}
            >
                <Grid
                    size={{
                        xs: 12,
                        md: 4,
                    }}
                >
                    <TextField
                        fullWidth
                        required
                        name="sku"
                        label="SKU"
                        value={form.sku}
                        onChange={handleChange}
                        error={Boolean(
                            fieldErrors.sku
                        )}
                        helperText={
                            fieldErrors.sku ??
                            "The product number."
                        }
                        disabled={submitting}
                    />
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        md: 8,
                    }}
                >
                    <TextField
                        fullWidth
                        required
                        name="name"
                        label="Product name"
                        value={form.name}
                        onChange={handleChange}
                        error={Boolean(
                            fieldErrors.name
                        )}
                        helperText={
                            fieldErrors.name ??
                            "The name displayed in the webshop."
                        }
                        disabled={submitting}
                    />
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        md: 4,
                    }}
                >
                    <TextField
                        fullWidth
                        required
                        name="ean"
                        label="EAN"
                        value={form.ean}
                        onChange={handleChange}
                        error={Boolean(
                            fieldErrors.ean
                        )}
                        helperText={
                            fieldErrors.ean ??
                            "The product's barcode number."
                        }
                        disabled={submitting}
                        slotProps={{
                            htmlInput: {
                                inputMode:
                                    "numeric",
                            },
                        }}
                    />
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        sm: 6,
                        md: 4,
                    }}
                >
                    <TextField
                        fullWidth
                        required
                        name="basePrice"
                        label="Base price"
                        type="number"
                        value={
                            form.basePrice
                        }
                        onChange={handleChange}
                        error={Boolean(
                            fieldErrors.basePrice
                        )}
                        helperText={
                            fieldErrors.basePrice ??
                            "Price excluding VAT."
                        }
                        disabled={submitting}
                        slotProps={{
                            htmlInput: {
                                min: 0.01,
                                step: 0.01,
                            },
                        }}
                    />
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        sm: 6,
                        md: 4,
                    }}
                >
                    <TextField
                        fullWidth
                        required
                        name="availableStock"
                        label="Available stock"
                        type="number"
                        value={form.availableStock}
                        onChange={handleChange}
                        error={Boolean(
                            fieldErrors.availableStock
                        )}
                        helperText={
                            fieldErrors.availableStock ??
                            (isEditing
                                ? "Stock is synchronized from Rackbeat and cannot be edited manually."
                                : "Initial available quantity.")
                        }
                        disabled={submitting}
                        slotProps={{
                            input: {
                                readOnly: isEditing,
                            },
                            htmlInput: {
                                min: 0,
                                step: 1,
                            },
                        }}
                        sx={{
                            ...(isEditing && {
                                "& .MuiInputBase-root": {
                                    bgcolor:
                                        "action.hover",
                                },
                            }),
                        }}
                    />
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        md: 6,
                    }}
                >
                    <TextField
                        select
                        fullWidth
                        required
                        name="brandId"
                        label="Brand"
                        value={form.brandId}
                        onChange={handleChange}
                        error={Boolean(
                            fieldErrors.brandId
                        )}
                        helperText={
                            fieldErrors.brandId ??
                            "Select the product manufacturer."
                        }
                        disabled={
                            optionsLoading ||
                            submitting ||
                            Boolean(optionsError)
                        }
                    >
                        <MenuItem
                            value={0}
                            disabled
                        >
                            {optionsLoading
                                ? "Loading brands..."
                                : "Select brand"}
                        </MenuItem>

                        {brands.map(
                            (brand) => (
                                <MenuItem
                                    key={
                                        brand.id
                                    }
                                    value={
                                        brand.id
                                    }
                                >
                                    {
                                        brand.name
                                    }
                                </MenuItem>
                            )
                        )}
                    </TextField>
                </Grid>

                <Grid
                    size={{
                        xs: 12,
                        md: 6,
                    }}
                >
                    <TextField
                        select
                        fullWidth
                        required
                        name="categoryId"
                        label="Category"
                        value={
                            form.categoryId
                        }
                        onChange={handleChange}
                        error={Boolean(
                            fieldErrors.categoryId
                        )}
                        helperText={
                            fieldErrors.categoryId ??
                            "Select where the product belongs in the catalogue."
                        }
                        disabled={
                            optionsLoading ||
                            submitting ||
                            Boolean(optionsError)
                        }
                    >
                        <MenuItem
                            value={0}
                            disabled
                        >
                            {optionsLoading
                                ? "Loading categories..."
                                : "Select category"}
                        </MenuItem>

                        {categories.map(
                            (category) => (
                                <MenuItem
                                    key={
                                        category.id
                                    }
                                    value={
                                        category.id
                                    }
                                >
                                    {
                                        category.name
                                    }
                                </MenuItem>
                            )
                        )}
                    </TextField>
                </Grid>
            </Grid>

            <Divider sx={{ my: 3 }} />

            <Stack
                direction="row"
                sx={{
                    justifyContent:
                        "flex-end",
                }}
            >
                <Button
                    type="submit"
                    variant="contained"
                    size="large"
                    startIcon={
                        submitting ? (
                            <CircularProgress
                                size={18}
                                color="inherit"
                            />
                        ) : (
                            <Save />
                        )
                    }
                    disabled={
                        submitting ||
                        optionsLoading ||
                        Boolean(optionsError)
                    }
                    sx={{
                        minWidth: 180,
                    }}
                >
                    {submitting
                        ? "Saving..."
                        : isEditing
                            ? "Save changes"
                            : "Create product"}
                </Button>
            </Stack>
        </Paper>
    );
};

export default ProductForm;