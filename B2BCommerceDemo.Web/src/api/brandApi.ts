import axiosInstance from "./axios";

export type Brand = {
    id: number;
    name: string;
};

export const getBrands = async (): Promise<Brand[]> => {
    const response = await axiosInstance.get("/brands");
    return response.data;
};