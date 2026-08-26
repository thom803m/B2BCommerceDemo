export const formatPrice = (
    price: number,
    currency = "EUR"
) => {
    return new Intl.NumberFormat("da-DK", {
        style: "currency",
        currency,
        minimumFractionDigits: 0,
        maximumFractionDigits: 0,
    }).format(price);
};