import axiosInstance from "./axios";

export interface LoginRequest {
    email: string;
    password: string;
}

export interface LoginResponse {
    token: string;
    companyId: number | null;
}

export type RegisterRequest = {
    companyName: string;
    email: string;
    password: string;
};

export type RegisterResponse = {
    message: string;
};

export const login = async (data: LoginRequest): Promise<LoginResponse> => {
    const response = await axiosInstance.post("/accounts/login", data);

    return {
        token: response.data.token ?? response.data.Token,
        companyId: response.data.companyId ?? response.data.CompanyId ?? null,
    };
};

export const register = async (data: RegisterRequest): Promise<RegisterResponse> => {
    const response = await axiosInstance.post<{
            message?: string;
            Message?: string;
        }>(
            "/accounts/register",
            data
        );

    return {
        message:
            response.data.message ??
            response.data.Message ??
            "Registration submitted and awaiting approval.",
    };
};

export type ForgotPasswordRequest = {
    email: string;
};

export const forgotPassword = async (
    data: ForgotPasswordRequest
): Promise<void> => {
    await axiosInstance.post(
        "/accounts/forgot-password",
        data
    );
};

export type ResetPasswordRequest = {
    userId: string;
    token: string;
    newPassword: string;
};

export const resetPassword = async (
    data: ResetPasswordRequest
): Promise<void> => {
    await axiosInstance.post(
        "/accounts/reset-password",
        data
    );
};

export type ChangePasswordRequest = {
    currentPassword: string;
    newPassword: string;
};

export const changePassword = async (
    data: ChangePasswordRequest
): Promise<void> => {
    await axiosInstance.post(
        "/accounts/change-password",
        data
    );
};