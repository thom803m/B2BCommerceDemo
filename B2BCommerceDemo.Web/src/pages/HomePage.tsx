import type { ReactNode } from "react";
import { Box, Button, Card, CardActionArea, CardContent, Chip, Grid, Stack, Typography, } from "@mui/material";
import { ArrowForward, Business, Devices, Inventory2, LocalShipping, PhoneInTalk, Print, QrCodeScanner, ReceiptLong, Router, VerifiedUser, } from "@mui/icons-material";
import { Link } from "react-router-dom";

const categories = [
    {
        title: "Barcode scanners",
        category: "Barcode Scanner",
        text: "Reliable scanners for warehouse, retail and office workflows.",
        icon: <QrCodeScanner />,
    },
    {
        title: "Label printers",
        category: "Label Printer",
        text: "Professional label printing for products, shipping and inventory.",
        icon: <Print />,
    },
    {
        title: "Receipt printers",
        category: "Receipt Printer",
        text: "Fast and dependable receipt printing for business environments.",
        icon: <ReceiptLong />,
    },
    {
        title: "IP phones",
        category: "IP Phone",
        text: "Business telephony for clear and reliable communication.",
        icon: <PhoneInTalk />,
    },
    {
        title: "Network accessories",
        category: "Network Accessories",
        text: "Essential accessories for stable and flexible business networks.",
        icon: <Router />,
    },
    {
        title: "Docking stations",
        category: "Docking Station",
        text: "Connect laptops, displays and workplace accessories with ease.",
        icon: <Devices />,
    },
];

