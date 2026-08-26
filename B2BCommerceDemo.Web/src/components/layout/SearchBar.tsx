import { Search } from "@mui/icons-material";
import { Box, InputBase } from "@mui/material";

const SearchBar = () => {
    return (
        <Box
            sx={{
                display: { xs: "none", md: "flex" },
                alignItems: "center",
                bgcolor: "rgba(255,255,255,0.12)",
                border: "1px solid rgba(255,255,255,0.18)",
                borderRadius: 3,
                px: 2,
                py: 0.5,
                minWidth: 320,
            }}
        >
            <Search sx={{ mr: 1, color: "grey.300" }} />

            <InputBase
                placeholder="Search products..."
                sx={{
                    color: "white",
                    width: "100%",
                    "& input::placeholder": {
                        color: "grey.300",
                        opacity: 1,
                    },
                }}
            />
        </Box>
    );
};

export default SearchBar;