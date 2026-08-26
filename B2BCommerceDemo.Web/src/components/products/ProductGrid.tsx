import { Grid } from "@mui/material";
import type { Product } from "../../api/productApi";
import ProductCard from "./ProductCard";

type ProductGridProps = {
    products: Product[];
};

const ProductGrid = ({ products }: ProductGridProps) => {
    return (
        <Grid container spacing={3}>
            {products.map((product) => (
                <Grid key={product.id} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
                    <ProductCard product={product} />
                </Grid>
            ))}
        </Grid>
    );
};

export default ProductGrid;