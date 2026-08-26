import axiosInstance from "./axios";

export type ImportResult = {
    created: number;
    updated: number;
    skipped: number;
    warnings: string[];
};

const uploadImportFile = async (
    endpoint: string,
    file: File
): Promise<ImportResult> => {
    const formData = new FormData();

    formData.append("file", file);

    const response =
        await axiosInstance.post<ImportResult>(
            endpoint,
            formData
        );

    return response.data;
};

export const importProducts = async (
    file: File
): Promise<ImportResult> => {
    return uploadImportFile(
        "/import/products",
        file
    );
};

export const importDeliveryDates = async (
    file: File
): Promise<ImportResult> => {
    return uploadImportFile(
        "/import/delivery-dates",
        file
    );
};