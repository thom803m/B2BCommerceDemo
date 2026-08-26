import {
    Box,
    Button,
    Card,
    CardContent,
    Checkbox,
    FormControl,
    FormControlLabel,
    InputLabel,
    MenuItem,
    Select,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { FilterAlt, RestartAlt } from "@mui/icons-material";

export type ProductFilterValues = {
    search: string;
    brand: string;
    category: string;
    inStock: boolean;
};

type FilterOption = {
    id: number;
    name: string;
};

type ProductFiltersProps = {
    values: ProductFilterValues;
    brands: FilterOption[];
    categories: FilterOption[];
    optionsLoading?: boolean;
    onChange: (values: ProductFilterValues) => void;
    onApply: () => void;
    onReset: () => void;
};

const ProductFilters = ({
    values,
    brands,
    categories,
    optionsLoading = false,
    onChange,
    onApply,
    onReset,
}: ProductFiltersProps) => {
    const updateField = (
        field: keyof ProductFilterValues,
        value: string | boolean
    ) => {
        onChange({
            ...values,
            [field]: value,
        });
    };

    return (
        <Card
            elevation={0}
            sx={{
                border: "1px solid",
                borderColor: "divider",
                borderRadius: 3,
            }}
        >
            <CardContent>
                <Stack spacing={2.5}>
                    <Box>
                        <Typography variant="h6" sx={{ fontWeight: 800 }}>
                            Filters
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                            Narrow down products by search, brand, category and stock.
                        </Typography>
                    </Box>

                    <TextField
                        label="Search"
                        placeholder="Search products..."
                        value={values.search}
                        onChange={(e) => updateField("search", e.target.value)}
                        fullWidth
                    />

                    <FormControl fullWidth disabled={optionsLoading}>
                        <InputLabel id="brand-filter-label">Brand</InputLabel>

                        <Select
                            labelId="brand-filter-label"
                            label="Brand"
                            value={values.brand}
                            onChange={(e) =>
                                updateField(
                                    "brand",
                                    e.target.value
                                )
                            }
                            MenuProps={{
                                variant: "menu",
                                anchorOrigin: {
                                    vertical: "bottom",
                                    horizontal: "left",
                                },
                                transformOrigin: {
                                    vertical: "top",
                                    horizontal: "left",
                                },
                                slotProps: {
                                    paper: {
                                        style: {
                                            maxHeight: 280,
                                        },
                                        sx: {
                                            mt: 0.75,
                                            borderRadius: 2,
                                            overflowY: "auto",
                                        },
                                    },
                                },
                            }}
                        >
                            <MenuItem value="">
                                <em>All brands</em>
                            </MenuItem>

                            {brands.map((brand) => (
                                <MenuItem
                                    key={brand.id}
                                    value={brand.name}
                                >
                                    {brand.name}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    <FormControl fullWidth disabled={optionsLoading}>
                        <InputLabel id="category-filter-label">Category</InputLabel>

                        <Select
                            labelId="category-filter-label"
                            label="Category"
                            value={values.category}
                            onChange={(e) =>
                                updateField(
                                    "category",
                                    e.target.value
                                )
                            }
                            MenuProps={{
                                variant: "menu",
                                anchorOrigin: {
                                    vertical: "bottom",
                                    horizontal: "left",
                                },
                                transformOrigin: {
                                    vertical: "top",
                                    horizontal: "left",
                                },
                                slotProps: {
                                    paper: {
                                        style: {
                                            maxHeight: 280,
                                        },
                                        sx: {
                                            mt: 0.75,
                                            borderRadius: 2,
                                            overflowY: "auto",
                                        },
                                    },
                                },
                            }}
                        >
                            <MenuItem value="">
                                <em>All categories</em>
                            </MenuItem>

                            {categories.map((category) => (
                                <MenuItem
                                    key={category.id}
                                    value={category.name}
                                >
                                    {category.name}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>

                    <FormControlLabel
                        control={
                            <Checkbox
                                checked={values.inStock}
                                onChange={(e) => updateField("inStock", e.target.checked)}
                            />
                        }
                        label="Only show products in stock"
                    />

                    <Stack spacing={1.25}>
                        <Button
                            variant="contained"
                            startIcon={<FilterAlt />}
                            onClick={onApply}
                            fullWidth
                        >
                            Apply filters
                        </Button>

                        <Button
                            variant="outlined"
                            startIcon={<RestartAlt />}
                            onClick={onReset}
                            fullWidth
                        >
                            Reset
                        </Button>
                    </Stack>
                </Stack>
            </CardContent>
        </Card>
    );
};

export default ProductFilters;