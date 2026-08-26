import { Box, Button, Container, Divider, Stack, Typography } from "@mui/material";
import { ContactSupportOutlined } from "@mui/icons-material";
import { Link } from "react-router-dom";

const Footer = () => {
    return (
        <Box
            component="footer"
            sx={{
                mt: 8,
                bgcolor: "primary.main",
                color: "white",
                py: 4,
            }}
        >
            <Container maxWidth="xl">
                <Stack
                    direction={{ xs: "column", md: "row" }}
                    spacing={2}
                    sx={{
                        justifyContent: "space-between",
                        alignItems: { xs: "flex-start", md: "center" },
                    }}
                >
                    <Box>
                        <Typography
                            variant="h6"
                            component="p"
                            sx={{ fontWeight: 800 }}
                        >
                            B2B COMMERCE DEMO
                        </Typography>

                        <Typography color="grey.300">
                            Professional B2B webshop for IT products.
                        </Typography>
                    </Box>

                    <Button
                        component={Link}
                        to="/contact"
                        color="inherit"
                        variant="outlined"
                        startIcon={<ContactSupportOutlined />}
                        sx={{
                            alignSelf: {
                                xs: "flex-start",
                                md: "center",
                            },
                            px: 2,
                            py: 1,
                            borderColor: "rgba(255, 255, 255, 0.4)",
                            borderRadius: 2,
                            textTransform: "none",
                            fontWeight: 700,
                            "&:hover": {
                                borderColor: "white",
                                bgcolor: "rgba(255, 255, 255, 0.08)",
                            },
                        }}
                    >
                        Contact us
                    </Button>

                    <Typography color="grey.300">
                        © {new Date().getFullYear()} B2B Commerce Demo. All rights reserved.
                    </Typography>
                </Stack>

                <Divider sx={{ my: 3, borderColor: "rgba(255,255,255,0.12)" }} />

                <Typography variant="body2" color="grey.400">
                    Product data, pricing and stock availability are provided for approved
                    business customers.
                </Typography>
            </Container>
        </Box>
    );
};

export default Footer;