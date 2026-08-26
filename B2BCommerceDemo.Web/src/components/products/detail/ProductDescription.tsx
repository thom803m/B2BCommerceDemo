import {
    Box,
    Card,
    CardContent,
    Stack,
    Typography,
} from "@mui/material";
import { useMemo } from "react";
import type { Product } from "../../../api/productApi";

type ProductDescriptionProps = {
    product: Product;
};

type DescriptionBlock =
    | {
        type: "heading";
        text: string;
    }
    | {
        type: "paragraph";
        text: string;
    }
    | {
        type: "list";
        items: string[];
    };

const ProductDescription = ({
    product,
}: ProductDescriptionProps) => {
    const blocks = useMemo(
        () => parseDescription(product.description),
        [product.description]
    );

    if (blocks.length === 0) {
        return null;
    }

    return (
        <Card
            component="section"
            variant="outlined"
            sx={{
                borderRadius: 4,
                bgcolor: "background.paper",
            }}
        >
            <CardContent
                sx={{
                    p: {
                        xs: 2.5,
                        sm: 3.5,
                    },
                    "&:last-child": {
                        pb: {
                            xs: 2.5,
                            sm: 3.5,
                        },
                    },
                }}
            >
                <Typography
                    variant="h5"
                    component="h2"
                    sx={{
                        mb: 3,
                        fontWeight: 800,
                        letterSpacing: "-0.02em",
                    }}
                >
                    Product description
                </Typography>

                <Stack
                    spacing={2.25}
                    sx={{
                        maxWidth: 900,
                    }}
                >
                    {blocks.map((block, index) => {
                        if (block.type === "heading") {
                            return (
                                <Typography
                                    key={`${block.text}-${index}`}
                                    component="h3"
                                    variant="subtitle1"
                                    sx={{
                                        pt: index === 0 ? 0 : 1,
                                        fontWeight: 800,
                                        letterSpacing: "0.01em",
                                    }}
                                >
                                    {block.text}
                                </Typography>
                            );
                        }

                        if (block.type === "list") {
                            return (
                                <Box
                                    key={`list-${index}`}
                                    component="ul"
                                    sx={{
                                        my: 0,
                                        pl: {
                                            xs: 2.5,
                                            sm: 3,
                                        },
                                        color: "text.secondary",
                                    }}
                                >
                                    {block.items.map(
                                        (item, itemIndex) => (
                                            <Typography
                                                key={`${item}-${itemIndex}`}
                                                component="li"
                                                color="text.secondary"
                                                sx={{
                                                    mb:
                                                        itemIndex ===
                                                            block.items.length - 1
                                                            ? 0
                                                            : 1,
                                                    pl: 0.5,
                                                    fontSize: {
                                                        xs: "1rem",
                                                        md: "1.05rem",
                                                    },
                                                    lineHeight: 1.7,
                                                }}
                                            >
                                                {item}
                                            </Typography>
                                        )
                                    )}
                                </Box>
                            );
                        }

                        return (
                            <Typography
                                key={`${block.text}-${index}`}
                                color="text.secondary"
                                sx={{
                                    fontSize: {
                                        xs: "1rem",
                                        md: "1.05rem",
                                    },
                                    lineHeight: 1.8,
                                    overflowWrap: "anywhere",
                                }}
                            >
                                {block.text}
                            </Typography>
                        );
                    })}
                </Stack>
            </CardContent>
        </Card>
    );
};

const parseDescription = (
    value?: string | null
): DescriptionBlock[] => {
    if (!value?.trim()) {
        return [];
    }

    const preparedValue = value
        .replace(
            /<(b|strong)>(.*?)<\/\1>/gi,
            "\n\n__HEADING__$2\n\n"
        )
        .replace(
            /<li[^>]*>(.*?)<\/li>/gi,
            "\n__LIST_ITEM__$1"
        )
        .replace(
            /<\/?(ul|ol)[^>]*>/gi,
            "\n\n"
        )
        .replace(/<br\s*\/?>/gi, "\n\n")
        .replace(/<\/p>/gi, "\n\n")
        .replace(/<p[^>]*>/gi, "")
        .replace(/<[^>]+>/g, "")
        .replace(/&nbsp;/gi, " ")
        .replace(/&amp;/gi, "&")
        .replace(/&quot;/gi, '"')
        .replace(/&#39;/gi, "'")
        .replace(/&lt;/gi, "<")
        .replace(/&gt;/gi, ">");

    const parts = preparedValue
        .split(/\n{2,}/)
        .map((part) => part.trim())
        .filter(Boolean);

    const blocks: DescriptionBlock[] = [];

    for (const part of parts) {
        if (part.startsWith("__HEADING__")) {
            blocks.push({
                type: "heading",
                text: part
                    .replace("__HEADING__", "")
                    .trim(),
            });

            continue;
        }

        if (part.includes("__LIST_ITEM__")) {
            const items = part
                .split("__LIST_ITEM__")
                .map((item) => item.trim())
                .filter(Boolean);

            if (items.length > 0) {
                blocks.push({
                    type: "list",
                    items,
                });
            }

            continue;
        }

        blocks.push({
            type: "paragraph",
            text: part,
        });
    }

    return blocks;
};

export default ProductDescription;