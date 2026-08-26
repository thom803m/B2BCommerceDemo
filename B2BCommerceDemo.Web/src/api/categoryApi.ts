import axiosInstance from "./axios";

export type Category = {
    id: number;
    name: string;
};

export const getCategories = async (): Promise<Category[]> => {
    const response = await axiosInstance.get("/categories");
    return response.data;
};