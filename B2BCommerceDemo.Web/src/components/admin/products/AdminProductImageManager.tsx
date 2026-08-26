import { AddPhotoAlternateOutlined, DeleteOutlined, ImageOutlined, Star, StarOutlined, } from "@mui/icons-material";
import { Alert, Box, Button, Card, CardActions, CardContent, CardMedia, Chip, Grid, Paper, Stack, TextField, Typography, } from "@mui/material";
import { type FormEvent, useEffect, useState, } from "react";
import { addProductImage, deleteProductImage, setPrimaryProductImage, type ProductImage, } from "../../../api/productApi";
import { getApiErrorMessage } from "../../../utils/getApiErrorMessage";
import ConfirmDialog from "../../common/ConfirmDialog";
import EmptyState from "../../common/EmptyState";

type AdminProductImageManagerProps = {
    productId: number;
    initialImages: ProductImage[];
    disabled?: boolean;
};

const AdminProductImageManager = ({
    productId,
    initialImages,
    disabled = false,
}: AdminProductImageManagerProps) => {
    const [images, setImages] =
        useState<ProductImage[]>(initialImages);

    const [imageUrl, setImageUrl] =
        useState("");

    const [adding, setAdding] =
        useState(false);

    const [
        processingImageId,
        setProcessingImageId,
    ] = useState<number | null>(null);

    const [
        imageToDelete,
        setImageToDelete,
    ] = useState<ProductImage | null>(null);

    const [error, setError] =
        useState<string | null>(null);

    const [
        successMessage,
        setSuccessMessage,
    ] = useState<string | null>(null);

    useEffect(() => {
        setImages(initialImages);
    }, [initialImages]);

    const handleAddImage = async (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        const normalizedUrl =
            imageUrl.trim();

        if (!normalizedUrl || adding) {
            return;
        }

        try {
            const parsedUrl =
                new URL(normalizedUrl);

            if (
                parsedUrl.protocol !== "http:" &&
                parsedUrl.protocol !== "https:"
            ) {
                throw new Error();
            }
        } catch {
            setError(
                "Please enter a valid HTTP or HTTPS image URL."
            );

            return;
        }

        setAdding(true);
        setError(null);
        setSuccessMessage(null);

        try {
            const addedImage =
                await addProductImage(
                    productId,
                    normalizedUrl
                );

            setImages((current) => {
                const alreadyExists =
                    current.some(
                        (image) =>
                            image.id ===
                            addedImage.id
                    );

                if (alreadyExists) {
                    return current;
                }

                return [
                    ...current,
                    addedImage,
                ];
            });

            setImageUrl("");

            setSuccessMessage(
                "The image was added successfully."
            );
        } catch (error) {
            console.error(
                "Failed to add product image",
                error
            );

            setError(
                getApiErrorMessage(
                    error,
                    "The image could not be added. Please try again."
                )
            );
        } finally {
            setAdding(false);
        }
    };

    const handleSetPrimary = async (
        imageId: number
    ) => {
        setProcessingImageId(imageId);
        setError(null);
        setSuccessMessage(null);

        try {
            await setPrimaryProductImage(
                productId,
                imageId
            );

            setImages((current) =>
                current.map((image) => ({
                    ...image,
                    isPrimary:
                        image.id === imageId,
                }))
            );

            setSuccessMessage(
                "The primary image was updated."
            );
        } catch (error) {
            console.error(
                "Failed to set primary image",
                error
            );

            setError(
                getApiErrorMessage(
                    error,
                    "The primary image could not be updated."
                )
            );
        } finally {
            setProcessingImageId(null);
        }
    };

    const handleConfirmDelete =
        async () => {
            if (!imageToDelete) {
                return;
            }

            const deletedImage =
                imageToDelete;

            setProcessingImageId(
                deletedImage.id
            );

            setError(null);
            setSuccessMessage(null);

            try {
                await deleteProductImage(
                    productId,
                    deletedImage.id
                );

                setImages((current) => {
                    const remaining =
                        current
                            .filter(
                                (image) =>
                                    image.id !==
                                    deletedImage.id
                            )
                            .sort(
                                (first, second) =>
                                    first.id -
                                    second.id
                            );

                    if (
                        deletedImage.isPrimary &&
                        remaining.length > 0
                    ) {
                        return remaining.map(
                            (image, index) => ({
                                ...image,
                                isPrimary:
                                    index === 0,
                            })
                        );
                    }

                    return remaining;
                });

                setImageToDelete(null);

                setSuccessMessage(
                    "The image was deleted."
                );
            } catch (error) {
                console.error(
                    "Failed to delete product image",
                    error
                );

                setError(
                    getApiErrorMessage(
                        error,
                        "The image could not be deleted."
                    )
                );
            } finally {
                setProcessingImageId(null);
            }
        };

    return (
        <>
            <Paper
                variant="outlined"
                sx={{
                    p: {
                        xs: 2,
                        md: 3,
                    },
                }}
            >
                <Box sx={{ mb: 3 }}>
                    <Typography
                        variant="h5"
                        component="h2"
                        sx={{ fontWeight: 800 }}
                    >
                        Product images
                    </Typography>

                    <Typography
                        color="text.secondary"
                        sx={{ mt: 0.5 }}
                    >
                        Add images, select the
                        primary webshop image or
                        remove images from the
                        product.
                    </Typography>
                </Box>

                {error && (
                    <Alert
                        severity="error"
                        sx={{ mb: 3 }}
                        onClose={() =>
                            setError(null)
                        }
                    >
                        {error}
                    </Alert>
                )}

                {successMessage && (
                    <Alert
                        severity="success"
                        sx={{ mb: 3 }}
                        onClose={() =>
                            setSuccessMessage(null)
                        }
                    >
                        {successMessage}
                    </Alert>
                )}

                <Box
                    component="form"
                    onSubmit={handleAddImage}
                    sx={{ mb: 4 }}
                >
                    <Stack
                        direction={{
                            xs: "column",
                            md: "row",
                        }}
                        spacing={2}
                    >
                        <TextField
                            fullWidth
                            label="Image URL"
                            placeholder="https://example.com/product-image.jpg"
                            value={imageUrl}
                            onChange={(event) => {
                                setImageUrl(
                                    event.target.value
                                );

                                setError(null);
                            }}
                            disabled={
                                disabled || adding
                            }
                        />

                        <Button
                            type="submit"
                            variant="contained"
                            startIcon={
                                <AddPhotoAlternateOutlined />
                            }
                            disabled={
                                disabled ||
                                adding ||
                                !imageUrl.trim()
                            }
                            sx={{
                                minWidth: {
                                    md: 160,
                                },
                            }}
                        >
                            {adding
                                ? "Adding..."
                                : "Add image"}
                        </Button>
                    </Stack>
                </Box>

                {images.length === 0 ? (
                    <EmptyState
                        icon={<ImageOutlined />}
                        title="No product images"
                        description="Add an image URL to display the product in the webshop."
                    />
                ) : (
                    <Grid
                        container
                        spacing={2}
                    >
                        {images.map((image) => {
                            const processing =
                                processingImageId ===
                                image.id;

                            return (
                                <Grid
                                    key={image.id}
                                    size={{
                                        xs: 12,
                                        sm: 6,
                                        lg: 4,
                                    }}
                                >
                                    <Card
                                        variant="outlined"
                                        sx={{
                                            height: "100%",
                                            display: "flex",
                                            flexDirection:
                                                "column",
                                        }}
                                    >
                                        {image.url ? (
                                            <CardMedia
                                                component="img"
                                                image={
                                                    image.url
                                                }
                                                alt="Product"
                                                sx={{
                                                    height: 220,
                                                    objectFit:
                                                        "contain",
                                                    bgcolor:
                                                        "grey.50",
                                                    p: 2,
                                                }}
                                            />
                                        ) : (
                                            <Box
                                                sx={{
                                                    height: 220,
                                                    display:
                                                        "grid",
                                                    placeItems:
                                                        "center",
                                                    bgcolor:
                                                        "grey.50",
                                                    color:
                                                        "text.disabled",
                                                }}
                                            >
                                                <ImageOutlined
                                                    sx={{
                                                        fontSize: 56,
                                                    }}
                                                />
                                            </Box>
                                        )}

                                        <CardContent
                                            sx={{
                                                flexGrow: 1,
                                            }}
                                        >
                                            {image.isPrimary ? (
                                                <Chip
                                                    icon={
                                                        <Star />
                                                    }
                                                    label="Primary image"
                                                    color="primary"
                                                    size="small"
                                                />
                                            ) : (
                                                <Chip
                                                    label="Additional image"
                                                    variant="outlined"
                                                    size="small"
                                                />
                                            )}

                                            <Typography
                                                variant="body2"
                                                color="text.secondary"
                                                sx={{
                                                    mt: 2,
                                                    overflowWrap:
                                                        "anywhere",
                                                }}
                                            >
                                                {image.url ??
                                                    "No URL available"}
                                            </Typography>
                                        </CardContent>

                                        <CardActions
                                            sx={{
                                                px: 2,
                                                pb: 2,
                                                gap: 1,
                                            }}
                                        >
                                            <Button
                                                size="small"
                                                variant={
                                                    image.isPrimary
                                                        ? "contained"
                                                        : "outlined"
                                                }
                                                startIcon={
                                                    image.isPrimary ? (
                                                        <Star />
                                                    ) : (
                                                        <StarOutlined />
                                                    )
                                                }
                                                disabled={
                                                    disabled ||
                                                    processing ||
                                                    image.isPrimary
                                                }
                                                onClick={() =>
                                                    void handleSetPrimary(
                                                        image.id
                                                    )
                                                }
                                            >
                                                {image.isPrimary
                                                    ? "Primary"
                                                    : "Set primary"}
                                            </Button>

                                            <Button
                                                size="small"
                                                color="error"
                                                startIcon={
                                                    <DeleteOutlined />
                                                }
                                                disabled={
                                                    disabled ||
                                                    processing
                                                }
                                                onClick={() =>
                                                    setImageToDelete(
                                                        image
                                                    )
                                                }
                                            >
                                                Delete
                                            </Button>
                                        </CardActions>
                                    </Card>
                                </Grid>
                            );
                        })}
                    </Grid>
                )}
            </Paper>

            <ConfirmDialog
                open={imageToDelete !== null}
                title="Delete product image?"
                description="The image will be removed from this product. This action cannot be undone."
                confirmLabel="Delete image"
                loading={
                    processingImageId ===
                    imageToDelete?.id
                }
                onClose={() =>
                    setImageToDelete(null)
                }
                onConfirm={() =>
                    void handleConfirmDelete()
                }
            />
        </>
    );
};

export default AdminProductImageManager;