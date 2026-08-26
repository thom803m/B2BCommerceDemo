import { useState } from "react";
import { Box, CardMedia } from "@mui/material";
import { Inventory2 } from "@mui/icons-material";

type ProductImageProps = {
    imageUrl?: string;
    alt: string;
};

const ProductImage = ({ imageUrl, alt }: ProductImageProps) => {
    const [imageError, setImageError] = useState(false);

    const showImage = imageUrl && !imageError;

    return (
        <CardMedia
            component="div"
            sx={{
                height: 180,
                bgcolor: "grey.100",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                p: 2,
            }}
        >
            {showImage ? (
                <Box
                    component="img"
                    src={imageUrl}
                    alt={alt}
                    onError={() => setImageError(true)}
                    sx={{
                        maxWidth: "100%",
                        maxHeight: "100%",
                        objectFit: "contain",
                        transition: "transform 220ms ease",
                    }}
                />
            ) : (
                <Box
                    sx={{
                        width: 72,
                        height: 72,
                        borderRadius: 3,
                        bgcolor: "white",
                        display: "flex",
                        alignItems: "center",
                        justifyContent: "center",
                        border: "1px solid",
                        borderColor: "divider",
                    }}
                >
                    <Inventory2 sx={{ fontSize: 42, color: "grey.400" }} />
                </Box>
            )}
        </CardMedia>
    );
};

export default ProductImage;