import axiosInstance from "./axios";

export type ExportField = {
    key: string;
    label: string;
};

export const getExportFields = async ():
    Promise<ExportField[]> => {
    const response =
        await axiosInstance.get<ExportField[]>(
            "/export/products/fields"
        );

    return response.data;
};

export const exportProducts = async (
    fields: string[]
): Promise<Blob> => {
    const response =
        await axiosInstance.post(
            "/export/products",
            {
                fields,
            },
            {
                responseType: "blob",
            }
        );

    return response.data;
};

export const exportProductsWithMarkup = async (
    fields: string[],
    percentage: number
): Promise<Blob> => {
    const response =
        await axiosInstance.post(
            "/export/products/markup",
            {
                fields,
                percentage,
            },
            {
                responseType: "blob",
            }
        );

    return response.data;
};