const HomePage = () => {
    return (
        <Stack spacing={{ xs: 7, md: 10 }}>
            <Box
                sx={{
                    position: "relative",
                    overflow: "hidden",
                    bgcolor: "primary.main",
                    color: "white",
                    borderRadius: { xs: 3, md: 5 },
                    px: { xs: 3, sm: 5, md: 8 },
                    py: { xs: 6, md: 10 },
                }}
            >
                <Box
                    sx={{
                        position: "absolute",
                        width: 420,
                        height: 420,
                        borderRadius: "50%",
                        bgcolor: "rgba(37, 99, 235, 0.22)",
                        top: -210,
                        right: -100,
                    }}
                />

                <Box
                    sx={{
                        position: "absolute",
                        width: 260,
                        height: 260,
                        borderRadius: "50%",
                        border: "1px solid rgba(255,255,255,0.12)",
                        right: 110,
                        bottom: -170,
                    }}
                />

                <Stack
                    spacing={3}
                    sx={{
                        position: "relative",
                        zIndex: 1,
                        maxWidth: 820,
                    }}
                >
                    <Chip
                        label="B2B webshop for approved business customers"
                        sx={{
                            bgcolor: "rgba(255,255,255,0.12)",
                            color: "white",
                            width: "fit-content",
                            fontWeight: 700,
                            border: "1px solid rgba(255,255,255,0.14)",
                        }}
                    />

                    <Typography
                        variant="h2"
                        component="h1"
                        sx={{
                            maxWidth: 760,
                            fontWeight: 600,
                            fontSize: { xs: "2.35rem", sm: "3rem", md: "4rem" },
                            lineHeight: 1.08,
                            letterSpacing: "-0.035em",
                        }}
                    >
                        IT products for your business gathered in one place
                    </Typography>

                    <Typography
                        variant="h6"
                        sx={{
                            maxWidth: 680,
                            color: "rgba(255,255,255,0.76)",
                            lineHeight: 1.7,
                            fontWeight: 400,
                        }}
                    >
                        Find products, company-specific prices, stock availability and
                        expected delivery dates through the integrated purchasing flow
                    </Typography>

                    <Stack direction={{ xs: "column", sm: "row" }} spacing={2}>
                        <Button
                            component={Link}
                            to="/products"
                            variant="contained"
                            color="secondary"
                            size="large"
                            endIcon={<ArrowForward />}
                            sx={{ px: 3, py: 1.35 }}
                        >
                            Browse products
                        </Button>

                        <Button
                            component={Link}
                            to="/login"
                            variant="outlined"
                            size="large"
                            sx={{
                                px: 3,
                                py: 1.35,
                                color: "white",
                                borderColor: "rgba(255,255,255,0.45)",
                                "&:hover": {
                                    borderColor: "white",
                                    bgcolor: "rgba(255,255,255,0.08)",
                                },
                            }}
                        >
                            Business login
                        </Button>
                    </Stack>
                </Stack>
            </Box>

            <Box component="section">
                <SectionHeading
                    eyebrow="Why this platform"
                    title="A simpler way to purchase business IT"
                    text="The webshop brings product information, purchasing prices and availability together in one clear solution."
                />

                <Grid container spacing={3}>
                    <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                        <InfoCard
                            icon={<Business />}
                            title="Built for businesses"
                            text="Access is tailored to approved business customers and their purchasing needs."
                        />
                    </Grid>

                    <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                        <InfoCard
                            icon={<Inventory2 />}
                            title="Detailed product data"
                            text="Review images, descriptions and specifications before making a purchase."
                        />
                    </Grid>

                    <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                        <InfoCard
                            icon={<LocalShipping />}
                            title="Stock and delivery"
                            text="See current availability and expected delivery dates directly on products."
                        />
                    </Grid>

                    <Grid size={{ xs: 12, sm: 6, lg: 3 }}>
                        <InfoCard
                            icon={<VerifiedUser />}
                            title="Secure company access"
                            text="Login, approval and roles ensure that prices and ordering stay protected."
                        />
                    </Grid>
                </Grid>
            </Box>

            <Box component="section">
                <SectionHeading
                    eyebrow="Product range"
                    title="Explore popular product areas"
                    text="Start with one of the most common business IT categories or browse the complete product range."
                />

                <Grid container spacing={3}>
                    {categories.map((category) => (
                        <Grid key={category.title} size={{ xs: 12, sm: 6, lg: 4 }}>
                            <CategoryCard {...category} />
                        </Grid>
                    ))}
                </Grid>

                <Box sx={{ display: "flex", justifyContent: "center", mt: 4 }}>
                    <Button
                        component={Link}
                        to="/products"
                        variant="outlined"
                        size="large"
                        endIcon={<ArrowForward />}
                    >
                        View all products
                    </Button>
                </Box>
            </Box>

            <Box
                component="section"
                sx={{
                    bgcolor: "white",
                    border: "1px solid",
                    borderColor: "divider",
                    borderRadius: { xs: 3, md: 4 },
                    px: { xs: 3, sm: 5, md: 7 },
                    py: { xs: 5, md: 7 },
                }}
            >
                <Grid container spacing={4} sx={{ alignItems: "center" }}>
                    <Grid size={{ xs: 12, md: 8 }}>
                        <Stack
                            spacing={1.5}
                            sx={{
                                textAlign: { xs: "center", md: "left" },
                                alignItems: { xs: "center", md: "flex-start" },
                            }}
                        >
                            <Typography
                                variant="overline"
                                sx={{ color: "secondary.main", fontWeight: 800 }}
                            >
                                Ready to get started?
                            </Typography>
                            <Typography
                                variant="h3"
                                component="h2"
                                sx={{
                                    color: "primary.main",
                                    fontWeight: 800,
                                    fontSize: { xs: "2rem", md: "2.65rem" },
                                    lineHeight: 1.15,
                                    letterSpacing: "-0.025em",
                                }}
                            >
                                Log in and see your company&apos;s product prices
                            </Typography>
                            <Typography
                                color="text.secondary"
                                sx={{ maxWidth: 720, lineHeight: 1.7 }}
                            >
                                Approved customers receive access to company-specific pricing,
                                ordering and an overview of their purchases.
                            </Typography>
                        </Stack>
                    </Grid>

                    <Grid size={{ xs: 12, md: 4 }}>
                        <Stack
                            direction={{ xs: "column", sm: "row", md: "column" }}
                            spacing={2}
                            sx={{ alignItems: { md: "stretch" } }}
                        >
                            <Button
                                component={Link}
                                to="/login"
                                variant="contained"
                                size="large"
                            >
                                Log in to the webshop
                            </Button>
                            <Button
                                component={Link}
                                to="/products"
                                variant="text"
                                size="large"
                                endIcon={<ArrowForward />}
                            >
                                Browse the catalogue
                            </Button>
                        </Stack>
                    </Grid>
                </Grid>
            </Box>
        </Stack>
    );
};

