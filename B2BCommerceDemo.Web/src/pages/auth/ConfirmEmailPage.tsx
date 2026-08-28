import {
    CheckCircleOutlined,
    ErrorOutlined,
} from "@mui/icons-material";
import {
    Alert,
    Box,
    Button,
    Card,
    CardContent,
    CircularProgress,
    Typography,
} from "@mui/material";
import {
    useEffect,
    useRef,
    useState,
} from "react";
import {
    Link,
    useSearchParams,
} from "react-router-dom";
import {
    confirmEmail,
} from "../../api/authApi";
import { getApiErrorMessage } from "../../utils/getApiErrorMessage";

type ConfirmationStatus =
    | "loading"
    | "success"
    | "error";

const ConfirmEmailPage = () => {
    const [searchParams] =
        useSearchParams();

    const confirmationStarted = useRef(false);

    const [
        status,
        setStatus,
    ] =
        useState<ConfirmationStatus>(
            "loading"
        );

    const [error, setError] =
        useState<string | null>(null);

    useEffect(() => {
        if (confirmationStarted.current) {
            return;
        }

        confirmationStarted.current = true;

        const confirm = async () => {
            const userId =
                searchParams.get("userId");

            const token =
                searchParams.get("token");

            if (!userId || !token) {
                setStatus("error");
                setError(
                    "The confirmation link is invalid or incomplete."
                );

                return;
            }

            try {
                await confirmEmail({
                    userId,
                    token,
                });

                setStatus("success");
            } catch (error) {
                console.error(
                    "Failed to confirm email",
                    error
                );

                setError(
                    getApiErrorMessage(
                        error,
                        "The email address could not be confirmed. The link may be invalid or expired."
                    )
                );

                setStatus("error");
            }
        };

        void confirm();
    }, [searchParams]);

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
                    {status ===
                        "loading" && (
                            <>
                                <CircularProgress />

                                <Typography
                                    variant="h4"
                                    component="h1"
                                    sx={{
                                        mt: 3,
                                        fontWeight: 800,
                                    }}
                                >
                                    Confirming your email
                                </Typography>

                                <Typography
                                    color="text.secondary"
                                    sx={{ mt: 1.5 }}
                                >
                                    Please wait while we
                                    confirm your email
                                    address.
                                </Typography>
                            </>
                        )}

                    {status ===
                        "success" && (
                            <>
                                <Box
                                    sx={{
                                        width: 72,
                                        height: 72,
                                        mx: "auto",
                                        borderRadius:
                                            "50%",
                                        display:
                                            "grid",
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
                                            fontSize:
                                                42,
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
                                    Email confirmed
                                </Typography>

                                <Typography
                                    color="text.secondary"
                                    sx={{ mt: 1.5 }}
                                >
                                    Your email address has
                                    been confirmed
                                    successfully.
                                </Typography>

                                <Alert
                                    severity="success"
                                    sx={{
                                        mt: 3,
                                        textAlign:
                                            "left",
                                    }}
                                >
                                    Your email address has been verified.
                                    Your company registration is still awaiting
                                    administrator approval.
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
                            </>
                        )}

                    {status ===
                        "error" && (
                            <>
                                <Box
                                    sx={{
                                        width: 72,
                                        height: 72,
                                        mx: "auto",
                                        borderRadius:
                                            "50%",
                                        display:
                                            "grid",
                                        placeItems:
                                            "center",
                                        bgcolor:
                                            "error.50",
                                        color:
                                            "error.main",
                                    }}
                                >
                                    <ErrorOutlined
                                        sx={{
                                            fontSize:
                                                42,
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
                                    Confirmation failed
                                </Typography>

                                <Alert
                                    severity="error"
                                    sx={{
                                        mt: 3,
                                        textAlign:
                                            "left",
                                    }}
                                >
                                    {error}
                                </Alert>

                                <Button
                                    component={Link}
                                    to="/login"
                                    variant="contained"
                                    size="large"
                                    fullWidth
                                    sx={{ mt: 3 }}
                                >
                                    Back to login
                                </Button>
                            </>
                        )}
                </CardContent>
            </Card>
        </Box>
    );
};

export default ConfirmEmailPage;