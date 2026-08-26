import { ArrowBack, MarkEmailReadOutlined, PasswordOutlined, } from "@mui/icons-material";
import { Alert, Box, Button, Card, CardContent, Stack, TextField, Typography, } from "@mui/material";
import { type FormEvent, useState, } from "react";
import { Link } from "react-router-dom";
import { forgotPassword, } from "../../api/authApi";
import { getApiErrorMessage } from "../../utils/getApiErrorMessage";

const ForgotPasswordPage = () => {
    const [email, setEmail] =
        useState("");

    const [loading, setLoading] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    const [submitted, setSubmitted] =
        useState(false);

    const handleSubmit = async (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        if (loading) {
            return;
        }

        const normalizedEmail =
            email.trim();

        if (!normalizedEmail) {
            setError(
                "Email address is required."
            );

            return;
        }

        setLoading(true);
        setError(null);

        try {
            await forgotPassword({
                email: normalizedEmail,
            });

            setSubmitted(true);
        } catch (error) {
            console.error(
                "Failed to request password reset",
                error
            );

            setError(
                getApiErrorMessage(
                    error,
                    "The password reset request could not be submitted. Please try again."
                )
            );
        } finally {
            setLoading(false);
        }
    };

    if (submitted) {
        return (
            <Box
                sx={{
                    minHeight: "65vh",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    py: 5,
                }}
            >
                <Card
                    variant="outlined"
                    sx={{
                        width: "100%",
                        maxWidth: 520,
                        borderRadius: 4,
                    }}
                >
                    <CardContent
                        sx={{
                            p: {
                                xs: 3,
                                sm: 5,
                            },
                            textAlign: "center",
                        }}
                    >
                        <Box
                            sx={{
                                width: 72,
                                height: 72,
                                mx: "auto",
                                borderRadius: "50%",
                                display: "grid",
                                placeItems: "center",
                                bgcolor: "success.50",
                                color: "success.main",
                            }}
                        >
                            <MarkEmailReadOutlined
                                sx={{
                                    fontSize: 40,
                                }}
                            />
                        </Box>

                        <Typography
                            variant="h4"
                            component="h1"
                            sx={{
                                mt: 3,
                                fontWeight: 800,
                            }}
                        >
                            Check your email
                        </Typography>

                        <Typography
                            color="text.secondary"
                            sx={{ mt: 1.5 }}
                        >
                            If an account exists for{" "}
                            <strong>{email.trim()}</strong>,
                            an email containing password
                            reset instructions has been
                            sent.
                        </Typography>

                        <Alert
                            severity="info"
                            sx={{
                                mt: 3,
                                textAlign: "left",
                            }}
                        >
                            The message may take a few
                            minutes to arrive. Remember to
                            check your spam folder.
                        </Alert>

                        <Button
                            component={Link}
                            to="/login"
                            variant="contained"
                            size="large"
                            fullWidth
                            startIcon={<ArrowBack />}
                            sx={{ mt: 3 }}
                        >
                            Back to login
                        </Button>
                    </CardContent>
                </Card>
            </Box>
        );
    }

    return (
        <Box
            sx={{
                minHeight: "65vh",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                py: 5,
            }}
        >
            <Card
                variant="outlined"
                sx={{
                    width: "100%",
                    maxWidth: 520,
                    borderRadius: 4,
                }}
            >
                <CardContent
                    sx={{
                        p: {
                            xs: 3,
                            sm: 5,
                        },
                    }}
                >
                    <Stack spacing={3}>
                        <Box
                            sx={{
                                textAlign: "center",
                            }}
                        >
                            <Box
                                sx={{
                                    width: 64,
                                    height: 64,
                                    mx: "auto",
                                    borderRadius: "50%",
                                    display: "grid",
                                    placeItems: "center",
                                    bgcolor: "primary.50",
                                    color: "primary.main",
                                }}
                            >
                                <PasswordOutlined
                                    sx={{
                                        fontSize: 34,
                                    }}
                                />
                            </Box>

                            <Typography
                                variant="h4"
                                component="h1"
                                sx={{
                                    mt: 2,
                                    fontWeight: 800,
                                }}
                            >
                                Forgot your password?
                            </Typography>

                            <Typography
                                color="text.secondary"
                                sx={{ mt: 1 }}
                            >
                                Enter your email address,
                                and we will send you
                                instructions for choosing
                                a new password.
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
                                    fullWidth
                                    required
                                    type="email"
                                    label="Email"
                                    value={email}
                                    onChange={(event) => {
                                        setEmail(
                                            event.target.value
                                        );

                                        setError(null);
                                    }}
                                    autoComplete="email"
                                    autoFocus
                                    disabled={loading}
                                />

                                <Button
                                    type="submit"
                                    variant="contained"
                                    size="large"
                                    fullWidth
                                    disabled={loading}
                                >
                                    {loading
                                        ? "Sending instructions..."
                                        : "Send reset instructions"}
                                </Button>
                            </Stack>
                        </Box>

                        <Button
                            component={Link}
                            to="/login"
                            color="inherit"
                            startIcon={<ArrowBack />}
                        >
                            Back to login
                        </Button>
                    </Stack>
                </CardContent>
            </Card>
        </Box>
    );
};

export default ForgotPasswordPage;