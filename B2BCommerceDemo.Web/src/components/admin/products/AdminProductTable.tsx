import {
    ArticleOutlined,
    DeleteOutlined,
    Edit,
    Inventory2,
    Visibility,
} from "@mui/icons-material";
import {
    Box,
    Button,
    Chip,
    Paper,
    Stack,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Typography,
} from "@mui/material";
import { Link, } from "react-router-dom";
import { getPrimaryImage, type Product, } from "../../../api/productApi";
import ProductStockBadge from "../../products/ProductStockBadge";
import { formatPrice } from "../../../utils/formatPrice";

type AdminProductTableProps = {
    products: Product[];
    deletingProductId?: number | null;
    productListUrl: string;
    onDelete: (
        product: Product
    ) => void;
};

const AdminProductTable = ({
    products,
    deletingProductId = null,
    productListUrl,
    onDelete,
}: AdminProductTableProps) => {
    return (
        <TableContainer
            component={Paper}
            variant="outlined"
        >
            <Table sx={{ minWidth: 1200 }}>
                <TableHead>
                    <TableRow>
                        <TableCell>
                            Product
                        </TableCell>

                        <TableCell>
                            SKU
                        </TableCell>

                        <TableCell>
                            Brand
                        </TableCell>

                        <TableCell>
                            Category
                        </TableCell>

                        <TableCell>
                            Stock
                        </TableCell>

                        <TableCell align="right">
                            Base price
                        </TableCell>

                        <TableCell>
                            Content
                        </TableCell>

                        <TableCell>
                            Status
                        </TableCell>

                        <TableCell align="right">
                            Actions
                        </TableCell>
                    </TableRow>
                </TableHead>

                <TableBody>
                    {products.map((product) => {
                        const imageUrl =
                            getPrimaryImage(
                                product
                            );

                        const isDeleting =
                            deletingProductId ===
                            product.id;

                        const isActive =
                            product.isActive !==
                            false;

                        return (
                            <TableRow
                                key={product.id}
                                hover
                            >
                                <TableCell>
                                    <Stack
                                        direction="row"
                                        spacing={2}
                                        sx={{
                                            alignItems:
                                                "center",
                                            minWidth:
                                                260,
                                        }}
                                    >
                                        <Box
                                            sx={{
                                                width: 64,
                                                height: 64,
                                                borderRadius:
                                                    2,
                                                border:
                                                    "1px solid",
                                                borderColor:
                                                    "divider",
                                                bgcolor:
                                                    "grey.50",
                                                display:
                                                    "grid",
                                                placeItems:
                                                    "center",
                                                overflow:
                                                    "hidden",
                                                flexShrink: 0,
                                            }}
                                        >
                                            {imageUrl ? (
                                                <Box
                                                    component="img"
                                                    src={
                                                        imageUrl
                                                    }
                                                    alt={
                                                        product.name
                                                    }
                                                    sx={{
                                                        width:
                                                            "100%",
                                                        height:
                                                            "100%",
                                                        objectFit:
                                                            "contain",
                                                        p: 0.75,
                                                    }}
                                                />
                                            ) : (
                                                <Inventory2
                                                    sx={{
                                                        color:
                                                            "grey.400",
                                                    }}
                                                />
                                            )}
                                        </Box>

                                        <Box
                                            sx={{
                                                minWidth: 0,
                                            }}
                                        >
                                            <Typography
                                                sx={{
                                                    fontWeight: 700,
                                                    display:
                                                        "-webkit-box",
                                                    WebkitLineClamp: 2,
                                                    WebkitBoxOrient:
                                                        "vertical",
                                                    overflow:
                                                        "hidden",
                                                }}
                                            >
                                                {
                                                    product.name
                                                }
                                            </Typography>

                                            <Typography
                                                variant="body2"
                                                color="text.secondary"
                                            >
                                                ID:{" "}
                                                {
                                                    product.id
                                                }
                                            </Typography>
                                        </Box>
                                    </Stack>
                                </TableCell>

                                <TableCell
                                    sx={{
                                        whiteSpace:
                                            "nowrap",
                                    }}
                                >
                                    {product.sku}
                                </TableCell>

                                <TableCell>
                                    {product.brand
                                        ?.name ??
                                        "Not assigned"}
                                </TableCell>

                                <TableCell>
                                    {product.category
                                        ?.name ??
                                        "Not assigned"}
                                </TableCell>

                                <TableCell>
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
                                </TableCell>

                                <TableCell
                                    align="right"
                                    sx={{
                                        whiteSpace:
                                            "nowrap",
                                        fontWeight: 700,
                                    }}
                                >
                                    {formatPrice(
                                        product.basePrice
                                    )}
                                </TableCell>

                                <TableCell>
                                    <Chip
                                        label={
                                            product.contentSource ??
                                            "None"
                                        }
                                        size="small"
                                        variant="outlined"
                                    />
                                </TableCell>

                                <TableCell>
                                    <Chip
                                        label={
                                            isActive
                                                ? "Active"
                                                : "Inactive"
                                        }
                                        color={
                                            isActive
                                                ? "success"
                                                : "default"
                                        }
                                        size="small"
                                        variant="outlined"
                                    />
                                </TableCell>

                                <TableCell align="right">
                                    <Stack
                                        direction="row"
                                        spacing={0.5}
                                        sx={{
                                            justifyContent:
                                                "flex-end",
                                        }}
                                    >
                                        <Button
                                            component={Link}
                                            to={`/products/${product.id}`}
                                            state={{
                                                productListUrl,
                                            }}
                                            size="small"
                                            startIcon={
                                                <Visibility />
                                            }
                                        >
                                            View
                                        </Button>

                                        <Button
                                            component={Link}
                                            to={`/admin/products/${product.id}/content`}
                                            state={{
                                                productListUrl,
                                            }}
                                            size="small"
                                            startIcon={
                                                <ArticleOutlined />
                                            }
                                        >
                                            Content
                                        </Button>

                                        <Button
                                            component={Link}
                                            to={`/admin/products/${product.id}`}
                                            state={{
                                                productListUrl,
                                            }}
                                            size="small"
                                            startIcon={
                                                <Edit />
                                            }
                                        >
                                            Edit
                                        </Button>

                                        <Button
                                            color="error"
                                            size="small"
                                            startIcon={
                                                <DeleteOutlined />
                                            }
                                            onClick={() =>
                                                onDelete(
                                                    product
                                                )
                                            }
                                            disabled={isDeleting}
                                        >
                                            {isDeleting
                                                ? "Deleting..."
                                                : "Delete"}
                                        </Button>
                                    </Stack>
                                </TableCell>
                            </TableRow>
                        );
                    })}
                </TableBody>
            </Table>
        </TableContainer>
    );
};

export default AdminProductTable;