import axiosInstance from "./axios";
import type { PriceGroup, } from "./priceGroupApi";

export type Company = {
    id: number;
    name: string;
    status: CompanyStatus;
    rackbeatCustomerNumber?: string | null;
    priceGroup?: PriceGroup | null;
};

export type ApproveCompanyRequest = {
    priceGroupId: number;
    rackbeatCustomerNumber: string;
};

export type UpdateCompanyPriceGroupRequest = {
    priceGroupId: number;
};

export type CompanyStatus =
    | "Pending"
    | "Active"
    | "Rejected"
    | "Suspended";

export const getCompanies = async ():
    Promise<Company[]> => {
    const response =
        await axiosInstance.get<Company[]>(
            "/companies"
        );

    return response.data;
};

export const getCompanyById = async (
    companyId: number
): Promise<Company> => {
    const response =
        await axiosInstance.get<Company>(
            `/companies/${companyId}`
        );

    return response.data;
};

export const getPendingCompanies = async ():
    Promise<Company[]> => {
    const response =
        await axiosInstance.get<Company[]>(
            "/companies/pending"
        );

    return response.data;
};

export const getAdminCompanies = async ():
    Promise<Company[]> => {
    const response =
        await axiosInstance.get<Company[]>(
            "/companies/admin"
        );

    return response.data;
};

export const approveCompany = async (
    companyId: number,
    request: ApproveCompanyRequest
): Promise<void> => {
    await axiosInstance.put(
        `/companies/${companyId}/approve`,
        request
    );
};

export const rejectCompany = async (
    companyId: number
): Promise<void> => {
    await axiosInstance.put(
        `/companies/${companyId}/reject`
    );
};

export const suspendCompany = async (
    companyId: number
): Promise<void> => {
    await axiosInstance.delete(
        `/companies/${companyId}`
    );
};

export const reactivateCompany = async (
    companyId: number
): Promise<void> => {
    await axiosInstance.put(
        `/companies/${companyId}/reactivate`
    );
};

export const updateCompanyPriceGroup = async (
    companyId: number,
    request: UpdateCompanyPriceGroupRequest
): Promise<void> => {
    await axiosInstance.put(
        `/companies/${companyId}/pricegroup`,
        request
    );
};