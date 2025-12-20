import { Cell, Legend, Pie, PieChart, Sector, Tooltip } from 'recharts';


// #endregion
const RADIAN = Math.PI / 180;
const COLORS = [
    '#0088FE', '#00C49F', '#FFBB28', '#FF8042',
    '#8A155D', '#4C158A', '#375050', '#6F82A7',
    '#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0',
    '#9966FF', '#FF9F40', '#2ECC71', '#E74C3C',
    '#3498DB', '#9B59B6', '#1ABC9C', '#F1C40F',
    '#E67E22', '#34495E', '#7F8C8D', '#D35400',
    '#16A085', '#27AE60', '#2980B9', '#8E44AD',
    '#C0392B', '#BDC3C7', '#95A5A6', '#F39C12'
];

const renderCustomizedLabel = ({ cx, cy, midAngle, innerRadius, outerRadius, percent }) => {
    if (cx == null || cy == null || innerRadius == null || outerRadius == null) {
        return null;
    }
    const radius = innerRadius + (outerRadius - innerRadius) * 0.5;
    const ncx = Number(cx);
    const x = ncx + 15 + radius * Math.cos(-(midAngle ?? 0) * RADIAN);
    const ncy = Number(cy);
    const y = ncy + 5 + radius * Math.sin(-(midAngle ?? 0) * RADIAN);

    return (
        <text x={x} y={y} fill="white" /*textAnchor={x > ncx ? 'start' : 'end'}*/ dominantBaseline="central" style={{ pointerEvents: "none" }}>
            {`${((percent ?? 1) * 100).toFixed(0)}%`}
        </text>
    );
};
const CustomTooltip = ({ active, payload, label }) => {
    const isVisible = active && payload && payload.length;
    return (
        <div className="custom-tooltip" style={{ visibility: isVisible ? 'visible' : 'hidden' }}>
            {isVisible && (
                <p style={{ backgroundColor: "black", color: "whitesmoke", fontSize: "0.8em", borderRadius: "5px", padding: "8px" }}>{`${payload[0].name} : ${payload[0].value?.toLocaleString()}`}</p>

            )}
        </div>
    );
};
export default function PieChartWithCustomizedLabel({ isAnimationActive = true, data }) {
    return (
        <PieChart style={{ width: '100%', maxWidth: '500px', maxHeight: '80vh', aspectRatio: 1 }} responsive>
            <Pie
                data={data}
                labelLine={false}
                label={renderCustomizedLabel}
                fill="#8884d8"
                dataKey="value"
                isAnimationActive={isAnimationActive}
                activeShape={<Sector fill='#23206aff' />}
            >
                {data.map((entry, index) => (
                    <Cell key={`cell-${entry.name}`} fill={COLORS[index % COLORS.length]} />
                ))}
            </Pie>
            <Tooltip content={CustomTooltip} />
            {/* <Legend
                layout="horizontal"
                verticalAlign="bottom"
                align="center"
                iconType="circle" // شکل رنگ‌ها
                wrapperStyle={{ fontSize: "0.7em", width: "100%", right: "50%", bottom: 0, transform: "translateX(50%)" }}
                iconSize={10}
            /> */}
        </PieChart>
    );
}