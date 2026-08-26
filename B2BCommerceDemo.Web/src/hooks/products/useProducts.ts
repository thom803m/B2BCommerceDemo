import { useCallback, useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { getProducts, type Product } from "../../api/productApi";
import { getBrands, type Brand } from "../../api/brandApi";
import { getCategories, type Category, } from "../../api/categoryApi";
import type { ProductFilterValues } from "../../components/products/ProductFilters";
import type { ProductSortValue } from "../../components/products/ProductSort";

const defaultFilters: ProductFilterValues = {
    search: "",
    brand: "",
    category: "",
    inStock: false,
};

const defaultSort: ProductSortValue =
    "name-asc";

const getFiltersFromSearchParams = (
    searchParams: URLSearchParams
): ProductFilterValues => ({
    search: searchParams.get("search") ?? "",
    brand: searchParams.get("brand") ?? "",
    category:
        searchParams.get("category") ?? "",
    inStock:
        searchParams.get("inStock") === "true",
});

const getSortFromSearchParams = (
    searchParams: URLSearchParams
): ProductSortValue => {
    const value = searchParams.get("sort");

    switch (value) {
        case "price-asc":
        case "price-desc":
        case "stock-desc":
        case "name-asc":
            return value;

        default:
            return defaultSort;
    }
};

const getPageFromSearchParams = (
    searchParams: URLSearchParams
) => {
    const value = Number(
        searchParams.get("page")
    );

    return Number.isInteger(value) && value > 0
        ? value
        : 1;
};

const getSortParameters = (sort: ProductSortValue) => {
    switch (sort) {
        case "price-asc":
            return {
                sortBy: "price" as const,
                sortDirection: "asc" as const,
            };

        case "price-desc":
            return {
                sortBy: "price" as const,
                sortDirection: "desc" as const,
            };

        case "stock-desc":
            return {
                sortBy: "stock" as const,
                sortDirection: "desc" as const,
            };

        case "name-asc":
        default:
            return {
                sortBy: "name" as const,
                sortDirection: "asc" as const,
            };
    }
};

export const useProducts = () => {
    const [products, setProducts] = useState<Product[]>([]);
    const [brands, setBrands] = useState<Brand[]>([]);
    const [categories, setCategories] = useState<Category[]>([]);
    const [optionsLoading, setOptionsLoading] = useState(false);
    const [searchParams, setSearchParams] = useSearchParams();
    const searchParamsKey = searchParams.toString();
    const [filters, setFilters] = useState<ProductFilterValues>(() => getFiltersFromSearchParams(searchParams));
    const [sort, setSort] = useState<ProductSortValue>(() => getSortFromSearchParams( searchParams ) );
    const [page, setPage] = useState(() => getPageFromSearchParams( searchParams ));
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [pageSize] = useState(12);
    const [totalCount, setTotalCount] = useState(0);

    const loadProducts = useCallback(
        async (
            currentFilters: ProductFilterValues,
            currentSort: ProductSortValue,
            currentPage: number
        ) => {
            try {
                setLoading(true);
                setError(null);

                const sortParameters =
                    getSortParameters(currentSort);

                const data = await getProducts({
                    search:
                        currentFilters.search ||
                        undefined,
                    brand:
                        currentFilters.brand ||
                        undefined,
                    category:
                        currentFilters.category ||
                        undefined,
                    inStock:
                        currentFilters.inStock ||
                        undefined,
                    page: currentPage,
                    pageSize,
                    ...sortParameters,
                });

                setProducts(data.items);
                setTotalCount(data.totalCount);
                setPage(data.page);
            } catch {
                setError(
                    "Something went wrong while loading products."
                );
            } finally {
                setLoading(false);
            }
        },
        [pageSize]
    );

    const updateProductUrl = (
        newFilters: ProductFilterValues,
        newSort: ProductSortValue,
        newPage: number
    ) => {
        const params = new URLSearchParams();

        const trimmedSearch =
            newFilters.search.trim();

        if (trimmedSearch) {
            params.set("search", trimmedSearch);
        }

        if (newFilters.brand) {
            params.set("brand", newFilters.brand);
        }

        if (newFilters.category) {
            params.set(
                "category",
                newFilters.category
            );
        }

        if (newFilters.inStock) {
            params.set("inStock", "true");
        }

        if (newSort !== defaultSort) {
            params.set("sort", newSort);
        }

        if (newPage > 1) {
            params.set(
                "page",
                newPage.toString()
            );
        }

        setSearchParams(params);
    };

    const changeSort = (
        newSort: ProductSortValue
    ) => {
        updateProductUrl(
            filters,
            newSort,
            1
        );
    };

    const applyFilters = () => {
        updateProductUrl(
            filters,
            sort,
            1
        );
    };

    const resetFilters = () => {
        setFilters(defaultFilters);

        updateProductUrl(
            defaultFilters,
            sort,
            1
        );
    };

    const changePage = (
        newPage: number
    ) => {
        updateProductUrl(
            filters,
            sort,
            newPage
        );
    };

    const loadFilterOptions =
        useCallback(async () => {
            try {
                setOptionsLoading(true);

                const [
                    brandData,
                    categoryData,
                ] = await Promise.all([
                    getBrands(),
                    getCategories(),
                ]);

                setBrands(
                    [...brandData].sort(
                        (a, b) =>
                            a.name.localeCompare(
                                b.name
                            )
                    )
                );

                setCategories(
                    [...categoryData].sort(
                        (a, b) =>
                            a.name.localeCompare(
                                b.name
                            )
                    )
                );
            } catch {
                setBrands([]);
                setCategories([]);
            } finally {
                setOptionsLoading(false);
            }
        }, []);

    useEffect(() => {
        void loadFilterOptions();
    }, [loadFilterOptions]);

    useEffect(() => {
        const currentSearchParams =
            new URLSearchParams(
                searchParamsKey
            );

        const currentFilters =
            getFiltersFromSearchParams(
                currentSearchParams
            );

        const currentSort =
            getSortFromSearchParams(
                currentSearchParams
            );

        const currentPage =
            getPageFromSearchParams(
                currentSearchParams
            );

        setFilters(currentFilters);
        setSort(currentSort);
        setPage(currentPage);

        void loadProducts(
            currentFilters,
            currentSort,
            currentPage
        );
    }, [
        searchParamsKey,
        loadProducts,
    ]);

    const refreshProducts =
        useCallback(async () => {
            const currentSearchParams =
                new URLSearchParams(
                    searchParamsKey
                );

            const currentFilters =
                getFiltersFromSearchParams(
                    currentSearchParams
                );

            const currentSort =
                getSortFromSearchParams(
                    currentSearchParams
                );

            const currentPage =
                getPageFromSearchParams(
                    currentSearchParams
                );

            await Promise.all([
                loadProducts(
                    currentFilters,
                    currentSort,
                    currentPage
                ),
                loadFilterOptions(),
            ]);
        }, [
            searchParamsKey,
            loadProducts,
            loadFilterOptions,
        ]);

    const pageCount = Math.ceil(
        totalCount / pageSize
    );

    return {
        products,
        filters,
        setFilters,
        brands,
        categories,
        applyFilters,
        resetFilters,
        optionsLoading,
        loading,
        error,
        sort,
        setSort: changeSort,
        page,
        pageSize,
        totalCount,
        pageCount,
        changePage,
        refreshProducts,
    };
};