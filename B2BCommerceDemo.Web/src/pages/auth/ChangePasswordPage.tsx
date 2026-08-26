import { CheckCircleOutlined, LockResetOutlined, } from "@mui/icons-material";
import { Alert, Box, Button, Card, CardContent, Stack, TextField, Typography, } from "@mui/material";
import { type FormEvent, useState, } from "react";
import { Link } from "react-router-dom";
import { changePassword, } from "../../api/authApi";
import { getApiErrorMessage, } from "../../utils/getApiErrorMessage";

type ChangePasswordForm = {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
};

const defaultForm: ChangePasswordForm = {
    currentPassword: "",
    newPassword: "",
    confirmPassword: "",
};

const ChangePasswordPage = () => {
    const [form, setForm] =
        useState<ChangePasswordForm>({
            ...defaultForm,
        });

    const [loading, setLoading] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    const [success, setSuccess] =
        useState(false);

    const updateField = (
        field: keyof ChangePasswordForm,
        value: string
    ) => {
        setForm((current) => ({
            ...current,
            [field]: value,
        }));

        setError(null);
    };

    const validateForm = () => {
        if (!form.currentPassword) {
            return "Your current password is required.";
        }

        if (form.newPassword.length < 6) {
            return "The new password must contain at least 6 characters.";
        }

        if (!/[A-Z]/.test(form.newPassword)) {
            return "The new password must contain at least one uppercase letter.";
        }

        if (!/[a-z]/.test(form.newPassword)) {
            return "The new password must contain at least one lowercase letter.";
        }

        if (!/\d/.test(form.newPassword)) {
            return "The new password must contain at least one number.";
        }

        if (
            form.newPassword !==
            form.confirmPassword
        ) {
            return "The new passwords do not match.";
        }

        if (
            form.currentPassword ===
            form.newPassword
        ) {
            return "The new password must be different from the current password.";
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
            await changePassword({
                currentPassword:
                    form.currentPassword,
                newPassword:
                    form.newPassword,
            });

            setForm({
                ...defaultForm,
            });

            setSuccess(true);
        } catch (error) {
            console.error(
                "Failed to change password",
                error
            );

            setError(
                getApiErrorMessage(
                    error,
                    "The password could not be changed. Please verify your current password and try again."
                )
            );
        } finally {
            setLoading(false);
        }
    };

    if (success) {
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
                            Password changed
                        </Typography>

                        <Typography
                            color="text.secondary"
                            sx={{ mt: 1.5 }}
                        >
                            Your password was changed
                            successfully. Use the new
                            password the next time you
                            log in.
                        </Typography>

                        <Button
                            component={Link}
                            to="/products"
                            variant="contained"
                            size="large"
                            fullWidth
                            sx={{ mt: 3 }}
                        >
                            Continue to products
                        </Button>

                        <Button
                            color="inherit"
                            fullWidth
                            onClick={() =>
                                setSuccess(false)
                            }
                            sx={{ mt: 1 }}
                        >
                            Change password again
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
                                Change password
                            </Typography>

                            <Typography
                                color="text.secondary"
                                sx={{ mt: 1 }}
                            >
                                Enter your current
                                password and choose a
                                new one.
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
                            noValidate
                        >
                            <Stack spacing={2.5}>
                                <TextField
                                    fullWidth
                                    required
                                    type="password"
                                    label="Current password"
                                    value={
                                        form.currentPassword
                                    }
                                    onChange={(event) =>
                                        updateField(
                                            "currentPassword",
                                            event.target.value
                                        )
                                    }
                                    autoComplete="current-password"
                                    autoFocus
                                    disabled={loading}
                                />

                                <TextField
                                    fullWidth
                                    required
                                    type="password"
                                    label="New password"
                                    value={
                                        form.newPassword
                                    }
                                    onChange={(event) =>
                                        updateField(
                                            "newPassword",
                                            event.target.value
                                        )
                                    }
                                    autoComplete="new-password"
                                    disabled={loading}
                                    helperText="Use at least 6 characters, including uppercase, lowercase and a number."
                                />

                                <TextField
                                    fullWidth
                                    required
                                    type="password"
                                    label="Confirm new password"
                                    value={
                                        form.confirmPassword
                                    }
                                    onChange={(event) =>
                                        updateField(
                                            "confirmPassword",
                                            event.target.value
                                        )
                                    }
                                    autoComplete="new-password"
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
                                        ? "Changing password..."
                                        : "Change password"}
                                </Button>
                            </Stack>
                        </Box>

                        <Button
                            component={Link}
                            to="/products"
                            color="inherit"
                        >
                            Cancel
                        </Button>
                    </Stack>
                </CardContent>
            </Card>
        </Box>
    );
};

export default ChangePasswordPage;