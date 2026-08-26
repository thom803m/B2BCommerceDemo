import axiosInstance from "./axios";

export type CompanyPrice = {
    id: number;
    productId: number;
    companyId: number;
    price: number;
};

export type CreateCompanyPriceRequest = {
    productId: number;
    companyId: number;
    price: number;
};

export type UpdateCompanyPriceRequest = {
    price: number;
};

export const getCompanyPrices = async (): Promise<CompanyPrice[]> => {
    const response = await axiosInstance.get<CompanyPrice[]>("/companyprices");

    return response.data;
};

export const createCompanyPrice = async (request: CreateCompanyPriceRequest): Promise<CompanyPrice> => {
    const response = await axiosInstance.post<CompanyPrice>("/companyprices", request);

    return response.data;
};

export const updateCompanyPrice = async (companyPriceId: number, request: UpdateCompanyPriceRequest): Promise<CompanyPrice> => {
    const response = await axiosInstance.put<CompanyPrice>(`/companyprices/${companyPriceId}`, request);

    return response.data;
};

export const deleteCompanyPrice = async (companyPriceId: number): Promise<void> => {
    await axiosInstance.delete(`/companyprices/${companyPriceId}`);
};