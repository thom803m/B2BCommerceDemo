import { isAxiosError } from "axios";

type ProblemDetails = {
    title?: string;
    detail?: string;
    message?: string;
    errors?:
    | string[]
    | Record<string, string[]>;
};

export const getApiErrorMessage = (
    error: unknown,
    fallbackMessage =
        "An unexpected error occurred. Please try again."
) => {
    if (!isAxiosError(error)) {
        return fallbackMessage;
    }

    if (!error.response) {
        return "The server could not be reached. Please check your connection and try again.";
    }

    const responseData =
        error.response.data as
        | ProblemDetails
        | string
        | undefined;

    if (
        typeof responseData === "string"
    ) {
        return (
            responseData.trim() ||
            fallbackMessage
        );
    }

    if (responseData?.errors) {
        const validationMessages =
            Array.isArray(
                responseData.errors
            )
                ? responseData.errors
                : Object.values(
                    responseData.errors
                ).flat();

        if (
            validationMessages.length > 0
        ) {
            return validationMessages.join(
                " "
            );
        }
    }

    if (responseData?.detail) {
        return responseData.detail;
    }

    if (responseData?.message) {
        return responseData.message;
    }

    switch (error.response.status) {
        case 400:
            return "The submitted information is invalid.";

        case 401:
            return "Your session has expired. Please log in again.";

        case 403:
            return "You do not have permission to perform this action.";

        case 404:
            return "The requested resource could not be found.";

        case 409:
            return "The action could not be completed because it conflicts with existing data.";

        case 502:
            return "The external service is currently unavailable. Please try again later.";

        case 500:
            return "An unexpected server error occurred. Please try again.";

        default:
            return fallbackMessage;
    }
};