import { useCallback, useEffect, useMemo, useState, type ReactNode, } from "react";
import { addCartItem, getCart, removeCartItem, updateCartItem, type Cart, } from "../api/cartApi";
import { CartContext } from "./CartContext";
import { useAuth } from "./AuthContext";

type CartProviderProps = {
    children: ReactNode;
};

export const CartProvider = ({
    children,
}: CartProviderProps) => {
    const {
        isAuthenticated,
        user,
        loading: authLoading,
    } = useAuth();

    const [cart, setCart] =
        useState<Cart | null>(null);

    const [loading, setLoading] =
        useState(false);

    const [error, setError] =
        useState<string | null>(null);

    const canUseCart =
        isAuthenticated && user?.role === "User";

    const refreshCart = useCallback(async () => {
        if (!canUseCart) {
            setCart(null);
            setError(null);
            return;
        }

        try {
            setLoading(true);
            setError(null);

            const cartData = await getCart();

            setCart(cartData);
        } catch {
            setError(
                "The cart could not be loaded. Please try again."
            );
        } finally {
            setLoading(false);
        }
    }, [canUseCart]);

    useEffect(() => {
        if (authLoading) {
            return;
        }

        void refreshCart();
    }, [authLoading, refreshCart]);

    const runCartAction = async (
        action: () => Promise<Cart>
    ): Promise<boolean> => {
        try {
            setLoading(true);
            setError(null);

            const updatedCart = await action();

            setCart(updatedCart);

            return true;
        } catch {
            setError(
                "The cart could not be updated. Please try again."
            );

            return false;
        } finally {
            setLoading(false);
        }
    };

    const addItem = async (
        productId: number,
        quantity: number
    ) => {
        return runCartAction(() =>
            addCartItem(productId, quantity)
        );
    };

    const updateItem = async (
        itemId: number,
        quantity: number
    ) => {
        return runCartAction(() =>
            updateCartItem(itemId, quantity)
        );
    };

    const removeItem = async (
        itemId: number
    ) => {
        return runCartAction(() =>
            removeCartItem(itemId)
        );
    };

    const itemCount = useMemo(() => {
        return (
            cart?.items.reduce(
                (total, item) =>
                    total + item.quantity,
                0
            ) ?? 0
        );
    }, [cart]);

    const clearError = () => {
        setError(null);
    };

    return (
        <CartContext.Provider
            value={{
                cart,
                itemCount,
                loading,
                error,
                refreshCart,
                addItem,
                updateItem,
                removeItem,
                clearError,
            }}
        >
            {children}
        </CartContext.Provider>
    );
};