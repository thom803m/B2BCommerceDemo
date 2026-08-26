import { CheckCircleOutlined, LockResetOutlined, } from "@mui/icons-material";
import { Alert, Box, Button, Card, CardContent, Stack, TextField, Typography, } from "@mui/material";
import { type FormEvent, useState, } from "react";
import { Link, useSearchParams, } from "react-router-dom";
import { resetPassword, } from "../../api/authApi";
import { getApiErrorMessage, } from "../../utils/getApiErrorMessage";

const ResetPasswordPage = () => {
    const [searchParams] =
        useSearchParams();

    const userId =
        searchParams.get("userId")?.trim() ??
        "";

    const token =
        searchParams.get("token") ?? "";

    const linkIsValid =
        userId.length > 0 &&
        token.length > 0;

    const [
        newPassword,
        setNewPassword,
    ] = useState("");

    const [
        confirmPassword,
        setConfirmPassword,
    ] = useState("");

    const [loading, setLoading] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    const [completed, setCompleted] =
        useState(false);

    const validateForm = () => {
        if (!linkIsValid) {
            return "The password reset link is invalid or incomplete.";
        }

        if (newPassword.length < 6) {
            return "The password must contain at least 6 characters.";
        }

        if (!/[A-Z]/.test(newPassword)) {
            return "The password must contain at least one uppercase letter.";
        }

        if (!/[a-z]/.test(newPassword)) {
            return "The password must contain at least one lowercase letter.";
        }

        if (!/\d/.test(newPassword)) {
            return "The password must contain at least one number.";
        }

        if (
            newPassword !==
            confirmPassword
        ) {
            return "The passwords do not match.";
        }

        return null;
    };

    const handleSubmit = async (
        event: FormEvent<HTMLFormElement>
    ) => {
        event.preventDefault();

        if (loading) {
            return;
        }

        const validationError =
            validateForm();

        if (validationError) {
            setError(validationError);
            return;
        }

        setLoading(true);
        setError(null);

        try {
            await resetPassword({
                userId,
                token,
                newPassword,
            });

            setCompleted(true);
            setNewPassword("");
            setConfirmPassword("");
        } catch (error) {
            console.error(
                "Failed to reset password",
                error
            );

            setError(
                getApiErrorMessage(
                    error,
                    "The password could not be reset. The link may be invalid or expired."
                )
            );
        } finally {
            setLoading(false);
        }
    };

    if (completed) {
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
                            <CheckCircleOutlined
                                sx={{
                                    fontSize: 42,
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
                            Password updated
                        </Typography>

                        <Typography
                            color="text.secondary"
                            sx={{ mt: 1.5 }}
                        >
                            Your password has been
                            changed successfully. You
                            can now log in with your new
                            password.
                        </Typography>

                        <Button
                            component={Link}
                            to="/login"
                            variant="contained"
                            size="large"
                            fullWidth
                            sx={{ mt: 3 }}
                        >
                            Go to login
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
                                <LockResetOutlined
                                    sx={{
                                        fontSize: 36,
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
                                Choose a new password
                            </Typography>

                            <Typography
                                color="text.secondary"
                                sx={{ mt: 1 }}
                            >
                                Enter a new password for
                                your account.
                            </Typography>
                        </Box>

                        {!linkIsValid && (
                            <Alert severity="error">
                                The password reset link
                                is invalid or incomplete.
                                Please request a new
                                password reset email.
                            </Alert>
                        )}

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
                                    type="password"
                                    label="New password"
                                    value={newPassword}
                                    onChange={(event) => {
                                        setNewPassword(
                                            event.target.value
                                        );

                                        setError(null);
                                    }}
                                    autoComplete="new-password"
                                    disabled={
                                        loading ||
                                        !linkIsValid
                                    }
                                    helperText="Use at least 6 characters, including uppercase, lowercase and a number."
                                />

                                <TextField
                                    fullWidth
                                    required
                                    type="password"
                                    label="Confirm new password"
                                    value={confirmPassword}
                                    onChange={(event) => {
                                        setConfirmPassword(
                                            event.target.value
                                        );

                                        setError(null);
                                    }}
                                    autoComplete="new-password"
                                    disabled={
                                        loading ||
                                        !linkIsValid
                                    }
                                />

                                <Button
                                    type="submit"
                                    variant="contained"
                                    size="large"
                                    fullWidth
                                    disabled={
                                        loading ||
                                        !linkIsValid
                                    }
                                >
                                    {loading
                                        ? "Updating password..."
                                        : "Update password"}
                                </Button>
                            </Stack>
                        </Box>

                        {!linkIsValid && (
                            <Button
                                component={Link}
                                to="/forgot-password"
                                variant="outlined"
                            >
                                Request a new link
                            </Button>
                        )}
                    </Stack>
                </CardContent>
            </Card>
        </Box>
    );
};

export default ResetPasswordPage;