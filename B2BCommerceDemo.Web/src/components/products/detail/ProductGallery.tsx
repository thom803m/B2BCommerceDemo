import { ArrowBackIosNew, ArrowForwardIos, BrokenImageOutlined, } from "@mui/icons-material";
import { Box, IconButton, Stack, Typography, } from "@mui/material";
import { useMemo, useState, } from "react";
import type { Product } from "../../../api/productApi";

type ProductGalleryProps = {
    product: Product;
};

const ProductGalleryContent = ({
    product,
}: ProductGalleryProps) => {
    const imageUrls = useMemo<string[]>(() => {
        return [...(product.images ?? [])]
            .sort(
                (firstImage, secondImage) =>
                    Number(secondImage.isPrimary) -
                    Number(firstImage.isPrimary)
            )
            .map((image) => image.url?.trim())
            .filter(
                (url): url is string =>
                    typeof url === "string" &&
                    url.length > 0
            );
    }, [product.images]);

    const [selectedIndex, setSelectedIndex] =
        useState(0);

    const [failedImages, setFailedImages] =
        useState<string[]>([]);

    const availableImages = imageUrls.filter(
        (url) => !failedImages.includes(url)
    );

    const safeSelectedIndex =
        availableImages.length === 0
            ? 0
            : Math.min(
                selectedIndex,
                availableImages.length - 1
            );

    const displayedImage =
        availableImages[safeSelectedIndex] ?? "";

    const hasMultipleImages =
        availableImages.length > 1;

    const showPreviousImage = () => {
        if (!hasMultipleImages) {
            return;
        }

        setSelectedIndex((currentIndex) =>
            currentIndex === 0
                ? availableImages.length - 1
                : currentIndex - 1
        );
    };

    const showNextImage = () => {
        if (!hasMultipleImages) {
            return;
        }

        setSelectedIndex((currentIndex) =>
            currentIndex ===
                availableImages.length - 1
                ? 0
                : currentIndex + 1
        );
    };

    const handleImageError = (
        imageUrl: string
    ) => {
        setFailedImages((currentImages) =>
            currentImages.includes(imageUrl)
                ? currentImages
                : [...currentImages, imageUrl]
        );

        setSelectedIndex(0);
    };

    return (
        <Stack spacing={2}>
            <Box
                sx={{
                    position: "relative",
                    minHeight: {
                        xs: 300,
                        sm: 360,
                        md: 390,
                    },
                    aspectRatio: "1 / 1",
                    border: "1px solid",
                    borderColor: "divider",
                    borderRadius: 4,
                    bgcolor: "background.paper",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    p: {
                        xs: 2,
                        sm: 3,
                    },
                    overflow: "hidden",
                }}
            >
                {displayedImage ? (
                    <Box
                        component="img"
                        src={displayedImage}
                        alt={`${product.name} - image ${safeSelectedIndex + 1
                            }`}
                        onError={() =>
                            handleImageError(
                                displayedImage
                            )
                        }
                        sx={{
                            width: "100%",
                            height: "100%",
                            maxHeight: {
                                xs: 260,
                                sm: 310,
                                md: 340,
                            },
                            objectFit: "contain",
                            transition:
                                "transform 180ms ease",
                            "&:hover": {
                                transform:
                                    "scale(1.025)",
                            },
                        }}
                    />
                ) : (
                    <Stack
                        spacing={1.5}
                        sx={{
                            alignItems: "center",
                            color: "text.secondary",
                            textAlign: "center",
                        }}
                    >
                        <BrokenImageOutlined
                            sx={{
                                fontSize: 56,
                                opacity: 0.7,
                            }}
                        />

                        <Typography variant="body2">
                            No product image available
                        </Typography>
                    </Stack>
                )}

                {hasMultipleImages && (
                    <>
                        <IconButton
                            onClick={showPreviousImage}
                            aria-label="Show previous product image"
                            sx={{
                                position: "absolute",
                                top: "50%",
                                left: 12,
                                transform: "translateY(-50%)",
                                width: 40,
                                height: 40,
                                bgcolor: "rgba(255, 255, 255, 0.9)",
                                border: "1px solid",
                                borderColor: "divider",
                                boxShadow: 1,
                                "&:hover": {
                                    bgcolor:
                                        "background.paper",
                                },
                            }}
                        >
                            <ArrowBackIosNew
                                sx={{ fontSize: 18 }}
                            />
                        </IconButton>

                        <IconButton
                            onClick={showNextImage}
                            aria-label="Show next product image"
                            sx={{
                                position: "absolute",
                                top: "50%",
                                right: 12,
                                transform: "translateY(-50%)",
                                width: 40,
                                height: 40,
                                bgcolor: "rgba(255, 255, 255, 0.9)",
                                border: "1px solid",
                                borderColor: "divider",
                                boxShadow: 1,
                                "&:hover": {
                                    bgcolor:
                                        "background.paper",
                                },
                            }}
                        >
                            <ArrowForwardIos
                                sx={{ fontSize: 18 }}
                            />
                        </IconButton>

                        <Box
                            sx={{
                                position: "absolute",
                                right: 14,
                                bottom: 14,
                                px: 1.25,
                                py: 0.5,
                                borderRadius: 5,
                                bgcolor:
                                    "rgba(15, 23, 42, 0.78)",
                                color: "common.white",
                            }}
                        >
                            <Typography
                                variant="caption"
                                sx={{
                                    display: "block",
                                    fontWeight: 700,
                                    lineHeight: 1.4,
                                }}
                            >
                                {safeSelectedIndex + 1} /{" "}
                                {availableImages.length}
                            </Typography>
                        </Box>
                    </>
                )}
            </Box>

            {hasMultipleImages && (
                <Stack
                    direction="row"
                    spacing={1.25}
                    sx={{
                        overflowX: "auto",
                        pb: 0.5,
                        scrollbarWidth: "none",
                        "&::-webkit-scrollbar": {
                            display: "none",
                        },
                    }}
                >
                    {availableImages.map(
                        (url, index) => {
                            const isSelected =
                                index ===
                                safeSelectedIndex;

                            return (
                                <Box
                                    key={`${url}-${index}`}
                                    component="button"
                                    type="button"
                                    onClick={() =>
                                        setSelectedIndex(
                                            index
                                        )
                                    }
                                    aria-label={`Show product image ${index + 1
                                        }`}
                                    aria-pressed={
                                        isSelected
                                    }
                                    sx={{
                                        width: 72,
                                        height: 72,
                                        flexShrink: 0,
                                        border: "2px solid",
                                        borderColor:
                                            isSelected
                                                ? "primary.main"
                                                : "divider",
                                        borderRadius: 2.5,
                                        bgcolor:
                                            "background.paper",
                                        cursor: "pointer",
                                        p: 0.75,
                                        opacity: isSelected
                                            ? 1
                                            : 0.72,
                                        transition:
                                            "border-color 150ms ease, opacity 150ms ease, transform 150ms ease",
                                        "&:hover": {
                                            opacity: 1,
                                            transform:
                                                "translateY(-2px)",
                                            borderColor:
                                                isSelected
                                                    ? "primary.main"
                                                    : "text.secondary",
                                        },
                                        "&:focus-visible": {
                                            outline:
                                                "3px solid",
                                            outlineColor:
                                                "primary.light",
                                            outlineOffset: 2,
                                        },
                                    }}
                                >
                                    <Box
                                        component="img"
                                        src={url}
                                        alt=""
                                        onError={() =>
                                            handleImageError(
                                                url
                                            )
                                        }
                                        sx={{
                                            width: "100%",
                                            height: "100%",
                                            objectFit:
                                                "contain",
                                        }}
                                    />
                                </Box>
                            );
                        }
                    )}
                </Stack>
            )}
        </Stack>
    );
};

const ProductGallery = ({
    product,
}: ProductGalleryProps) => {
    return (
        <ProductGalleryContent
            key={product.id}
            product={product}
        />
    );
};

export default ProductGallery;