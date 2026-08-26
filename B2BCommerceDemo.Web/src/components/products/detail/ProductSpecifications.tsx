import { ExpandMore } from "@mui/icons-material";
import {
    Accordion,
    AccordionDetails,
    AccordionSummary,
    Box,
    Button,
    Card,
    CardContent,
    Divider,
    Stack,
    Typography,
} from "@mui/material";
import { useMemo, useState, } from "react";
import type { Product } from "../../../api/productApi";

type ProductSpecificationsProps = {
    product: Product;
};

type SpecificationItem = {
    Name: string;
    Value: string;
};

type SpecificationGroup = {
    GroupName: string;
    Items: SpecificationItem[];
};

const ProductSpecificationsContent = ({
    product,
}: ProductSpecificationsProps) => {
    const specificationGroups = useMemo(
        () =>
            parseSpecifications(
                product.specificationsJson
            ),
        [product.specificationsJson]
    );

    const [expandedGroups, setExpandedGroups] =
        useState<Set<number>>(
            () => new Set([0])
        );

    if (specificationGroups.length === 0) {
        return null;
    }

    const handleGroupChange = (
        groupIndex: number
    ) => {
        setExpandedGroups((currentGroups) => {
            const updatedGroups =
                new Set(currentGroups);

            if (updatedGroups.has(groupIndex)) {
                updatedGroups.delete(groupIndex);
            } else {
                updatedGroups.add(groupIndex);
            }

            return updatedGroups;
        });
    };

    const expandAll = () => {
        setExpandedGroups(
            new Set(
                specificationGroups.map(
                    (_, index) => index
                )
            )
        );
    };

    const collapseAll = () => {
        setExpandedGroups(new Set());
    };

    const allGroupsExpanded =
        expandedGroups.size ===
        specificationGroups.length;

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
                <Stack
                    direction={{
                        xs: "column",
                        sm: "row",
                    }}
                    spacing={1.5}
                    sx={{
                        mb: 3,
                        alignItems: {
                            xs: "flex-start",
                            sm: "center",
                        },
                        justifyContent:
                            "space-between",
                    }}
                >
                    <Typography
                        variant="h5"
                        component="h2"
                        sx={{
                            fontWeight: 800,
                            letterSpacing:
                                "-0.02em",
                        }}
                    >
                        Specifications
                    </Typography>

                    <Button
                        type="button"
                        size="small"
                        onClick={
                            allGroupsExpanded
                                ? collapseAll
                                : expandAll
                        }
                        sx={{
                            minWidth: "auto",
                            px: 1,
                            textTransform: "none",
                            fontWeight: 700,
                        }}
                    >
                        {allGroupsExpanded
                            ? "Collapse all"
                            : "Expand all"}
                    </Button>
                </Stack>

                <Stack spacing={1.25}>
                    {specificationGroups.map(
                        (group, groupIndex) => (
                            <Accordion
                                key={`${group.GroupName}-${groupIndex}`}
                                expanded={expandedGroups.has(
                                    groupIndex
                                )}
                                onChange={() =>
                                    handleGroupChange(
                                        groupIndex
                                    )
                                }
                                disableGutters
                                elevation={0}
                                sx={{
                                    border: "1px solid",
                                    borderColor:
                                        "divider",
                                    borderRadius:
                                        "12px !important",
                                    overflow: "hidden",
                                    "&::before": {
                                        display: "none",
                                    },
                                }}
                            >
                                <AccordionSummary
                                    expandIcon={
                                        <ExpandMore />
                                    }
                                    aria-controls={`specification-group-${groupIndex}`}
                                    id={`specification-heading-${groupIndex}`}
                                    sx={{
                                        px: 2,
                                        py: 0.5,
                                        minHeight: 58,
                                        "& .MuiAccordionSummary-content":
                                        {
                                            my: 1.25,
                                        },
                                    }}
                                >
                                    <Box
                                        sx={{
                                            width: "100%",
                                            pr: 1,
                                            display: "flex",
                                            justifyContent:
                                                "space-between",
                                            alignItems:
                                                "center",
                                            gap: 2,
                                        }}
                                    >
                                        <Typography
                                            component="h3"
                                            variant="subtitle1"
                                            sx={{
                                                fontWeight: 800,
                                            }}
                                        >
                                            {group.GroupName}
                                        </Typography>

                                        <Typography
                                            variant="caption"
                                            color="text.secondary"
                                            sx={{
                                                flexShrink: 0,
                                            }}
                                        >
                                            {group.Items.length}{" "}
                                            {group.Items
                                                .length === 1
                                                ? "detail"
                                                : "details"}
                                        </Typography>
                                    </Box>
                                </AccordionSummary>

                                <AccordionDetails
                                    sx={{
                                        px: 2,
                                        pt: 0,
                                        pb: 1.5,
                                    }}
                                >
                                    <Stack
                                        divider={
                                            <Divider />
                                        }
                                    >
                                        {group.Items.map(
                                            (
                                                item,
                                                itemIndex
                                            ) => (
                                                <Box
                                                    key={`${item.Name}-${itemIndex}`}
                                                    sx={{
                                                        display:
                                                            "grid",
                                                        gridTemplateColumns:
                                                        {
                                                            xs: "1fr",
                                                            sm: "minmax(130px, 42%) 1fr",
                                                        },
                                                        columnGap: 2,
                                                        rowGap: 0.5,
                                                        py: 1.25,
                                                    }}
                                                >
                                                    <Typography
                                                        variant="body2"
                                                        color="text.secondary"
                                                        sx={{
                                                            overflowWrap:
                                                                "anywhere",
                                                        }}
                                                    >
                                                        {item.Name}
                                                    </Typography>

                                                    <Typography
                                                        variant="body2"
                                                        sx={{
                                                            fontWeight: 600,
                                                            overflowWrap:
                                                                "anywhere",
                                                        }}
                                                    >
                                                        {item.Value}
                                                    </Typography>
                                                </Box>
                                            )
                                        )}
                                    </Stack>
                                </AccordionDetails>
                            </Accordion>
                        )
                    )}
                </Stack>
            </CardContent>
        </Card>
    );
};

