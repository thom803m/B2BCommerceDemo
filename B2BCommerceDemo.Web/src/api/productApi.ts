import axiosInstance from "./axios";

export type ProductImage = {
    id: number;
    url: string | null;
    isPrimary: boolean;
};

export interface Product {
    id: number;
    sku: string;
    name: string;
    basePrice: number;
    ean: string;
    availableStock: number;
    purchasedQuantity: number;
    expectedDeliveryDate?: string | null;
    isActive?: boolean;
    icecatName?: string | null;
    description?: string | null;
    specificationsJson?: string | null;
    icecatProductId?: string | null;
    icecatLastSynced?: string | null;
    contentSource?: string | null;
    contentLocked: boolean;

    brand?: {
        id: number;
        name: string;
    } | null;

    category?: {
        id: number;
        name: string;
    } | null;

    images: ProductImage[];
}

export type ProductWriteRequest = {
    sku: string;
    name: string;
    basePrice: number;
    ean: string;
    availableStock: number;
    brandId: number;
    categoryId: number;
};

export type ProductUpdateRequest = Omit<
    ProductWriteRequest,
    "availableStock"
>;

export type ProductContentWriteRequest = {
    description?: string | null;
    specificationsJson?: string | null;
    contentLocked: boolean;
};

export type ProductQueryParameters = {
    search?: string;
    brand?: string;
    category?: string;
    sku?: string;
    ean?: string;
    inStock?: boolean;
    isPurchased?: boolean;
    minPrice?: number;
    maxPrice?: number;
    contentSource?: string;
    contentLocked?: boolean;
    hasIcecatProductId?: boolean;
    hasContent?: boolean;
    sortBy?: "name" | "price" | "stock";
    sortDirection?: "asc" | "desc";
    page?: number;
    pageSize?: number;
};

export type PagedResult<T> = {
    items: T[];
    totalCount: number;
    page: number;
    pageSize: number;
};

export const getPrimaryImage = (product: Product): string | undefined => {
    return (
        product.images?.find(
            (image) => image.isPrimary && Boolean(image.url))?.url ??
        product.images?.find(
            (image) => Boolean(image.url))?.url ??
        undefined
    );
};

export const getProducts = async (params?: ProductQueryParameters): Promise<PagedResult<Product>> => {
    const response = await axiosInstance.get("/products", { params });

    return response.data;
};

export const getProductById = async (id: number): Promise<Product> => {
    const response = await axiosInstance.get<Product>(`/products/${id}`);

    return response.data;
};

export const createProduct = async (data: ProductWriteRequest): Promise<Product> => {
    const response = await axiosInstance.post<Product>("/products", data);

    return response.data;
};

export const updateProduct = async (id: number, data: ProductUpdateRequest): Promise<Product> => {
    const response = await axiosInstance.put<Product>(`/products/${id}`, data);

    return response.data;
};

export const deleteProduct = async (id: number): Promise<void> => {
    await axiosInstance.delete(`/products/${id}`);
};

export const updateProductContent = async (
    productId: number,
    data: ProductContentWriteRequest
): Promise<Product> => {
    const response = await axiosInstance.put<Product>(`/products/${productId}/content`, data);

    return response.data;
};

export const enrichProduct = async (productId: number): Promise<Product> => {
    const response = await axiosInstance.post<Product>(`/products/${productId}/enrich`);

    return response.data;
};

export const addProductImage = async (productId: number, imageUrl: string): Promise<ProductImage> => {
    const response = await axiosInstance.post<ProductImage>(`/products/${productId}/images/url`,
        JSON.stringify(imageUrl.trim()),
        {
            headers: {
                "Content-Type": "application/json",
            },
        }
    );

    return response.data;
};

export const setPrimaryProductImage = async (productId: number, imageId: number): Promise<void> => {
    await axiosInstance.post(`/products/${productId}/images/${imageId}/primary`);
};

export const deleteProductImage = async (productId: number, imageId: number): Promise<void> => {
        await axiosInstance.delete(`/products/${productId}/images/${imageId}`);
};