import axiosInstance from "../../api/axios";
import { useState } from "react";
import { Alert, Box, Button, Card, CardContent, Divider, Stack, TextField, Typography, } from "@mui/material";
import { getApiErrorMessage } from "../../utils/getApiErrorMessage";
import { Login } from "@mui/icons-material";
import { Link, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

const LoginPage = () => {
    const navigate = useNavigate();
    const { login } = useAuth();

    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState<string | null>(null);
    const [loading, setLoading] = useState(false);

    const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault();

        try {
            setLoading(true);
            setError(null);

            const response = await axiosInstance.post("/accounts/login", {
                email,
                password,
            });

            login(response.data.token);
            navigate("/products");
        } catch (error) {
            setError(
                getApiErrorMessage(
                    error,
                    "Invalid email or password."
                )
            );
        } finally {
            setLoading(false);

        }
    };

    return (
        <Box
            sx={{
                minHeight: "65vh",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
            }}
        >
            <Card
                elevation={0}
                sx={{
                    width: "100%",
                    maxWidth: 440,
                    border: "1px solid",
                    borderColor: "divider",
                    borderRadius: 4,
                }}
            >
                <CardContent sx={{ p: 4 }}>
                    <Stack spacing={3}>
                        <Box sx={{ textAlign: "center" }}>
                            <Typography
                                variant="h4"
                                component="h1"
                                sx={{ fontWeight: 800 }}
                            >
                                Log in
                            </Typography>

                            <Typography color="text.secondary" sx={{ mt: 1 }}>
                                Access the B2B Commerce Demo.
                            </Typography>
                        </Box>

                        {error && (
                            <Alert severity="error">
                                {error}
                            </Alert>
                        )}

                        <Box
                            component="form"
                            onSubmit={handleSubmit}
                        >
                            <Stack spacing={2.5}>
                                <TextField
                                    label="Email"
                                    type="email"
                                    value={email}
                                    onChange={(e) => setEmail(e.target.value)}
                                    fullWidth
                                    required
                                    autoComplete="email"
                                    disabled={loading}
                                />

                                <TextField
                                    label="Password"
                                    type="password"
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    fullWidth
                                    required
                                    autoComplete="current-password"
                                    disabled={loading}
                                />

                                <Box
                                    sx={{
                                        display: "flex",
                                        justifyContent: "flex-end",
                                    }}
                                >
                                    <Button
                                        component={Link}
                                        to="/forgot-password"
                                        size="small"
                                    >
                                        Forgot password?
                                    </Button>
                                </Box>

                                <Button
                                    type="submit"
                                    variant="contained"
                                    size="large"
                                    startIcon={<Login />}
                                    disabled={loading}
                                    fullWidth
                                >
                                    {loading ? "Logging in..." : "Log in"}
                                </Button>
                            </Stack>
                        </Box>

                        <Divider>
                            New to B2B Commerce Demo?
                        </Divider>

                        <Box sx={{ textAlign: "center" }}>
                            <Typography
                                color="text.secondary"
                                sx={{ mb: 2 }}
                            >
                                Register your company and
                                request access to the B2B
                                webshop.
                            </Typography>

                            <Button
                                component={Link}
                                to="/register"
                                variant="outlined"
                                size="large"
                                fullWidth
                                disabled={loading}
                            >
                                Register company
                            </Button>
                        </Box>
                    </Stack>
                </CardContent>
            </Card>
        </Box>
    );
};

export default LoginPage;