import { type ReactNode, useState, } from "react";
import { getUserFromToken, type JwtPayload, } from "../utils/jwt";
import { AuthContext, type AuthUser, } from "./AuthContext";

type AuthProviderProps = {
    children: ReactNode;
};

const isTokenValid = (
    decoded: JwtPayload | null
) => {
    return (
        decoded !== null &&
        decoded.exp * 1000 > Date.now()
    );
};

const mapUser = (
    decoded: JwtPayload
): AuthUser => {
    return {
        id: decoded.sub ?? "",

        email:
            decoded.email ??
            decoded.Email ??
            decoded[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
            ] ??
            "",

        role:
            decoded[
            "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
            ] ?? "",

        companyId:
            decoded.CompanyId ??
            decoded.companyId ??
            null,
    };
};

const getInitialToken = () => {
    const storedToken =
        localStorage.getItem("token");

    if (!storedToken) {
        return null;
    }

    const decoded =
        getUserFromToken(storedToken);

    if (!isTokenValid(decoded)) {
        localStorage.removeItem("token");
        return null;
    }

    return storedToken;
};

export const AuthProvider = ({
    children,
}: AuthProviderProps) => {
    const [token, setToken] =
        useState<string | null>(
            getInitialToken
        );

    const decoded =
        getUserFromToken(token);

    const user =
        isTokenValid(decoded) && decoded
            ? mapUser(decoded)
            : null;

    const login = (
        newToken: string
    ) => {
        const decodedToken =
            getUserFromToken(newToken);

        if (!isTokenValid(decodedToken)) {
            localStorage.removeItem(
                "token"
            );

            setToken(null);
            return;
        }

        localStorage.setItem(
            "token",
            newToken
        );

        setToken(newToken);
    };

    const logout = () => {
        localStorage.removeItem(
            "token"
        );

        setToken(null);
    };

    return (
        <AuthContext.Provider
            value={{
                user,
                loading: false,
                isAuthenticated:
                    user !== null,
                isAdmin:
                    user?.role === "Admin",
                companyId:
                    user?.companyId ??
                    null,
                login,
                logout,
            }}
        >
            {children}
        </AuthContext.Provider>
    );
};