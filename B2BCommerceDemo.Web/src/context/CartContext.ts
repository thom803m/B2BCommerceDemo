import {
    createContext,
    useContext,
} from "react";
import type { Cart } from "../api/cartApi";

export type CartContextValue = {
    cart: Cart | null;
    itemCount: number;
    loading: boolean;
    error: string | null;

    refreshCart: () => Promise<void>;

    addItem: (
        productId: number,
        quantity: number
    ) => Promise<boolean>;

    updateItem: (
        itemId: number,
        quantity: number
    ) => Promise<boolean>;

    removeItem: (
        itemId: number
    ) => Promise<boolean>;

    clearError: () => void;
};

export const CartContext =
    createContext<CartContextValue | null>(
        null
    );

export const useCart = () => {
    const context = useContext(CartContext);

    if (!context) {
        throw new Error(
            "useCart must be used within CartProvider"
        );
    }

    return context;
};