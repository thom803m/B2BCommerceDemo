import { lazy, Suspense, } from "react";
import { BrowserRouter, Route, Routes, } from "react-router-dom";
import LoadingSpinner from "../components/common/LoadingSpinner";
import ScrollToTop from "../components/common/ScrollToTop";
import AdminLayout from "../layout/AdminLayout";
import MainLayout from "../layout/MainLayout";
import ProtectedRoute from "./ProtectedRoute";

const HomePage = lazy(
    () => import(
            "../pages/HomePage"
    )
);

const LoginPage = lazy(
    () =>
        import(
            "../pages/auth/LoginPage"
    )
);

const ForgotPasswordPage = lazy(
    () =>
        import(
            "../pages/auth/ForgotPasswordPage"
        )
);

const ResetPasswordPage = lazy(
    () =>
        import(
            "../pages/auth/ResetPasswordPage"
        )
);

const ChangePasswordPage = lazy(
    () =>
        import(
            "../pages/auth/ChangePasswordPage"
        )
);

const RegisterPage = lazy(
    () =>
        import(
            "../pages/auth/RegisterPage"
    )
);

const ConfirmEmailPage = lazy(
    () =>
        import(
            "../pages/auth/ConfirmEmailPage"
        )
);

const ProductGridPage = lazy(
    () =>
        import(
            "../pages/products/ProductGridPage")
);

const ProductDetailPage = lazy(
    () =>
        import(
            "../pages/products/ProductDetailPage"
        )
);

const CartPage = lazy(
    () =>
        import(
            "../pages/cart/CartPage"
    )
);

const CheckoutPage = lazy(
    () =>
        import(
            "../pages/checkout/CheckoutPage"
        )
);

const ContactPage = lazy(
    () => import("../pages/contact/ContactPage")
);

const OrderHistoryPage = lazy(
    () =>
        import(
            "../pages/orders/OrderHistoryPage"
        )
);

const NotFoundPage = lazy(
    () =>
        import(
            "../pages/errors/NotFoundPage"
        )
);

const AdminDashboardPage = lazy(
    () =>
        import(
            "../pages/admin/AdminDashboardPage"
        )
);

const AdminProductsPage = lazy(
    () =>
        import(
            "../pages/admin/products/AdminProductsPage"
        )
);

const CreateProductPage = lazy(
    () =>
        import(
            "../pages/admin/products/CreateProductPage"
        )
);

const EditProductPage = lazy(
    () =>
        import(
            "../pages/admin/products/EditProductPage"
        )
);

const AdminProductContentPage = lazy(
    () =>
        import(
            "../pages/admin/products/AdminProductContentPage"
        )
);

const CompaniesPage = lazy(
    () =>
        import(
            "../pages/admin/companies/CompaniesPage"
        )
);

const AdminOrdersPage = lazy(
    () =>
        import(
            "../pages/admin/orders/AdminOrdersPage"
        )
);

const AdminOrderDetailPage = lazy(
    () =>
        import(
            "../pages/admin/orders/AdminOrderDetailPage"
        )
);

const PriceGroupsPage = lazy(
    () =>
        import(
            "../pages/admin/pricing/PriceGroupsPage"
        )
);

const CompanyPricingPage = lazy(
    () =>
        import(
            "../pages/admin/company-prices/CompanyPricingPage"
        )
);

const IntegrationsPage = lazy(
    () =>
        import(
            "../pages/admin/integrations/IntegrationsPage"
        )
);

const AppRoutes = () => {
    return (
        <BrowserRouter>
            <ScrollToTop />

            <Suspense
                fallback={
                    <LoadingSpinner text="Loading page..." />
                }
            >
                <Routes>
                    <Route
                        element={<MainLayout />}
                    >
                        <Route
                            path="/"
                            element={<HomePage />}
                        />

                        <Route
                            path="/login"
                            element={<LoginPage />}
                        />

                        <Route
                            path="/forgot-password"
                            element={<ForgotPasswordPage />}
                        />

                        <Route
                            path="/reset-password"
                            element={<ResetPasswordPage />}
                        />

                        <Route
                            path="/change-password"
                            element={
                                <ProtectedRoute roles={["User", "Admin"]}>
                                    <ChangePasswordPage />
                                </ProtectedRoute>
                            }
                        />

                        <Route
                            path="/register"
                            element={<RegisterPage />}
                        />

                        <Route
                            path="/confirm-email"
                            element={<ConfirmEmailPage />}
                        />

                        <Route
                            path="/products"
                            element={
                                <ProductGridPage />
                            }
                        />

                        <Route
                            path="/products/:id"
                            element={
                                <ProductDetailPage />
                            }
                        />

                        <Route
                            path="/cart"
                            element={
                                <ProtectedRoute
                                    roles={["User"]}
                                >
                                    <CartPage />
                                </ProtectedRoute>
                            }
                        />

                        <Route
                            path="/checkout"
                            element={
                                <ProtectedRoute
                                    roles={["User"]}
                                >
                                    <CheckoutPage />
                                </ProtectedRoute>
                            }
                        />

                        <Route
                            path="/contact"
                            element={<ContactPage />}
                        />

                        <Route
                            path="/orders"
                            element={
                                <ProtectedRoute
                                    roles={["User"]}
                                >
                                    <OrderHistoryPage />
                                </ProtectedRoute>
                            }
                        />

                        <Route
                            path="/admin"
                            element={
                                <ProtectedRoute
                                    roles={["Admin"]}
                                >
                                    <AdminLayout />
                                </ProtectedRoute>
                            }
                        >
                            <Route
                                index
                                element={
                                    <AdminDashboardPage />
                                }
                            />

                            <Route
                                path="products"
                                element={
                                    <AdminProductsPage />
                                }
                            />

                            <Route
                                path="products/create"
                                element={
                                    <CreateProductPage />
                                }
                            />

                            <Route
                                path="products/:id"
                                element={
                                    <EditProductPage />
                                }
                            />

                            <Route
                                path="products/:id/content"
                                element={
                                    <AdminProductContentPage />
                                }
                            />

                            <Route
                                path="companies"
                                element={
                                    <CompaniesPage />
                                }
                            />

                            <Route
                                path="orders"
                                element={
                                    <AdminOrdersPage />
                                }
                            />

                            <Route
                                path="orders/:id"
                                element={
                                    <AdminOrderDetailPage />
                                }
                            />

                            <Route
                                path="pricing"
                                element={
                                    <PriceGroupsPage />
                                }
                            />

                            <Route
                                path="pricing/company-prices"
                                element={
                                    <CompanyPricingPage />
                                }
                            />

                            <Route
                                path="integrations"
                                element={
                                    <IntegrationsPage />
                                }
                            />

                            <Route
                                path="*"
                                element={
                                    <NotFoundPage />
                                }
                            />
                        </Route>

                        <Route
                            path="*"
                            element={
                                <NotFoundPage />
                            }
                        />
                    </Route>
                </Routes>
            </Suspense>
        </BrowserRouter>
    );
};

export default AppRoutes;