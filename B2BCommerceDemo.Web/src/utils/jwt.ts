import { jwtDecode } from "jwt-decode";

export interface JwtPayload {
    sub?: string;
    email?: string;
    Email?: string;

    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"?: string;

    "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"?: string;

    CompanyId?: string | null;
    companyId?: string | null;

    exp: number;
}

export const getUserFromToken = (
    token: string | null
): JwtPayload | null => {
    if (!token) {
        return null;
    }

    try {
        return jwtDecode<JwtPayload>(
            token
        );
    } catch {
        return null;
    }
};