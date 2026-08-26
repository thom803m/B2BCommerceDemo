import axiosInstance from "./axios";

export type PriceGroup = {
    id: number;
    name: string;
    percentageAdjustment: number;
};

export type UpdatePriceGroupRequest = {
    name: string;
    percentageAdjustment: number;
};

export const getPriceGroups = async ():
    Promise<PriceGroup[]> => {
    const response =
        await axiosInstance.get<PriceGroup[]>(
            "/pricegroups"
        );

    return response.data;
};

export const updatePriceGroup = async (
    priceGroupId: number,
    request: UpdatePriceGroupRequest
): Promise<PriceGroup> => {
    const response =
        await axiosInstance.put<PriceGroup>(
            `/pricegroups/${priceGroupId}`,
            request
        );

    return response.data;
};