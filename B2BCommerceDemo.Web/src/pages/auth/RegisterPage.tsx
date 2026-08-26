import { BusinessOutlined, CheckCircleOutlined, PersonAddAlt, } from "@mui/icons-material";
import {
    Alert,
    Box,
    Button,
    Card,
    CardContent,
    Divider,
    Stack,
    TextField,
    Typography,
} from "@mui/material";
import { type FormEvent, useState, } from "react";
import { Link } from "react-router-dom";
import { register } from "../../api/authApi";
import { getApiErrorMessage } from "../../utils/getApiErrorMessage";

type RegistrationForm = {
    companyName: string;
    email: string;
    password: string;
    confirmPassword: string;
};

const defaultForm: RegistrationForm = {
    companyName: "",
    email: "",
    password: "",
    confirmPassword: "",
};

const RegisterPage = () => {
    const [form, setForm] =
        useState<RegistrationForm>({
            ...defaultForm,
        });

    const [loading, setLoading] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    const [
        successMessage,
        setSuccessMessage,
    ] = useState<string | null>(null);

    const updateField = (
        field: keyof RegistrationForm,
        value: string
    ) => {
        setForm((current) => ({
            ...current,
            [field]: value,
        }));

        setError(null);
    };

    const validateForm = () => {
        const password =
            form.password;

        if (!form.companyName.trim()) {
            return "Company name is required.";
        }

        if (!form.email.trim()) {
            return "Email address is required.";
        }

        if (password.length < 6) {
            return "The password must contain at least 6 characters.";
        }

        if (!/[A-Z]/.test(password)) {
            return "The password must contain at least one uppercase letter.";
        }

        if (!/[a-z]/.test(password)) {
            return "The password must contain at least one lowercase letter.";
        }

        if (!/\d/.test(password)) {
            return "The password must contain at least one number.";
        }

        if (
            password !==
            form.confirmPassword
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
            const result =
                await register({
                    companyName:
                        form.companyName.trim(),
                    email:
                        form.email.trim(),
                    password:
                        form.password,
                });

            setSuccessMessage(
                result.message
            );

            setForm({
                ...defaultForm,
            });
        } catch (error) {
            console.error(
                "Failed to register company",
                error
            );

            setError(
                getApiErrorMessage(
                    error,
                    "The registration could not be submitted. Please try again."
                )
            );
        } finally {
            setLoading(false);
        }
    };

    if (successMessage) {
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
                        maxWidth: 560,
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
                                borderRadius:
                                    "50%",
                                display: "grid",
                                placeItems:
                                    "center",
                                bgcolor:
                                    "success.50",
                                color:
                                    "success.main",
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
                            Registration submitted
                        </Typography>

                        <Typography
                            color="text.secondary"
                            sx={{ mt: 1.5 }}
                        >
                            {successMessage}
                        </Typography>

                        <Alert
                            severity="info"
                            sx={{
                                mt: 3,
                                textAlign: "left",
                            }}
                        >
                            An administrator must
                            approve the company and
                            assign its Rackbeat
                            customer number and price
                            group before webshop
                            access is granted.
                        </Alert>

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
                    maxWidth: 640,
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
                                    borderRadius:
                                        "50%",
                                    display: "grid",
                                    placeItems:
                                        "center",
                                    bgcolor:
                                        "primary.50",
                                    color:
                                        "primary.main",
                                }}
                            >
                                <BusinessOutlined
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
                                Register your company
                            </Typography>

                            <Typography
                                color="text.secondary"
                                sx={{ mt: 1 }}
                            >
                                Request access to the
                                B2B Commerce Demo.
                            </Typography>
                        </Box>

                        <Alert severity="info">
                            New registrations require
                            approval before the
                            company can access
                            business pricing and place
                            orders.
                        </Alert>

                        {error && (
                            <Alert severity="error">
                                {error}
                            </Alert>
                        )}

                        <Box
                            component="form"
                            onSubmit={
                                handleSubmit
                            }
                            noValidate
                        >
                            <Stack spacing={2.5}>
                                <TextField
                                    fullWidth
                                    required
                                    label="Company name"
                                    value={
                                        form.companyName
                                    }
                                    onChange={(
                                        event
                                    ) =>
                                        updateField(
                                            "companyName",
                                            event
                                                .target
                                                .value
                                        )
                                    }
                                    autoComplete="organization"
                                    disabled={
                                        loading
                                    }
                                    helperText="Enter the registered business name."
                                />

                                <TextField
                                    fullWidth
                                    required
                                    type="email"
                                    label="Email"
                                    value={
                                        form.email
                                    }
                                    onChange={(
                                        event
                                    ) =>
                                        updateField(
                                            "email",
                                            event
                                                .target
                                                .value
                                        )
                                    }
                                    autoComplete="email"
                                    disabled={
                                        loading
                                    }
                                    helperText="This email will be used to access the webshop."
                                />

                                <Divider />

                                <TextField
                                    fullWidth
                                    required
                                    type="password"
                                    label="Password"
                                    value={
                                        form.password
                                    }
                                    onChange={(
                                        event
                                    ) =>
                                        updateField(
                                            "password",
                                            event
                                                .target
                                                .value
                                        )
                                    }
                                    autoComplete="new-password"
                                    disabled={
                                        loading
                                    }
                                    helperText="Use at least 6 characters, including uppercase, lowercase and a number."
                                />

                                <TextField
                                    fullWidth
                                    required
                                    type="password"
                                    label="Confirm password"
                                    value={
                                        form.confirmPassword
                                    }
                                    onChange={(
                                        event
                                    ) =>
                                        updateField(
                                            "confirmPassword",
                                            event
                                                .target
                                                .value
                                        )
                                    }
                                    autoComplete="new-password"
                                    disabled={
                                        loading
                                    }
                                />

                                <Button
                                    type="submit"
                                    variant="contained"
                                    size="large"
                                    fullWidth
                                    startIcon={
                                        <PersonAddAlt />
                                    }
                                    disabled={
                                        loading
                                    }
                                >
                                    {loading
                                        ? "Submitting registration..."
                                        : "Submit registration"}
                                </Button>
                            </Stack>
                        </Box>

                        <Typography
                            color="text.secondary"
                            sx={{
                                textAlign: "center",
                            }}
                        >
                            Already registered?{" "}
                            <Button
                                component={Link}
                                to="/login"
                                size="small"
                            >
                                Log in
                            </Button>
                        </Typography>
                    </Stack>
                </CardContent>
            </Card>
        </Box>
    );
};

export default RegisterPage;