const ProductSpecifications = ({
    product,
}: ProductSpecificationsProps) => {
    return (
        <ProductSpecificationsContent
            key={product.id}
            product={product}
        />
    );
};

const parseSpecifications = (
    specificationsJson?: string | null
): SpecificationGroup[] => {
    if (!specificationsJson?.trim()) {
        return [];
    }

    try {
        const parsedValue: unknown =
            JSON.parse(specificationsJson);

        if (!Array.isArray(parsedValue)) {
            return [];
        }

        return parsedValue
            .filter(isSpecificationGroup)
            .map((group) => ({
                GroupName:
                    group.GroupName.trim(),
                Items: group.Items
                    .filter(isSpecificationItem)
                    .map((item) => ({
                        Name: item.Name.trim(),
                        Value: item.Value.trim(),
                    }))
                    .filter(
                        (item) =>
                            item.Name.length > 0 &&
                            item.Value.length > 0
                    ),
            }))
            .filter(
                (group) =>
                    group.GroupName.length > 0 &&
                    group.Items.length > 0
            );
    } catch {
        return [];
    }
};

const isSpecificationGroup = (
    value: unknown
): value is SpecificationGroup => {
    if (
        typeof value !== "object" ||
        value === null
    ) {
        return false;
    }

    const group =
        value as Partial<SpecificationGroup>;

    return (
        typeof group.GroupName === "string" &&
        Array.isArray(group.Items)
    );
};

const isSpecificationItem = (
    value: unknown
): value is SpecificationItem => {
    if (
        typeof value !== "object" ||
        value === null
    ) {
        return false;
    }

    const item =
        value as Partial<SpecificationItem>;

    return (
        typeof item.Name === "string" &&
        typeof item.Value === "string"
    );
};

export default ProductSpecifications;