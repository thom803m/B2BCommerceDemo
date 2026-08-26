import axiosInstance from "./axios";

export type OrderItem = {
    productId: number;
    sku?: string | null;
    productName?: string | null;
    quantity: number;
    unitPrice: number;
    lineTotal: number;
};

export type Order = {
    id: number;
    companyId: number;
    createdAt: string;
    total: number;
    status?: OrderStatus | null;

    rackbeatOrderNumber?: string | null;
    rackbeatSyncStatus?: RackbeatSyncStatus | null;
    rackbeatSyncError?: string | null;
    rackbeatSyncedAt?: string | null;

    items: OrderItem[];
};

type CreateOrderRequest = {
    idempotencyKey: string;
};

export type OrderStatus =
    | "Pending"
    | "Confirmed"
    | "Processing"
    | "Shipped"
    | "Completed"
    | "Cancelled";

export type RackbeatSyncStatus =
    | "Pending"
    | "Synced"
    | "Failed";

export type AdminOrderListItem = {
    id: number;
    companyId: number;
    status: OrderStatus;
    total: number;
    createdAt: string;
};

export type AdminOrderQueryParameters = {
    status?: OrderStatus | "";
    companyId?: number;
    fromDate?: string;
    toDate?: string;
    page?: number;
    pageSize?: number;
};

export type PagedResult<T> = {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
};

const orderStatusValues: Record<
    OrderStatus,
    number
> = {
    Pending: 0,
    Confirmed: 1,
    Processing: 2,
    Shipped: 3,
    Completed: 4,
    Cancelled: 5,
};

export const createOrder = async (
    idempotencyKey: string
): Promise<Order> => {
    const request: CreateOrderRequest = {
        idempotencyKey,
    };

    const response =
        await axiosInstance.post<Order>(
            "/orders",
            request
        );

    return response.data;
};

export const getOrders = async (): Promise<
    Order[]
> => {
    const response =
        await axiosInstance.get<Order[]>(
            "/orders"
        );

    return response.data;
};

export const getOrderById = async (
    orderId: number
): Promise<Order> => {
    const response =
        await axiosInstance.get<Order>(
            `/orders/${orderId}`
        );

    return response.data;
};

export const getAdminOrders = async (
    params?: AdminOrderQueryParameters
): Promise<PagedResult<AdminOrderListItem>> => {
    const response =
        await axiosInstance.get<
            PagedResult<AdminOrderListItem>
        >(
            "/orders/admin",
            {
                params,
            }
        );

    return response.data;
};

export const getAdminOrderById = async (
    orderId: number
): Promise<Order> => {
    const response =
        await axiosInstance.get<Order>(
            `/orders/admin/${orderId}`
        );

    return response.data;
};

export const updateOrderStatus = async (
    orderId: number,
    status: OrderStatus
): Promise<Order> => {
    const statusValue =
        orderStatusValues[status];

    const response =
        await axiosInstance.put<Order>(
            `/orders/${orderId}/status`,
            JSON.stringify(statusValue),
            {
                headers: {
                    "Content-Type":
                        "application/json",
                },
            }
        );

    return response.data;
};