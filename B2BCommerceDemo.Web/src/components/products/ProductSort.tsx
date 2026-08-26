import {
    FormControl,
    InputLabel,
    MenuItem,
    Select,
    type SelectChangeEvent,
} from "@mui/material";

export type ProductSortValue =
    | "name-asc"
    | "price-asc"
    | "price-desc"
    | "stock-desc";

type ProductSortProps = {
    value: ProductSortValue;
    onChange: (value: ProductSortValue) => void;
};

const ProductSort = ({ value, onChange }: ProductSortProps) => {
    const handleChange = (event: SelectChangeEvent) => {
        onChange(event.target.value as ProductSortValue);
    };

    return (
        <FormControl size="small" sx={{ minWidth: 220 }}>
            <InputLabel>Sort by</InputLabel>
            <Select value={value} label="Sort by" onChange={handleChange}>
                <MenuItem value="name-asc">Name A-Z</MenuItem>
                <MenuItem value="price-asc">Price: Low to high</MenuItem>
                <MenuItem value="price-desc">Price: High to low</MenuItem>
                <MenuItem value="stock-desc">Stock: High to low</MenuItem>
            </Select>
        </FormControl>
    );
};

export default ProductSort;