import { Box, Card, CardActionArea, CardContent, Chip, Stack, } from "@mui/material";
import { Link, useLocation, } from "react-router-dom";
import { getPrimaryImage, type Product } from "../../api/productApi";
import ProductActions from "./ProductActions";
import ProductImage from "./ProductImage";
import ProductInfo from "./ProductInfo";
import ProductStockBadge from "./ProductStockBadge";
import ProductPrice from "./ProductPrice";

type ProductCardProps = {
    product: Product;
};

const ProductCard = ({ product }: ProductCardProps) => {
    const isInStock = product.availableStock > 0;
    const primaryImage = getPrimaryImage(product);
    const location = useLocation();
    const productListUrl = `${location.pathname}${location.search}`;

    return (
        <Card
            elevation={0}
            sx={{
                height: "100%",
                display: "flex",
                flexDirection: "column",
                border: "1px solid",
                borderColor: "divider",
                borderRadius: 3,
                overflow: "hidden",
                transition: "transform 200ms ease, box-shadow 200ms ease",
                "&:hover": {
                    transform: "translateY(-5px)",
                    boxShadow: "0 18px 42px rgba(15, 23, 42, 0.12)",

                    "& img": {
                        transform: "scale(1.04)",
                    },
                },
            }}
        >
            <CardActionArea
                component={Link}
                to={`/products/${product.id}`}
                state={{ productListUrl }}
                aria-label={`View ${product.name}`}
                sx={{
                    flexGrow: 1,
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "stretch",
                }}
            >
                <ProductImage
                    imageUrl={primaryImage}
                    alt={product.name}
                />

                <CardContent
                    sx={{
                        width: "100%",
                        flexGrow: 1,
                        display: "flex",
                        flexDirection: "column",
                        gap: 2,
                    }}
                >
                    <ProductInfo
                        name={product.name}
                        sku={product.sku}
                        brand={product.brand?.name}
                    />

                    <Stack
                        direction="row"
                        spacing={1}
                        sx={{
                            flexWrap: "wrap",
                            gap: 1,
                        }}
                    >
                        <ProductStockBadge
                            availableStock={
                                product.availableStock
                            }
                            purchasedQuantity={
                                product.purchasedQuantity
                            }
                            expectedDeliveryDate={
                                product.expectedDeliveryDate
                            }
                        />

                        {product.category?.name && (
                            <Chip
                                size="small"
                                label={product.category.name}
                                variant="outlined"
                            />
                        )}
                    </Stack>

                    <Box
                        sx={{
                            mt: "auto",
                            pt: 2,
                        }}
                    >
                        <ProductPrice
                            amount={product.basePrice}
                        />
                    </Box>
                </CardContent>
            </CardActionArea>

            <Box
                sx={{
                    px: 2,
                    pb: 2,
                }}
            >
                <ProductActions
                    productId={product.id}
                    productName={product.name}
                    disabled={!isInStock}
                />
            </Box>
        </Card>
    );
};

export default ProductCard;