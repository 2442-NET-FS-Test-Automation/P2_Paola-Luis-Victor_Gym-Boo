import { Search } from "lucide-react";
import "./SearchBar.css";

interface SearchBarProps {
    value: string;
    onChange: (value: string) => void;
    placeholder?: string;
}

const SearchBar = ({ value, onChange, placeholder }: SearchBarProps) => {
    return (
        <div className="search-bar">
            <Search size={18} className="search-bar__icon" />
            <input
                type="text"
                value={value}
                placeholder={placeholder}
                onChange={(e) => onChange(e.target.value)}
            />
        </div>
    );
};

export default SearchBar;