type SectionHeadingProps = {
    eyebrow: string;
    title: string;
    text: string;
};

const SectionHeading = ({ eyebrow, title, text }: SectionHeadingProps) => (
    <Stack
        spacing={1.25}
        sx={{
            maxWidth: 760,
            mx: "auto",
            mb: 5,
            textAlign: "center",
            alignItems: "center",
        }}
    >
        <Typography
            variant="overline"
            sx={{
                color: "secondary.main",
                fontWeight: 800,
                letterSpacing: 1.2,
            }}
        >
            {eyebrow}
        </Typography>

        <Typography
            variant="h3"
            component="h2"
            sx={{
                color: "text.primary",
                fontWeight: 800,
                fontSize: { xs: "2rem", md: "2.7rem" },
                lineHeight: 1.15,
                letterSpacing: "-0.025em",
            }}
        >
            {title}
        </Typography>

        <Typography
            sx={{
                maxWidth: 680,
                color: "text.secondary",
                lineHeight: 1.75,
            }}
        >
            {text}
        </Typography>
    </Stack>
);

type InfoCardProps = {
    icon: ReactNode;
    title: string;
    text: string;
};

const InfoCard = ({ icon, title, text }: InfoCardProps) => (
    <Card
        elevation={0}
        sx={{
            height: "100%",
            border: "1px solid",
            borderColor: "divider",
            borderRadius: 3,
            transition: "transform 180ms ease, box-shadow 180ms ease",
            "&:hover": {
                transform: "translateY(-4px)",
                boxShadow: "0 16px 36px rgba(15, 23, 42, 0.08)",
            },
        }}
    >
        <CardContent sx={{ p: 3, "&:last-child": { pb: 3 } }}>
            <Box
                sx={{
                    width: 48,
                    height: 48,
                    borderRadius: 2.5,
                    bgcolor: "rgba(37, 99, 235, 0.1)",
                    color: "secondary.main",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    mb: 2.25,
                }}
            >
                {icon}
            </Box>

            <Typography variant="h6" component="h3" sx={{ fontWeight: 750 }} gutterBottom>
                {title}
            </Typography>

            <Typography color="text.secondary" sx={{ lineHeight: 1.7 }}>
                {text}
            </Typography>
        </CardContent>
    </Card>
);

type CategoryCardProps = {
    icon: ReactNode;
    title: string;
    category: string;
    text: string;
};

const CategoryCard = ({ icon, title, category, text }: CategoryCardProps) => (
    <Card
        elevation={0}
        sx={{
            height: "100%",
            border: "1px solid",
            borderColor: "divider",
            borderRadius: 3,
            overflow: "hidden",
        }}
    >
        <CardActionArea
            component={Link}
            to={`/products?category=${encodeURIComponent(
                category
            )}`}
            aria-label={`View ${title}`}
            sx={{
                height: "100%",
                p: 0.5,
            }}
        >
            <CardContent sx={{ p: 3, "&:last-child": { pb: 3 } }}>
                <Stack direction="row" spacing={2.5} sx={{ alignItems: "flex-start" }}>
                    <Box
                        sx={{
                            width: 52,
                            height: 52,
                            flexShrink: 0,
                            borderRadius: 2.5,
                            bgcolor: "primary.main",
                            color: "white",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                        }}
                    >
                        {icon}
                    </Box>

                    <Box sx={{ flexGrow: 1 }}>
                        <Typography
                            variant="h6"
                            component="h3"
                            sx={{ fontWeight: 750, mb: 0.75 }}
                        >
                            {title}
                        </Typography>
                        <Typography color="text.secondary" sx={{ lineHeight: 1.65 }}>
                            {text}
                        </Typography>
                    </Box>

                    <ArrowForward sx={{ color: "text.disabled", mt: 0.5 }} />
                </Stack>
            </CardContent>
        </CardActionArea>
    </Card>
);

export default HomePage;