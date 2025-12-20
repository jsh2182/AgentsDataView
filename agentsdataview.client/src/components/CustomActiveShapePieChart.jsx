import { useState } from 'react';
import { Pie, PieChart, Sector, Tooltip, Cell, Legend, ResponsiveContainer } from 'recharts';

const COLORS = [
    '#0088FE', '#00C49F', '#FFBB28', '#FF8042',
    '#8A155D', '#4C158A', '#375050', '#6F82A7',
    '#FF6384', '#36A2EB', '#c9bb97ff', '#4BC0C0',
    '#9966FF', '#FF9F40', '#2ECC71', '#E74C3C',
    '#3498DB', '#9B59B6', '#1ABC9C', '#F1C40F',
    '#E67E22', '#34495E', '#7F8C8D', '#D35400',
    '#16A085', '#27AE60', '#2980B9', '#7a4d79ff',
    '#C0392B', '#BDC3C7', '#95A5A6', '#F39C12'
];

const renderActiveShape = ({
  cx,
  cy,
  midAngle,
  innerRadius,
  outerRadius,
  startAngle,
  endAngle,
  fill,
  payload,
  percent,
  value,
}) => {
  const RADIAN = Math.PI / 180;
  const sin = Math.sin(-RADIAN * midAngle);
  const cos = Math.cos(-RADIAN * midAngle);
  const sx = cx + (outerRadius + 7) * cos;
  const sy = cy + (outerRadius + 7) * sin;
  const mx = cx + (outerRadius + 15) * cos;
  const my = cy + (outerRadius + 15) * sin;
  const ex = mx + (cos >= 0 ? 1 : -1) * 22;
  const ey = my;
  const textAnchor = cos >= 0 ? 'start' : 'end';

  return (
    <g>
      <text x={cx} y={cy} dy={8} textAnchor="middle" fill={fill} style={{ fontSize: "12px" }}>
        {payload.name.substring(0,11)}
      </text>
      <Sector
        cx={cx}
        cy={cy}
        innerRadius={innerRadius}
        outerRadius={outerRadius}
        startAngle={startAngle}
        endAngle={endAngle}
        fill={fill}
      />
      <Sector
        cx={cx}
        cy={cy}
        startAngle={startAngle}
        endAngle={endAngle}
        innerRadius={outerRadius + 6}
        outerRadius={outerRadius + 10}
        fill={fill}
      />
      <path d={`M${sx},${sy}L${mx},${my}L${ex},${ey}`} stroke={fill} fill="none" />
      <circle cx={ex} cy={ey} r={2} fill={fill} stroke="none" />
      {/* <text x={ex} y={ey} dy={-5} textAnchor={textAnchor} fill="#333" style={{ fontSize: "10px", fontWeight: "700", textAlign: "center", direction: "ltr" }}>
        {`${value}`}
      </text> */}
      <text x={ex} y={ey} dy={15} textAnchor={textAnchor} fill="#474e7aff" style={{ fontSize: "10px", fontWeight: "700", direction: "ltr" }}>
        {`${(percent * 100).toFixed(1)}%`}
      </text>
    </g>
  );
};

export default function CustomActiveShapePieChart({
  isAnimationActive = true,
  data,
}) {
  const [hide, setHide] = useState(false);
  return (
      <PieChart
        style={{ width: '100%', maxWidth:"500px", maxHeight: '80vh', aspectRatio: 1 }}
        margin={{ top: 35, right: 35, bottom: 35, left: 35 }}
      >
        <Pie
          activeShape={renderActiveShape}
          data={data}
          cx="50%"
          cy="50%"
          innerRadius="60%"
          outerRadius="80%"
          dataKey="value"
          isAnimationActive={isAnimationActive}
          onMouseEnter={() => setHide(true)}
          onMouseLeave={() => setHide(false)}
        >
          {data.map((entry, index) => (
            <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
          ))}
        </Pie>
        {/* <Legend
          layout="horizontal"
          verticalAlign="bottom"
          align="center"
          iconType="circle" // شکل رنگ‌ها
          wrapperStyle={{ fontSize: "0.7em", width: "100%", right: "50%", bottom:0, transform: "translateX(50%)" }}
          iconSize={10}
        /> */}
      </PieChart>
  );
}
