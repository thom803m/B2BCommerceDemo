import axiosInstance from "./axios";

export type CartItem = {
    id: number;
    productId: number;
    productName: string;
    imageUrl?: string | null;
    quantity: number;
    unitPrice: number;
    total: number;
};

export type Cart = {
    id: number;
    companyId: number;
    items: CartItem[];
    total: number;
};

export const getCart = async (): Promise<Cart> => {
    const response =
        await axiosInstance.get<Cart>("/cart");

    return response.data;
};

export const addCartItem = async (
    productId: number,
    quantity: number
): Promise<Cart> => {
    const response =
        await axiosInstance.post<Cart>("/cart/items", {
            productId,
            quantity,
        });

    return response.data;
};

export const updateCartItem = async (
    itemId: number,
    quantity: number
): Promise<Cart> => {
    const response =
        await axiosInstance.put<Cart>(
            `/cart/items/${itemId}`,
            {
                quantity,
            }
        );

    return response.data;
};

export const removeCartItem = async (
    itemId: number
): Promise<Cart> => {
    const response =
        await axiosInstance.delete<Cart>(
            `/cart/items/${itemId}`
        );

    return response.data;
};