import { getAdminCompanies, getPendingCompanies, } from "./companyApi";
import { getAdminOrders } from "./orderApi";
import { getProducts } from "./productApi";

export type AdminDashboardSummary = {
    pendingCompanies: number;
    activeCompanies: number;
    totalOrders: number;
    totalProducts: number;
    productsWithoutContent: number;
};

export const getAdminDashboardSummary =
    async (): Promise<AdminDashboardSummary> => {
        const [
            companies,
            pendingCompanies,
            orders,
            products,
            productsWithoutContent,
        ] = await Promise.all([
            getAdminCompanies(),
            getPendingCompanies(),

            getAdminOrders({
                page: 1,
                pageSize: 1,
            }),

            getProducts({
                page: 1,
                pageSize: 1,
            }),

            getProducts({
                hasContent: false,
                page: 1,
                pageSize: 1,
            }),
        ]);

        return {
            pendingCompanies:
                pendingCompanies.length,

            activeCompanies:
                companies.filter(
                    (company) =>
                        company.status === "Active"
                ).length,

            totalOrders:
                orders.totalCount,

            totalProducts:
                products.totalCount,

            productsWithoutContent:
                productsWithoutContent.totalCount,
        };
    };