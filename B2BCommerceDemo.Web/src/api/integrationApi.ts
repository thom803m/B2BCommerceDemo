import axiosInstance from "./axios";

export type IntegrationImportResult = {
    created: number;
    updated: number;
    skipped: number;
    warnings: string[];
};

export type IcecatEnrichmentResult = {
    checked: number;
    fullyEnriched: number;
    partiallyEnriched: number;
    fullIcecatRequired: number;
    notFound: number;
    failed: number;
    warnings: string[];
};

export const syncRackbeatProducts = async (): Promise<IntegrationImportResult> => {
        const response = await axiosInstance.post<IntegrationImportResult>("/rackbeat/sync-products");

        return response.data;
    };

export const syncExpectedDeliveries = async (): Promise<IntegrationImportResult> => {
        const response = await axiosInstance.post<IntegrationImportResult>("/rackbeat/sync-expected-deliveries");

        return response.data;
    };

export const syncOrderStatuses = async (): Promise<IntegrationImportResult> => {
        const response = await axiosInstance.post<IntegrationImportResult>("/rackbeat/sync-order-statuses");

        return response.data;
    };

export const enrichMissingProductContent = async (): Promise<IcecatEnrichmentResult> => {
        const response = await axiosInstance.post<IcecatEnrichmentResult>("/products/enrich-missing-content");

        return response.data;
    };