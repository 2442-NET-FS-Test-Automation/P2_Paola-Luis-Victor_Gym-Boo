import "./Tabs.css";

export interface TabItem {
    key: string;
    label: string;
    count: number;
}

interface TabsProps {
    tabs: TabItem[];
    activeKey: string;
    onChange: (key: string) => void;
}

const Tabs = ({ tabs, activeKey, onChange }: TabsProps) => {
    return (
        <div className="tabs">
            {tabs.map((tab) => (
                <button
                    key={tab.key}
                    type="button"
                    className={`tabs__item ${tab.key === activeKey ? "is-active" : ""}`}
                    onClick={() => onChange(tab.key)}
                >
                    <span>{tab.label}</span>
                    <span className="tabs__count">{tab.count}</span>
                </button>
            ))}
        </div>
    );
};

export default Tabs;