import { createContext, useContext, } from "react";

export type AuthUser = {
    id: string;
    email: string;
    role: string;
    companyId: string | null;
};

export type AuthContextValue = {
    user: AuthUser | null;
    isAuthenticated: boolean;
    isAdmin: boolean;
    companyId: string | null;
    loading: boolean;
    login: (token: string) => void;
    logout: () => void;
};

export const AuthContext =
    createContext<AuthContextValue | null>(
        null
    );

export const useAuth = () => {
    const context = useContext(AuthContext);

    if (!context) {
        throw new Error(
            "useAuth must be used within AuthProvider"
        );
    }

    return context;
};