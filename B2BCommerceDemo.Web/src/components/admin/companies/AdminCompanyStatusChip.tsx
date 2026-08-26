import { BlockOutlined, CancelOutlined, CheckCircleOutlined, Schedule, } from "@mui/icons-material";
import { Chip } from "@mui/material";
import type { CompanyStatus, } from "../../../api/companyApi";

type AdminCompanyStatusChipProps = {
    status: CompanyStatus;
};

const AdminCompanyStatusChip = ({
    status,
}: AdminCompanyStatusChipProps) => {
    switch (status) {
        case "Pending":
            return (
                <Chip
                    label="Pending"
                    color="warning"
                    size="small"
                    variant="outlined"
                    icon={<Schedule />}
                />
            );

        case "Active":
            return (
                <Chip
                    label="Active"
                    color="success"
                    size="small"
                    variant="outlined"
                    icon={
                        <CheckCircleOutlined />
                    }
                />
            );

        case "Rejected":
            return (
                <Chip
                    label="Rejected"
                    color="error"
                    size="small"
                    variant="outlined"
                    icon={
                        <CancelOutlined />
                    }
                />
            );

        case "Suspended":
            return (
                <Chip
                    label="Suspended"
                    color="default"
                    size="small"
                    variant="outlined"
                    icon={
                        <BlockOutlined />
                    }
                />
            );

        default:
            return null;
    }
};

export default AdminCompanyStatusChip;