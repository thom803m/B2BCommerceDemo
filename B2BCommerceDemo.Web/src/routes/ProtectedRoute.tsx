import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

interface Props {
    children: React.ReactNode;
    roles?: string[];
}

const ProtectedRoute = ({ children, roles }: Props) => {
    const { isAuthenticated, user, loading } = useAuth();

    console.log("ProtectedRoute user:", user);
    console.log("ProtectedRoute roles:", roles);
    console.log("ProtectedRoute loading:", loading);

    if (loading) {
        return <div>Loading...</div>;
    }
  
    if (!isAuthenticated || !user) {
        return <Navigate to="/login" replace />;
    }

    if (roles && !roles.includes(user.role)) {
        return <Navigate to="/" replace />;
    }

    return <>{children}</>;
};

export default ProtectedRoute;